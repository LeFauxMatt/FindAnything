using LeFauxMods.Common.Integrations.FindAnything;
using LeFauxMods.Common.Utilities;
using LeFauxMods.FindAnything.Models;
using LeFauxMods.FindAnything.Modules;
using LeFauxMods.FindAnything.Services;
using StardewModdingAPI.Events;
using StardewValley.TokenizableStrings;

namespace LeFauxMods.FindAnything;

/// <inheritdoc />
internal sealed class ModEntry : Mod
{
    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        // Init
        I18n.Init(helper.Translation);
        ModState.Init(helper);
        Log.Init(this.Monitor, ModState.Config);

        // Events
        helper.Events.Display.MenuChanged += OnMenuChanged;
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
        helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        helper.Events.Player.Warped += OnWarped;
        helper.Events.Input.ButtonsChanged += this.OnButtonsChanged;
    }

    /// <inheritdoc />
    public override object GetApi(IModInfo mod) => new ModApi(mod);

    private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (!Context.IsPlayerFree)
        {
            return;
        }

        if (ModState.Config.ShowSearch.JustPressed())
        {
            this.Helper.Input.SuppressActiveKeybinds(ModState.Config.ShowSearch);
            if (ModState.Config.ClearBeforeSearch)
            {
                ModState.SearchText = string.Empty;
            }

            Game1.activeClickableMenu = new SearchMenu(this.Helper);
            return;
        }

        if (ModState.Config.ToggleVisible.JustPressed())
        {
            this.Helper.Input.SuppressActiveKeybinds(ModState.Config.ToggleVisible);
            ModState.Config.Visible = !ModState.Config.Visible;
        }
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        var mod = this.Helper.ModRegistry.Get(this.ModManifest.UniqueID)!;
        var api = new ModApi(mod);

        // Built in modules
        _ = new FindInStorage(api);
        _ = new FindInLocation(api);
        _ = new FindInTerrain(api);

        api.Subscribe(OnSearchUpdated);
        _ = new ConfigMenu(this.Helper, this.ModManifest);
    }

    private static void OnMenuChanged(object? sender, MenuChangedEventArgs e)
    {
        if (e.OldMenu is not SearchMenu || e.NewMenu is not null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(ModState.SearchText))
        {
            ModState.Pointers?.UpdateResults([]);
            return;
        }

        Log.Trace("Searching for: {0}", ModState.SearchText);
        var results = new List<IFoundEntity>();
        ModEvents.Publish(new SearchSubmitted(ModState.SearchText, Game1.currentLocation, results.Add));
        ModState.Pointers?.UpdateResults(results);
    }

    private static void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e) =>
        Game1.onScreenMenus.Remove(ModState.Pointers);

    private static void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        ModState.Pointers = new Pointers();
        Game1.onScreenMenus.Add(ModState.Pointers);
    }

    private static void OnSearchUpdated(ISearchUpdated e)
    {
        // Initialize keywords
        if (ModState.CachedKeywords.Count == 0)
        {
            // Add big craftable display names
            ModState.CachedKeywords.UnionWith(
                Game1.bigCraftableData.Values.Select(static bigCraftableData =>
                    TokenParser.ParseText(bigCraftableData.DisplayName)));

            // Add building names
            ModState.CachedKeywords.UnionWith(
                Game1.buildingData.Values.Select(static buildingData => TokenParser.ParseText(buildingData.Name)));

            // Add character display names
            ModState.CachedKeywords.UnionWith(
                Game1.characterData.Values.Select(static characterData =>
                    TokenParser.ParseText(characterData.DisplayName)));

            // Add farm animal display names
            ModState.CachedKeywords.UnionWith(Game1.farmAnimalData.Values.Select(static farmAnimalData =>
                TokenParser.ParseText(farmAnimalData.DisplayName)));

            // Add fruit tree display names
            ModState.CachedKeywords.UnionWith(Game1.fruitTreeData.Values.Select(static fruitTreeData =>
                TokenParser.ParseText(fruitTreeData.DisplayName)));

            // Add object display names
            ModState.CachedKeywords.UnionWith(
                Game1.objectData.Values.Select(static objectData => TokenParser.ParseText(objectData.DisplayName)));
        }

        var keywords =
            ModState.CachedKeywords.Where(keyword => keyword.Contains(e.Text, StringComparison.OrdinalIgnoreCase))
                .ToList();

        if (keywords.Count > 0)
        {
            e.AddKeywords(keywords);
        }
    }

    private static void OnWarped(object? sender, WarpedEventArgs e) => ModState.Pointers?.UpdateResults([]);
}