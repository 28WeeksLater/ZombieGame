using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyCollider : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    void FixedUpdate()
    {
        transform.rotation = new Quaternion(0, 0, 0, transform.rotation.w);
        transform.position = target.position + Vector3.up * offset.y + Vector3.ProjectOnPlane(target.right, Vector3.up).normalized * offset.x
    + Vector3.ProjectOnPlane(target.forward, Vector3.up).normalized * offset.z;
    }
}
