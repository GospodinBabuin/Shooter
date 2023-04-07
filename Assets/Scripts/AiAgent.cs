using UnityEngine;
using UnityEngine.AI;

public class AiAgent : MonoBehaviour
{
    public AiStateMachine stateMachine;
    public AiStateId initialState;
    public NavMeshAgent navMeshAgent;
    public AiAgentConfig config;
    public Ragdoll ragdoll;
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public Gun gun;
    public Footsteps _footsteps;
    public Transform _playerTransform;
    public AiLocomotion _aiLocomotion;
    public Animator _animator;
    public Rigs _rigs;
    public Transform aimTarget;

    private int _animIDSpeed;
    public int AnimIDSpeed
    {
        get { return _animIDSpeed; }
    }
    private int _animIDAim;
    public int AnimIDAim
    {
        get { return _animIDAim; }
    }
    private int _animIDReload;
    public int AnimIDReload
    {
        get { return _animIDReload; }
    }

    void Start()
    {
        if (_playerTransform == null)
        {
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
        AssignAnimationIDs();
        _aiLocomotion = GetComponent<AiLocomotion>();
        _footsteps = GetComponent<Footsteps>();
        _animator = GetComponent<Animator>();
        _rigs = GetComponent<Rigs>();
        ragdoll = GetComponent<Ragdoll>();
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        gun = GetComponentInChildren<Gun>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        stateMachine = new AiStateMachine(this);
        stateMachine.RegisterState(new AiChasePlayerState());
        stateMachine.RegisterState(new AiDeathState());
        stateMachine.RegisterState(new AiCombatState());
        stateMachine.ChangeState(initialState);
    }

    void Update()
    {
        stateMachine.Update();
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDAim = Animator.StringToHash("Aim");
        _animIDReload = Animator.StringToHash("Reload");
    }

    private void OnFootstep()
    {
        _footsteps.PlayFootstepSound();
    }

    public void OnReloadEnd()
    {
        _rigs.RigsToOne();
        gun.Reload();
        _animator.SetBool(_animIDReload, false);
    }

    public void OnPlayReloadSound()
    {
        gun.PlayReloadSound();
    }
}
