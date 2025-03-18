namespace GameNamespace.GameAssets
{
	using Godot;
	using System;
	using System.Threading.Tasks;
    using GameNamespace.DataBase;

    public partial class Projectile : Area2D
	{
		// Class vars
		public string id;
        public string name;
        public float speed;
        public float damage;
		public string effect;
		public float effectRate;
		public float effectDuration;
		public bool aoe;
		public float aoeRange;
		public float aoeDamagePerc;
		public string prefab;

		// Helper Vars
		public bool aoeActive;

		// Game objects
		private CollisionPolygon2D projCollider;
		private Sprite2D sprite;
		public Enemy target;

		public override void _Ready()
		{
			projCollider = GetNode<CollisionPolygon2D>("Collider");
			sprite = GetNode<Sprite2D>("Sprite");
			id = (string)GetMeta("projectileId");
			SetVars();

			BodyEntered += OnBodyEntered;
		}

		public void SetVars()
		{
			// Ping the game DB for Projectile meta data.
			ProjectileData data = GameDataBase.Instance.QueryProjectileData(id);
			name = data.name;
			speed = data.speed;
			damage = data.damage;
			effect = data.effect;
			effectRate = data.effectRate;
			effectDuration = data.effectDuration;
			aoe = data.aoe;
			aoeRange = data.aoeRange;
			aoeDamagePerc = data.aoeDamagePerc;

			prefab = data.prefab;
		}

		public override void _Process(double delta)
		{
			try
			{
				Vector2 dir;
				if(aoeActive)
				{
					return;
				}
				else {
					// Move projectile towards the target
					CollisionShape2D targetCollider = target.GetChild<CollisionShape2D>(1);
					dir = (targetCollider.GlobalPosition - GlobalPosition).Normalized();
					GlobalRotation = (float)(dir.Angle() + Math.PI / 2.0f);
					Translate(dir * (float)delta * speed);
				}
			}
			catch(ObjectDisposedException)
			{
				QueueFree();
			}
		}

		public async void OnBodyEntered(Node body)
		{
			if(body is Enemy collidedEnemy)
			{
				if(collidedEnemy == target)
				{
					target.HitByProjectile(damage);
					applyEffects(target);

					if(aoe)
					{
						await HandleAOE();
					}

					QueueFree();
				}
			}

		}

		private void applyEffects(Enemy enemy)
		{
			if(effect?.ToLower() == "slow")
			{
				enemy.Slow(effectRate, 1.5f);
			}
		}

		private async Task HandleAOE()
		{
			sprite.Visible = false;
			projCollider.Visible = false;
			aoeActive = true;
			Area2D aoeArea = GetNode<Area2D>("AOE");
			AnimatedSprite2D animator = aoeArea.GetNode<AnimatedSprite2D>("AnimatedSprite2D");
			aoeArea.Visible = true;
			aoeArea.BodyEntered += OnAoeEnter;

			animator.Play("default");
			int framecount = animator.SpriteFrames.GetFrameCount("default");
			float fps = (float)animator.SpriteFrames.GetAnimationSpeed("default");
			float animationDuration = framecount / fps;
			await Task.Delay((int)animationDuration * 1000);
		}

		private void OnAoeEnter(Node body)
		{
			if(body is Enemy collidedEnemy)
			{
				// Want to apply the effect to everything but the target, otherwise we would be applying the effect twice
				// to the original target
				if(collidedEnemy != target)
				{
					collidedEnemy.HitByProjectile((float)(damage * aoeDamagePerc));
					applyEffects(collidedEnemy);
				}
			}
		}

	}
}
