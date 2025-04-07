namespace GameNamespace.UI
{
    using Godot;

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
		public CanvasLayer uiCanvasLayer;

		public override void _Ready()
		{

			uiCanvasLayer = GetParent<CanvasLayer>();
			levelState = GetNode<Control>("LevelState");
			waveHud = GetNode<Control>("WaveHud");
			pauseMenu = GetNode<Control>("PauseMenu");
			playerControl = GetNode<Control>("PlayerHud");
		}

		public TextureButton CreateWaveButton(string buttonText)
		{
			GameButton button = UITools.Instance.CreateGameButtonFromRegion(parent:waveHud, buttonType:"Menu");
			Label label = UITools.Instance.CreateLabel(text:buttonText, parent:button, fontSize:25);
			label.Size = button.Size;

			float windowWidth = waveHud.Size.X;
			float windowHeight = waveHud.Size.Y;
			float xPos = (windowWidth - button.Size.X) / 2;
			float yPos = (windowHeight - button.Size.Y) / 2;
			button.Position = new Vector2(xPos, yPos);

			label.OffsetLeft = -button.Size.X / 2;
			label.OffsetTop = -button.Size.Y / 2;
			label.OffsetRight = button.Size.X / 2;
			label.OffsetBottom = button.Size.Y / 2;

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

		public void ShowGameOverScreen()
		{
			VBoxContainer vBox = new VBoxContainer();
			vBox.Alignment = BoxContainer.AlignmentMode.Center;
			vBox.AnchorLeft = 0.5f;
			vBox.AnchorTop = 0.5f;
			vBox.AnchorRight = 0.5f;
			vBox.AnchorBottom = 0.5f;
			vBox.OffsetLeft = -300;
			vBox.OffsetTop = -100;
			vBox.OffsetRight = 300;
			vBox.OffsetBottom = 100;
			AddChild(vBox, true);

			Label gameOverLabel = UITools.Instance.CreateLabel(text:"GAME OVER", parent:vBox, fontSize:100);
			gameOverLabel.SizeFlagsHorizontal = SizeFlags.ShrinkCenter;
			gameOverLabel.HorizontalAlignment = HorizontalAlignment.Center;

			HBoxContainer buttonBox = new HBoxContainer();
			buttonBox.Alignment = BoxContainer.AlignmentMode.Begin;
			buttonBox.SizeFlagsHorizontal = SizeFlags.ShrinkCenter;
			vBox.AddChild(buttonBox);

			GameButton restartButton = UITools.Instance.CreateGameButtonFromRegion(parent:buttonBox, buttonType:"Menu");
			Label restartLabel = UITools.Instance.CreateLabel(text:"Restart", parent:restartButton, fontSize:25);
			restartLabel.Size = restartButton.Size;
			restartButton.SizeFlagsHorizontal = SizeFlags.Expand;

			GameButton exitButton = UITools.Instance.CreateGameButtonFromRegion(parent:buttonBox, buttonType:"Menu");
			Label exitLabel = UITools.Instance.CreateLabel(text:"Exit", parent:exitButton, fontSize:25);
			exitLabel.Size = exitButton.Size;
			exitButton.SizeFlagsHorizontal = SizeFlags.Expand;

		}
	}
}
