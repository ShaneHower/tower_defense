namespace GameNamespace.UI
{
    using Godot;
    using GameNamespace.GameManager;

    public partial class StartMenu : Control
    {
        public VBoxContainer menu;
        private AudioStreamPlayer bgMusic;
        public override void _Ready()
        {
            menu = GetNode<VBoxContainer>("Menu");
            Sound.Instance.AddToMusicBank("6-icy");
            Sound.Instance.PlayMusic("6-icy", volume: -20f);
            CreateStartMenu();
        }

        public void CreateStartMenu()
        {
            Button levelOne = UITools.Instance.CreateButton(text:"Demo", fontSize:40);
            menu.AddChild(levelOne);
            levelOne.Pressed += () => GetTree().ChangeSceneToFile("res://scenes/level1.tscn");

            Button quit = UITools.Instance.CreateButton(text:"Quit", fontSize:40);
            menu.AddChild(quit);
            quit.Pressed += () => GetTree().Quit();
        }


        public override void _ExitTree()
        {
            Sound.Instance.StopMusic();
        }
    }
}
