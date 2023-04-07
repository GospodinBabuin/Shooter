using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public class Ragdoll : MonoBehaviour
{
    private Animator _animator;
    private Rigidbody[] _rigidbodies;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbodies = GetComponentsInChildren<Rigidbody>();
        DeactivateRagdoll();
    }

    public void ActivateRagdoll()
    {
        foreach (var rigidbody in _rigidbodies)
        {
            rigidbody.isKinematic = false;
        }

        _animator.enabled = false;
    }

    public void DeactivateRagdoll()
    {
        foreach (var rigidbody in _rigidbodies)
        {
            rigidbody.isKinematic = true;
        }

        _animator.enabled = true;
    }

    public void ApplyForce(Vector3 force)
    {
        var rigidBody = _animator.GetBoneTransform(HumanBodyBones.Hips).GetComponent<Rigidbody>();
        force.y = 0.2f;
        rigidBody.AddForce(force, ForceMode.VelocityChange);
    }
}
