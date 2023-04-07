using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiChasePlayerState : AiState
{
    private Transform _playerTransform;
    private float timer = 0.0f;
    public Transform PlayerTransform
    {
        get { return _playerTransform; }
    }

    public void Enter(AiAgent agent)
    {
        if (_playerTransform == null)
        {
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    public void Exit(AiAgent agent)
    {

    }

    public AiStateId GetId()
    {
        return AiStateId.ChasePlayer;
    }

    public void Update(AiAgent agent)
    {
        if (!agent.enabled) return;

        timer -= Time.deltaTime;

        if (!agent.navMeshAgent.hasPath) agent.navMeshAgent.destination = _playerTransform.position;

        if (timer <= 0.0f)
        {
            if (agent.navMeshAgent.pathStatus != NavMeshPathStatus.PathPartial)
            {
                agent.navMeshAgent.destination = _playerTransform.position;
            }
            timer = agent.config.maxTime;
        }

        if (agent.navMeshAgent.enabled)
        {
            agent.navMeshAgent.destination = _playerTransform.position;
            if (agent._aiLocomotion.AgentSpeed > 0)
            {
                agent._rigs.RigsToIdle();
                agent._animator.SetBool(agent.AnimIDAim, false);
            }
        }
    }
}
