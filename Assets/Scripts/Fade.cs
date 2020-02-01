using UnityEngine;

public class Fade : MonoBehaviour {
	[Tooltip("How long, in seconds, to wait before starting to fade")]
	public float delay = 1f;

	[Tooltip("How long, in seconds, the fade takes to complete after starting")]
	public float period = 1f;

	// The sprite renderer.
	private SpriteRenderer sprite;

	// The initial alpha value.
	private float initialAlpha = 0f;

	// The amount of alpha decrease per second.
	private float alphaSlope = 0f;

	// How far through the period the fade is.
	private float periodProgress = 0f;

	void Start() {
		sprite = GetComponent<SpriteRenderer>();
		initialAlpha = sprite.color.a;
		alphaSlope = -initialAlpha / period;
	}

	void Update() {
		float dt = Time.deltaTime;
		if(delay != 0f) {
			float sub = Mathf.Min(delay, dt);
			delay -= sub;
			dt -= sub;
		}
		periodProgress += dt;
		if(periodProgress < period) {
			Color c = sprite.color;
			c.a = initialAlpha + alphaSlope * periodProgress;
			sprite.color = c;
		} else {
			Destroy(gameObject);
		}
	}
}
