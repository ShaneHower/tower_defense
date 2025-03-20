namespace GameNamespace.UI
{
	using DevTools;
    using Godot;

    public partial class PauseMenu : UIControl
    {
        public Panel mainMenu;
		public Panel settingsMenu;
        private Control parentUI;
		private DevWindow devWindow;

        public override void _Ready()
        {
            mainMenu = GetNode<Panel>("MainMenu");
			settingsMenu = GetNode<Panel>("SettingsMenu");
            parentUI = GetParent<Control>();
			devWindow = parentUI.GetNode<DevWindow>("DevWindow");

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
			VBoxContainer buttonContainer = mainMenu.GetNode<VBoxContainer>("VBoxContainer");

			Button continueButton = UITools.Instance.CreateButton(text:"Continue", parent:buttonContainer);
			continueButton.Pressed += OnContinueButtonPressed;

			UITools.Instance.CreateButton(text:"Save", parent:buttonContainer);

			Button devButton = UITools.Instance.CreateButton(text:"Settings", parent:buttonContainer);
            devButton.Pressed += OnSettingsButtonPressed;

			Button quitButton = UITools.Instance.CreateButton(text:"Quit", parent:buttonContainer);
			quitButton.Pressed += () => GetTree().Quit();

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
			GetTree().Paused = false;
			mainMenu.Visible = false;
		}


        private void OnSettingsButtonPressed()
        {
            mainMenu.Visible = false;
			settingsMenu.Visible = true;
        }
    }

}
