namespace GameNamespace.UI
{
    using System.Runtime.Serialization.Formatters;
    using Godot;

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

        public Button CreateButton(string text, Control parent, string name=null, int fontSize = 0)
		{
			Button button = new();
			button.Text = text;
			button.Name = string.IsNullOrEmpty(name) ? text : name;
			button.TextureFilter = TextureFilterEnum.Nearest;

            int fontSizeOverride = fontSize == 0 ? 12 : fontSize;
			Theme theme = GetGameFontTheme(fontSize:fontSizeOverride, gameObject:"Button");
			button.Theme = theme;
			parent.AddChild(button);

			return button;
		}

        public CheckBox CreateCheckBox(string text, Control parent, string name=null)
        {
            CheckBox checkBox = new();
            checkBox.Text = text;
            checkBox.Name = string.IsNullOrEmpty(name) ? text : name;
            checkBox.TextureFilter = TextureFilterEnum.Nearest;

            Theme theme = GetGameFontTheme(fontSize: 12, gameObject: "CheckBox");
            checkBox.Theme = theme;
            parent.AddChild(checkBox);

            return checkBox;

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
		public void SpawnWarning(string message, Button pressedButton)
		{
			// Generate the label. TODO I should maybe do this entirely in code instead of using a prefab.
			PackedScene prefab = GD.Load<PackedScene>($"{uiPrefabLoc}/warning_label.tscn");
			Label warning = (Label)prefab.Instantiate();
			warning.Text = message;
			pressedButton.AddChild(warning);

			// Animate a fade out.
			Tween warningTween = GetTree().CreateTween();
			warningTween.TweenProperty(warning, "modulate:a", 0.0f, 2.0f);
			warningTween.TweenCallback(Callable.From(() => warning.QueueFree()));
		}
    }
}
