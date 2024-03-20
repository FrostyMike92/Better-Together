using Godot;
using System;

public partial class CameraTracker : Node2D
{
    private Node2D player1;
    private Node2D player2;
    private Camera2D camera;

    public override void _Ready()
    {
        // Assuming you have references to your players and camera
        player1 = GetNode<Node2D>("PlayerOne"); // Replace "Player1" with the actual path to your first player
        player2 = GetNode<Node2D>("PlayerTwo"); // Replace "Player2" with the actual path to your second player
        camera = GetNode<Camera2D>("Camera2D"); // Replace "Camera" with the actual path to your camera
    }

    public override void _Process(double delta)
    {
        if (player1 != null && player2 != null)
        {
            // Calculate the average x position between the two players
            float averageX = (player1.GlobalPosition.X + player2.GlobalPosition.X) / 2f;

            // Set the camera position to the calculated average x position
            camera.GlobalPosition = new Vector2(averageX, camera.GlobalPosition.Y);
        }
    }
}

