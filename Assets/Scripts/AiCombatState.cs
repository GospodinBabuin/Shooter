using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Windows;

public class AiCombatState : AiState
{
    private float _reloadTime = 3.0f;
    private float _reloadTimeDelta = 0.0f;
    Enemy enemy;
    public AiStateId GetId()
    {
        return AiStateId.Combat;
    }

    public void Enter(AiAgent agent)
    {
        if (agent._playerTransform == null)
        {
            agent._playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }

    }

    public void Update(AiAgent agent)
    {
        if (agent.gun.CurrentAmmo > 0)
        {
            AimAtTarget(agent);
            agent.gun.ShootForAi(agent, agent.aimTarget.position);
        }
        else
        {
            agent._rigs.RigsToZero();
            agent._animator.SetBool(agent.AnimIDReload, true);
        }
    }

    public void Exit(AiAgent agent)
    {

    }

    private void AimAtTarget(AiAgent agent)
    {
        agent.aimTarget.position = agent._playerTransform.position + new Vector3(0, 1.3f, 0);
        Vector3 worldAimTarget = agent.aimTarget.position;
        worldAimTarget.y = agent.transform.position.y;
        Vector3 aimDirection = (worldAimTarget - agent.transform.position).normalized;

        agent.transform.forward = Vector3.Lerp(agent.transform.forward, aimDirection, Time.deltaTime * 20.0f);
        agent._rigs.RigsToAim();
        agent._animator.SetBool(agent.AnimIDAim, true);
    }
}
