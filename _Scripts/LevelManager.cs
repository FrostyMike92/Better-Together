using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class LevelManager : Node
{
    // Transistions to the next level when the player activates both sensors

    private int activeSensors = 0;
    private int totalSensors = 2;

    private const string levelPath = "res://Levels/Level_";
    private int currentLevel = 1;

    public void SetSensorActive()
    {
        activeSensors++;

        if (activeSensors >= totalSensors)
        {
            GD.Print("Level Complete!");

            // Load the next level
            string path = (levelPath + $"{++currentLevel}.tscn");
            GD.Print("Next Level Path:", path);  // Add this line
            LoadLevel(path);

            // Reset the active sensors for the next level
            activeSensors = 0;
        }
    }


    public void SetSensorInactive()
    {
        activeSensors--;

        if (activeSensors < 0)
            activeSensors = 0;
    }

    private void LoadLevel(string path)
    {
        GetTree().ChangeSceneToFile(path);
    }
}
