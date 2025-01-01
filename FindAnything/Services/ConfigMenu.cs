using LeFauxMods.Common.Integrations.GenericModConfigMenu;
using LeFauxMods.Common.Services;

namespace LeFauxMods.FindAnything.Services;

/// <summary>Responsible for handling the mod configuration menu.</summary>
internal sealed class ConfigMenu
{
    private readonly IGenericModConfigMenuApi api = null!;
    private readonly GenericModConfigMenuIntegration gmcm;
    private readonly IManifest manifest;

    public ConfigMenu(IModHelper helper, IManifest manifest)
    {
        this.manifest = manifest;
        this.gmcm = new GenericModConfigMenuIntegration(manifest, helper.ModRegistry);
        if (!this.gmcm.IsLoaded)
        {
            return;
        }

        this.api = this.gmcm.Api;
        this.SetupMenu();
    }

    private static ModConfig Config => ModState.ConfigHelper.Temp;

    private static ConfigHelper<ModConfig> ConfigHelper => ModState.ConfigHelper;

    private void SetupMenu()
    {
        this.gmcm.Register(ConfigHelper.Reset, ConfigHelper.Save);

        this.api.AddBoolOption(
            this.manifest,
            static () => Config.ClearBeforeSearch,
            static value => Config.ClearBeforeSearch = value,
            I18n.ConfigOption_ClearBeforeSearch_Name,
            I18n.ConfigOption_ClearBeforeSearch_Description);

        this.api.AddKeybindList(
            this.manifest,
            static () => Config.ShowSearch,
            static value => Config.ShowSearch = value,
            I18n.ConfigOption_ShowSearch_Name,
            I18n.ConfigOption_ShowSearch_Description);

        this.api.AddKeybindList(
            this.manifest,
            static () => Config.ToggleVisible,
            static value => Config.ToggleVisible = value,
            I18n.ConfigOption_ToggleVisible_Name,
            I18n.ConfigOption_ToggleVisible_Description);
    }
}