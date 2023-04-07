using System.Collections.Generic;
using UnityEngine;


public class Footsteps : MonoBehaviour
{
    [SerializeField] private AudioClip[] _footstepAudioClips;
    private readonly float _footstepTimer = 0.1f;
    private float _footstepTimerDelta = 0.0f;

    private void Start()
    {
        _footstepAudioClips = LoadFotstepClips();
    }

    private void Update()
    {
        if (_footstepTimerDelta > 0.0f) _footstepTimerDelta -= Time.deltaTime;
        else _footstepTimerDelta = 0.0f;
    }

    private AudioClip[] LoadFotstepClips()
    {
        List<AudioClip> audioClips = new List<AudioClip>();
        audioClips.Add(Resources.Load<AudioClip>("SFX/SFX Footsteps/Player_Footstep_01"));
        audioClips.Add(Resources.Load<AudioClip>("SFX/SFX Footsteps/Player_Footstep_02"));
        audioClips.Add(Resources.Load<AudioClip>("SFX/SFX Footsteps/Player_Footstep_03"));
        audioClips.Add(Resources.Load<AudioClip>("SFX/SFX Footsteps/Player_Footstep_04"));
        audioClips.Add(Resources.Load<AudioClip>("SFX/SFX Footsteps/Player_Footstep_05"));
        audioClips.Add(Resources.Load<AudioClip>("SFX/SFX Footsteps/Player_Footstep_06"));
        audioClips.Add(Resources.Load<AudioClip>("SFX/SFX Footsteps/Player_Footstep_07"));
        audioClips.Add(Resources.Load<AudioClip>("SFX/SFX Footsteps/Player_Footstep_08"));
        audioClips.Add(Resources.Load<AudioClip>("SFX/SFX Footsteps/Player_Footstep_09"));
        audioClips.Add(Resources.Load<AudioClip>("SFX/SFX Footsteps/Player_Footstep_10"));

        return audioClips.ToArray();
    }

    public void PlayFootstepSound()
    {
        if (_footstepAudioClips.Length > 0)
        {
            var index = Random.Range(0, _footstepAudioClips.Length);
            if (_footstepTimerDelta == 0.0f)
            {
                AudioSource.PlayClipAtPoint(_footstepAudioClips[index], gameObject.transform.position);
                _footstepTimerDelta = _footstepTimer;
            }
        }
    }
}
