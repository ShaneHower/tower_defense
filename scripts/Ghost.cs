public partial class Ghost : Enemy
{
	public float speed = 0.06f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	 	InitializeEnemy();
		health = 100.0f;
		name = "ghost";
	}

	public override void _PhysicsProcess(double delta)
	{
		animateMovement((float)delta, speed);
	}

}
