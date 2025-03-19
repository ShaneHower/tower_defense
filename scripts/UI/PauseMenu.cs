namespace GameNamespace.UI
{
    using Godot;

    /// <summary>
    /// The UI class is responsible for all UI features, buttons, labels, etc.
    /// </summary>
    public partial class PauseMenu : UIControl
    {
        public Panel menu;
        private Control parentUI;

        public override void _Ready()
        {
            menu = GetNode<Panel>("Panel");
            parentUI = GetParent<Control>();
            CreatePauseMenu();
        }

        public override void _UnhandledInput(InputEvent @event)
		{
			if (@event is InputEventKey keyEvent && keyEvent.Pressed)
			{
				if(keyEvent.Keycode == Key.Escape)
				{
					bool paused = GetTree().Paused;
					Visible = !Visible;
					GetTree().Paused = !paused;
				}
			}
		}

        private void CreatePauseMenu()
		{
			Visible = false;

			// Center the Pause menu
			menu.AnchorLeft = 0.5f;
			menu.AnchorTop = 0.5f;
			menu.AnchorRight = 0.5f;
			menu.AnchorBottom = 0.5f;

			// Create needed buttons
			VBoxContainer buttonContainer = menu.GetNode<VBoxContainer>("VBoxContainer");

			Button continueButton = CreateButton(text: "Continue", parent: buttonContainer);
			continueButton.Pressed += OnContinueButtonPressed;

			CreateButton(text: "Settings", parent: buttonContainer);

			Button devButton = CreateButton(text: "Dev Mode", parent: buttonContainer);
            devButton.Pressed += OnDevModeButtonPressed;

			CreateButton(text: "Quit", parent: buttonContainer);
		}

        private void OnContinueButtonPressed()
		{
			GetTree().Paused = false;
			Visible = false;
		}

        private void OnDevModeButtonPressed()
        {
            Control devWindow = parentUI.GetNode<Control>("DevWindow");
            GetTree().Paused = false;
            Visible = false;
            devWindow.Visible = true;
        }
    }

}
