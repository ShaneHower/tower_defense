namespace GameNamespace.Player
{
    using System.Runtime.CompilerServices;
    using GameNamespace.GameManager;
    using Godot;

    public partial class PlayerInterface : Control
	{
		private Level level;
		public Button towerButton;
		public Tower chosenTower;
		public bool towerUiActive = false;
		private InputEventMouseButton mouseEvent;
		public string towerPrefabLoc = "res://prefabs/towers";

		public override void _Ready()
		{
			level = GetTree().Root.GetNode<Level>("Level");
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
			}
        }

        private void OnButtonDown()
		{
			towerUiActive = true;
			string towerName = towerButton.Name;
			PackedScene prefab = GD.Load<PackedScene>($"{towerPrefabLoc}/{towerName.ToLower()}.tscn");
			chosenTower = (Tower) prefab.Instantiate();
			level.AddChild(chosenTower);
			chosenTower.beingPlaced = true;
		}

		private void PlaceTower(InputEventMouseButton mouseButton)
		{
			if(chosenTower != null)
			{
				if(mouseButton.ButtonIndex == MouseButton.Left)
				{
					towerUiActive = false;
					chosenTower.beingPlaced = false;
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
