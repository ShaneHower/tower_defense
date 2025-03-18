namespace GameNamespace.GameAssets
{
	using Godot;
	using System.Threading.Tasks;
	using GameNamespace.GameManager;
	using GameNamespace.DataBase;
    using System.Threading;

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
		public bool isSlowed = false;
		public EnemyData passedData;

		// Status affect tracking
		private Task currentSlowTask;
		private CancellationTokenSource slowCanelTokenSource;

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
			// Either the enemyData was passed on instantiation or we have to pull it from the db.
			EnemyData enemyData = passedData ?? GameDataBase.Instance.QueryEnemyData(id);
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

	    /// <summary>
		/// I learned something new here so lets explain it.  Obviously async tasks are tasks that can happen many times
		/// over in parallel.  Sometimes there is a task happening that should take precident over every other of it's
		/// parallel task. In other words, if some behavior X is happening, we want to remove all other previous iterations
		/// of this task that are still running.  In this case, I want my slow effect to reset it's timer whenever the enemy is
		/// hit by a new instance of the slow status effect. To do this, we create a cancellation token for the task that
		/// is tracking the duration of the slow.  So if the enemy is hit again with a slow, it cancels and clears out the
		/// old cancelation token, and creates a new duration timer (with its own cancellation token).  This timer will
		/// run for the full duration unless a new instance of slow is applied.
		/// </summary>
		/// <param name="slowRate"></param>
		/// <param name="duration"></param>
		public async void Slow(float slowRate, float duration)
		{
			if (!isSlowed)
			{
				isSlowed = true;
				speed *= 1-slowRate;
			}
			else
			{
				slowCanelTokenSource?.Cancel();
				slowCanelTokenSource?.Dispose();
			}

			slowCanelTokenSource = new CancellationTokenSource();
			CancellationToken token = slowCanelTokenSource.Token;

			currentSlowTask = Task.Run(async () =>
			{
				try
				{
					float elapsed = 0;
					while(elapsed < duration)
					{
						await Task.Delay(100, token);
						elapsed += 0.10f;
					}

					// Wait time is up
					speed = GameDataBase.Instance.QueryEnemyData(id).speed;
					isSlowed = false;
				}
				catch (TaskCanceledException)
				{

				}
			});
		}

	}
}
