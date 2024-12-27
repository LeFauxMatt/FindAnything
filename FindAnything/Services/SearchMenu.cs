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
    private readonly Func<string> getText;
    private readonly IModHelper helper;
    private readonly Action<string> setText;
    private readonly TextBox textBox;

    private Queue<string>? cachedKeywords;

    private int debounceTimer;
    private string previousText;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SearchMenu" /> class.
    /// </summary>
    /// <param name="helper">Dependency for events, input, and content.</param>
    /// <param name="getText">Get the search text.</param>
    /// <param name="setText">Set the search text.</param>
    public SearchMenu(IModHelper helper, Func<string> getText, Action<string> setText)
        : base(
            (Game1.uiViewport.Width - Math.Min(800, Game1.uiViewport.Width)) / 2,
            (Game1.uiViewport.Height - 48) / 2,
            Math.Min(800, Game1.uiViewport.Width), 48)
    {
        this.helper = helper;
        this.getText = getText;
        this.setText = setText;
        this.previousText = string.Empty;
        this.textBox = new TextBox(
            this.helper.GameContent.Load<Texture2D>("LooseSprites/textBox"),
            null,
            Game1.smallFont,
            Game1.textColor)
        {
            X = this.xPositionOnScreen,
            Y = this.yPositionOnScreen,
            Width = this.width,
            limitWidth = false,
            Selected = true,
            Text = getText()
        };

        this.textBox.OnEnterPressed += this.OnEnterPressed;
        this.textBox.OnTabPressed += this.OnTabPressed;
    }

    private string Text
    {
        get => this.getText();
        set => this.setText(value);
    }

    public override void draw(SpriteBatch b)
    {
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
                this.Text = string.Empty;
                this.exitThisMenuNoSound();
                return;
            case Keys.Enter:
                this.Text = this.textBox.Text;
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
        if (this.debounceTimer > 0 && --this.debounceTimer == 0 && this.textBox.Text != this.Text)
        {
            this.Text = this.textBox.Text;
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
        this.Text = this.textBox.Text;
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

            foreach (var keyword in keywords.OrderBy(keyword => keyword.Trim().ToLower(CultureInfo.InvariantCulture))
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
