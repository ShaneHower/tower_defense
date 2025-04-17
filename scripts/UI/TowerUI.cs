namespace GameNamespace.UI
{
    using Godot;
    using GameNamespace.GameAssets;
    using GameNamespace.GameManager;
    using GameNamespace.DataBase;
    using System.Collections.Generic;


    public partial class TowerUI : Control
    {
        public bool upgradeButtonDisabled = false;
        public bool isHovered;
        private Tower tower;
        private Level gameLevel;
        public TextureRect towerUI;
		private TextureButton upgradeButton;
		private TextureButton sellButton;
		private TextureButton basicSkillTreeButton;
		private TextureButton specialSkillTreeButton;
        private Texture2D textureNormal;
        private Texture2D texturePressed;

        public override void _Ready()
        {
            towerUI = GetNode<TextureRect>("TowerUI");
            towerUI.Visible = false;

            gameLevel = GetTree().Root.GetNode<Level>("Level");
            tower = GetParent<Tower>();

            MouseEntered  += () => isHovered=true;
			MouseExited  += () => isHovered=false;

            InitTowerUI();
        }

        public override void _Process(double delta)
        {
			if(tower.nextLevelId is null && !upgradeButtonDisabled)
			{
				upgradeButtonDisabled = true;
				upgradeButton.Modulate = new Color(0.5f, 0.5f, 0.5f, 1f);
				upgradeButton.TexturePressed = textureNormal;
                upgradeButton.Pressed += () => {return;};
			}
            else if (tower.nextLevelId is not null && upgradeButtonDisabled)
            {
                upgradeButtonDisabled = true;
				upgradeButton.Modulate = new Color(1f, 1f, 1f, 1f);
				upgradeButton.TexturePressed = texturePressed;
                upgradeButton.Pressed += Upgrade;
            }
        }

        /// <summary>
		/// Currently this input method is controlling the interactable tower UI.  This allows the player to bring up
		/// the upgrade menu when they click the tower.
		/// </summary>
		/// <param name="event"></param>
		public override void _Input(InputEvent @event)
        {
			// Mouse Inputs
            if(@event is InputEventMouseButton mouseEvent)
			{
				bool towerClicked = tower.isHovered && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left;
				bool upgradeExitMouseR = towerUI.Visible && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Right;
				bool upgradeExitMouseL = !isHovered && towerUI.Visible && mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left;

				if(towerClicked)
				{
					if(GameCoordinator.Instance.towerUIActive is not null)
					{
						// If there is a tower upgrade menu already open on another tower, close it and open the new tower upgrade option
						GameCoordinator.Instance.towerUIActive.towerUI.Visible = false;
					}
					GameCoordinator.Instance.towerUIActive = this;
					towerUI.Visible = true;
				}

				if(upgradeExitMouseR || upgradeExitMouseL)
				{
					GameCoordinator.Instance.towerUIActive = null;
					towerUI.Visible = false;
				}
			}

			// Key Inputs
			if (@event is InputEventKey keyEvent && keyEvent.Pressed)
			{
				bool upgradeMenuKeyExit = towerUI.Visible && keyEvent.Keycode == Key.Escape;

				if(upgradeMenuKeyExit)
				{
					GameCoordinator.Instance.towerUIActive = null;
					towerUI.Visible = false;
				}
			}
        }


        private void InitTowerUI()
		{
			// Buttons
			upgradeButton = towerUI.GetNode<TextureButton>("Upgrade");
			upgradeButton.Pressed += Upgrade;
            upgradeButton.MouseEntered += () => isHovered=true;
            upgradeButton.MouseExited  += () => isHovered=false;
			upgradeButton.Visible = true;

			sellButton = towerUI.GetNode<TextureButton>("Sell");
			sellButton.Pressed += Sell;
            sellButton.MouseEntered += () => isHovered=true;
            sellButton.MouseExited  += () => isHovered=false;
			sellButton.Visible = true;

			TextureButton specialSkillTreeButton = towerUI.GetNode<TextureButton>("SpecialSkillTree");
			specialSkillTreeButton.Visible = false;

			TextureButton basicSkillTreeButton = towerUI.GetNode<TextureButton>("BasicSkillTree");
			basicSkillTreeButton.Visible = false;

            // Store texture normal and texture pressed
            textureNormal = upgradeButton.TextureNormal;
            texturePressed = upgradeButton.TexturePressed;
		}

        private void Upgrade()
		{
			if(tower.nextLevelId is null)
			{
				return;
			}

			TowerData data = GameDataBase.Instance.QueryTowerData(tower.nextLevelId);
			if(GameCoordinator.Instance.currentGold > data.gold)
			{
				// Generate the tower prefab.
				PackedScene prefab = GD.Load<PackedScene>($"{GameCoordinator.Instance.towerPrefabLoc}/{data.prefab}");
				Tower upgrade = (Tower)prefab.Instantiate();
				gameLevel.AddChild(upgrade);
				upgrade.Position = tower.Position;
				GameCoordinator.Instance.currentGold -= upgrade.gold;
				List<string> validIds =  ["103", "108"];
				if(validIds.Contains(upgrade.id)){ _ = upgrade.AnimateSpawn(); }
				tower.QueueFree();
			}
			else
			{
				UITools.Instance.SpawnWarning(message:"Not Enough Gold!", pressedButton:upgradeButton);
			}
		}

		private void Sell()
		{
			GameCoordinator.Instance.currentGold += (int)(tower.gold * 0.5);
			tower.QueueFree();
		}

        public override void _ExitTree()
        {
            tower = null;
            towerUI = null;
            gameLevel = null;
			GameCoordinator.Instance.towerUIActive = null;
        }
    }
}
