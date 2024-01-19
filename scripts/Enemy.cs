using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

public partial class Enemy : CharacterBody2D
{
	private Path2D path;
	private PathFollow2D pathFollow;
	private AnimatedSprite2D animator;

	public float defaultRotation = 0.0f;
	public float downRotation = -90.0f;
	public bool slowDown = false;
	public string direction;

	// Called when the node enters the scene tree for the first time.
	public void InitializeEnemy()
	{
		// Direct parent and decendents.
		pathFollow = GetParent<PathFollow2D>();
		animator = GetNode<AnimatedSprite2D>("SkeletonAnimator");

		// The path parent have to pull from further into the tree.
		path = pathFollow.GetParent<Path2D>();
	}

	public void AnimateEnemy(float defaultSpeedRatio, float slowSpeedRatio)
	{
		animator.Play(direction);
		RotationDegrees = direction == "down" ? downRotation : defaultRotation;
		pathFollow.ProgressRatio += defaultSpeedRatio;
	}


}
