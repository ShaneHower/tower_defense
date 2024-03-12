public partial class Ghost : Enemy
{
	public float defaultSpeedRatio = 0.001f;
	public float slowSpeedRatio = 0.0001f;
	public string name = "ghost";
	public float health = 100.0f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	 	InitializeEnemy();
	}

	public override void _PhysicsProcess(double delta)
	{
		AnimateEnemy(defaultSpeedRatio, slowSpeedRatio);
	}

}
