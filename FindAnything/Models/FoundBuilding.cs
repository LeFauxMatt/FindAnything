using LeFauxMods.Common.Integrations.FindAnything;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Buildings;
using StardewValley.ItemTypeDefinitions;

namespace LeFauxMods.FindAnything.Models;

/// <inheritdoc />
internal sealed class FoundBuilding : IFoundEntity
{
    private readonly WeakReference<Building> context;

    /// <summary>Initializes a new instance of the <see cref="FoundBuilding" /> class.</summary>
    /// <param name="building">The building.</param>
    /// <param name="itemData">An optional item to draw as the icon.</param>
    public FoundBuilding(Building building, ParsedItemData? itemData = null)
    {
        this.context = new WeakReference<Building>(building);
        this.Offset = Game1.tileSize * new Vector2(building.tilesWide.Value / 2f, -building.tilesHigh.Value / 2f);

        if (itemData is null)
        {
            return;
        }

        this.Texture = itemData.GetTexture();
        this.SourceRectangle = itemData.GetSourceRect();
    }

    /// <inheritdoc />
    public object? Context => this.context.TryGetTarget(out var building) ? building : null;

    /// <inheritdoc />
    public GameLocation? Location => this.context.TryGetTarget(out var building) ? building.GetParentLocation() : null;

    /// <inheritdoc />
    public Vector2 Offset { get; }

    /// <inheritdoc />
    public Rectangle? SourceRectangle { get; }

    /// <inheritdoc />
    public Texture2D? Texture { get; }

    /// <inheritdoc />
    public Vector2 Tile =>
        this.context.TryGetTarget(out var building)
            ? new Vector2(building.tileX.Value, building.tileY.Value)
            : Vector2.Zero;
}