namespace GameNamespace.Player
{
	using Godot;
	using System;
	using GameNamespace.Enemies;

	public partial class Projectile : Area2D
	{
		public string name;
		public Enemy target;
		public float speed;
		public float damage = 20;
		private CollisionShape2D projCollider;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			projCollider = GetNode<CollisionShape2D>("Collider");
			name = (string)GetMeta("name");

			BodyEntered += OnBodyEntered;
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
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
				}
			}

		}

	}
}
