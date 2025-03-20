namespace GameNamespace.UI.DevTools
{
    using GameNamespace.GameManager;
    using Godot;

    public partial class CombatLogConsole: Panel
    {
        public ScrollContainer scrollContainer;
        public VBoxContainer scrollVBox;

        public override void _Ready()
        {
            scrollContainer = GetNode<ScrollContainer>("ScrollContainer");
            scrollVBox = scrollContainer.GetNode<VBoxContainer>("VBoxContainer");
            GameCoordinator.Instance.combatLog = this;
        }

        public void Write(string message)
        {
            Label label = new();
            label.Text = message;

            Theme theme = UITools.Instance.GetGameFontTheme(fontSize:10, gameObject:"Label");
            label.Theme = theme;

            scrollVBox.AddChild(label);
            scrollContainer.ScrollVertical = (int)scrollContainer.GetVScrollBar().MaxValue;
        }
    }
}
