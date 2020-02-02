using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	[Tooltip("How many targets the player must rescue in order to win")]
	public uint winTargets = 5;

	[Tooltip("The scene name to migrate to on victory")]
	public string winScene = "Game Over";

	[Tooltip("The scene name to migrate to on loss")]
	public string loseScene = "Game Over";

	[Tooltip("How long to wait before migrating to the next scene")]
	public float delay = 5f;

	[Tooltip("The message to display if the game is lost due to lack of targets")]
	public GameObject outOfTargetsMessage;

	// All targets.
	private Target[] targets;

	// Whether the outcome has been determined and no further calls should
	// work.
	private bool locked = false;

	void Start() {
		GameObject[] targetObjects = GameObject.FindGameObjectsWithTag("Target");
		targets = new Target[targetObjects.Length];
		for(uint i = 0; i != targetObjects.Length; ++i) {
			targets[i] = targetObjects[i].GetComponent<Target>();
		}
	}

	// Gets the GameManager object. This is expensive so it should only be
	// called in Start or other initialization routines.
	public static GameManager get() {
		return GameObject.Find("Game Manager").GetComponent<GameManager>();
	}

	// Checks the targets to see if the player has won or lost.
	public void CheckTargets() {
		uint solved = 0, alive = 0;
		foreach(Target i in targets) {
			if(i.solved) {
				++solved;
			} else if(i.hp != 0) {
				++alive;
			}
		}

		if(solved >= winTargets) {
			Win();
		} else if(solved + alive < winTargets) {
			LoseByTargets();
		}
	}

	// Initiates victory.
	public void Win() {
		if(!locked) {
			locked = true;
			StartCoroutine(DelayedAction(winScene, false));
		}
	}

	// Initiates defeat due to not enough targets left.
	public void LoseByTargets() {
		if(!locked) {
			locked = true;
			StartCoroutine(DelayedAction(loseScene, true));
		}
	}

	// Initiates defeat due to car exploded.
	public void LoseByCar() {
		if(!locked) {
			locked = true;
			StartCoroutine(DelayedAction(loseScene, false));
		}
	}

	private IEnumerator<WaitForSeconds> DelayedAction(string nextScene, bool showTargetsMessage) {
		if(showTargetsMessage) {
			outOfTargetsMessage.SetActive(true);
		}
		yield return new WaitForSeconds(delay);
		SceneManager.LoadSceneAsync(nextScene);
	}
}
