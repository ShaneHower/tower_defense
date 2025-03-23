namespace GameNamespace.UI
{
    using GameNamespace.DataBase;
	using GameNamespace.GameAssets;
    using GameNamespace.GameManager;
    using Godot;

    /// <summary>
	/// Governs the everything that the player can interact with.
	/// </summary>
    public partial class PlayerControl : Control
	{
		// Class vars
		public bool towerUiActive = false;
		public bool ruinsHovered = false;

		// Game objects
		private Level level;
		public Button towerButton;
		public Tower chosenTower = null;
		public Ruins ruins;
		private InputEventMouseButton mouseEvent;

		public override void _Ready()
		{
			level = GetTree().Root.GetNode<Level>("Level");
			CreatePlayerHud();
		}

        public override void _Input(InputEvent @event)
        {
            if(@event is InputEventMouseButton mouseEvent)
			{
				if(mouseEvent.Pressed && chosenTower is not null)
				{
					PlaceTower(mouseEvent);
				}
			}
        }

		/// <summary>
		/// This governs the behavior for tower placement.  The player has chosen a tower to place and now they are
		/// dragging a tower to a ruin's location to place it.  When over the ruins, it snaps the tower to the valid
		/// build location (aligning the bottom of the ruin's sprite with the tower's sprite).
		///
		/// An explanation about the algorithm for snapping the tower to the ruins.
		///     yDiff = (towerSpriteSize.Y / 2) - (ruinsSpriteSize.Y / 2)
		///
		/// We have to offset the location of the tower in relation to the size ratio of the tower:ruins. Both sprites
		/// anchor points are in the center of the image, so when we try to lock the tower in place of the ruin they
		/// don't line up in the way we would expect (the bottom of the tower is aligned with the bottom of the ruin).
		/// We have to take half of each object and find the difference between the two in order to shift the tower
		/// up enough pixels so that they are overlapping at the right location.
		///
		/// </summary>
		/// <param name="delta"></param>
        public override void _Process(double delta)
        {
			if(towerUiActive)
			{
				chosenTower.GlobalPosition = GetGlobalMousePosition();
				if(ruinsHovered)
				{
					// Get current animation meta data for the tower.
					string currentAnimation = chosenTower.animator.Animation;
					SpriteFrames towerSpriteFames = chosenTower.animator.SpriteFrames;
					Vector2 towerSpriteSize = towerSpriteFames.GetFrameTexture(currentAnimation, 0).GetSize();

					// Calculate tower offset and snap tower to ruins.
					Vector2 ruinsSpriteSize = ruins.sprite.Texture.GetSize();
					float yDiff = (towerSpriteSize.Y / 2) - (ruinsSpriteSize.Y / 2);
					chosenTower.GlobalPosition = ruins.GlobalPosition - new Vector2(0, yDiff);
				}
			}
        }

		public void CreatePlayerHud()
		{
			HBoxContainer container = GetNode<HBoxContainer>("HBoxContainer");

			TextureButton basicTower = UITools.Instance.CreateTextureButtonFromRegion(parent:container, buttonType:"Tower");
			HandleTowerButtonBehavior(button:basicTower, towerId:101, spriteSheet:"BasicTower-Sheet.png");

			TextureButton iceTower = UITools.Instance.CreateTextureButtonFromRegion(parent:container, buttonType:"Tower");
			HandleTowerButtonBehavior(button:iceTower, towerId:102, spriteSheet:"IceTowerLv1-Sheet.png");

			TextureButton fireTower = UITools.Instance.CreateTextureButtonFromRegion(parent:container, buttonType:"Tower");
			HandleTowerButtonBehavior(button:fireTower, towerId:107, spriteSheet:"FireTowerLv1-Sheet.png");

			TextureButton earthTower = UITools.Instance.CreateTextureButtonFromRegion(parent:container, buttonType:"Tower");
			HandleTowerButtonBehavior(button:earthTower, towerId:104, spriteSheet:"EarthTowerLv1-Sheet.png");
		}

		public void HandleTowerButtonBehavior(TextureButton button, int towerId, string spriteSheet)
		{
			var spritePath = $"{GameCoordinator.Instance.spriteLoc}/{spriteSheet}";
			var region = new Rect2(0, 0, 48, 72);

			TextureRect tower = UITools.Instance.GetTextureRect(texturePath:spritePath, parent:button, region:region);
			tower.OffsetLeft = -button.Size.X / 2f;
            tower.OffsetTop = -button.Size.Y / 2f;
            tower.OffsetRight = button.Size.X / 2f;
            tower.OffsetBottom = button.Size.Y / 2f;

			button.SetMeta("towerId", towerId);
			button.Pressed += () => OnButtonDown(button);
		}

        private void OnButtonDown(TextureButton pressedButton)
		{
			if(!towerUiActive)
			{
				// Get tower data and extract the cost so we can check if the player has enough money to buy the tower.
				string towerId = (string)pressedButton.GetMeta("towerId");
				TowerData towerData = GameDataBase.Instance.QueryTowerData(towerId);

				if(GameCoordinator.Instance.currentGold < towerData.gold)
				{
					// Flash a warning message that there isn't enough gold.
					UITools.Instance.SpawnWarning("Not Enough Gold!", pressedButton);
				}
				else
				{
					// Generate the tower prefab.
					PackedScene prefab = GD.Load<PackedScene>($"{GameCoordinator.Instance.towerPrefabLoc}/{towerData.prefab}");
					chosenTower = (Tower)prefab.Instantiate();
					towerUiActive = true;
					level.AddChild(chosenTower);
					chosenTower.beingPlaced = true;
				}
			}
		}

		/// <summary>
		/// This method only places the tower if there is a chosen tower that's snapped to a ruin.  Can also delete the
		/// chosen tower by right clicking.
		/// </summary>
		/// <param name="mouseButton"></param>
		private void PlaceTower(InputEventMouseButton mouseButton)
		{
			if(mouseButton.ButtonIndex == MouseButton.Left && ruinsHovered && towerUiActive)
			{
				// Place Tower
				towerUiActive = false;
				chosenTower.beingPlaced = false;
				GameCoordinator.Instance.currentGold -= chosenTower.gold;
				ruinsHovered = false;
				ruins.QueueFree();

				// Free up for garbage collection.
				ruins = null;
				chosenTower = null;

			}
			else if (towerUiActive && mouseButton.ButtonIndex == MouseButton.Right)
			{
				// Cancel Placement
				towerUiActive = false;
				chosenTower.QueueFree();
				chosenTower = null;
			}
		}

    }
}
