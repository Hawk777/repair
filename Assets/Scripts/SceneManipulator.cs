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

	public void LoadNextLevel() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	public void LoadGameOver() {
		StartCoroutine(WaitAndLoad());
	}

	IEnumerator WaitAndLoad() {
		yield return new WaitForSeconds(gameOverDelayInSeconds);
		SceneManager.LoadScene("Game Over");
	}

	public void QuitGame() {
		Application.Quit();
	}
}
