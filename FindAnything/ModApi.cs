using LeFauxMods.Common.Integrations.FindAnything;
using LeFauxMods.Common.Services;
using LeFauxMods.Common.Utilities;
using LeFauxMods.FindAnything.Models;

namespace LeFauxMods.FindAnything;

/// <inheritdoc />
public sealed class ModApi : IFindAnythingApi
{
    private readonly EventManager eventManager = new();

    /// <summary>Initializes a new instance of the <see cref="ModApi" /> class.</summary>
    /// <param name="mod">The mod's information.</param>
    public ModApi(IModInfo mod)
    {
        ModEvents.Subscribe<SearchUpdated>(this.OnSearchUpdated);
        ModEvents.Subscribe<SearchSubmitted>(this.OnSearchSubmitted);
    }

    /// <inheritdoc />
    public void Subscribe(Action<ISearchUpdated> handler) => this.eventManager.Subscribe(handler);

    /// <inheritdoc />
    public void Subscribe(Action<ISearchSubmitted> handler) => this.eventManager.Subscribe(handler);

    /// <inheritdoc />
    public void Unsubscribe(Action<ISearchUpdated> handler) => this.eventManager.Unsubscribe(handler);

    /// <inheritdoc />
    public void Unsubscribe(Action<ISearchSubmitted> handler) => this.eventManager.Unsubscribe(handler);

    private void OnSearchSubmitted(ISearchSubmitted e) => this.eventManager.Publish(e);

    private void OnSearchUpdated(ISearchUpdated e) => this.eventManager.Publish(e);
}