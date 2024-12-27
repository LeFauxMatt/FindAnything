using LeFauxMods.Common.Integrations.FindAnything;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.ItemTypeDefinitions;
using StardewValley.TerrainFeatures;

namespace LeFauxMods.FindAnything.Models;

/// <summary>Represents a found terrain in a location.</summary>
internal sealed class FoundTerrain : IFoundEntity
{
    private readonly WeakReference<TerrainFeature> context;

    /// <summary>Initializes a new instance of the <see cref="FoundTerrain" /> class.</summary>
    /// <param name="terrain">The terrain.</param>
    /// <param name="itemData">An optional item to draw as the icon.</param>
    public FoundTerrain(TerrainFeature terrain, ParsedItemData? itemData = null)
    {
        this.context = new WeakReference<TerrainFeature>(terrain);
        this.Offset = new Vector2(0, -Game1.tileSize);

        if (itemData is null)
        {
            return;
        }

        this.Texture = itemData.GetTexture();
        this.SourceRectangle = itemData.GetSourceRect();
    }

    /// <inheritdoc />
    public object? Context => this.context.TryGetTarget(out var terrainFeature) ? terrainFeature : null;

    /// <inheritdoc />
    public GameLocation? Location => this.context.TryGetTarget(out var terrainFeature) ? terrainFeature.Location : null;

    /// <inheritdoc />
    public Vector2 Offset { get; }

    /// <inheritdoc />
    public Rectangle? SourceRectangle { get; }

    /// <inheritdoc />
    public Texture2D? Texture { get; }

    /// <inheritdoc />
    public Vector2 Tile => this.context.TryGetTarget(out var terrainFeature) ? terrainFeature.Tile : Vector2.Zero;
}
