using LeFauxMods.Common.Integrations.FindAnything;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace LeFauxMods.FindAnything.Services;

/// <summary>Represents a hud for displaying pointers to searched entities.</summary>
internal sealed class Pointers : IClickableMenu
{
    private readonly ModConfig config;

    public Pointers(ModConfig config)
    {
        this.config = config;
        this.allClickableComponents = [];
    }

    /// <inheritdoc />
    public override void draw(SpriteBatch b)
    {
        if (!Game1.IsHudDrawn || !this.config.Visible || this.allClickableComponents.Count == 0)
        {
            return;
        }

        for (var i = this.allClickableComponents.Count - 1; i >= 0; i--)
        {
            if (this.allClickableComponents[i] is not Pointer pointer)
            {
                continue;
            }

            if (pointer.Entity.Location != Game1.currentLocation)
            {
                this.allClickableComponents.Remove(pointer);
                continue;
            }

            pointer.draw(b);
        }
    }

    public void UpdateResults(List<IFoundEntity> results)
    {
        this.allClickableComponents.Clear();
        foreach (var result in results)
        {
            this.allClickableComponents.Add(new Pointer(result));
        }
    }
}
