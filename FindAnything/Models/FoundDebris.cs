using LeFauxMods.Common.Integrations.FindAnything;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.ItemTypeDefinitions;

namespace LeFauxMods.FindAnything.Models;

/// <inheritdoc />
internal sealed class FoundDebris : IFoundEntity
{
    private readonly WeakReference<Debris> context;

    public FoundDebris(Debris debris, GameLocation location, ParsedItemData? itemData = null)
    {
        this.context = new WeakReference<Debris>(debris);
        this.Location = location;
        this.Offset = new Vector2(Game1.tileSize, 0);

        if (itemData is null)
        {
            return;
        }

        this.Texture = itemData.GetTexture();
        this.SourceRectangle = itemData.GetSourceRect();
    }

    /// <inheritdoc />
    public object? Context => this.context.TryGetTarget(out var debris) ? debris : null;

    /// <inheritdoc />
    public GameLocation? Location { get; }

    /// <inheritdoc />
    public Vector2 Offset { get; }

    /// <inheritdoc />
    public Rectangle? SourceRectangle { get; }

    /// <inheritdoc />
    public Texture2D? Texture { get; }

    /// <inheritdoc />
    public Vector2 Tile =>
        this.context.TryGetTarget(out var debris)
            ? debris.Chunks.FirstOrDefault()?.GetVisualPosition() / Game1.tileSize ?? Vector2.Zero
            : Vector2.Zero;
}