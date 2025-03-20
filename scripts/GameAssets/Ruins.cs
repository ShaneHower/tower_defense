namespace  GameNamespace.GameAssets
{
    using GameNamespace.UI;
    using Godot;
    using Serilog;


    /// <summary>
    /// Ruins are nodes on the map that allow tower placement. Towers can only be placed on a ruin. There can be many
    /// ruins, each ruin needs to communicate to the PlayerHud to tell it if the mouse is triggering the area collider.
    /// It is much easier to grab the persistent PlayerHud object here vs having the PlayerHud look for each ruin and
    /// check for collision.
    /// </summary>
    public partial class Ruins : Area2D
    {
        // Game objects
        public Sprite2D sprite;
        public PlayerControl playerHud;
        private static readonly ILogger log = Log.ForContext<Ruins>();

        public override void _Ready()
        {
            UIControl ui = GetTree().Root.GetNode<UIControl>("Level/UICanvas/UI");
            playerHud = ui.GetNode<PlayerControl>("PlayerHud");
            sprite = GetNode<Sprite2D>("Sprite2D");

            MouseEntered += OnMouseEnter;
            MouseExited += OnMouseExit;

            log.Information($"Ruin {this} with name {this.Name} instantiated.");
        }

        private void OnMouseEnter()
        {
            playerHud.ruinsHovered = true;
            playerHud.ruins = this;
        }

        private void OnMouseExit()
        {
            playerHud.ruinsHovered = false;
            playerHud.ruins = null;
        }

    }
}
