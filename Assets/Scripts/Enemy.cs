using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {
	[Tooltip("The thing to attack")]
	public Rigidbody2D target;

	// The agent for this enemy.
	private NavMeshAgent agent;

	void Start() {
		agent = GetComponent<NavMeshAgent>();
		agent.updateRotation = false;
		agent.updateUpAxis = false;
	}

	void FixedUpdate() {
		agent.SetDestination(target.position);
	}
}
