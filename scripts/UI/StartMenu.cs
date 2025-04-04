namespace GameNamespace.UI
{
    using Godot;

    public partial class StartMenu : Control
    {
        public VBoxContainer menu;
        public override void _Ready()
        {
            menu = GetNode<VBoxContainer>("Menu");
            CreateStartMenu();
        }

        public void CreateStartMenu()
        {
           Button levelOne = UITools.Instance.CreateButton(text:"Demo", parent:menu, fontSize:40);
           levelOne.Pressed += () => GetTree().ChangeSceneToFile("res://scenes/level1.tscn");

           Button quit = UITools.Instance.CreateButton(text:"Quit", parent:menu, fontSize:40);
           quit.Pressed += () => GetTree().Quit();
        }
    }
}
