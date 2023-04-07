using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Ragdoll))]
public class Health : MonoBehaviour
{
    [SerializeField] private int _maxHealh = 100;
    [SerializeField] private int _currentHeath;
    [SerializeField] private float _dieForce;
    private bool _isDead = false;

    [SerializeField] private Transform _deathParticlesPosition;
    [SerializeField] private ParticleSystem _deathParticles;

    [Header("TakeDamageBlink")]
    [SerializeField] private float blinkIntencity;
    [SerializeField] private float blinkDuration;
    private float _blinkDurationDelta = 0.0f;
    private float _blinkIntencityDefault = 1.0f;

    private AiAgent _agent;

    private void Start()
    {
        _agent = GetComponent<AiAgent>();
        _currentHeath = _maxHealh;

        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rigidbody in rigidbodies)
        {
            HitBox hitBox = rigidbody.AddComponent<HitBox>();
            hitBox.health = this;
            if (hitBox.gameObject != gameObject)
            {
                hitBox.gameObject.layer = LayerMask.NameToLayer("HitBox");
            }
        }
    }

    private void Update()
    {
        TakeDamageBlink();
    }

    public void TakeDamage(int damage, Vector3 direction)
    {
        _currentHeath -= damage;
        _blinkDurationDelta = blinkDuration;

        if (_currentHeath <= 0)
        {
            if (!_isDead)
            {
                AiLocomotion aiLocomotion = GetComponent<AiLocomotion>();
                aiLocomotion.DeactivateNavMeshAgent();
                Die(direction);
                _isDead = true;
            }
        }
    }

    private void Die(Vector3 direction)
    {
        AiDeathState deathState = _agent.stateMachine.GetState(AiStateId.Death) as AiDeathState;
        deathState.direction = direction;
        _agent.stateMachine.ChangeState(AiStateId.Death);

        _deathParticles = Resources.Load<ParticleSystem>("DeathParticle");
        Instantiate(_deathParticles, _deathParticlesPosition.position, Quaternion.identity);

        Destroy(gameObject, 5.0f);
    }

    private void TakeDamageBlink()
    {
        if (_blinkDurationDelta > 0)
        {
            _blinkDurationDelta -= Time.deltaTime;
            float lerp = Mathf.Clamp01(_blinkDurationDelta / blinkDuration);
            float intencity = (lerp * blinkIntencity) + _blinkIntencityDefault;
            _agent.skinnedMeshRenderer.material.color = Color.white * intencity;
        }
    }
}
