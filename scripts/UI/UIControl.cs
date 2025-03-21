namespace GameNamespace.UI
{
    using Godot;
	using GameManager;

	/// <summary>
	/// The UI class is responsible for all UI features, buttons, labels, etc.
	/// </summary>
	public partial class UIControl : Control
	{
		// Game objects
		public Control levelState;
		public Control waveHud;
		public Control pauseMenu;
		public Control playerControl;

		public override void _Ready()
		{
			levelState = GetNode<Control>("LevelState");
			waveHud = GetNode<Control>("WaveHud");
			pauseMenu = GetNode<Control>("PauseMenu");
			playerControl = GetNode<Control>("PlayerHud");
		}

		public Button CreateWaveButton(string buttonText)
		{
			Button button = UITools.Instance.CreateButton(text:buttonText, parent:waveHud, fontSize:25);

			float windowWidth = waveHud.Size.X;
			float windowHeight = waveHud.Size.Y;
			float xPos = (windowWidth - button.Size.X) / 2;
			float yPos = (windowHeight - button.Size.Y) / 2;

			button.Position = new Vector2(xPos, yPos);

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
	}
}
