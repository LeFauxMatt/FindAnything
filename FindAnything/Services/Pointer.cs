using LeFauxMods.Common.Integrations.FindAnything;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace LeFauxMods.FindAnything.Services;

/// <summary>Represents a component that points to a searched entity.</summary>
internal sealed class Pointer : ClickableTextureComponent
{
    private readonly Rectangle iconRect;
    private readonly float iconScale;

    /// <summary>Initializes a new instance of the <see cref="Pointer" /> class.</summary>
    /// <param name="entity">The entity being pointed to.</param>
    public Pointer(IFoundEntity entity) : base(Rectangle.Empty, Game1.mouseCursors, new Rectangle(412, 495, 5, 4),
        Game1.pixelZoom)
    {
        this.Entity = entity;
        this.iconScale = Game1.pixelZoom;
        if (entity.Texture is not null)
        {
            this.iconRect = entity.SourceRectangle ?? new Rectangle(0, 0, entity.Texture.Width, entity.Texture.Height);
            this.iconScale = 24f / Math.Max(this.iconRect.Width, this.iconRect.Height);
        }
    }

    public IFoundEntity Entity { get; }

    /// <inheritdoc />
    public override void draw(SpriteBatch b)
    {
        if (this.Entity.Context is null)
        {
            return;
        }

        var position = (new Vector2(this.Entity.Tile.X, this.Entity.Tile.Y) * Game1.tileSize) + this.Entity.Offset;
        var onScreenPos = default(Vector2);
        var entityPos = default(Vector2);

        if (Utility.isOnScreen(position, Game1.tileSize))
        {
            var offsetY = 5 * Math.Sin(Game1.ticks / 20f);
            onScreenPos = Game1.GlobalToLocal(Game1.viewport, position + new Vector2(0, (float)offsetY));
            onScreenPos = Utility.ModifyCoordinatesForUIScale(onScreenPos);
            b.Draw(
                this.texture,
                onScreenPos,
                this.sourceRect,
                Color.White,
                (float)Math.PI,
                new Vector2(2f, 2f),
                this.scale,
                SpriteEffects.None,
                1f);

            if (this.Entity.Texture is null)
            {
                return;
            }

            b.Draw(
                this.Entity.Texture,
                onScreenPos - new Vector2(0, 24f),
                this.iconRect,
                Color.White,
                0f,
                new Vector2(this.iconRect.Width / 2f, this.iconRect.Height / 2f),
                this.iconScale,
                SpriteEffects.None,
                1f);

            return;
        }

        var viewport = Game1.graphics.GraphicsDevice.Viewport.Bounds;
        var rotation = 0f;
        if (position.X > Game1.viewport.MaxCorner.X - Game1.tileSize)
        {
            onScreenPos.X = viewport.Right - 8f;
            entityPos.X = onScreenPos.X - 36f;
            rotation = (float)Math.PI / 2f;
        }
        else if (position.X < Game1.viewport.X)
        {
            onScreenPos.X = 8f;
            entityPos.X = onScreenPos.X + 36f;
            rotation = -(float)Math.PI / 2f;
        }
        else
        {
            onScreenPos.X = position.X - Game1.viewport.X;
            entityPos.X = onScreenPos.X;
        }

        if (position.Y > Game1.viewport.MaxCorner.Y - Game1.tileSize)
        {
            onScreenPos.Y = viewport.Bottom - 8f;
            entityPos.Y = onScreenPos.Y - 36f;
            rotation = (float)Math.PI;
        }
        else if (position.Y < Game1.viewport.Y)
        {
            onScreenPos.Y = 8f;
            entityPos.Y = onScreenPos.Y + 36f;
        }
        else
        {
            onScreenPos.Y = position.Y - Game1.viewport.Y;
            entityPos.Y = onScreenPos.Y;
        }

        if ((int)onScreenPos.X == 8 && (int)onScreenPos.Y == 8)
        {
            rotation += (float)Math.PI / 4f;
            entityPos += new Vector2(-8f, -8f);
        }
        else if ((int)onScreenPos.X == 8 && (int)onScreenPos.Y == viewport.Bottom - 8)
        {
            rotation += (float)Math.PI / 4f;
            entityPos += new Vector2(-8f, 8f);
        }
        else if ((int)onScreenPos.X == viewport.Right - 8 && (int)onScreenPos.Y == 8)
        {
            rotation -= (float)Math.PI / 4f;
            entityPos += new Vector2(8f, -8f);
        }
        else if ((int)onScreenPos.X == viewport.Right - 8 && (int)onScreenPos.Y == viewport.Bottom - 8)
        {
            rotation -= (float)Math.PI / 4f;
            entityPos += new Vector2(8f, 8f);
        }

        onScreenPos = Utility.makeSafe(onScreenPos, new Vector2(5, 4) * Game1.pixelZoom);

        b.Draw(
            this.texture,
            onScreenPos,
            this.sourceRect,
            Color.White,
            rotation,
            new Vector2(2f, 2f),
            this.scale,
            SpriteEffects.None,
            1f);

        if (this.Entity.Texture is null)
        {
            return;
        }

        entityPos = Utility.makeSafe(entityPos, this.iconRect.Size.ToVector2() * 2f);

        b.Draw(
            this.Entity.Texture,
            entityPos,
            this.iconRect,
            Color.White,
            0f,
            new Vector2(this.iconRect.Width / 2f, this.iconRect.Height / 2f),
            this.iconScale,
            SpriteEffects.None,
            1f);
    }
}
