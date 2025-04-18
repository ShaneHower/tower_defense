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
            Sound.Instance.PlayMusic("6-icy");
            CreateStartMenu();
        }

        public void CreateStartMenu()
        {
            Button levelOne = UITools.Instance.CreateButton(text:"Demo", parent:menu, fontSize:40);
            levelOne.Pressed += () => GetTree().ChangeSceneToFile("res://scenes/level1.tscn");

            Button quit = UITools.Instance.CreateButton(text:"Quit", parent:menu, fontSize:40);
            quit.Pressed += () => GetTree().Quit();
        }


        public override void _ExitTree()
        {
            Sound.Instance.StopMusic();
        }
    }
}
