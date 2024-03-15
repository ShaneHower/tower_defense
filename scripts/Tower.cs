using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading.Tasks;

public partial class Tower : Node
{
	public PackedScene projectile;
	public float projectileSpeed = 100f;
	public float attackSpeed = 1.0f;
	public bool canFire = true;
	private List<Enemy> targetEnemies = new List<Enemy>();
	Projectile proj_instance;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		AttackTarget(delta);
	}

	private async void AttackTarget(double delta)
	{
		if(targetEnemies.Count > 0)
		{
			Enemy target = targetEnemies.First();

			if(canFire)
			{
				canFire = false;
				// Instantiate projectile
				projectile = GD.Load<PackedScene>("res://prefabs/arrow.tscn");
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
		enemy.targetOrder = targetEnemies.Count + 1;
		targetEnemies.Add(enemy);
    }

    // Called when another body exits the area
    private void OnExit(Enemy enemy)
    {
		// Where should target order be stored?  Technically an enemy shouldn't know what order they are in
		// the list.  If OOO is real life, a skeleton wouldn't know how he is being targeted, just that he is targeted.
		enemy.targeted = false;
		targetEnemies.RemoveAt(enemy.targetOrder  - 1);
    }

}
