using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;
public class Climber : MonoBehaviour
{

    private CharacterController character;
    public static XRController climbingHand;
    private DeviceBasedContinuousMoveProvider cm;
    private Rigidbody rigid;
    void Start()
    {
        character = GetComponent<CharacterController>();
        cm = GetComponent<DeviceBasedContinuousMoveProvider>();
        if(cm)
            cm.gravityApplicationMode = ContinuousMoveProviderBase.GravityApplicationMode.Immediately;
        rigid = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (climbingHand)
        {
            if(cm)
                cm.enabled = false;
            Climb();
        }
        else if(!climbingHand && cm)
            cm.enabled = true;
    }
    
    void Climb()
    {
        InputDevices.GetDeviceAtXRNode(climbingHand.controllerNode).TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 velocity);
        character.Move(transform.rotation*-velocity * Time.fixedDeltaTime);
    }
}

