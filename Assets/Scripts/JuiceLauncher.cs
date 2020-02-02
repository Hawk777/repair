using UnityEngine;

public class JuiceLauncher : MonoBehaviour {
	[Tooltip("The angular velocity at which juice starts dispensing")]
	public float threshold = 70f;

	[Tooltip("The scale factor for distance in world units between car and juice splatter, as a product of angular velocity")]
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

	// The number of seconds until the next splatter can be placed.
	private float splatterCountdown = 0f;

	// The unit vector to the car side where juice should be thrown during
	// positive rotation.
	private static readonly Vector2 sideVector = new Vector2(1, 0);

	void FixedUpdate() {
		if(splatterCountdown > 0f) {
			splatterCountdown -= Time.fixedDeltaTime;
		} else if(tankLevel != 0) {
			Rigidbody2D body = GetComponent<Rigidbody2D>();
			float avel = body.angularVelocity;
			if(Mathf.Abs(avel) >= threshold) {
				float distance = avel * scale;
				Vector2 target = body.GetRelativePoint(sideVector * distance);
				GameObject obj = Instantiate(splatterPrefabs[Random.Range(0, splatterPrefabs.Length)], target, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)), null);
				obj.GetComponent<SpriteRenderer>().color = juiceType.color;
				splatterCountdown = splatterPeriod;
				--tankLevel;
			}
		}
	}
}
