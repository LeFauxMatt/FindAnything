using System.Globalization;
using LeFauxMods.Common.Utilities;
using LeFauxMods.FindAnything.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.Menus;

namespace LeFauxMods.FindAnything.Services;

/// <summary>Menu for displaying the search bar.</summary>
internal sealed class SearchMenu : IClickableMenu
{
    private const int DebounceTime = 20;
    private readonly TextBox textBox;

    private Queue<string>? cachedKeywords;

    private int debounceTimer;
    private string previousText;

    /// <inheritdoc />
    /// <param name="helper">Dependency for events, input, and content.</param>
    public SearchMenu(IModHelper helper)
        : base(
            (Game1.uiViewport.Width - Math.Min(800, Game1.uiViewport.Width)) / 2,
            (Game1.uiViewport.Height - 48) / 2,
            Math.Min(800, Game1.uiViewport.Width), 48)
    {
        this.previousText = string.Empty;
        this.textBox = new TextBox(
            helper.GameContent.Load<Texture2D>("LooseSprites/textBox"),
            null,
            Game1.smallFont,
            Game1.textColor)
        {
            X = this.xPositionOnScreen,
            Y = this.yPositionOnScreen,
            Width = this.width,
            limitWidth = false,
            Selected = true,
            Text = ModState.SearchText
        };

        this.textBox.OnEnterPressed += this.OnEnterPressed;
        this.textBox.OnTabPressed += this.OnTabPressed;
    }

    public override void draw(SpriteBatch b)
    {
        b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.uiViewport.Width, Game1.uiViewport.Height),
            Color.Black * 0.5f);
        this.textBox.Draw(b, false);
        this.drawMouse(b);
    }

    /// <inheritdoc />
    public override void performHoverAction(int x, int y)
    {
        base.performHoverAction(x, y);
        this.textBox.Hover(x, y);
    }

    /// <inheritdoc />
    public override void receiveKeyPress(Keys key)
    {
        switch (key)
        {
            case Keys.Escape:
                ModState.SearchText = string.Empty;
                this.exitThisMenuNoSound();
                return;
            case Keys.Enter:
                ModState.SearchText = this.textBox.Text;
                this.exitThisMenuNoSound();
                return;
            case Keys.Tab:
                return;
            default:
                this.cachedKeywords = null;
                return;
        }
    }

    /// <inheritdoc />
    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        base.receiveLeftClick(x, y, playSound);
        this.textBox.Selected = this.isWithinBounds(x, y);
    }

    /// <inheritdoc />
    public override void receiveRightClick(int x, int y, bool playSound = true)
    {
        base.receiveRightClick(x, y, playSound);
        if (!this.isWithinBounds(x, y))
        {
            this.textBox.Selected = false;
            return;
        }

        this.textBox.Selected = true;
        this.textBox.Text = string.Empty;
    }

    /// <inheritdoc />
    public override void update(GameTime time)
    {
        if (this.debounceTimer > 0 && --this.debounceTimer == 0 && this.textBox.Text != ModState.SearchText)
        {
            ModState.SearchText = this.textBox.Text;
        }

        if (this.textBox.Text.Equals(this.previousText, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        this.debounceTimer = DebounceTime;
        this.previousText = this.textBox.Text;
    }

    private void OnEnterPressed(TextBox sender)
    {
        ModState.SearchText = this.textBox.Text;
        if (this.readyToClose())
        {
            this.exitThisMenuNoSound();
        }
    }

    private void OnTabPressed(TextBox sender)
    {
        if (this.cachedKeywords is null)
        {
            this.cachedKeywords = new Queue<string>();
            var keywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            ModEvents.Publish(new SearchUpdated(this.textBox.Text, keywords.UnionWith));

            if (keywords.Count == 0)
            {
                return;
            }

            foreach (var keyword in keywords
                         .OrderBy(static keyword => keyword.Trim().ToLower(CultureInfo.InvariantCulture))
                         .Distinct())
            {
                this.cachedKeywords.Enqueue(keyword);
            }
        }

        if (this.cachedKeywords.TryDequeue(out var suggestedText))
        {
            this.textBox.Text = suggestedText;
        }
    }
}