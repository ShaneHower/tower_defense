using Godot;

public partial class Enemy : CharacterBody2D
{
	private Path2D path;
	private PathFollow2D pathFollow;
	private AnimatedSprite2D animator;

	public float defaultRotation = 0.0f;
	public float downRotation = -90.0f;
	public bool slowDown = false;
	public float health;
	public string name;
	public string direction;
	public bool targeted;
	public int targetOrder;

	// Called when the node enters the scene tree for the first time.
	public void InitializeEnemy()
	{
		// Direct parent and decendents.
		pathFollow = GetParent<PathFollow2D>();
		animator = GetNode<AnimatedSprite2D>("Animator");

		// The path parent have to pull from further into the tree.
		path = pathFollow.GetParent<Path2D>();
	}

	public void animateMovement(float delta, float defaultSpeedRatio)
	{
		animator.Play(direction);
		pathFollow.ProgressRatio += defaultSpeedRatio * delta;
	}

	public void animateDeath()
	{
		QueueFree();
	}

	public void hitByProjectile(float damage)
	{
		health = health - damage;
		if(health <= 0)
		{
			animateDeath();
		}


	}

}
