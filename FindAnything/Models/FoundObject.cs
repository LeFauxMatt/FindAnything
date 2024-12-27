using LeFauxMods.Common.Integrations.FindAnything;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.ItemTypeDefinitions;

namespace LeFauxMods.FindAnything.Models;

/// <summary>Represents a found placed object in a location.</summary>
internal sealed class FoundObject : IFoundEntity
{
    private readonly WeakReference<SObject> context;

    /// <summary>Initializes a new instance of the <see cref="FoundObject" /> class.</summary>
    /// <param name="object">The object.</param>
    /// <param name="itemData">An optional item to draw as the icon.</param>
    public FoundObject(SObject @object, ParsedItemData? itemData = null)
    {
        this.context = new WeakReference<SObject>(@object);
        this.Offset = new Vector2(32f, -32f);

        var iconData = itemData ?? ItemRegistry.GetDataOrErrorItem(@object.QualifiedItemId);
        this.Texture = iconData.GetTexture();
        this.SourceRectangle = iconData.GetSourceRect();
    }

    /// <inheritdoc />
    public object? Context => this.context.TryGetTarget(out var @object) ? @object : null;

    /// <inheritdoc />
    public GameLocation? Location => this.context.TryGetTarget(out var @object) ? @object.Location : null;

    /// <inheritdoc />
    public Vector2 Offset { get; }

    /// <inheritdoc />
    public Rectangle? SourceRectangle { get; }

    /// <inheritdoc />
    public Texture2D? Texture { get; }

    /// <inheritdoc />
    public Vector2 Tile => this.context.TryGetTarget(out var @object) ? @object.TileLocation : Vector2.Zero;
}
