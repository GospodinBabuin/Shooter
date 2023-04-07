using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Rigs : MonoBehaviour
{
    [SerializeField] private Rig _aimRig;
    [SerializeField] private Rig _idleRig;
    [SerializeField] private float _targetAimRigWeight = 0;
    [SerializeField] private float _targetIdleRigWeight = 0;

    private void Update()
    {
        _aimRig.weight = Mathf.Lerp(_aimRig.weight, _targetAimRigWeight, Time.deltaTime * 20.0f);
        _idleRig.weight = Mathf.Lerp(_idleRig.weight, _targetIdleRigWeight, Time.deltaTime * 20.0f);
    }

    public void RigsToAim()
    {
        _targetAimRigWeight = 1.0f;
        _targetIdleRigWeight = 0.0f;
    }
    public void RigsToIdle()
    {
        _targetAimRigWeight = 0.0f;
        _targetIdleRigWeight = 1.0f;
    }

    public void RigsToZero()
    {
        _targetIdleRigWeight = 0.0f;
        _targetAimRigWeight = 0.0f;
    }

    public void RigsToOne()
    {
        _targetIdleRigWeight = 1.0f;
        _targetAimRigWeight = 1.0f;
    }
}
