// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using Microsoft.AspNetCore.Components.Web.Virtualization;
using System.Diagnostics.CodeAnalysis;
using System.Timers;
using System.Text.Json;
using System.Text;
using System.Text.Json.Nodes;

namespace BlazorTableExample;

/// <summary>
/// A component that displays a grid.
/// </summary>
/// <typeparam name="TGridItem">The type of data represented by each row in the grid.</typeparam>
[CascadingTypeParameter(nameof(TGridItem))]
public partial class Grid<TGridItem> : ComponentBase, IAsyncDisposable
{
    [Inject] IJSRuntime Script { get; set; }
    [Inject] private IServiceProvider Services { get; set; } = default!;

    /// <summary>
    /// A queryable source of data for the grid.
    ///
    /// This could be in-memory data converted to queryable using the
    /// <see cref="System.Linq.Queryable.AsQueryable(System.Collections.IEnumerable)"/> extension method,
    /// or an EntityFramework DataSet or an <see cref="IQueryable"/> derived from it.
    ///
    /// You should supply either <see cref="Items"/> or <see cref="ItemsProvider"/>, but not both.
    /// </summary>
    [Parameter] public List<TGridItem>? Items { get; set; } = new();

    /// <summary>
    /// A callback that supplies data for the rid.
    ///
    /// You should supply either <see cref="Items"/> or <see cref="ItemsProvider"/>, but not both.
    /// </summary>
    [Parameter] public GridItemsProvider<TGridItem>? ItemsProvider { get; set; }

    /// <summary>
    /// An optional CSS class name. If given, this will be included in the class attribute of the rendered grid.
    /// </summary>
    [Parameter] public string? Class { get; set; }

    /// <summary>
    /// Defines the child components of this instance. For example, you may define columns by adding
    /// components derived from the <see cref="ColumnBase{TGridItem}"/> base class.
    /// </summary>
    //[Parameter] public RenderFragment<TGridItem>? ChildContent { get; set; }
    [Parameter] public RenderFragment? Columns { get; set; }
    [Parameter] public bool AllowPaging { get; set; } = false;
    [Parameter] public bool HideFooter { get; set; } = false;
    [Parameter] public int PageSize { get; set; } = 0;

    /// <summary>
    /// This is applicable only when using <see cref="Virtualize"/>. It defines an expected height in pixels for
    /// each row, allowing the virtualization mechanism to fetch the correct number of items to match the display
    /// size and to ensure accurate scrolling.
    /// </summary>
    [Parameter] public float ItemSize { get; set; } = 53;
    [Parameter] public string Height { get; set; } = string.Empty;
    [Parameter] public string Width { get; set; } = "fit-content";
    [Parameter, EditorRequired] public string ID { get; set; } = string.Empty;
    [Parameter, AllowNull] public RenderFragment? FooterTemplate { get; set; }

    /// <summary>
    /// If true, renders draggable handles around the column headers, allowing the user to resize the columns
    /// manually. Size changes are not persisted.
    /// </summary>
    [Parameter] public bool ResizableColumns { get; set; } = false;

    /// <summary>
    /// Optionally defines a value for @key on each rendered row. Typically this should be used to specify a
    /// unique identifier, such as a primary key value, for each data item.
    ///
    /// This allows the grid to preserve the association between row elements and data items based on their
    /// unique identifiers, even when the <see cref="TGridItem"/> instances are replaced by new copies (for
    /// example, after a new query against the underlying data store).
    ///
    /// If not set, the @key will be the <see cref="TGridItem"/> instance itself.
    /// </summary>
    [Parameter] public Func<TGridItem, object> ItemKey { get; set; } = x => x!;

    /// <summary>
    /// The parameter used to indicate the the EditItemTemplate should be displayed instead of the ItemTemplate
    /// </summary>
    [Parameter] public int EditIndex { get; set; } = -1;

    [Parameter] public string SearchWidth { get; set; } = "250px";
    [Parameter] public bool Searchable { get; set; } = true;
    [Parameter] public EventCallback ColumnWidthChanged { get; set; }

    /// <summary>
    /// When true, includes a default add button in the header cell of the column.
    /// <exception cref="System.ArgumentException">Thrown if the argument is added to more than one column</exception>
    /// </summary>
    [Parameter] public bool ShowAddButton { get; set; }

    /// <summary>
    /// EventCallback for the parent to execute some code to add a new row. No call to StateHasChanged is needed for the grid to update
    /// </summary>
    [Parameter] public EventCallback AddRecord_Click { get; set; }

