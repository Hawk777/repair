using UnityEngine;

public class Target : MonoBehaviour {
	[Tooltip("The type of goop which solves this target")]
	public Material solveGoop;

	void OnTriggerEnter2D(Collider2D other) {
		if(other.CompareTag("Splatter")) {
			if(other.GetComponent<SpriteRenderer>().color == solveGoop.color) {
				foreach(SpriteRenderer renderer in gameObject.GetComponentsInChildren<SpriteRenderer>()) {
					renderer.enabled = !renderer.enabled;
				}
				GetComponent<Rigidbody2D>().simulated = false;
			}
		}
	}
}
