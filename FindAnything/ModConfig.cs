using LeFauxMods.Common.Interface;
using LeFauxMods.Common.Models;
using StardewModdingAPI.Utilities;

namespace LeFauxMods.FindAnything;

/// <summary>Represents the mod's configuration.</summary>
internal sealed class ModConfig : IConfigWithLogAmount
{
    /// <summary>Gets or sets a value indicating whether to clear search every time.</summary>
    public bool ClearBeforeSearch { get; set; }

    /// <inheritdoc />
    public LogAmount LogAmount { get; set; }

    /// <summary>Gets or sets the keybind to show the search menu.</summary>
    public KeybindList ShowSearch { get; set; } =
        new(new Keybind(SButton.LeftControl, SButton.F),
            new Keybind(SButton.RightControl, SButton.F));

    /// <summary>Gets or sets the keybind for toggling pointers on or off.</summary>
    public KeybindList ToggleVisible { get; set; } =
        new(new Keybind(SButton.LeftControl, SButton.Space),
            new Keybind(SButton.RightControl, SButton.Space));

    /// <summary>Gets or sets a value indicating whether icons should be visible.</summary>
    public bool Visible { get; set; } = true;

    /// <summary>
    ///     Copies the values from another instance of <see cref="ModConfig" />.
    /// </summary>
    /// <param name="other">The other config to copy to.</param>
    public void CopyTo(ModConfig other)
    {
        other.ClearBeforeSearch = this.ClearBeforeSearch;
        other.LogAmount = this.LogAmount;
        other.ShowSearch = this.ShowSearch;
        other.ToggleVisible = this.ToggleVisible;
        other.Visible = this.Visible;
    }
}