    private ElementReference _gridReference;
    private Virtualize<TGridItem>? _virtualizeComponent;
    private List<TGridItem> ItemsFiltered = new(); //the collection we do near everything with. This is the one that matters most
    private List<TGridItem> ItemsState = new(); //holds the current itemcollection so we can run some code only when the collection changes.. which we wont know until the component reloads

    // IQueryable only exposes synchronous query APIs. IAsyncQueryExecutor is an adapter that lets us invoke any
    // async query APIs that might be available. We have built-in support for using EF Core's async query APIs.
    private IAsyncQueryExecutor? _asyncQueryExecutor;

    // We cascade the InternalGridContext to descendants, which in turn call it to add themselves to _columns
    // This happens on every render so that the column list can be updated dynamically
    private InternalGridContext<TGridItem> _internalGridContext;
    private List<ColumnBase<TGridItem>> _columns;
    private bool _collectingColumns; // Columns might re-render themselves arbitrarily. We only want to capture them at a defined time.

    // Tracking state for options and sorting
    private ColumnBase<TGridItem>? _displayOptionsForColumn;
    private ColumnBase<TGridItem>? _sortByColumn;
    public bool _sortByAscending;
    public SortDirection _lastDirection;

    // The associated ES6 module, which uses document-level event listeners
    private IJSObjectReference? _jsModule;
    private IJSObjectReference? _jsEventDisposable;

    // Caches of method->delegate conversions
    private readonly RenderFragment _renderColumnHeaders;
    private readonly RenderFragment _renderNonVirtualizedRows;
    private object? _lastAssignedItemsOrProvider;
    private CancellationTokenSource? _pendingDataLoadCancellationTokenSource;
    private bool ShowSearchSpinner = false;
    private List<string> lstSearch = new();
    private string SearchText = string.Empty;
    private System.Timers.Timer timer = default!;
    private CancellationTokenSource _token;
    private bool ShowSearchCount = false;
    public TGridItem? EditItem = default!;
    private int PageIndex = 1;
    private int PageCount = 1;
    private int StartIndex = 1;
    private bool ShowOverflowForward = false;
    private bool ShowOverflowBack = false;
    private bool ShowNextBtn = false;
    private bool ShowPrvBtn = false;
    private int VisiblePageCnt = 1;
    private int SkipCnt = 0;
    private bool AllowRender = true;
    private bool FilterMode = false;
    protected override bool ShouldRender() => AllowRender;


    /// <summary>
    /// The component auto sets virtualization based off the number of rows in the data set. 
    /// Virtualization comes with a cost. Use it wonly when needed
    /// </summary>
    private bool Virtualize { get; set; }

    /// <summary>
    /// Constructs an instance of <see cref="Grid{TGridItem}"/>.
    /// </summary>
    public Grid()
    {
        _columns = new();
        _internalGridContext = new(this);
        _renderColumnHeaders = RenderColumnHeaders;
        _renderNonVirtualizedRows = RenderNonVirtualizedRows;

        // As a special case, we don't issue the first data load request until we've collected the initial set of columns
        // This is so we can apply default sort order (or any future per-column options) before loading data
        // We use EventCallbackSubscriber to safely hook this async operation into the synchronous rendering flow
        EventCallbackSubscriber<object?> columnsFirstCollectedSubscriber = new(EventCallback.Factory.Create<object?>(this, RefreshDataCoreAsync));
        columnsFirstCollectedSubscriber.SubscribeOrMove(_internalGridContext.ColumnsFirstCollected);
    }

    protected override async Task OnInitializedAsync()
    {
        ItemsState = new();
        _token = new();
        timer = new System.Timers.Timer(250);
        timer.Elapsed += OnTypingDone;
        timer.AutoReset = false;

        await base.OnInitializedAsync();
    }

    private void SetVirtualization()
    {
        if (Debugger.IsAttached)
        {
            if (ItemsFiltered.Count > 20 && !AllowPaging)
                Virtualize = true;
            else
                Virtualize = false;
        }
        else
        {
            if (ItemsFiltered.Count > 100 && !AllowPaging)
                Virtualize = true;
            else
                Virtualize = false;
        }
    }

