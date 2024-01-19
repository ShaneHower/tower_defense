using Godot;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

public partial class Skeleton : Enemy
{
	public float defaultSpeedRatio = 0.0009f;
	public float slowSpeedRatio = 0.0001f;
	public string name = "skeleton";
	public float health = 100.0f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	 	InitializeEnemy();
	}

	public override void _PhysicsProcess(double delta)
	{
		AnimateEnemy(defaultSpeedRatio, slowSpeedRatio);
	}

}
