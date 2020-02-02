using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {
	[Tooltip("The scene of the first level")]
	public string firstLevel = "Level 1";

	[Tooltip("The scene of the main menu")]
	public string mainMenu = "Start Menu";

	// Goes to the first level.
	public void FirstLevel() {
		SceneManager.LoadSceneAsync(firstLevel);
	}

	// Goes to the main menu.
	public void MainMenu() {
		SceneManager.LoadSceneAsync(mainMenu);
	}

	// Quits the game.
	public void Quit() {
		Application.Quit();
	}
}
