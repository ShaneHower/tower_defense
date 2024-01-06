using Godot;
using System;
using System.Collections.Generic;

public partial class Enemy : CharacterBody2D
{
	private Path2D path;
	private PathFollow2D pathFollow;
	private AnimatedSprite2D animator;
	private List<Node2D> pathIndices = new List<Node2D>();

	// Called when the node enters the scene tree for the first time.
	public void InitializeEnemy()
	{
		// Direct parent and decendents.
		pathFollow = GetParent<PathFollow2D>();
		animator = GetNode<AnimatedSprite2D>("SkeletonAnimator");

		// The path parent have to pull from further into the tree.
		path = pathFollow.GetParent<Path2D>();

		// Get all of the path indices on the map.  We use this to change animations.
		GetPathIndices();
	}

	public void AnimateEnemy(float speedRatio)
	{
		Animate();
		pathFollow.ProgressRatio += speedRatio;
	}

	public void GetPathIndices()
	{
		foreach(Node2D child in path.GetChildren())
		{
			if(child.HasMeta("tag"))
			{
				String tag = child.GetMeta("tag").ToString();
				if (tag == "path_index")
				{
					pathIndices.Add(child);
				}
			}
		}
	}

	public void Animate()
	{
		foreach(Node2D index in pathIndices)
		{
            PathIndex pathIndex = index.GetNode<PathIndex>($"/root/TileMap/Path2D/{index.Name}");
            if(pathIndex.active)
            {
                string direction = pathIndex.indexDirection;
                animator.Play(direction);
            }

		}
	}

}
