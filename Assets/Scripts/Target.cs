using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour {
	[Tooltip("The type of goop which solves this target")]
	public Material solveGoop;

	[Tooltip("Whether the target has already been solved")]
	public bool solved;

	[Tooltip("How long it takes the smoke to fully appear")]
	public float smokeInTime = 1f;

	[Tooltip("How long the fully appeared smoke remains before disappearing")]
	public float smokeHoldTime = 1f;

	[Tooltip("How long it takes the smoke to fully disappear")]
	public float smokeOutTime = 1f;

	[Tooltip("The number of hit points the target has")]
	public uint hp = 10;

	// The input and output sprites.
	private SpriteRenderer[] inputs, outputs;

	// The cloud sprite.
	private SpriteRenderer cloud;

	// The cloud dispersal particles.
	private ParticleSystem cloudDispersal;

	// The minimap sprite.
	private SpriteRenderer minimapDot;

	// The game manager.
	private GameManager manager;

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
		cloudDispersal = transform.Find("Cloud Dispersal").GetComponent<ParticleSystem>();
		minimapDot = transform.Find("Minimap").GetComponent<SpriteRenderer>();
		manager = GameManager.get();
	}

	void OnTriggerEnter2D(Collider2D other) {
		if(other.CompareTag("Splatter")) {
			if(other.GetComponent<SpriteRenderer>().color == solveGoop.color) {
				GetComponent<Rigidbody2D>().simulated = false;
				solved = true;
				manager.CheckTargets();
				StartCoroutine(AnimateSolving());
			}
		}
	}

	private IEnumerator<WaitForSeconds> AnimateSolving() {
		// Fade in.
		cloud.enabled = true;
		{
			float counter = 0f;
			while(counter < smokeInTime) {
				counter += Time.deltaTime;
				Color c = cloud.color;
				c.a = Mathf.Min(1f, counter / smokeInTime);
				cloud.color = c;
				yield return null;
			}
		}

		// Hold.
		yield return new WaitForSeconds(smokeHoldTime);

		// Swap animals.
		foreach(SpriteRenderer i in inputs) {
			i.enabled = false;
		}
		foreach(SpriteRenderer i in outputs) {
			i.enabled = true;
		}

		// Fade out with particle burst.
		cloudDispersal.Play();
		{
			float counter = smokeOutTime;
			while(counter > 0) {
				counter -= Time.deltaTime;
				Color c = cloud.color;
				c.a = Mathf.Max(0f, counter / smokeOutTime);
				cloud.color = c;
				yield return null;
			}
		}

		// Kill cloud completely.
		cloud.enabled = false;

		// Remove from minimap.
		minimapDot.enabled = false;
	}

	public bool attackable {
		get {
			return !solved && hp != 0;
		}
	}

	public void Hurt() {
		if(--hp == 0) {
			GetComponent<Rigidbody2D>().simulated = false;
			manager.CheckTargets();
			StartCoroutine(AnimateDeath());
		}
	}

	private IEnumerator<WaitForSeconds> AnimateDeath() {
		// Remove animals.
		foreach(SpriteRenderer i in inputs) {
			i.enabled = false;
		}

		// Poof.
		cloudDispersal.Play();
		while(cloudDispersal.isPlaying) {
			yield return new WaitForSeconds(1);
		}

		// Destroy object.
		Destroy(gameObject);
	}
}
