using LeFauxMods.Common.Integrations.FindAnything;

namespace LeFauxMods.FindAnything.Models;

/// <inheritdoc />
internal sealed class SearchSubmitted(string text, GameLocation location, Action<IFoundEntity> addResult)
    : ISearchSubmitted
{
    /// <inheritdoc />
    public GameLocation Location { get; } = location;

    /// <inheritdoc />
    public string Text { get; } = text;

    /// <inheritdoc />
    public void AddResult(IFoundEntity result) => addResult(result);
}