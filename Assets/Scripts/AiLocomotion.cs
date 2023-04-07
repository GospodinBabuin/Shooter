using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]

public class AiLocomotion : MonoBehaviour
{
    private AiAgent agent;

    private float _agentSpeed;
    public float AgentSpeed
    {
        get { return _agentSpeed; }
    }

    void Start()
    {
        agent = GetComponent<AiAgent>();
    }

    private void Update()
    {
        if (agent.navMeshAgent.enabled)
        {
            if (Vector3.Distance(agent._playerTransform.position, transform.position) < 15f)
            {
                agent.stateMachine.ChangeState(AiStateId.Combat);
            }
            else
            {
                agent.stateMachine.ChangeState(AiStateId.ChasePlayer);
            }
            _agentSpeed = agent.navMeshAgent.velocity.magnitude;
            agent._animator.SetFloat(agent.AnimIDSpeed, _agentSpeed);
        }
    }

    public void DeactivateNavMeshAgent()
    {
        agent.navMeshAgent.enabled = false;
    }
}
