public partial class Slime: Enemy
{
	public float speed = 0.05f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	 	InitializeEnemy();
		health = 60.0f;
		name = "slime";
	}

	public override void _PhysicsProcess(double delta)
	{
		animateMovement((float)delta, speed);
	}

}
