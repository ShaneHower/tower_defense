using System.Collections.Generic;
using GameNamespace.Enemies;
using Godot;

public partial class GameCoordinator : Node
{
    public static GameCoordinator Instance { get; private set; }
    public List<Enemy> activeEnemies = new();

    public override void _Ready()
    {
        // This is a singleton class.  It would be bad if we had more than one, so we force duplicates
        // to delete if there are any.
        if(Instance != null)
        {
            QueueFree();
            return;
        }

        Instance = this;
    }
}
