namespace GameNamespace.UI
{
    using Godot;
    using GameNamespace.GameManager;
    using System;

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

        public TextureButton CreateTextureButtonFromRegion(Control parent, string buttonType="Menu")
        {
            string spriteSheet;
            Rect2 idleRegion;
            Rect2 pressedRegion;

            if (buttonType == "Menu")
            {
                spriteSheet = $"{GameCoordinator.Instance.spriteLoc}/Button-Sheet.png";
                idleRegion = new Rect2(0, 0, 120, 35);
                pressedRegion = new Rect2(120, 0, 120, 35);
            }
            else if (buttonType == "Tower")
            {
                spriteSheet = $"{GameCoordinator.Instance.spriteLoc}/TowerButton-Sheet.png";
                idleRegion = new Rect2(0, 0, 56, 80);
                pressedRegion = new Rect2(56, 0, 56, 80);
            }
            else
            {
                throw new Exception("CreateUITextureButtonFromRegion buttonType not valid.");

            }

            Texture2D texture = GD.Load<Texture2D>(spriteSheet);
            AtlasTexture atlasIdleTexture = new AtlasTexture
            {
                Atlas = texture,
                Region = idleRegion
            };

            AtlasTexture atlasPressedTexture = new AtlasTexture
            {
                Atlas = texture,
                Region = pressedRegion
            };

            TextureButton button = new TextureButton
            {
                TextureNormal = atlasIdleTexture,
                TexturePressed = atlasPressedTexture,
                TextureHover = atlasIdleTexture,
                CustomMinimumSize = idleRegion.Size,
                MouseFilter = Control.MouseFilterEnum.Stop,
                SizeFlagsHorizontal = Control.SizeFlags.Fill,
                TextureFilter = TextureFilterEnum.Nearest
            };

            button.StretchMode = TextureButton.StretchModeEnum.KeepCentered;
            parent.AddChild(button);

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
            label.SetAnchorsPreset(Control.LayoutPreset.Center);
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.VerticalAlignment = VerticalAlignment.Center;
            return label;
        }

        public TextureRect GetTextureRect(string texturePath, Control parent, Rect2 region)
        {
            Texture2D texture = GD.Load<Texture2D>(texturePath);
            AtlasTexture atlasTexture = new AtlasTexture
            {
                Atlas = texture,
                Region = region
            };

            var textureRect = new TextureRect
            {
                Texture = atlasTexture,
                StretchMode = TextureRect.StretchModeEnum.KeepCentered
            };

            // Center the image
            textureRect.AnchorLeft = 0.5f;
            textureRect.AnchorTop = 0.5f;
            textureRect.AnchorRight = 0.5f;
            textureRect.AnchorBottom = 0.5f;

            parent.AddChild(textureRect);

            return textureRect;
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

        public Line2D CreateCircleColliderOutline(CollisionShape2D collider)
        {
			CircleShape2D circleCollider = (CircleShape2D)collider.Shape;

			// the higher the value of points the smoother the circle
			float scale = collider.Scale.X;
			float radius = circleCollider.Radius * scale;
			int points = 60;

			Line2D circleLine = new();
			circleLine.Width = 1;
			circleLine.DefaultColor = new Color(1, 0, 0, 1);

			Vector2[] circlePoints = new Vector2[points + 1];
			for(int i = 0; i <= points; i++)
			{
				// Mathf.Tau = 2 * PI
				float angle = (i / (float)points) * Mathf.Tau;
				circlePoints[i] = new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
			}

			circleLine.Points = circlePoints;

			return circleLine;
        }
    }
}
