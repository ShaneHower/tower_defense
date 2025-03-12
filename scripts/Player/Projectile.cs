namespace GameNamespace.Player
{
	using Godot;
	using System;
	using GameNamespace.Enemies;
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
		public string prefab;

		// Game objects
		private CollisionPolygon2D projCollider;
		public Enemy target;

		public override void _Ready()
		{
			projCollider = GetNode<CollisionPolygon2D>("Collider");
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
			prefab = data.prefab;
		}

		public override void _Process(double delta)
		{
			try
			{
				// Move projectile towards the target
				CollisionShape2D targetCollider = target.GetChild<CollisionShape2D>(1);
				var dir = (targetCollider.GlobalPosition - GlobalPosition).Normalized();
				GlobalRotation = (float)(dir.Angle() + Math.PI / 2.0f);

				Translate(dir * (float)delta * speed);
			}
			catch(ObjectDisposedException)
			{
				QueueFree();
			}
		}

		public void OnBodyEntered(Node body)
		{
			if(body is Enemy collidedEnemy)
			{
				if(collidedEnemy == target)
				{
					QueueFree();
					target.HitByProjectile(damage);

					if(effect?.ToLower() == "slow")
					{
						target.Slow(effectRate, 1.5f);
					}
				}
			}

		}

	}
}
