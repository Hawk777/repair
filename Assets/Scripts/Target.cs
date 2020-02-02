using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour {
	[Tooltip("The type of goop which solves this target")]
	public Material solveGoop;

	[Tooltip("Whether the target has already been solved")]
	public bool solved;

	[Tooltip("How long it takes the smoke to fully appear")]
	public float smokeFadeTime = 1f;

	[Tooltip("How long the fully appeared smoke remains before disappearing")]
	public float smokeHoldTime = 1f;

	[Tooltip("The number of hit points the target has")]
	public uint hp = 10;

	// The input and output sprites.
	private SpriteRenderer[] inputs, outputs;

	// The cloud sprite.
	private SpriteRenderer cloud;

	void Start() {
		List<SpriteRenderer> inputs_ = new List<SpriteRenderer>();
		List<SpriteRenderer> outputs_ = new List<SpriteRenderer>();
		foreach(Transform i in transform) {
			SpriteRenderer r = i.gameObject.GetComponent<SpriteRenderer>();
			if(i.name == "Input") {
				inputs_.Add(r);
			} else if(i.name == "Output") {
				outputs_.Add(r);
			} else if(i.name == "Cloud") {
				cloud = r;
			}
		}
		inputs = inputs_.ToArray();
		outputs = outputs_.ToArray();
	}

	void OnTriggerEnter2D(Collider2D other) {
		if(other.CompareTag("Splatter")) {
			if(other.GetComponent<SpriteRenderer>().color == solveGoop.color) {
				GetComponent<Rigidbody2D>().simulated = false;
				solved = true;
				StartCoroutine(AnimateSolving());
			}
		}
	}

	private IEnumerator<WaitForSeconds> AnimateSolving() {
		cloud.enabled = true;
		{
			float smokeCounter = 0f;
			while(smokeCounter < smokeFadeTime) {
				smokeCounter += Time.deltaTime;
				Color c = cloud.color;
				c.a = Mathf.Min(1f, smokeCounter / smokeFadeTime);
				cloud.color = c;
				yield return null;
			}
		}
		yield return new WaitForSeconds(smokeHoldTime);
		foreach(SpriteRenderer i in inputs) {
			i.enabled = false;
		}
		foreach(SpriteRenderer i in outputs) {
			i.enabled = true;
		}
		cloud.enabled = false;
	}
}
