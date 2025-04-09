namespace  GameNamespace.GameAssets
{
	using Godot;
	using System.Collections.Generic;
	using GameNamespace.GameManager;
    using GameNamespace.DataBase;
	using Serilog;
    using GameNamespace.UI;
    using System.Threading.Tasks;
    using System;



    /// <summary>
    /// Basic tower class. Holds high level behavior and meta data about all towers.
    /// </summary>
    public partial class Tower : Area2D
	{
		// Data
		public string id;
		public string type;
		public string name;
		public int level;
		public float radius;
		public int gold;
		public float attackSpeed;
		public string prefab;
		public string projectileId;
		public string attackModifier;
		public int attackModCounter;
		public int multiShot;
		public string nextLevelId;

		// Class vars
		public int attackCounter = 1;
		public bool canFire = true;
		public bool beingPlaced = false;
		private List<Enemy> targetEnemies = new();
		private int targetOrder = 0;
		public bool isHovered = false;
		private static readonly ILogger log = Log.ForContext<Tower>();

		// Game objects
		public PackedScene projectile;
		public Projectile proj_instance;
		public AnimatedSprite2D animator;
		private Line2D towerRange;
		private Area2D hoverArea;
		// Tower UI
		public bool upgradeButtonHovered = false;

		public override void _Ready()
		{
			// Init work
			id = (string)GetMeta("towerId");
			SetVars();

			animator = GetNode<AnimatedSprite2D>("Animator");
			animator.Play("idle");

			// Tower range outline
			var collider = GetNode<CollisionShape2D>("Collider");
			towerRange = UITools.Instance.CreateCircleColliderOutline(collider);
			AddChild(towerRange);

			// Detect player mouse
			hoverArea = GetNode<Area2D>("HoverArea");
			hoverArea.MouseEntered += () => isHovered = true;
			hoverArea.MouseExited += () => isHovered = false;

			// Connect signals
			Connect("body_entered", new Callable(this, nameof(OnEnter)));
			Connect("body_exited", new Callable(this, nameof(OnExit)));

			log.Information($"Tower {this} with name {Name} instantiated.");
		}

		public void SetVars()
		{
			// Ping the game DB for Enemy meta data.
			TowerData data = GameDataBase.Instance.QueryTowerData(id);
			type = data.type;
			name = data.name;
			level = data.level;
			radius = data.radius;
			gold = data.gold;
			attackSpeed = data.attackSpeed;
			prefab = data.prefab;
			projectileId = data.projectileId;
			attackModifier = data.attackModifier;
			attackModCounter = data.attackModCounter;
			multiShot = data.multiShot;
			nextLevelId = data.nextLevelId;
		}

		public override void _Process(double delta)
		{
			if(beingPlaced)
			{
				towerRange.Visible = true;
				hoverArea.Visible = false;
			}
			else
			{
				towerRange.Visible = false;
				hoverArea.Visible = true;
				CheckAndResolveAttack();
			}
		}

		private async void CheckAndResolveAttack()
		{
			if(targetEnemies.Count != 0 && canFire)
			{
				canFire = false;

				if(attackCounter * attackModCounter != 0 && attackCounter % attackModCounter == 0)
				{
					AttackTarget(attackModifier);
				}
				else
				{
					AttackTarget(projectileId);
				}

				// If some how the tower deletes we resolve any hanging async tasks.
				await ToSignal(GetTree().CreateTimer(20/attackSpeed), "timeout");
				if (!IsInsideTree()) return;

				canFire = true;
				attackCounter ++;
			}
		}

		private async void AttackTarget(string projectileId, int targetIndex=0)
		{
			try
			{
				// Sometimes an enemy may have been killed by another tower in this case the tower can get stuck looking for an enemy that no longer exists.
				Enemy target = targetEnemies[targetIndex];
				if(target.isDying)
				{
					// If the target is dying move onto the next target
					target = targetEnemies[targetIndex+1];
				}

				if(GameCoordinator.Instance.activeEnemies.Contains(target))
				{
					log.Information($"Tower {this} with name {Name} is attacking Enemy {target}");
					SpawnProjectile(projectileId, target);

					// Resolve multishot if applicable/
					if(targetIndex == 0 && multiShot != 0)
					{
						for(int i = 1; i < multiShot; i++)
						{
							// If some how the tower deletes we resolve any hanging async tasks.
							await Task.Delay(50);
							if (!IsInsideTree()) return;

							log.Information($"Multishot triggered.");
							AttackTarget(projectileId, targetIndex=i);
						}
					}
				}
			}
			catch(ArgumentOutOfRangeException)
			{
				return;
			}
		}

		private void SpawnProjectile(string projectileId, Enemy target)
		{
			ProjectileData projectileData = GameDataBase.Instance.QueryProjectileData(projectileId);
			string projectilePrefab = projectileData.prefab;
			projectile = GD.Load<PackedScene>($"{GameCoordinator.Instance.projectilePrefabLoc}/{projectilePrefab}");
			proj_instance = (Projectile) projectile.Instantiate();
			AddChild(proj_instance);
			proj_instance.target = target;
		}

		private void OnEnter(Node body)
		{
			if(body is Enemy enemy)
			{
				enemy.targeted = true;
				targetEnemies.Add(enemy);
			}
		}

		private void OnExit(Node body)
		{
			if(body is Enemy enemy)
			{
				// For now I'm treating this like a stack, first in first out.
				enemy.targeted = false;
				targetEnemies.Remove(enemy);
			}

		}

		public async Task AnimateSpawn()
		{
			var origPosition = Position;
			Position = new Vector2(Position.X, Position.Y - 12);
			string spawnAnim = "spawn";
			animator.Play(spawnAnim);
			int framecount = animator.SpriteFrames.GetFrameCount(spawnAnim);
			float fps = (float)animator.SpriteFrames.GetAnimationSpeed(spawnAnim);
			float animationDuration = framecount / fps;
			await Task.Delay((int)(animationDuration * 1000));
			Position = origPosition;
			animator.Play("idle");

		}

		public override void _ExitTree()
		{
			Disconnect("body_entered", new Callable(this, nameof(OnEnter)));
			Disconnect("body_exited", new Callable(this, nameof(OnExit)));
		}

	}
}
