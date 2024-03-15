using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class Tower : Node
{
	public PackedScene projectile;
	public float projectileSpeed = 100f;
	public float attackSpeed = 1.0f;
	public bool canFire = true;
	public Projectile proj_instance;
	private Dictionary<int, Enemy> targetEnemies = new Dictionary<int, Enemy>();
	private int targetOrder;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		targetOrder = 0;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		AttackTarget();
	}

	private async void AttackTarget()
	{
		if(targetEnemies.Count > 0)
		{
			List<int> keys = new List<int>(targetEnemies.Keys);
			int minKey = keys.Min();
			Enemy target = targetEnemies[minKey];

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
		targetOrder += 1;
		enemy.targetOrder = targetOrder;
		GD.Print($"{enemy.Name} Entered Attack Zone: {enemy.targetOrder}");
		targetEnemies.Add(enemy.targetOrder, enemy);
    }

    // Called when another body exits the area
    private void OnExit(Enemy enemy)
    {
		// For now I'm treating this like a stack, first in first out.
		enemy.targeted = false;
		GD.Print($"{enemy.Name} Leaving Attack Zone: {enemy.targetOrder}");
		targetEnemies.Remove(enemy.targetOrder);
    }

}
