// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Components.Rendering;
using System.Linq.Expressions;

namespace BlazorTableExample;

/// <summary>
/// Represents a <see cref="Grid{TGridItem}"/> column whose cells render a supplied template.
/// </summary>
/// <typeparam name="TGridItem">The type of data represented by each row in the grid.</typeparam>
public class TemplateField<TGridItem> : ColumnBase<TGridItem>, ISortBuilderColumn<TGridItem>
{
    private readonly static RenderFragment<TGridItem> EmptyChildContent = _ => builder => { };
    [Parameter] public RenderFragment<TGridItem>? ItemTemplate { get; set; } = EmptyChildContent;
    [Parameter] public RenderFragment<TGridItem>? EditItemTemplate { get; set; }

    /// <summary>
    /// Optionally specifies sorting rules for this column.
    /// </summary>
    [Parameter] public Expression<Func<TGridItem, object>>? SortBy { get; set; }
    private GridSort<TGridItem>? SortRule { get; set; }

    GridSort<TGridItem>? ISortBuilderColumn<TGridItem>.SortBuilder => SortRule;

    protected override Task OnInitializedAsync()
    {
        if (SortBy is not null)
            SortRule = GridSort<TGridItem>.ByDescending(SortBy);

        return base.OnInitializedAsync();
    }

    /// <inheritdoc />
    protected internal override void CellContent(RenderTreeBuilder builder, TGridItem item)
    {
        if (EditItemTemplate != null && Grid.EditIndex > -1 && Grid.EditItem.Equals(item))
            builder.AddContent(0, EditItemTemplate(item));
        else
            builder.AddContent(0, ItemTemplate(item));
    }

    /// <inheritdoc />
    protected override bool IsSortableByDefault()
        => SortBy is not null;

    public string GetName<TSource, TField>(Expression<Func<TSource, TField>> Field)
    {
        if (Equals(Field, null))
            throw new NullReferenceException("Field is required");

        MemberExpression expr;
        if (Field.Body is MemberExpression)
            expr = (MemberExpression)Field.Body;
        else if (Field.Body is UnaryExpression)
            expr = (MemberExpression)((UnaryExpression)Field.Body).Operand;
        else
            throw new ArgumentException($"Expression '{Field}' not supported.", "Field");

        return expr.Member.Name;
    }
}
