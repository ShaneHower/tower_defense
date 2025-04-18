using System;
using System.Collections.Generic;
using System.Linq;
using GameNamespace.UI;
using Godot;

namespace GameNamespace.GameManager
{
    public partial class Sound: Node
    {
        public static Sound Instance { get; private set; }
        public Dictionary<string, AudioStreamWav> soundBank = new();
        public Dictionary<string, AudioStreamOggVorbis> musicBank = new();
        private AudioStreamPlayer musicPlayer;

        public override void _Ready()
        {
            Instance = this;

            musicPlayer = new AudioStreamPlayer();
            musicPlayer.Bus = "Music";
            musicPlayer.Autoplay = false;
            AddChild(musicPlayer);
        }

        public void AddToSoundBank(string sfx)
        {
            soundBank[sfx] = GD.Load<AudioStreamWav>($"res://audio/foley/{sfx}.wav");
        }

        public void AddToMusicBank(string song)
        {
            musicBank[song] = GD.Load<AudioStreamOggVorbis>($"res://audio/song/{song}.ogg");
        }

        public void PlayFoley(string sfx, float pitch = 1f, float volume = -2f)
        {
            if(!soundBank.ContainsKey(sfx)) return;

            AudioStreamPlayer sfxPlayer = new AudioStreamPlayer();
            sfxPlayer.Stream = soundBank[sfx];
            sfxPlayer.PitchScale = pitch;
            sfxPlayer.VolumeDb = volume;
            sfxPlayer.Bus = "SFX";
            AddChild(sfxPlayer);

            // Play and delete the stream player when done
            sfxPlayer.Play();
            sfxPlayer.Connect("finished", Callable.From(() => sfxPlayer.QueueFree()));
        }

        public void PlayMusic(string song, bool loop = true, float volume = -2f)
        {
            if(!musicBank.ContainsKey(song)) return;

            musicBank[song].Loop = loop;
            musicPlayer.Stream = musicBank[song];
            musicPlayer.VolumeDb = volume;
            musicPlayer.Play();
        }

        public void StopMusic()
        {
            musicPlayer.Stop();
        }
    }

}
