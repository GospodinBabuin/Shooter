using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]

public class BulletProjectile : MonoBehaviour
{
    public float Speed = 100.0f;
    public ParticleSystem HitParticleSystem;

    private float _destroyBulletTime = 10.0f;
    private float _destroyBulletTimeDelta;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _rigidbody.velocity = transform.forward * Speed;
    }

    private void FixedUpdate()
    {
        if (_destroyBulletTimeDelta <= _destroyBulletTime)
        {
            _destroyBulletTimeDelta += Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Instantiate(HitParticleSystem, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
