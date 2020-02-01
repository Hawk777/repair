using UnityEngine;

public class JuiceLauncher : MonoBehaviour {
	[Tooltip("The angular velocity at which juice starts dispensing")]
	public float threshold = 100f;

	[Tooltip("The scale factor for distance in world units between car and juice splatter, as a product of angular velocity above threshold")]
	public float scale = 0.2f;

	[Tooltip("The number of seconds between consecutive splatters")]
	public float splatterPeriod = 0.1f;

	[Tooltip("The available prefabs for splatters")]
	public GameObject[] splatterPrefabs = null;

	[Tooltip("The currently loaded juice type")]
	public Material juiceType = null;

	[Tooltip("The number of splatters of juice left in the tank")]
	public uint tankLevel = 50;

	[Tooltip("The capacity of the juice tank")]
	public uint tankCapacity = 100;

	// The splatter period in ticks, minus one.
	private uint splatterCountdownMax;

	// The number of ticks until the next splatter can be placed.
	private uint splatterCountdown = 0;

	// The unit vector to the car side where juice should be thrown during
	// positive rotation.
	private static readonly Vector2 sideVector = new Vector2(0, -1);

	void Start() {
		splatterCountdownMax = (uint) Mathf.Max(0f, splatterPeriod / Time.fixedDeltaTime);
	}

	void FixedUpdate() {
		if(splatterCountdown != 0) {
			--splatterCountdown;
		} else if(tankLevel != 0) {
			Rigidbody2D body = GetComponent<Rigidbody2D>();
			float avel = body.angularVelocity;
			if(Mathf.Abs(avel) >= threshold) {
				float distance = (avel - Mathf.Sign(avel) * threshold) * scale;
				Vector2 target = body.GetRelativePoint(sideVector * distance);
				GameObject obj = Instantiate(splatterPrefabs[Random.Range(0, splatterPrefabs.Length)], target, Quaternion.identity, null);
				obj.GetComponent<SpriteRenderer>().color = juiceType.color;
				splatterCountdown = splatterCountdownMax;
				--tankLevel;
			}
		}
	}
}
