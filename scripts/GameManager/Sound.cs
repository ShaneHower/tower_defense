using Godot;

namespace GameNamespace.GameManager
{
    public partial class Sound: Node
    {
        public static Sound Instance { get; private set; }

        public override void _Ready()
        {
            Instance = this;
        }

        public AudioStreamPlayer CreateBackgroundMusic()
        {
            AudioStreamPlayer bgMusic = new AudioStreamPlayer();
            var ogg = GD.Load<AudioStreamOggVorbis>("res://audio/song/6-icy.ogg");
            ogg.Loop = true;
            bgMusic.Stream = ogg;
            bgMusic.VolumeDb = -10;

            return bgMusic;
        }

        public AudioStreamPlayer CreateFoley(string file)
        {
            AudioStreamPlayer foley = new AudioStreamPlayer();
            var ogg = GD.Load<AudioStreamWav>($"res://audio/foley/{file}.wav");
            foley.Stream = ogg;
            foley.VolumeDb = -10;
            return foley;
        }
    }

}
