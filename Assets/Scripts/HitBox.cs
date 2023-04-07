using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public Health health;

    public void OnRaycastHit(Gun gun, Vector3 direction)
    {
        health.TakeDamage(gun.Damage, direction);
    }
}
