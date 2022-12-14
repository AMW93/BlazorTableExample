// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace BlazorTableExample;

// The grid cascades this so that descendant columns can talk back to it. It's an internal type
// so that it doesn't show up by mistake in unrelated components.
internal class InternalGridContext<TGridItem>
{
    public Grid<TGridItem> Grid { get; }
    public EventCallbackSubscribable<object?> ColumnsFirstCollected { get; } = new();

    public InternalGridContext(Grid<TGridItem> grid)
    {
        Grid = grid;
    }
}
