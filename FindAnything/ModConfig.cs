using System.Globalization;
using System.Text;
using LeFauxMods.Common.Interface;
using LeFauxMods.Common.Models;
using StardewModdingAPI.Utilities;

namespace LeFauxMods.FindAnything;

/// <inheritdoc cref="IModConfig{TConfig}" />
internal sealed class ModConfig : IModConfig<ModConfig>, IConfigWithLogAmount
{
    /// <summary>Gets or sets a value indicating whether to clear search every time.</summary>
    public bool ClearBeforeSearch { get; set; }

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

    /// <inheritdoc />
    public LogAmount LogAmount { get; set; }

    /// <inheritdoc />
    public void CopyTo(ModConfig other)
    {
        other.ClearBeforeSearch = this.ClearBeforeSearch;
        other.LogAmount = this.LogAmount;
        other.ShowSearch = this.ShowSearch;
        other.ToggleVisible = this.ToggleVisible;
        other.Visible = this.Visible;
    }

    /// <inheritdoc />
    public string GetSummary() =>
        new StringBuilder()
            .AppendLine(CultureInfo.InvariantCulture, $"{nameof(this.ClearBeforeSearch),25}: {this.ClearBeforeSearch}")
            .AppendLine(CultureInfo.InvariantCulture, $"{nameof(this.ShowSearch),25}: {this.ShowSearch}")
            .AppendLine(CultureInfo.InvariantCulture, $"{nameof(this.ToggleVisible),25}: {this.ToggleVisible}")
            .AppendLine(CultureInfo.InvariantCulture, $"{nameof(this.Visible),25}: {this.Visible}")
            .ToString();
}