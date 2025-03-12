using System.Collections.Generic;
using GameNamespace.Enemies;
using Godot;

namespace GameNamespace.GameManager
{
    /// <summary>
    /// This singlton passes data between all of the scripts at run time.
    /// </summary>
    public partial class GameCoordinator : Node
    {
        public static GameCoordinator Instance { get; private set; }
        public List<Enemy> activeEnemies = new();
        public Level level;
        public bool enemyBreach = false;
        public int breachNum = 0;
        public int currentGold = 0;

        /// <summary>
        /// This is a singleton class.  It would be bad if we had more than one, so we force duplicates
        /// to delete if there are any.
        /// </summary>
        public override void _Ready()
        {
            if(Instance != null)
            {
                QueueFree();
                return;
            }

            Instance = this;
        }
    }
}
