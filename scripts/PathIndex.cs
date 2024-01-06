using Godot;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public partial class PathIndex : Area2D
{
	public string indexDirection;
    public bool active;

	public override void _Ready()
	{
		indexDirection = (string)GetMeta("direction");
	}

	private void OnEnter(Enemy enemy)
    {
        active = true;
    }

    // Called when another body exits the area
    private void OnExit(Enemy enemy)
    {
        active = false;
    }

}
