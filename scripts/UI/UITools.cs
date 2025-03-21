namespace GameNamespace.UI
{
    using Godot;
    using GameNamespace.GameManager;

    public partial class UITools: Control
	{
        public static UITools Instance { get; private set; }
        private string fontLoc = "res://misc/font/CelticTime.ttf";
        public string uiPrefabLoc = "res://prefabs/ui";

        public override void _Ready()
        {
            Instance = this;
        }

        public Theme GetGameFontTheme(int fontSize, string gameObject)
		{
			Theme theme = new();
			Font font = ResourceLoader.Load<Font>(fontLoc);
			theme.SetFont("font", gameObject, font);
            theme.SetFontSize("font_size", gameObject, fontSize);
			return theme;
		}

        public void ConfigureControl(Control control, string gameObject, Control parent, int fontSize=0)
        {
            control.TextureFilter = TextureFilterEnum.Nearest;
            int fontSizeOverride = fontSize == 0 ? 12 : fontSize;
            control.Theme = GetGameFontTheme(fontSize:fontSizeOverride,  gameObject:gameObject);
            parent.AddChild(control);
        }

        public Button CreateButton(string text, Control parent, int fontSize=0)
		{
			Button button = new();
			button.Text = text;
            ConfigureControl(control:button, gameObject:"Button", parent:parent, fontSize:fontSize);
			return button;
		}

        public TextureRect CreateButtonHover()
        {
            Texture2D hoverSheet = GD.Load<Texture2D>($"{GameCoordinator.Instance.spriteLoc}/Icons Tileset.png");

            // This is a constant here because all of our buttons will have the same button overlay.
            Rect2 hoverRegion = new Rect2(48, 0, 24, 24); // x=24, y=0, width=24, height=24

            AtlasTexture hoverTexture = new AtlasTexture
            {
                Atlas = hoverSheet,
                Region = hoverRegion
            };

            TextureRect hoverOverlay = new TextureRect
            {
                Texture = hoverTexture,
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                StretchMode = TextureRect.StretchModeEnum.Scale,
                SizeFlagsHorizontal = Control.SizeFlags.Fill,
                SizeFlagsVertical = Control.SizeFlags.Fill,
                MouseFilter = Control.MouseFilterEnum.Ignore,
                TextureFilter = TextureFilterEnum.Nearest, 
                Name = "HoverOverlay",
                Visible = false
            };

            return hoverOverlay;
        }

        public TextureButton CreateTextureButtonFromRegion(string texturePath, Rect2 region)
        {
            // Create button
            Texture2D texture = GD.Load<Texture2D>(texturePath);
            AtlasTexture atlasTexture = new AtlasTexture
            {
                Atlas = texture,
                Region = region
            };

            TextureButton button = new TextureButton
            {
                TextureNormal = atlasTexture,
                TexturePressed = atlasTexture,
                TextureHover = atlasTexture,
                CustomMinimumSize = region.Size,
                MouseFilter = Control.MouseFilterEnum.Stop,
                SizeFlagsHorizontal = Control.SizeFlags.Fill,
                TextureFilter = TextureFilterEnum.Nearest
            };

            // Attach hover behavior
            TextureRect hoverOverlay = CreateButtonHover();
            hoverOverlay.Size = button.Size;
            button.AddChild(hoverOverlay);

            // Keep hover size in sync with button
            button.Resized += () => hoverOverlay.Size = button.Size;

            return button;
        }

        public CheckBox CreateCheckBox(string text, Control parent, int fontSize=0)
        {
            CheckBox checkBox = new();
            checkBox.Text = text;
            ConfigureControl(control:checkBox, gameObject:"CheckBox", parent:parent, fontSize:fontSize);
            return checkBox;
        }

        public Label CreateLabel(string text, Control parent, int fontSize=0)
        {
            Label label = new();
            label.Text = text;
            ConfigureControl(control:label, gameObject:"Label", parent:parent, fontSize:fontSize);
            return label;
        }

        /// <summary>
		/// We spawn a warning and attach it to the button that was pressed.
		///
		/// The warning system uses something called Tweens. Tweens are special objects (stand for between) which can
		/// generate a simple animation.  In this case, I am generating a warning label, and use a Tween to slowly make
		/// the label transparent, finally deleting the object once its completely invisible.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="pressedButton"></param>
		public void SpawnWarning(string message, Control pressedButton)
		{
			// Create label with light red color and raise it above the parent by 20 pixels
            Label warning = CreateLabel(text:message, parent:pressedButton, fontSize:20);
            warning.AddThemeColorOverride("font_color", new Color(0.9f, 0.3f, 0.3f));
            warning.Position -= new Vector2(0, 20);

            // Animate a fade out
			Tween warningTween = GetTree().CreateTween();
			warningTween.TweenProperty(warning, "modulate:a", 0.0f, 2.0f);
			warningTween.TweenCallback(Callable.From(() => warning.QueueFree()));
		}
    }
}
