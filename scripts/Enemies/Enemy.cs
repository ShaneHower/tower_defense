namespace GameNamespace.Enemies
{
	using Godot;
	using System.Threading.Tasks;
	using GameNamespace.GameManager;
	using GameNamespace.DataBase;

	public partial class Enemy : CharacterBody2D
	{
		// Class vars
		public string id;
		public float defaultRotation = 0.0f;
		public float downRotation = -90.0f;
		public bool slowDown = false;
		public bool isDead = false;
		public bool isDying = false;
		public float health;
		public int gold;
		public string name;
		public float speed;
		public string direction;
		public bool targeted;

		// Game objects
		private Path2D path;
		private PathFollow2D pathFollow;
		private AnimatedSprite2D animator;

		public override void _Ready()
		{
			// Direct parent and decendents.
			pathFollow = GetParent<PathFollow2D>();
			path = pathFollow.GetParent<Path2D>();
			animator = GetNode<AnimatedSprite2D>("Animator");
			animator.SpriteFrames.SetAnimationLoop("death", false);

			// Set class vars
			id = (string)GetMeta("enemyId");
			SetVars();
		}
		private void SetVars()
		{
			// Ping the game DB for Enemy meta data.
			EnemyData enemyData = GameDataBase.Instance.QueryEnemyData(id);
			speed = enemyData.speed;
			health = enemyData.health;
			gold = enemyData.gold;
			name = enemyData.name;
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

			// Task.Delay() expects milliseconds while the animation system works in seconds.
			int framecount = animator.SpriteFrames.GetFrameCount("death");
			float fps = (float)animator.SpriteFrames.GetAnimationSpeed("death");
			float animationDuration = framecount / fps;
			await Task.Delay((int)animationDuration * 1000);

			// Update the current gold
			GameCoordinator.Instance.currentGold += gold;
			Destroy();
		}

		public void Destroy()
		{
			// Remove the enemy from the coordinator's list of enemies
			GameCoordinator.Instance.activeEnemies.Remove(this);
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
