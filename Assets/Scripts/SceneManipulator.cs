using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManipulator : MonoBehaviour
{
	[SerializeField] float gameOverDelayInSeconds = 2f;

	public void LoadStartMenu() {
		SceneManager.LoadScene(0);
	}

	public void LoadLevel(int levelId) {
		SceneManager.LoadScene(levelId);
	}

	public void LoadNextLevel() {
		Debug.Log("Load Next Level");
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	public void LoadGameOver() {
		StartCoroutine(WaitAndLoad());
	}

	IEnumerator WaitAndLoad() {
		Debug.Log("Waiting " + gameOverDelayInSeconds + " seconds to load Game Over.");
		yield return new WaitForSeconds(gameOverDelayInSeconds);
		Debug.Log("Load Game Over");
		SceneManager.LoadScene("Game Over");
	}

	public void QuitGame() {
		Debug.Log("Quit Game");
		Application.Quit();
	}
}
