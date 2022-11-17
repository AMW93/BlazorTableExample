namespace BlazorTableExample;
public partial class StringShrink : ComponentBase
{
    [Inject] public IJSRuntime Script { get; set; }
    [Parameter] public string Text { get; set; }
    [Parameter] public int MaxCharacters { get; set; } = 200;
    public bool IsExpanded { get; set; }
    public bool CanBeExpanded => Text.Length > MaxCharacters;

    private ElementReference _ss;

    public string GetDisplayText() => IsExpanded ? Text : Truncate(Text, MaxCharacters);

    public string Truncate(string value, int maxChars) => value.Length <= maxChars ? value : value.Substring(0, maxChars) + "...";
}
