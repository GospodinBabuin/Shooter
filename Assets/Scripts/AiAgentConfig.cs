using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class AiAgentConfig : ScriptableObject
{
    public float maxTime = 1.0f;
    public float maxDistance = 1.0f;
    public float dieForce = 20.0f;
    public Vector3 bulletSpreadVariance = new Vector3(1f, 1f, 1f);

}
