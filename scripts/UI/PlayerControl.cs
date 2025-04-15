namespace GameNamespace.UI
{
    using System.Collections.Generic;

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
		private Level level;
		public Tower chosenTower;
		public Area2D towerDeck;
		public HBoxContainer towerButtonContainer;
		public bool towerDeckHovered = false;
		public bool towerButtonsVisible = false;
		public bool enoughGold = false;
		public GameButton activeTowerButton;
		public List<GameButton> towerButtons = new();
		private InputEventMouseButton mouseEvent;

		public override void _Ready()
		{
			level = GetTree().Root.GetNode<Level>("Level");
			towerButtonContainer = GetNode<HBoxContainer>("HBoxContainer");
			towerDeck = GetNode<Area2D>("TowerDeck");

			InitTowerDeck();
			CreateTowerButtons();
		}

        public override void _Input(InputEvent @event)
        {
            if(@event is InputEventMouseButton mouseEvent)
			{
				if(mouseEvent.Pressed && chosenTower is not null)
				{
					if(mouseEvent.ButtonIndex == MouseButton.Left)
					{
						PlaceTower();
					}
					else if (mouseEvent.ButtonIndex == MouseButton.Right)
					{
						// Cancel Placement
						ClearTowerButtonState();
					}
				}
			}
        }

        public override void _Process(double delta)
        {
			if(chosenTower is not null)
			{
				chosenTower.GlobalPosition = GetGlobalMousePosition();

                // Keep track of our gold, do we have enough to buy this tower at any given time?
				if(chosenTower.gold > GameCoordinator.Instance.currentGold) { enoughGold = false; }
				else { enoughGold = true; }
			}
        }

		// Init Methods
		public void CreateTowerButtons()
		{
			GameButton basicTower = UITools.Instance.CreateGameButtonFromRegion(parent:towerButtonContainer, buttonType:"tower");
			HandleTowerButtonBehavior(button:basicTower, towerId:101, spriteSheet:"BasicTower-Sheet.png");
			towerButtons.Add(basicTower);

			GameButton iceTower = UITools.Instance.CreateGameButtonFromRegion(parent:towerButtonContainer, buttonType:"tower");
			HandleTowerButtonBehavior(button:iceTower, towerId:102, spriteSheet:"IceTowerLv1-Sheet.png");
			towerButtons.Add(iceTower);

			GameButton fireTower = UITools.Instance.CreateGameButtonFromRegion(parent:towerButtonContainer, buttonType:"tower");
			HandleTowerButtonBehavior(button:fireTower, towerId:107, spriteSheet:"FireTowerLv1-Sheet.png");
			towerButtons.Add(fireTower);

			GameButton earthTower = UITools.Instance.CreateGameButtonFromRegion(parent:towerButtonContainer, buttonType:"tower");
			HandleTowerButtonBehavior(button:earthTower, towerId:104, spriteSheet:"EarthTowerLv1-Sheet.png");
			towerButtons.Add(earthTower);
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

			// Set deck reveal and deck hide behavior triggers
			towerDeck.InputEvent += (viewport, ev, shapeIdx) => {
				if(ev is InputEventMouseButton mb && mb.Pressed && mb.ButtonIndex == MouseButton.Left)
				{
					float step = 0.02f;
					float delay = 0.01f;
					if(towerButtonsVisible)
					{
					    _ = AnimateDeckHide(step, delay);
					}
					else
					{
						_ = AnimateDeckReveal(step, delay);
					}

					towerButtonsVisible = !towerButtonsVisible;
				}
			};
		}

		// Main behavior methods
		private void OnButtonDown(GameButton pressedButton)
		{
			if(activeTowerButton is null)
			{
				string towerId = (string)pressedButton.GetMeta("towerId");
				TowerData towerData = GameDataBase.Instance.QueryTowerData(towerId);

				chosenTower = CreateTowerFromPrefab(towerData.prefab);
				chosenTower.beingPlaced = true;
				pressedButton.ShiftPositionUp();
				activeTowerButton = pressedButton;
			}
			else
			{
				ClearTowerButtonState();
				OnButtonDown(pressedButton);
			}
		}

		private void PlaceTower()
		{
			if(enoughGold && chosenTower.canPlace)
			{
				Tower tower = CreateTowerFromPrefab(chosenTower.prefab);
				GameCoordinator.Instance.currentGold -= chosenTower.gold;
				tower.Position = chosenTower.Position;
				List<string> validIds =  ["101", "102"];
				if(validIds.Contains(tower.id)){ _ = tower.AnimateSpawn(); }
			}
			else
			{
				UITools.Instance.SpawnWarning("Not Enough Gold!", activeTowerButton);
			}
		}

		// Animation Methods
		private async Task AnimateDeckReveal(float step, float delay)
		{
			var tween = CreateTween();
			foreach(var node in towerButtons)
			{
				if(node is TextureButton button)
				{
					button.Visible = true;

					// Animate fade in
					tween.TweenProperty(button, "modulate:a", 1.0f, step).SetDelay(delay);

					// Animate scale
					tween.TweenProperty(button, "scale", Vector2.One, step).SetDelay(delay);
				}
			}

			await ToSignal(tween, "finished");
		}

		private async Task AnimateDeckHide(float step, float delay)
		{
			var tween = CreateTween();
			for(int i = towerButtons.Count - 1; i >= 0; i--)
			{
				var node = towerButtonContainer.GetChild(i);
				if(node is TextureButton button)
				{
					// Fade out
					tween.TweenProperty(button, "modulate:a", 0.0f, step).SetDelay(delay);

					// Shrink scale
					tween.TweenProperty(button, "scale", new Vector2(0.8f, 0.8f), step).SetDelay(delay);
				}
			}

			await ToSignal(tween, "finished");

			foreach (var node in towerButtons)
			{
				if(node is TextureButton button)
				{
					button.Visible = false;
				}
			}
		}

		// Helper methods
		public void HandleTowerButtonBehavior(GameButton button, int towerId, string spriteSheet)
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

		private void ClearTowerButtonState()
		{
			activeTowerButton.ResetToInitPosition();
			chosenTower.QueueFree();
			activeTowerButton = null;
			chosenTower = null;
		}

		private Tower CreateTowerFromPrefab(string towerPrefab)
		{
			PackedScene prefab = GD.Load<PackedScene>($"{GameCoordinator.Instance.towerPrefabLoc}/{towerPrefab}");
			Tower tower = (Tower)prefab.Instantiate();
			Node2D towerGroupNode = level.GetNode<Node2D>("Towers");
			towerGroupNode.AddChild(tower);
			return tower;
		}
    }
}
