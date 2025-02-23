namespace GameNamespace.Enemies
{
	using Godot;
	using System.Threading.Tasks;

	public partial class Enemy : CharacterBody2D
	{
		private Path2D path;
		private PathFollow2D pathFollow;
		private AnimatedSprite2D animator;

		public float defaultRotation = 0.0f;
		public float downRotation = -90.0f;
		public bool slowDown = false;
		public bool isDead = false;
		public bool isDying = false;
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
			animator.SpriteFrames.SetAnimationLoop("death", false);

			// The path parent have to pull from further into the tree.
			path = pathFollow.GetParent<Path2D>();
		}

		public void AnimateMovement(float delta, float defaultSpeedRatio)
		{
			animator.Play(direction);
			pathFollow.ProgressRatio += defaultSpeedRatio * delta;
		}

		public async Task AnimateDeath()
		{
			if(isDying) return;
			isDying = true;

			animator.Play("death");

			// Wait out the attack speed.  It makes sense that we get frame count, it makes sense that we get the speed at
			// which the animation plays, but why multiply by 1000? Task.Delay() expects milliseconds while the animation
			// system works in seconds.
			int framecount = animator.SpriteFrames.GetFrameCount("death");
			float fps = (float)animator.SpriteFrames.GetAnimationSpeed("death");
			float animationDuration = framecount / fps;

			await Task.Delay((int)animationDuration * 1000);
			QueueFree();
		}

		public void HitByProjectile(float damage)
		{
			health -= damage;
			if(health <= 0)
			{
				isDead = true;
			}
		}

	}
}
