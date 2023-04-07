using UnityEngine;

[RequireComponent(typeof(AiLocomotion))]
[RequireComponent(typeof(Footsteps))]
[RequireComponent(typeof(Health))]

public class Enemy : MonoBehaviour
{
    private Footsteps _footsteps;
    private Transform _playerTransform;
    private AiLocomotion _aiLocomotion;
    private Animator _animator;
    private Rigs _rigs;
    [SerializeField] private Transform aimTarget;

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

    private void Start()
    {
        if (_playerTransform == null)
        {
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
        _aiLocomotion = GetComponent<AiLocomotion>();
        _footsteps = GetComponent<Footsteps>();
        _animator = GetComponent<Animator>();
        _rigs = GetComponent<Rigs>();
    }

}
