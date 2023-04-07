using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private Transform _bulletSpawnPosition;
    [SerializeField] private bool _addBulletSpread = true;
    [SerializeField] private Vector3 _BulletSpreadVariance = new Vector3(0.1f, 0.1f, 0.1f);

    [SerializeField] private ParticleSystem _gunShotParticleSystem;
    [SerializeField] private ParticleSystem _gunEffectParticleSystem;
    [SerializeField] private ParticleSystem _bulletImpactParticleSystem;
    [SerializeField] private TrailRenderer _bulletTrail;

    private AudioSource _gunAudioSource;
    [SerializeField] private AudioClip ReloadClip;
    [SerializeField] private AudioClip ShootClip;

    [SerializeField] private int _damage;
    [SerializeField] private LayerMask damageLayerMask;
    public int Damage
    {
        get { return _damage; }
        set { _damage = value; }
    }

    private int _clipSize = 30;
    public int ClipSize
    {
        get { return _clipSize; }
        set { _clipSize = value; }
    }

    private int _currentAmmo;
    public int CurrentAmmo
    {
        get { return _currentAmmo; }
        set { _currentAmmo = value; }
    }

    private float _shootDelay = 0.07f;

    private float _shootDelayDelta;

    private void Start()
    {
        _gunAudioSource = GetComponentInChildren<AudioSource>();
        _currentAmmo = _clipSize;
    }

    private void Update()
    {
        if (_shootDelayDelta > 0.0f) _shootDelayDelta -= Time.deltaTime;
        else _shootDelayDelta = 0.0f;
    }

    public void ShootForPlayer(Vector3 direction)
    {
        if (_shootDelayDelta == 0.0f)
        {
            Vector3 newDirection = GetSpreadDirection(direction, _BulletSpreadVariance);

            if (_currentAmmo <= 0) return;

            _gunEffectParticleSystem.Play();
            _gunShotParticleSystem.Play();

            _gunAudioSource.PlayOneShot(ShootClip);

            TrailRenderer trail = Instantiate(_bulletTrail, _bulletSpawnPosition.position, Quaternion.identity);
            StartCoroutine(SpawnTrail(trail, newDirection));

            Vector2 screenCenterPoint = new Vector2(Screen.width / 2.0f, Screen.height / 2.0f);
            Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue))
            {
                Transform hitTransform = raycastHit.transform;

                if (hitTransform != null)
                {
                    HitBox hitBox = hitTransform.GetComponentInParent<HitBox>();
                    if (hitBox != null)
                    {
                        hitBox.OnRaycastHit(this, ray.direction);
                    }
                }
            }

            _currentAmmo--;

            _shootDelayDelta = _shootDelay;
        }
    }

    public void ShootForAi(AiAgent agent, Vector3 direction)
    {
        if (_shootDelayDelta == 0.0f)
        {
            Vector3 newDirection = GetSpreadDirection(direction, agent.config.bulletSpreadVariance);

            if (_currentAmmo <= 0) return;

            _gunEffectParticleSystem.Play();
            _gunShotParticleSystem.Play();

            _gunAudioSource.PlayOneShot(ShootClip);

            TrailRenderer trail = Instantiate(_bulletTrail, _bulletSpawnPosition.position, Quaternion.identity);
            StartCoroutine(SpawnTrail(trail, newDirection));
            Ray ray = new Ray(_bulletSpawnPosition.position, newDirection);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, damageLayerMask))
            {
                Transform hitTransform = raycastHit.transform;
                if (hitTransform != null)
                {
                    HitBox hitBox = hitTransform.GetComponentInParent<HitBox>();
                    if (hitBox != null)
                    {
                        hitBox.OnRaycastHit(this, newDirection);
                    }
                }
            }

            _currentAmmo--;

            _shootDelayDelta = _shootDelay;
        }
    }


    public void Reload()
    {
        _currentAmmo = _clipSize;
    }

    private Vector3 GetSpreadDirection(Vector3 direction, Vector3 spread)
    {
        Vector3 newDirecrion = direction;

        if (_addBulletSpread)
        {
            newDirecrion += new Vector3(
                Random.Range(-spread.x, spread.x),
                Random.Range(-spread.y, spread.y),
                Random.Range(-spread.z, spread.z)
                );

            // newDirecrion.Normalize();
        }

        return newDirecrion;
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, Vector3 direction)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;

        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, direction, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }

        trail.transform.position = direction;
        Instantiate(_bulletImpactParticleSystem, direction, Quaternion.identity);

        Destroy(trail.gameObject, trail.time);
    }

    public void PlayReloadSound()
    {
        _gunAudioSource.PlayOneShot(ReloadClip);
    }

    public void BecomeIndependent()
    {
        transform.SetParent(null);
        gameObject.GetComponent<BoxCollider>().enabled = true;
        gameObject.AddComponent<Rigidbody>();
        Destroy(gameObject, 5.0f);
    }
}
