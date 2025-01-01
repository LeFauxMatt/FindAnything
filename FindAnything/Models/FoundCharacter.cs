using LeFauxMods.Common.Integrations.FindAnything;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Characters;

namespace LeFauxMods.FindAnything.Models;

/// <inheritdoc />
internal sealed class FoundCharacter : IFoundEntity
{
    private readonly WeakReference<Character> context;

    /// <summary>Initializes a new instance of the <see cref="FoundCharacter" /> class.</summary>
    /// <param name="character">The character.</param>
    public FoundCharacter(Character character)
    {
        this.context = new WeakReference<Character>(character);
        this.Offset = Game1.pixelZoom *
            new Vector2(character.GetSpriteWidthForPositioning(), -character.Sprite.SpriteHeight) / 2f;

        // TBD: Make this into a data model on the content pipeline
        this.SourceRectangle ??= character switch
        {
            FarmAnimal farmAnimal => farmAnimal.type.Value switch
            {
                "White Chicken" or "Blue Chicken" or "Void Chicken" or "Golden Chicken" => new Rectangle(27, 45, 9, 9),
                "Brown Chicken" => new Rectangle(18, 45, 9, 9),
                "Duck" => new Rectangle(81, 45, 9, 9),
                "Rabbit" => new Rectangle(90, 45, 9, 9),
                "White Cow" or "Brown Cow" => new Rectangle(54, 45, 9, 9),
                "Goat" => new Rectangle(72, 45, 9, 9),
                "Sheep" => new Rectangle(63, 45, 9, 9),
                "Pig" => new Rectangle(99, 45, 9, 9),
                _ => null
            },
            Pet pet => pet.petType.Value switch
            {
                "Cat" => new Rectangle(108, 45, 9, 9),
                "Dog" => new Rectangle(117, 45, 9, 9),
                _ => null
            },
            _ => character.Name switch
            {
                "Abigail" => new Rectangle(0, 99, 9, 9),
                "Penny" => new Rectangle(9, 99, 9, 9),
                "Maru" => new Rectangle(18, 99, 9, 9),
                "Leah" => new Rectangle(27, 99, 9, 9),
                "Haley" => new Rectangle(36, 99, 9, 9),
                "Emily" => new Rectangle(45, 99, 9, 9),
                "Alex" => new Rectangle(54, 99, 9, 9),
                "Shane" => new Rectangle(63, 99, 9, 9),
                "Sebastian" => new Rectangle(72, 99, 9, 9),
                "Sam" => new Rectangle(81, 99, 9, 9),
                "Harvey" => new Rectangle(90, 99, 9, 9),
                "Elliott" => new Rectangle(99, 99, 9, 9),
                "Sandy" => new Rectangle(108, 99, 9, 9),
                "Evelyn" => new Rectangle(117, 99, 9, 0),
                "Marnie" => new Rectangle(0, 108, 9, 9),
                "Caroline" => new Rectangle(9, 108, 9, 9),
                "Robin" => new Rectangle(18, 108, 9, 9),
                "Pierre" => new Rectangle(27, 108, 9, 9),
                "Pam" => new Rectangle(36, 108, 9, 9),
                "Jodi" => new Rectangle(45, 108, 9, 9),
                "Lewis" => new Rectangle(54, 108, 9, 9),
                "Linus" => new Rectangle(63, 108, 9, 9),
                "Marlon" => new Rectangle(72, 108, 9, 9),
                "Willy" => new Rectangle(81, 108, 9, 9),
                "Wizard" => new Rectangle(90, 108, 9, 9),
                "Jas" => new Rectangle(108, 108, 9, 9),
                "Vincent" => new Rectangle(117, 108, 9, 9),
                "Krobus" => new Rectangle(0, 117, 9, 9),
                "Dwarf" => new Rectangle(9, 117, 9, 9),
                "Gus" => new Rectangle(18, 117, 9, 9),
                "Gunther" => new Rectangle(27, 117, 9, 9),
                "George" => new Rectangle(36, 117, 9, 9),
                "Demetrius" => new Rectangle(45, 117, 9, 9),
                "Clint" => new Rectangle(54, 117, 9, 9),
                "Bear" => new Rectangle(81, 117, 9, 9),
                _ => null
            }
        };

        if (!this.SourceRectangle.Equals(Rectangle.Empty))
        {
            this.Texture = Game1.content.Load<Texture2D>("LooseSprites\\emojis");
        }
    }

    /// <inheritdoc />
    public object? Context => this.context.TryGetTarget(out var character) ? character : null;

    /// <inheritdoc />
    public GameLocation? Location => this.context.TryGetTarget(out var character) ? character.currentLocation : null;

    /// <inheritdoc />
    public Vector2 Offset { get; }

    /// <inheritdoc />
    public Rectangle? SourceRectangle { get; }

    /// <inheritdoc />
    public Texture2D? Texture { get; }

    /// <inheritdoc />
    public Vector2 Tile =>
        this.context.TryGetTarget(out var character) ? character.Position / Game1.tileSize : Vector2.Zero;
}