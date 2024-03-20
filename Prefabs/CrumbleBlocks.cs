using Godot;
using System;

public partial class CrumbleBlocks : Node2D
{
    public void FadeComplete(string animName)
    {
        GD.Print("animation finished");
        QueueFree();
    }
}
