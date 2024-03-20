using Godot;
using System;
using System.Collections.Generic;

public partial class SoundManager : Node
{
    [Export]
    public AudioStream MusicStream;

    private AudioStreamPlayer musicPlayer;


    public enum Type
    {
        NON_POSITIONAL,
        POSITIONAL_2D,
    }

    public override void _Ready()
    {
       StartMusic();
    }

    public void Play(int type, Node parent, AudioStream stream, float volumeDb = 0f, float pitchScale = 1f)
    {
        Node audioStreamPlayer;
        switch ((Type)type)
        {
            case Type.NON_POSITIONAL:
                audioStreamPlayer = new AudioStreamPlayer();
                break;
            case Type.POSITIONAL_2D:
                audioStreamPlayer = new AudioStreamPlayer2D();
                break;
            default:
                GD.Print("Invalid audio player type");
                return;
        }

        parent.AddChild(audioStreamPlayer); // replace with this.AddChild(audioStreamPlayer)
        if (audioStreamPlayer is AudioStreamPlayer audioPlayer)
        {
            audioPlayer.Bus = "Effects";
            audioPlayer.Stream = stream;
            audioPlayer.VolumeDb = volumeDb;
            audioPlayer.PitchScale = pitchScale;
            audioPlayer.Play();
            audioPlayer.Connect("finished", Callable.From(audioPlayer.QueueFree));
        }
    }

    // Function to start the music for the game
    public void StartMusic()
    {
        if (MusicStream != null)
        {
            if (musicPlayer == null)
            {
                musicPlayer = new AudioStreamPlayer();
                AddChild(musicPlayer);
                musicPlayer.Stream = MusicStream;
                musicPlayer.Bus = "Music";
                musicPlayer.VolumeDb = 0f; // Adjust as needed
            }
            musicPlayer.Play();
            musicPlayer.VolumeDb = -20f; // Unmute the volume
            // How to make sure that it loops??
        }
    }


    // Function to stop the music in the game
    public void StopMusic()
    {
        if (musicPlayer != null)
        {
            musicPlayer.Stop();
            musicPlayer.VolumeDb = -80f; // Mute the volume
        }
    }

}