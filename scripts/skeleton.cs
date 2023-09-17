using Godot;
using System;

public partial class skeleton : Node
{
	private PathFollow2D pathFollow;
	private AnimatedSprite2D animator;

	public float speed_ratio = 0.001f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		pathFollow = GetParent<PathFollow2D>();
		animator = GetNode<AnimatedSprite2D>("SkeletonAnimator");
	}

	public override void _PhysicsProcess(double delta)
	{
		animator.Play("walk_right");
		pathFollow.ProgressRatio += speed_ratio;
	}
}
