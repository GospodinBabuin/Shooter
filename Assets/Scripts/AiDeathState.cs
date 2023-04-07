using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AiDeathState : AiState
{
    public Vector3 direction;

    public void Enter(AiAgent agent)
    {
        agent.ragdoll.ActivateRagdoll();
        agent.gun.BecomeIndependent();
        agent.ragdoll.ApplyForce(direction * agent.config.dieForce);
    }

    public void Exit(AiAgent agent)
    {

    }

    public AiStateId GetId()
    {
        return AiStateId.Death;
    }

    public void Update(AiAgent agent)
    {

    }
}
