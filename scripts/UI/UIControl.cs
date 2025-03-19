namespace GameNamespace.UI
{
	using Godot;

	/// <summary>
	/// The UI class is responsible for all UI features, buttons, labels, etc.
	/// </summary>
	public partial class UIControl : Control
	{
		// Class vars
		public string uiPrefabLoc = "res://prefabs/ui";
		private string fontLoc = "res://misc/font/CelticTime.ttf";

		// Game objects
		public Control levelState;
		public Control waveHud;
		public Control pauseMenu;

		public override void _Ready()
		{
			levelState = GetNode<Control>("LevelState");
			waveHud = GetNode<Control>("WaveHud");
			pauseMenu = GetNode<Control>("PauseMenu");
		}

		public Button CreateWaveButton(string buttonText)
		{
			// Get size of window
			float windowWidth = waveHud.Size.X;
			float windowHeight = waveHud.Size.Y;

			PackedScene prefab = GD.Load<PackedScene>($"{uiPrefabLoc}/button.tscn");
			Button button = (Button)prefab.Instantiate();
			waveHud.AddChild(button);
			button.Text = buttonText;

			float xPos = (windowWidth - button.Size.X) / 2;
			float yPos = (windowHeight - button.Size.Y) / 2;
			button.Position = new Vector2(xPos, yPos);

			return button;
		}

		public Button CreateButton(string text, Control parent, string name=null)
		{
			Button button = new();
			button.Text = text;
			button.Name = string.IsNullOrEmpty(name) ? text : name;
			button.TextureFilter = TextureFilterEnum.Nearest;

			var font = ResourceLoader.Load<Font>(fontLoc);
			button.AddThemeFontOverride("font", font);
			parent.AddChild(button);

			return button;
		}

		public void UpdateGoldValue(int amt)
		{
			Sprite2D gold = levelState.GetNode<Sprite2D>("Gold");
			Label goldValue = gold.GetNode<Label>("GoldValue");
			goldValue.Text = amt.ToString();
		}

		public void UpdateHealthValue(int amt)
		{
			Sprite2D health = levelState.GetNode<Sprite2D>("Health");
			Label healthValue = health.GetNode<Label>("HealthValue");
			healthValue.Text = amt.ToString();
		}

		public Label SpawnWarning()
		{
			PackedScene prefab = GD.Load<PackedScene>($"{uiPrefabLoc}/warning_label.tscn");
			Label warning = (Label)prefab.Instantiate();
			return warning;
		}
	}
}
