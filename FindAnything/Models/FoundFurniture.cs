using LeFauxMods.Common.Integrations.FindAnything;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Objects;

namespace LeFauxMods.FindAnything.Models;

/// <inheritdoc />
internal sealed class FoundFurniture : IFoundEntity
{
    private readonly WeakReference<Furniture> context;

    /// <summary>Initializes a new instance of the <see cref="FoundFurniture" /> class.</summary>
    /// <param name="furniture">The furniture.</param>
    /// <param name="itemData">An optional item to draw as the icon.</param>
    public FoundFurniture(Furniture furniture, ParsedItemData? itemData = null)
    {
        this.context = new WeakReference<Furniture>(furniture);
        this.Offset = new Vector2((Game1.tileSize * furniture.getTilesWide() / 2f) - 32f,
            -Game1.tileSize * furniture.getTilesHigh());

        var iconData = itemData ?? ItemRegistry.GetDataOrErrorItem(furniture.QualifiedItemId);
        this.Texture = iconData.GetTexture();
        this.SourceRectangle = iconData.GetSourceRect();
    }

    /// <inheritdoc />
    public object? Context => this.context.TryGetTarget(out var furniture) ? furniture : null;

    /// <inheritdoc />
    public GameLocation? Location => this.context.TryGetTarget(out var furniture) ? furniture.Location : null;

    /// <inheritdoc />
    public Vector2 Offset { get; }

    /// <inheritdoc />
    public Rectangle? SourceRectangle { get; }

    /// <inheritdoc />
    public Texture2D? Texture { get; }

    /// <inheritdoc />
    public Vector2 Tile => this.context.TryGetTarget(out var furniture) ? furniture.TileLocation : Vector2.Zero;
}