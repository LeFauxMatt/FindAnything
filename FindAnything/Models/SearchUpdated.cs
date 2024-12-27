using LeFauxMods.Common.Integrations.FindAnything;

namespace LeFauxMods.FindAnything.Models;

/// <inheritdoc />
internal sealed class SearchUpdated(string text, Action<IEnumerable<string>> addKeywords) : ISearchUpdated
{
    /// <inheritdoc />
    public string Text { get; } = text;

    /// <inheritdoc />
    public void AddKeywords(IEnumerable<string> keywords) => addKeywords(keywords);
}
