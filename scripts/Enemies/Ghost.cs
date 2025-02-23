namespace GameNamespace.Enemies
{
	public partial class Ghost : Enemy
	{
		public float speed = 0.06f;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			InitializeEnemy();
			health = 40.0f;
			name = "ghost";
		}

		public override async void _PhysicsProcess(double delta)
		{
			if (isDead)
			{
				await AnimateDeath();
			}
			else
			{
				AnimateMovement((float)delta, speed);
			}
		}

	}
}
