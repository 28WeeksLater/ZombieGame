using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GasPumpGrab : MonoBehaviour
{
    public void Grab(Rigidbody rigidbody)
    {
        rigidbody.constraints = RigidbodyConstraints.None;
    }
}
