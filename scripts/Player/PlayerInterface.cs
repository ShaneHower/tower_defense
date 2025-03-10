namespace GameNamespace.Player
{
    using System.Collections.Generic;

    using GameNamespace.GameManager;
    using Godot;

    public partial class PlayerInterface : Control
	{
		private Level level;
		private UI ui;
		public Button towerButton;
		public Tower chosenTower;
		public bool towerUiActive = false;
		public bool ruinsHovered = false;
		public Ruins ruins;
		private InputEventMouseButton mouseEvent;
		public string towerPrefabLoc = "res://prefabs/towers";

		public override void _Ready()
		{
			level = GetTree().Root.GetNode<Level>("Level");
			ui = GetParent<UI>();
			towerButton = GetNode<Button>("Tower");
			towerButton.Pressed += OnButtonDown;
		}

        public override void _Input(InputEvent @event)
        {
            if(@event is InputEventMouseButton mouseEvent)
			{
				if(mouseEvent.Pressed)
				{
					PlaceTower(mouseEvent);
				}
			}
        }

        public override void _Process(double delta)
        {
			if(towerUiActive)
			{
				chosenTower.GlobalPosition = GetGlobalMousePosition();
				if(ruinsHovered)
				{

					// This statement is a little long, but its just illustrating that we are pulling the animator from
					// chosen tower and getting the size.
					Vector2 towerSpriteSize = chosenTower.animator.SpriteFrames.GetFrameTexture(chosenTower.animator.Animation, 0).GetSize();
					Vector2 ruinsSpriteSize = ruins.sprite.Texture.GetSize();
					float yDiff = (towerSpriteSize.Y / 2) - (ruinsSpriteSize.Y / 2);
					chosenTower.GlobalPosition = ruins.GlobalPosition - new Vector2(0, yDiff);
				}
			}
        }

        private void OnButtonDown()
		{
			// I have to generate the prefab in order to get the towers gold value.  I probably want to refactor into
			// config files at some point so I'm not generating game objects if I don't need them.
			string towerName = towerButton.Name;
			PackedScene prefab = GD.Load<PackedScene>($"{towerPrefabLoc}/{towerName.ToLower()}.tscn");
			chosenTower = (Tower) prefab.Instantiate();
			int currentGold = GameCoordinator.Instance.currentGold;

			if(currentGold < chosenTower.gold)
			{
				Label warning = ui.SpawnWarning();
				AddChild(warning);
				warning.Text = "Not Enough Gold!";

				// Tweens are special objects (stand for between) which can generate a simple animation. This one fades
				// the text over time until its no longer visible.  This code will then delete the object when the animation
				// is finished.
				Tween warningTween = GetTree().CreateTween();
				warningTween.TweenProperty(warning, "modulate:a", 0.0f, 2.0f);
				warningTween.TweenCallback(Callable.From(() => warning.QueueFree()));

				chosenTower.QueueFree();
				chosenTower = null;
			}
			else
			{
				towerUiActive = true;
				level.AddChild(chosenTower);
				chosenTower.beingPlaced = true;
			}
		}

		private void PlaceTower(InputEventMouseButton mouseButton)
		{
			if(chosenTower != null && ruinsHovered)
			{
				if(mouseButton.ButtonIndex == MouseButton.Left)
				{
					towerUiActive = false;
					chosenTower.beingPlaced = false;
					GameCoordinator.Instance.currentGold -= chosenTower.gold;
					ruinsHovered = false;
					ruins.QueueFree();
				}
				else if (mouseButton.ButtonIndex == MouseButton.Right)
				{
					towerUiActive = false;
					chosenTower.QueueFree();
				}
			}
		}

    }
}
