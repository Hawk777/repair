using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {
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
