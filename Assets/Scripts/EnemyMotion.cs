using UnityEngine;
using UnityEngine.AI;

public class EnemyMotion : MonoBehaviour {
	[Tooltip("How far away a target or player can be when the enemy starts to chase it")]
	public float viewDistance = 150f;

	[Tooltip("How close to the wandering point to be before picking a new wandering point")]
	public float wanderThreshold = 1f;

	[Tooltip("The maximum distance to wander in one step when wandering")]
	public float wanderStep = 200f;

	[Tooltip("The maximum time the enemy will stay still during wandering")]
	public float wanderDelay = 3f;

	// My own rigid body.
	private Rigidbody2D body;

	// The agent for this enemy.
	private NavMeshAgent agent;

	// The player.
	private Rigidbody2D player;

	// The targets.
	private GameObject[] targets;

	// The wander delay time left.
	private float wanderDelayLeft;

	void Start() {
		body = GetComponent<Rigidbody2D>();
		agent = GetComponent<NavMeshAgent>();
		player = GameObject.FindWithTag("Player").GetComponent<Rigidbody2D>();
		targets = GameObject.FindGameObjectsWithTag("Target");
		wanderDelayLeft = Random.Range(0f, wanderDelay);

		agent.updateRotation = false;
		agent.updateUpAxis = false;
	}

	void FixedUpdate() {
		float viewDistanceSq = viewDistance * viewDistance;

		if((player.position - body.position).sqrMagnitude <= viewDistanceSq) {
			agent.SetDestination(player.position);
		} else {
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
			if(closest != null && closestDistSq <= viewDistanceSq) {
				agent.SetDestination(closest.transform.position);
			} else {
				if(!agent.pathPending && ((Vector2) agent.destination - body.position).sqrMagnitude <= wanderThreshold * wanderThreshold) {
					if(wanderDelayLeft < 0) {
						agent.SetDestination(body.position + Random.insideUnitCircle * wanderStep);
						wanderDelayLeft = Random.Range(0f, wanderDelay);
					} else {
						wanderDelayLeft -= Time.fixedDeltaTime;
					}
				}
			}
		}
	}
}
