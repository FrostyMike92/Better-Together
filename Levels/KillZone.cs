using Godot;
using System;

public partial class KillZone : Area2D
{
    [Export] public bool killPlayerOne, killPlayerTwo;

    public void OnBodyEntered(Node body)
    {
        if (body.IsInGroup("player") && body is PlayerMovement player)
        {
            // Check if the body is PlayerMovement
            // PlayerMovement script should have a bool property named isPlayerTwo

            if (player.isPlayerTwo && killPlayerTwo)
            {
                // If it's PlayerTwo and killPlayerTwo is true
                player.KillPlayer(null);
            }
            else if (!player.isPlayerTwo && killPlayerOne)
            {
                // If it's not PlayerTwo and killPlayerOne is true
                player.KillPlayer(null);
            }
        }
    }
}

