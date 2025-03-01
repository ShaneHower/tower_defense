namespace GameNamespace.Player
{
	using Godot;
	using System.Collections.Generic;
	using System.Linq;
	using GameNamespace.Enemies;
    using System.Runtime.Intrinsics;

    public partial class Tower : Area2D
	{
		public string projectilePrefab = "res://prefabs/projectiles/arrow.tscn";
		public PackedScene projectile;
		public float projectileSpeed = 200f;
		public float attackSpeed = 1.0f;
		public bool canFire = true;
		public bool beingPlaced = false;
		public Projectile proj_instance;
		private Dictionary<int, Enemy> targetEnemies = new Dictionary<int, Enemy>();
		private int targetOrder;
		private AnimatedSprite2D animator;
		private Line2D towerRange;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			animator = GetNode<AnimatedSprite2D>("Animator");
			animator.Play("idle");
			targetOrder = 0;

			towerRange = BuildTowerRange();
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			if(beingPlaced)
			{
				towerRange.Visible = true;
			}
			else
			{
				towerRange.Visible = false;
				AttackTarget();
				GD.Print($"{Name} Can Fire: {canFire}");
			}
		}

		private Line2D BuildTowerRange()
		{
			// All of my tower colliders are circles, but this could be a weak point
			// as I'm assuming all colliders will be CircleShape2D, this will break otherwise
			CollisionShape2D collider = GetNode<CollisionShape2D>("Collider");
			CircleShape2D circleCollider = (CircleShape2D)collider.Shape;

			// the higher the value of points the smoother the circle
			float scale = collider.Scale.X;
			float radius = circleCollider.Radius * scale;
			int points = 32;

			Line2D circleLine = new();
			circleLine.Width = 1;
			circleLine.DefaultColor = new Color(1, 0, 0, 1);

			Vector2[] circlePoints = new Vector2[points + 1];
			for(int i = 0; i <= points; i++)
			{
				// Mathf.Tau = 2 * PI
				float angle = (i / (float)points) * Mathf.Tau;
				circlePoints[i] = new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
			}

			circleLine.Points = circlePoints;
			AddChild(circleLine);

			return circleLine;
		}

		private async void AttackTarget()
		{
			if(targetEnemies.Count > 0)
			{
				List<int> keys = new List<int>(targetEnemies.Keys);
				int minKey = keys.Min();
				Enemy target = targetEnemies[minKey];
				GD.Print($"{Name} Enemy Target: {target.Name}");

				if(canFire && !target.isDying)
				{
					canFire = false;
					// Instantiate projectile
					projectile = GD.Load<PackedScene>(projectilePrefab);
					proj_instance = (Projectile) projectile.Instantiate();
					AddChild(proj_instance);
					proj_instance.target = target;
					proj_instance.speed = projectileSpeed;

					// Wait out the attack speed
					await ToSignal(GetTree().CreateTimer(attackSpeed), "timeout");
					canFire = true;
				}

			}
		}

		private void OnEnter(Enemy enemy)
		{
			enemy.targeted = true;
			targetOrder += 1;
			enemy.targetOrder = targetOrder;
			targetEnemies.Add(enemy.targetOrder, enemy);
		}

		// Called when another body exits the area
		private void OnExit(Enemy enemy)
		{
			// For now I'm treating this like a stack, first in first out.
			enemy.targeted = false;
			targetEnemies.Remove(enemy.targetOrder);
		}

	}
}
