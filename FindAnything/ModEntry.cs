using LeFauxMods.Common.Integrations.FindAnything;
using LeFauxMods.Common.Integrations.GenericModConfigMenu;
using LeFauxMods.Common.Services;
using LeFauxMods.Common.Utilities;
using LeFauxMods.FindAnything.Models;
using LeFauxMods.FindAnything.Modules;
using LeFauxMods.FindAnything.Services;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley.TokenizableStrings;

namespace LeFauxMods.FindAnything;

/// <inheritdoc />
internal sealed class ModEntry : Mod
{
    private readonly HashSet<string> cachedKeywords = new(StringComparer.OrdinalIgnoreCase);
    private readonly PerScreen<string> searchText = new(() => string.Empty);
    private ModConfig config = null!;
    private ConfigHelper<ModConfig> configHelper = null!;

    private Pointers? pointers;

    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        // Init
        I18n.Init(helper.Translation);
        this.configHelper = new ConfigHelper<ModConfig>(this.Helper);
        this.config = this.configHelper.Load();
        Log.Init(this.Monitor, this.config);

        // Events
        helper.Events.Display.MenuChanged += this.OnMenuChanged;
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.GameLoop.ReturnedToTitle += this.OnReturnedToTitle;
        helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
        helper.Events.Player.Warped += this.OnWarped;
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

        if (this.config.ShowSearch.JustPressed())
        {
            this.Helper.Input.SuppressActiveKeybinds(this.config.ShowSearch);
            if (this.config.ClearBeforeSearch)
            {
                this.searchText.Value = string.Empty;
            }

            Game1.activeClickableMenu = new SearchMenu(this.Helper, () => this.searchText.Value,
                value => this.searchText.Value = value);

            return;
        }

        if (this.config.ToggleVisible.JustPressed())
        {
            this.Helper.Input.SuppressActiveKeybinds(this.config.ToggleVisible);
            this.config.Visible = !this.config.Visible;
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

        api.Subscribe(this.OnSearchUpdated);

        var gmcm = new GenericModConfigMenuIntegration(this.ModManifest, this.Helper.ModRegistry);
        if (!gmcm.IsLoaded)
        {
            return;
        }

        var defaultConfig = new ModConfig();
        var tempConfig = this.configHelper.Load();

        gmcm.Register(
            () => defaultConfig.CopyTo(tempConfig),
            () =>
            {
                tempConfig.Visible = this.config.Visible;
                tempConfig.CopyTo(this.config);
                this.configHelper.Save(tempConfig);
            });

        gmcm.Api.AddBoolOption(
            this.ModManifest,
            () => tempConfig.ClearBeforeSearch,
            value => tempConfig.ClearBeforeSearch = value,
            I18n.ConfigOption_ClearBeforeSearch_Name,
            I18n.ConfigOption_ClearBeforeSearch_Description);

        gmcm.Api.AddKeybindList(
            this.ModManifest,
            () => tempConfig.ShowSearch,
            value => tempConfig.ShowSearch = value,
            I18n.ConfigOption_ShowSearch_Name,
            I18n.ConfigOption_ShowSearch_Description);

        gmcm.Api.AddKeybindList(
            this.ModManifest,
            () => tempConfig.ToggleVisible,
            value => tempConfig.ToggleVisible = value,
            I18n.ConfigOption_ToggleVisible_Name,
            I18n.ConfigOption_ToggleVisible_Descriptoin);
    }

    private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
    {
        if (e.OldMenu is not SearchMenu || e.NewMenu is not null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(this.searchText.Value))
        {
            this.pointers?.UpdateResults([]);
            return;
        }

        Log.Trace("Searching for: {0}", this.searchText.Value);
        var results = new List<IFoundEntity>();
        ModEvents.Publish(new SearchSubmitted(this.searchText.Value, Game1.currentLocation, results.Add));
        this.pointers?.UpdateResults(results);
    }

    private void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e) =>
        Game1.onScreenMenus.Remove(this.pointers);

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        this.pointers = new Pointers(this.config);
        Game1.onScreenMenus.Add(this.pointers);
    }

    private void OnSearchUpdated(ISearchUpdated e)
    {
        // Initialize keywords
        if (this.cachedKeywords.Count == 0)
        {
            // Add big craftable display names
            this.cachedKeywords.UnionWith(
                Game1.bigCraftableData.Values.Select(bigCraftableData =>
                    TokenParser.ParseText(bigCraftableData.DisplayName)));

            // Add building names
            this.cachedKeywords.UnionWith(
                Game1.buildingData.Values.Select(buildingData => TokenParser.ParseText(buildingData.Name)));

            // Add character display names
            this.cachedKeywords.UnionWith(
                Game1.characterData.Values.Select(characterData => TokenParser.ParseText(characterData.DisplayName)));

            // Add farm animal display names
            this.cachedKeywords.UnionWith(Game1.farmAnimalData.Values.Select(farmAnimalData =>
                TokenParser.ParseText(farmAnimalData.DisplayName)));

            // Add fruit tree display names
            this.cachedKeywords.UnionWith(Game1.fruitTreeData.Values.Select(fruitTreeData =>
                TokenParser.ParseText(fruitTreeData.DisplayName)));

            // Add object display names
            this.cachedKeywords.UnionWith(
                Game1.objectData.Values.Select(objectData => TokenParser.ParseText(objectData.DisplayName)));
        }

        var keywords =
            this.cachedKeywords.Where(keyword => keyword.Contains(e.Text, StringComparison.OrdinalIgnoreCase)).ToList();

        if (keywords.Count > 0)
        {
            e.AddKeywords(keywords);
        }
    }

    private void OnWarped(object? sender, WarpedEventArgs e) => this.pointers?.UpdateResults([]);
}
