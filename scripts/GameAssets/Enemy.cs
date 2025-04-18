namespace GameNamespace.GameAssets
{
	using Godot;
	using System.Threading.Tasks;
	using GameNamespace.GameManager;
	using GameNamespace.DataBase;
    using System.Threading;
	using Serilog;


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
		public bool reachedEnd;
		public bool isSlowed = false;
		public bool isBurned = false;
		public int pathIndexTarget = 1;
		public EnemyData passedData;

		// Status affect tracking
		private Task currentSlowTask;
		private Task currentBurnTask;
		private CancellationTokenSource slowCancelTokenSource;
		private CancellationTokenSource burnCancelTokenSource;
		private static readonly ILogger log = Log.ForContext<Enemy>();

		// Game objects
		private LevelPath levelPath;
		private AnimatedSprite2D animator;

		public override void _Ready()
		{
			// Set class vars
			id = (string)GetMeta("enemyId");
			SetVars();

			// Direct parent and decendents.
			var spawn = GetParent<Node2D>();
			levelPath = spawn.GetParent<LevelPath>();
			animator = GetNode<AnimatedSprite2D>("Animator");
			animator.SpriteFrames.SetAnimationLoop("death", false);

			log.Information($"Enemy {this} with name {this.Name} has spawned.");
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

		public override void _Process(double delta)
		{
			if (isDead)
			{
				AnimateDeath();
			}

			else if(reachedEnd)
			{
				return;
			}
			else
			{
				PathIndex pathIndex = levelPath.pathIndices[pathIndexTarget];
				animator.Play(pathIndex.direction);
				Vector2 target = pathIndex.Position;
				Vector2 dir = (target - GlobalPosition).Normalized();
				GlobalPosition += dir * speed * (float)delta;
			}
		}

		public async Task AnimateDeath()
		{
			if(isDying) return;
			isDying = true;
			animator.Play("death");

			// Update the gold early so there is no lag on the UI.
			GameCoordinator.Instance.currentGold += gold;

			// Task.Delay() expects milliseconds while the animation system works in seconds.
			int framecount = animator.SpriteFrames.GetFrameCount("death");
			float fps = (float)animator.SpriteFrames.GetAnimationSpeed("death");
			float animationDuration = framecount / fps;
			await Task.Delay((int)animationDuration * 1000);
			string msg = $"Enemy {this} with name {this.Name} is dead.";
			log.Information(msg);
			GameCoordinator.Instance.combatLog.Write(msg);

			Destroy();
		}

		public void Destroy()
		{
			log.Information($"Destroying {this} with name {this.Name}");
			// Remove the enemy from the coordinator's list of enemies
			GameCoordinator.Instance.activeEnemies.Remove(this);
			QueueFree();
		}

		public void HitByProjectile(float damage)
		{
			health -= damage;
			string msg = $"Enemy {this} with name {this.Name} hp was reduced by {damage}. Current health = {health}.";
			log.Information(msg);
			GameCoordinator.Instance.combatLog.Write(msg);
			if(health <= 0)
			{
				isDead = true;
			}
		}

		public async Task Stun(float duration)
		{
			speed = 0;
			var stunEffectNode = GetNode<Node2D>("StunEffect");
			stunEffectNode.Visible = true;
			AnimatedSprite2D stunAnim = stunEffectNode.GetNode<AnimatedSprite2D>("AnimatedSprite2D");
			stunAnim.Play("default");

			// await delay is in milliseconds, we have to multiply the duration by 1000 to convert.
			await Task.Delay((int)(1000 * duration));
			speed = GameDataBase.Instance.QueryEnemyData(id).speed;
			stunEffectNode.Visible = false;
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
		public Task Slow(float slowRate, float duration)
		{
			if (!isSlowed)
			{
				string msg = $"Enemy {this} with name {this.Name} is slowed by {slowRate} for {duration}.";
				log.Information(msg);
				GameCoordinator.Instance.combatLog.Write(msg);
				isSlowed = true;
				speed *= 1-slowRate;
			}
			else
			{
				string msg = $"Enemy {this} with name {this.Name} hit by slow again, restart duration of {duration}";
				log.Information(msg);
				GameCoordinator.Instance.combatLog.Write(msg);
				slowCancelTokenSource?.Cancel();
				slowCancelTokenSource?.Dispose();
			}

			slowCancelTokenSource = new CancellationTokenSource();
			CancellationToken token = slowCancelTokenSource.Token;
			currentSlowTask = RunSlowEffect(duration, token);

			return currentSlowTask;
		}

		private async Task RunSlowEffect(float duration, CancellationToken token)
		{
			try
			{
				animator.Modulate = new Color(0.4f, 0.8f, 1f, 1f);
				float elapsed = 0;
				while(elapsed < duration)
				{
					await Task.Delay(100, token);
					elapsed += 0.10f;
				}

				// Wait time is up
				speed = GameDataBase.Instance.QueryEnemyData(id).speed;
				isSlowed = false;
				animator.Modulate = new Color(1, 1, 1, 1);
			}
			catch (TaskCanceledException)
			{
				log.Information($"Slow effect was cancelled early at Time: {Time.GetTicksMsec()}ms");
			}
		}

		public Task Burn(float burnDamage, float duration)
		{
			if(!isBurned)
			{
				string msg = $"Enemy {this} with name {this.Name} is burned by {burnDamage} for {duration}.";
				log.Information(msg);
				GameCoordinator.Instance.combatLog.Write(msg);
				isBurned = true;
			}
			else
			{
				string msg = $"Enemy {this} with name {this.Name} hit by burn again, restart duration of {duration}";
				log.Information(msg);
				GameCoordinator.Instance.combatLog.Write(msg);
				burnCancelTokenSource?.Cancel();
				burnCancelTokenSource?.Dispose();
			}

			burnCancelTokenSource = new CancellationTokenSource();
			CancellationToken token = burnCancelTokenSource.Token;
			currentBurnTask = RunBurnEffect(burnDamage, duration, token);

			return currentBurnTask;
		}

		private async Task RunBurnEffect(float damage, float duration, CancellationToken token)
		{
			try
			{
				animator.Modulate = new Color(1f, 0.6f, 0.6f, 1f);
				float elapsed = 0;
				while(elapsed < duration)
				{
					// TODO change to every second
					await Task.Delay(1000, token);
					HitByProjectile(damage);
					elapsed ++;
				}

				animator.Modulate = new Color(1f, 1f, 1f, 1f);
				isBurned = false;
			}
			catch (TaskCanceledException)
			{
				log.Information($"Burn effect was cancelled early at Time: {Time.GetTicksMsec()}ms");
			}

		}

	}
}
