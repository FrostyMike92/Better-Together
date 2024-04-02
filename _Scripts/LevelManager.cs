using Godot;
using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

// Transistions to the next level when the player activates both sensors
public partial class LevelManager : Node
{
	[Export] AudioStream levelCompleteSound;

	// Variables
	private int activeSensors = 0;
	private int totalSensors = 2;

	// Constant paths - make sure levels are stored here
	private const string levelPath = "res://Levels/Level_";
	private const string mainMenuPath = "res://Levels/Main_Menu.tscn";
	private string queuedLevelPath;

	private int currentLevel = 1; // default
	private int maxLevel = 19; // adjust this as we add more levels

	private AnimationPlayer animPlayer;
	private Callable animationFinishedCallable;
	private SoundManager soundManager;

	public override void _Ready()
	{
		// Get the animation player reference
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		soundManager = GetNode<SoundManager>("/root/SoundManager");
	}

	public void SetSensorActive()
	{
		activeSensors++;
		if (activeSensors >= totalSensors)
		{            
			CallDeferred("LoadLevel", ++currentLevel);  // Load the next level
			activeSensors = 0;  // Reset the active sensors for the next level
		}
	}

	public void SetSensorInactive()
	{
		activeSensors--;
		if (activeSensors < 0)
			activeSensors = 0;
	}

	public void LoadLevel(int levelToLoad)
	{
		currentLevel = levelToLoad;

		levelToLoad = (levelToLoad % (maxLevel + 1) + maxLevel + 1) % (maxLevel + 1);

		string path = (levelToLoad == 0) ? mainMenuPath : $"{levelPath}{levelToLoad}.tscn"; // if level = 0, load menu

		if (levelToLoad != 0) soundManager.Play(0, soundManager, levelCompleteSound);

		animationFinishedCallable = new Callable(this, MethodName.OnTransitionAnimationFinished);
		animPlayer.Connect("animation_finished", animationFinishedCallable); // connect animation_finished signal
		animPlayer.Play("transition"); // play animation
		queuedLevelPath = path; // Store the path to load after the animation finishes
	}

	// when the animation is finished we call this method
	public void OnTransitionAnimationFinished(string name)
	{        
		GetTree().ChangeSceneToFile(queuedLevelPath);   // Change to the next level
		animPlayer.Disconnect("animation_finished", animationFinishedCallable); // disconnect signal to avoid looping method
		animPlayer.PlayBackwards("transition"); // play the transition animation in reverse
	}
}
