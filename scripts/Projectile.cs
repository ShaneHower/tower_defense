using Godot;
using System;

public partial class Projectile : CharacterBody2D
{
	public string name;
	public Enemy target;
	public float speed;
	public float damage = 20;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		name = (string)GetMeta("name");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		try
		{
			// Move projectile towards the target
			CollisionShape2D target_collider = target.GetChild<CollisionShape2D>(1);
			var dir = (target_collider.GlobalPosition - GlobalPosition).Normalized();
			GlobalRotation = (float)(dir.Angle() + Math.PI / 2.0f);
			var collider = MoveAndCollide(dir * (float)delta * speed);


			if(collider != null)
			{
				Node collided_node = (Node)collider.GetCollider();
				if(collided_node is Enemy)
				{
					// I'm anticipating hoards of enemies, I don't want a projectile to hit an enemy that is not
					// the original target.  If only deal damage and kill the projectile by target order, I should
					// avoid this issue.
					Enemy collided_enemy = (Enemy)collided_node;
					if(collided_enemy.targetOrder == target.targetOrder)
					{
						QueueFree();
						target.hitByProjectile(damage);
					}
				}
			}
		}
		catch(ObjectDisposedException)
		{
			QueueFree();
		}
	}

}
