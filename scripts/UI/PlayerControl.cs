namespace GameNamespace.UI
{
    using System.Threading.Tasks;
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
		public Area2D towerDeck;
		public HBoxContainer towerButtonContainer;
		public bool towerDeckHovered = false;
		public bool towerButtonsVisible = false;
		private InputEventMouseButton mouseEvent;

		public override void _Ready()
		{
			level = GetTree().Root.GetNode<Level>("Level");
			towerButtonContainer = GetNode<HBoxContainer>("HBoxContainer");
			towerDeck = GetNode<Area2D>("TowerDeck");

			InitTowerDeck();
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
			TextureButton basicTower = UITools.Instance.CreateTextureButtonFromRegion(parent:towerButtonContainer, buttonType:"Tower");
			HandleTowerButtonBehavior(button:basicTower, towerId:101, spriteSheet:"BasicTower-Sheet.png");

			TextureButton iceTower = UITools.Instance.CreateTextureButtonFromRegion(parent:towerButtonContainer, buttonType:"Tower");
			HandleTowerButtonBehavior(button:iceTower, towerId:102, spriteSheet:"IceTowerLv1-Sheet.png");

			TextureButton fireTower = UITools.Instance.CreateTextureButtonFromRegion(parent:towerButtonContainer, buttonType:"Tower");
			HandleTowerButtonBehavior(button:fireTower, towerId:107, spriteSheet:"FireTowerLv1-Sheet.png");

			TextureButton earthTower = UITools.Instance.CreateTextureButtonFromRegion(parent:towerButtonContainer, buttonType:"Tower");
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

			// The tower deck reveals/hides buttons so we make the button invisible initially.
			button.Visible = false;
			button.Modulate = new Color(1, 1, 1, 0);
			button.Scale = new Vector2(0.8f, 0.8f);
		}

		private void InitTowerDeck()
		{
			AnimatedSprite2D animator = towerDeck.GetNode<AnimatedSprite2D>("AnimatedSprite2D");
			animator.Play("idle");

			towerDeck.MouseEntered += () => {
				towerDeckHovered = true;
				animator.Play("default");
			};

			towerDeck.MouseExited += () => {
				towerDeckHovered = false;
				animator.Play("idle");
			};

			towerDeck.InputEvent += (viewport, ev, shapeIdx) => {
				if(ev is InputEventMouseButton mb && mb.Pressed && mb.ButtonIndex == MouseButton.Left)
				{
					if(towerButtonsVisible)
					    _ = AnimateDeckHide();
					else
						_ = AnimateDeckReveal();

					towerButtonsVisible = !towerButtonsVisible;
				}
			};
		}

		public async Task AnimateDeckReveal()
		{
			var tween = CreateTween();
			float delayBetween = 0.01f;
			int index = 0;
			float step = 0.02f;

			foreach(var node in towerButtonContainer.GetChildren())
			{
				if(node is TextureButton button)
				{
					button.Visible = true;

					// Animate fade in
					tween.TweenProperty(button, "modulate:a", 1.0f, step).SetDelay(delayBetween);

					// Animate scale
					tween.TweenProperty(button, "scale", Vector2.One, step).SetDelay(delayBetween);
					index++;
				}
			}

			await ToSignal(tween, "finished");
		}

		public async Task AnimateDeckHide()
		{
			var tween = CreateTween();
			float delayBetween = 0.01f;
			int index = 0;
			float step = 0.02f;

			for(int i = towerButtonContainer.GetChildCount() - 1; i >= 0; i--)
			{
				var node = towerButtonContainer.GetChild(i);
				if(node is TextureButton button)
				{
					// Fade out
					tween.TweenProperty(button, "modulate:a", 0.0f, step).SetDelay(index * delayBetween);

					// Shrink scale
					tween.TweenProperty(button, "scale", new Vector2(0.8f, 0.8f), step).SetDelay(index * delayBetween);
					index++;
				}
			}

			await ToSignal(tween, "finished");

			foreach (var node in towerButtonContainer.GetChildren())
			{
				if(node is TextureButton button)
				{
					button.Visible = false;
				}
			}
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
