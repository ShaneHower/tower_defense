namespace GameNamespace.GameManager
{
    using GameNamespace.Player;
    using Godot;

    public partial class Ruins : Area2D
    {
        // Ruins are an Area that can create a tower.
        public PlayerInterface playerHud;
        public Sprite2D sprite;

        public override void _Ready()
        {
            Node level = GetTree().CurrentScene;
            CanvasLayer hud = level.GetNode<CanvasLayer>("HUD");
            playerHud = hud.GetNode<PlayerInterface>("PlayerHud");

            sprite = GetNode<Sprite2D>("Sprite2D");

            MouseEntered += OnMouseEnter;
            MouseExited += OnMouseExit;
        }

        private void OnMouseEnter()
        {
            playerHud.ruinsHovered = true;
            playerHud.ruins = this;
        }

        // Called when another body exits the area
        private void OnMouseExit()
        {
            playerHud.ruinsHovered = false;
            playerHud.ruins = null;
        }


    }
}
