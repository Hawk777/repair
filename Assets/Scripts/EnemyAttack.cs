using UnityEngine;

public class EnemyAttack : MonoBehaviour {
	[Tooltip("How far away the target can be before it is attackable")]
	public float distance = 10f;

	[Tooltip("How long the enemy must recharge before attacking again")]
	public float interval = 2f;

	// My own rigid body.
	private Rigidbody2D body;

	// The targets.
	private GameObject[] targets;

	// The recharge time left.
	private float rechargeTimeLeft;

	void Start() {
		body = GetComponent<Rigidbody2D>();
		targets = GameObject.FindGameObjectsWithTag("Target");
		rechargeTimeLeft = 0f;
	}

	void FixedUpdate() {
		if(rechargeTimeLeft <= 0f) {
			float distanceSq = distance * distance;

			GameObject closest = null;
			float closestDistSq = 1f / 0f;
			foreach(GameObject i in targets) {
				if(i != null) { /* overloaded to check not destroyed */
					if(i.GetComponent<Target>().attackable) {
						float distSq = (i.GetComponent<Rigidbody2D>().position - body.position).sqrMagnitude;
						if(distSq < closestDistSq) {
							closest = i;
							closestDistSq = distSq;
						}
					}
				}
			}
			if(closest != null && closestDistSq <= distanceSq) {
				closest.GetComponent<Target>().Hurt();
				rechargeTimeLeft = interval;
			}
		} else {
			rechargeTimeLeft -= Time.fixedDeltaTime;
		}
	}
}