    /// <inheritdoc />
    protected override Task OnParametersSetAsync()
    {
        if (Searchable && lstSearch.Count == 0 || Searchable && JsonSerializer.Serialize(ItemsState) != JsonSerializer.Serialize(Items))
        {
            ItemsState = new(JsonSerializer.Deserialize<List<TGridItem>>(JsonSerializer.Serialize(Items)));
            lstSearch.Clear();
            //Run this on a separate thread so we don't have to wait for something that's not going to IMMEDIATELY be needed
            //But it will speed up the perceived load time by a bit
            _ = Task.Run(() =>
            {
                Stopwatch sw = Stopwatch.StartNew();
                StringBuilder sbSearch = new();
                Type itemType = typeof(TGridItem);
                for (int i = 0; i < Items.Count; i++)
                    AddToSearch(Items[i], itemType, sbSearch);
                sw.Stop();
                Debug.WriteLine($"Cached Type {sw.ElapsedTicks}");
            });
        }

        if ((!FilterMode && Items.Count != ItemsFiltered.Count) || (ItemsFiltered.Count == 0 && Items.Count > 0))
            ItemsFiltered = new(Items);

        if (Items is not null && ItemsProvider is not null)
            throw new InvalidOperationException($"{nameof(Grid<TGridItem>)} requires one of {nameof(Items)} or {nameof(ItemsProvider)}, but both were specified.");

        if (AllowPaging)
        {
            if (PageSize == 0)
                PageSize = 10;

            ShowOverflowBack = false;
            PageCount = (int)Math.Ceiling(Items.Count / (double)PageSize);
            if (PageCount > 5)
            {
                ShowOverflowForward = true;
                VisiblePageCnt = 5;
            }
            else
            {
                VisiblePageCnt = PageCount;
                ShowOverflowForward = false;
            }
            ShowNextPrv();
        }

        // Perform a re-query only if the data source or something else has changed
        object? _newItemsOrItemsProvider = Items ?? (object?)ItemsProvider;
        bool dataSourceHasChanged = _newItemsOrItemsProvider != _lastAssignedItemsOrProvider;
        if (dataSourceHasChanged)
        {
            _lastAssignedItemsOrProvider = _newItemsOrItemsProvider;
            _asyncQueryExecutor = AsyncQueryExecutorSupplier.GetAsyncQueryExecutor(Services, Items.AsQueryable());
        }

        // We don't want to trigger the first data load until we've collected the initial set of columns,
        // because they might perform some action like setting the default sort order, so it would be wasteful
        // to have to re-query immediately
        return _columns.Count > 0 ? RefreshDataCoreAsync() : Task.CompletedTask;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            DotNetObjectReference<Grid<TGridItem>>? DotNet = DotNetObjectReference.Create(this);
            _jsModule = await Script.InvokeAsync<IJSObjectReference>("import", "./Grid/Grid.razor.js");
            _jsEventDisposable = await _jsModule.InvokeAsync<IJSObjectReference>("init", _gridReference, DotNet);
        }
    }

    private void ResetTimer()
    {
        AllowRender = false;
        timer.Stop();
        timer.Start();
    }

    private async void OnTypingDone(object source, ElapsedEventArgs e)
    {
        FilterMode = true;
        ShowSearchCount = false;
        if (_token != null)
        {
            _token?.Cancel();
            _token?.Dispose();
        }

        _token = new();
        ShowSearchSpinner = false;
        Task task = Task.Run(Filter, _token.Token);

        //give the method a 250 milliseconds to complete before we check. this prevents screen flicker on tasks that complete near instantly
        await Task.Delay(250);
        while (!task.IsCompleted)
        {
            ShowSearchSpinner = true;
            await InvokeAsync(StateHasChanged);
            await Task.Delay(500); //wait another half a second before we check again as to not clog up the stack
        }

        ShowSearchSpinner = false;
        await InvokeAsync(StateHasChanged);

        if (AllowPaging)
        {
            StartIndex = 1;
            PageIndex = 1;
            SkipCnt = 0;
            PageCount = (int)Math.Ceiling(ItemsFiltered.Count / (double)PageSize);
            ShowNextPrv();

            if (PageCount > 5)
            {
                ShowOverflowForward = true;
                ShowOverflowBack = false;
                VisiblePageCnt = 5;
            }
            else
            {
                VisiblePageCnt = PageCount;
                ShowOverflowForward = false;
                ShowOverflowBack = false;
            }
        }
        ShowSearchCount = true;
        AllowRender = true;
        await InvokeAsync(StateHasChanged);
    }

    private async Task Filter()
    {
        EditIndex = -1;
        if (SearchText.IsNullOrEmpty())
        {
            ItemsFiltered = new(Items);
            FilterMode = false;
        }
        else
        {
            ItemsFiltered = new(
                lstSearch.Where(x => x.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                         .Select(x => Items.ElementAt(lstSearch.IndexOf(x)))
            );
        }

        await SortByColumnAsync(_sortByColumn, _lastDirection, true);
    }

    private void AddToSearch(TGridItem item, Type type, StringBuilder sb)
    {
        PropertyInfo[] props = type.GetProperties();
        int length = props.Length;
        int i = 0;
        while(i < length)
        {
            sb.Append(props[i].GetValue(item, null));
            i++;
        }
        lstSearch.Add(sb.ToString());
        sb.Clear();
    }

    // Invoked by descendant columns at a special time during rendering
    internal void AddColumn(ColumnBase<TGridItem> column, SortDirection? isDefaultSortDirection)
    {
        if (_collectingColumns)
        {
            _columns.Add(column);
            if (_sortByColumn is null && isDefaultSortDirection.HasValue)
            {
                _sortByColumn = column;
                _sortByAscending = isDefaultSortDirection.Value != SortDirection.Descending;
            }
        }
    }

    private void StartCollectingColumns()
    {
        _columns.Clear();
        _collectingColumns = true;
    }

    private void FinishCollectingColumns()
    {
        _collectingColumns = false;
    }

    /// <summary>
    /// Sets the grid's current sort column to the specified <paramref name="column"/>.
    /// </summary>
    /// <param name="column">The column that defines the new sort order.</param>
    /// <param name="direction">The direction of sorting. If the value is <see cref="SortDirection.Auto"/>, then it will toggle the direction on each call.</param>
    /// <returns>A <see cref="Task"/> representing the completion of the operation.</returns>
    public Task SortByColumnAsync(ColumnBase<TGridItem> column, SortDirection direction = SortDirection.Auto, bool isOverride = false)
    {
        _sortByColumn = column;
        if (!isOverride)
        {
            _sortByAscending = direction switch
            {
                SortDirection.Ascending => true,
                SortDirection.Descending => false,
                SortDirection.Auto => _sortByColumn != column || !_sortByAscending,
                _ => throw new NotSupportedException($"Unknown sort direction {direction}"),
            };
            _lastDirection = _sortByAscending ? SortDirection.Ascending : SortDirection.Descending;

            StateHasChanged(); // We want to see the updated sort order in the header, even before the data query is completed
        }
        else
            _sortByAscending = _lastDirection == SortDirection.Ascending;
        return RefreshDataAsync();
    }

    /// <summary>
    /// Instructs the grid to re-fetch and render the current data from the supplied data source
    /// (either <see cref="Items"/> or <see cref="ItemsProvider"/>).
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents the completion of the operation.</returns>
    public async Task RefreshDataAsync()
    {
        await RefreshDataCoreAsync();
        StateHasChanged();
    }

    // Same as RefreshDataAsync, except without forcing a re-render. We use this from OnParametersSetAsync
    // because in that case there's going to be a re-render anyway.
    private async Task RefreshDataCoreAsync()
    {
        // Move into a "loading" state, cancelling any earlier-but-still-pending load
        _pendingDataLoadCancellationTokenSource?.Cancel();
        CancellationTokenSource thisLoadCts = _pendingDataLoadCancellationTokenSource = new();

        //if (_virtualizeComponent is not null)
        //{
        // If we're using Virtualize, we have to go through its RefreshDataAsync API otherwise:
        // (1) It won't know to update its own internal state if the provider output has changed
        // (2) We won't know what slice of data to query for
        //await _virtualizeComponent.RefreshDataAsync();
        //_pendingDataLoadCancellationTokenSource = null;
        //}
        //else
        //{
        // If we're not using Virtualize, we build and execute a request against the items provider directly
        GridItemsProviderRequest<TGridItem> request = new(0, null, _sortByColumn, _sortByAscending, thisLoadCts.Token);
        GridItemsProviderResult<TGridItem> result = await ResolveItemsRequestAsync(request);
        if (!thisLoadCts.IsCancellationRequested)
        {
            ItemsFiltered = new(result.Items);
            _pendingDataLoadCancellationTokenSource = null;
        }
        //}

        SetVirtualization();
    }

    // Normalizes all the different ways of configuring a data source so they have common GridItemsProvider-shaped API
    private async ValueTask<GridItemsProviderResult<TGridItem>> ResolveItemsRequestAsync(GridItemsProviderRequest<TGridItem> request)
    {
        if (ItemsProvider is not null)
            return await ItemsProvider(request);
        else if (ItemsFiltered is not null)
        {
            IQueryable<TGridItem> result = request.ApplySorting(ItemsFiltered.AsQueryable());
            if (request.Count.HasValue)
                result = result.Take(request.Count.Value);
            TGridItem[] resultArray = _asyncQueryExecutor is null ? result.ToArray() : await _asyncQueryExecutor.ToArrayAsync(result);
            return GridItemsProviderResult.From(resultArray, resultArray.Length);
        }
        else
            return GridItemsProviderResult.From(Array.Empty<TGridItem>(), 0);
    }

    private string AriaSortValue(ColumnBase<TGridItem> column) => _sortByColumn == column ? (_sortByAscending ? "ascending" : "descending") : "none";

    private string? ColumnHeaderClass(ColumnBase<TGridItem> column) => _sortByColumn == column ? $"{ColumnClass(column)} {(_sortByAscending ? "col-sort-asc" : "col-sort-desc")}" : ColumnClass(column);

    private static string? ColumnHeaderTooltip(ColumnBase<TGridItem> column) => column.HeaderTooltip;

    private bool ShowHeaderAddButton() => ShowAddButton;

    private async Task HeaderAddButton_Clicked(ColumnBase<TGridItem> column)
    {
        if (column is null)
            throw new ArgumentNullException($"Column is null {nameof(column)}");

        if (AddRecord_Click.HasDelegate)
            await AddRecord_Click.InvokeAsync();
        else
            throw new ArgumentNullException($"AddRecord_CLick has no matching delegate which accepts a parameter of type '{typeof(TGridItem).Name}'");
    }

    private string GridClass() => $"tblComponent {Class} {(_pendingDataLoadCancellationTokenSource is null ? null : "loading")}";

    private static string? ColumnClass(ColumnBase<TGridItem> column) => column.Class;

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_jsEventDisposable is not null)
            {
                await _jsEventDisposable.InvokeVoidAsync("stop");
                await _jsEventDisposable.DisposeAsync();
            }

            if (_jsModule is not null)
            {
                await _jsModule.DisposeAsync();
            }
        }
        catch (JSDisconnectedException)
        {
            // The JS side may routinely be gone already if the reason we're disposing is that
            // the client disconnected. This is not an error.
        }
    }

    private void PageIndex_Changed(int ndx)
    {
        ShowOverflowBack = false;
        ShowOverflowForward = false;
        switch (ndx)
        {
            case -4://it's time to go to the previous set of page numbers in the list
                StartIndex -= VisiblePageCnt;
                PageIndex = StartIndex;
                if (PageIndex <= 1)
                {
                    PageIndex = 1;
                    SkipCnt = 0;
                }
                else
                    SkipCnt = (PageIndex - 1) * PageSize;
                break;
            case -3: //it's time to go to the next set of page numbers in the list
                StartIndex += VisiblePageCnt;
                PageIndex = StartIndex;
                SkipCnt = (StartIndex - 1) * PageSize;
                break;
            case -2: //go back a page
                if ((PageIndex - 1) % VisiblePageCnt == 0)
                    StartIndex = PageIndex - VisiblePageCnt;
                PageIndex--;
                if (PageIndex <= 1)
                {
                    PageIndex = 1;
                    SkipCnt = 0;
                    ShowPrvBtn = false;
                }
                else
                    SkipCnt = (PageIndex - 1) * PageSize;
                break;
            case -1: //go forward a page
                if (PageIndex % VisiblePageCnt == 0)
                    StartIndex = PageIndex + 1;
                PageIndex++;
                if (PageIndex >= PageCount)
                {
                    PageIndex = PageCount;
                    ShowNextBtn = false;
                }
                SkipCnt = (PageIndex - 1) * PageSize;
                break;
            default: //go to whatever page is selected
                PageIndex = ndx;
                SkipCnt = (PageIndex - 1) * PageSize;
                break;
        }


        if (StartIndex + VisiblePageCnt < PageCount)
            ShowOverflowForward = true;

        if (PageIndex > VisiblePageCnt && VisiblePageCnt < PageCount)
            ShowOverflowBack = true;

        if (VisiblePageCnt <= PageCount && StartIndex <= VisiblePageCnt)
            StartIndex = 1;

        ShowNextPrv();
    }

    private void PageSize_Changed(ChangeEventArgs e)
    {
        StartIndex = 1;
        PageIndex = 1;
        SkipCnt = 0;
        PageSize = Convert.ToInt32(e.Value);
        PageCount = (int)Math.Ceiling(ItemsFiltered.Count / (double)PageSize);
        ShowNextPrv();

        if (PageCount > 5)
        {
            ShowOverflowForward = true;
            ShowOverflowBack = false;
            VisiblePageCnt = 5;
        }
        else
        {
            VisiblePageCnt = PageCount;
            ShowOverflowForward = false;
            ShowOverflowBack = false;
        }
    }

    private void ShowNextPrv()
    {
        if (PageCount > 1)
        {
            if (PageIndex < PageCount)
                ShowNextBtn = true;
            if (PageIndex > 1)
                ShowPrvBtn = true;
        }
        else
        {
            ShowNextBtn = false;
            ShowPrvBtn = false;
        }
    }
}
