namespace GameNamespace.UI
{
	using DevTools;
    using GameNamespace.GameManager;

    using Godot;

    public partial class PauseMenu : UIControl
    {
        public Panel mainMenu;
		public VBoxContainer buttonContainer;
		public Panel settingsMenu;
        private Control parentUI;
		private DevWindow devWindow;

        public override void _Ready()
        {
            mainMenu = GetNode<Panel>("MainMenu");
			buttonContainer = mainMenu.GetNode<VBoxContainer>("VBoxContainer");
			buttonContainer.Alignment = BoxContainer.AlignmentMode.Center;

			settingsMenu = GetNode<Panel>("SettingsMenu");
            parentUI = GetParent<Control>();
			devWindow = parentUI.GetNode<DevWindow>("DevWindow");
			Sound.Instance.AddToSoundBank("ButtonPress");

            CreateMainMenu();
			CreateSettingsMenu();
        }

        public override void _UnhandledInput(InputEvent @event)
		{
			if (@event is InputEventKey keyEvent && keyEvent.Pressed)
			{
				if(keyEvent.Keycode == Key.Escape && mainMenu.Visible)
				{
					mainMenu.Visible = false;
					GetTree().Paused = false;
				}
				else if(keyEvent.Keycode == Key.Escape && settingsMenu.Visible)
				{
					mainMenu.Visible = true;
					settingsMenu.Visible = false;
				}
				else if (keyEvent.Keycode == Key.Escape)
				{
					mainMenu.Visible = true;
					GetTree().Paused = true;
				}
			}
		}

        private void CreateMainMenu()
		{
			mainMenu.Visible = false;

			TextureButton continueButton = CreateMenuButton("Continue");
			continueButton.Pressed += OnContinueButtonPressed;

			TextureButton saveButton = CreateMenuButton("Save");
			saveButton.Pressed += () => {Sound.Instance.PlayFoley("ButtonPress");};

			TextureButton settingsButton = CreateMenuButton("Settings");
			settingsButton.Pressed += OnSettingsButtonPressed;

			TextureButton restartButton = CreateMenuButton("Restart");
			restartButton.Pressed += () => {
				Sound.Instance.PlayFoley("ButtonPress");
				GetTree().Paused = false;
				GetTree().ReloadCurrentScene();
			};


			TextureButton quitButton = CreateMenuButton("Quit");
			quitButton.Pressed += () => {
				Sound.Instance.PlayFoley("ButtonPress");
				GetTree().Quit();
			};
		}

		private TextureButton CreateMenuButton(string text)
		{
			TextureButton button = UITools.Instance.CreateGameButtonFromRegion(parent:buttonContainer, buttonType:"Menu");
			Label buttonLabel = UITools.Instance.CreateLabel(text:text, parent:button, fontSize:25);
			buttonLabel.Size = button.Size;

			return button;
		}

		private void CreateSettingsMenu()
		{
			settingsMenu.Visible = false;
			VBoxContainer buttonContainer = settingsMenu.GetNode<VBoxContainer>("VBoxContainer");

			Button spawnBoxButton = UITools.Instance.CreateCheckBox(text:"Spawn Box", parent:buttonContainer);
			spawnBoxButton.Pressed += () => devWindow.ToggleSpawnBox();

			Button combatLogConsole = UITools.Instance.CreateCheckBox(text:"Combat Log Console", parent: buttonContainer);
			combatLogConsole.Pressed += () => devWindow.ToggleCombatLog();
		}

        private void OnContinueButtonPressed()
		{
			Sound.Instance.PlayFoley("ButtonPress");
			GetTree().Paused = false;
			mainMenu.Visible = false;
		}


        private void OnSettingsButtonPressed()
        {
			Sound.Instance.PlayFoley("ButtonPress");
            mainMenu.Visible = false;
			settingsMenu.Visible = true;
        }
    }

}
