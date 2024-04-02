using Godot;
using System;
using System.Linq;
using System.Runtime.Intrinsics.X86;

public partial class MainMenu : Node2D
{
	[Export] Control startScreen, levelScreen, settingsScreen, creditsScreen;
	[Export] Control levelButtonParent;
	[Export] AudioStream buttonSound, backButtonSound;

	private LevelManager levelManager;
	private Button[] levelButtons;

	private SoundManager soundManager;

	public override void _Ready()
	{
		// Get reference to the Level Manager
		levelManager = GetNode<LevelManager>("/root/LevelManager");
		soundManager = GetNode<SoundManager>("/root/SoundManager");

		// Make sure that only the start screen is visible on ready
		OpenScreen(startScreen);

		levelButtons = levelButtonParent.GetChildren().Cast<Button>().ToArray();

		foreach (Button button in levelButtons)
		{
			button.Pressed += () => StartAtLevel(button.Text.ToInt());
			button.Pressed += () => PlayButtonSound();
		}
	}

	public void OpenScreen(Control screen)
	{
		startScreen.Visible = false;
		levelScreen.Visible = false;
		settingsScreen.Visible = false;
		creditsScreen.Visible = false;

		screen.Visible = true;
	}

	public void OpenSettingsScreen()
	{
		OpenScreen(settingsScreen);
	}

	public void OpenCreditsScreen()
	{
		OpenScreen(creditsScreen);
	}

	public void OpenLevelScreen()
	{
		OpenScreen(levelScreen);
	}

	public void OpenStartScreen()
	{
		OpenScreen(startScreen);
	}

	public void StartGame()
	{
		// load the first level of the game
		StartAtLevel(1);
	}

	public void StartAtLevel(int startLevel)
	{
		// load the correct level
		levelManager.LoadLevel(startLevel);        
	}

	public void QuitGame()
	{
		GetTree().Quit();
	}

	public void PlayButtonSound()
	{
		soundManager.Play(0, soundManager, buttonSound);
	}

	public void PlayBackButtonSound()
	{
		soundManager.Play(0, soundManager, backButtonSound);
	}
	
	public void SetMasterVolume(float sliderValue)
	{
		// set the volume from the main menu using a slider
		soundManager.SetMasterVolume(sliderValue/100);
	}
}