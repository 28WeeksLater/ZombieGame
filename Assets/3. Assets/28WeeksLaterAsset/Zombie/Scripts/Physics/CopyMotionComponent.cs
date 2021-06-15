using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[DisallowMultipleComponent]
public sealed class CopyMotionComponent : MonoBehaviour
{
    public CopyMotion _copyMotion;
    private bool IsActive => _copyMotion != null && _copyMotion.isActive;
    private bool isUsable = true;
    private bool IsInit = false;
    private bool IsReActivated = false;
    
    private Rigidbody _rigidbody;
    public Transform target;
    private Transform connectedBodyTarget;
    private ConfigurableJoint joint;
    
    private Transform Transform { get; set; }

    private Transform targetParent;
    private float mappingWeightMlp = 1f;

    private Quaternion defaultLocalRotation,
        localRotationConvert,
        toParentSpace,
        toJointSpaceInverse,
        toJointSpaceDefault,
        targetAnimatedRotation;
    
    private float lastJointDriveRotationWeight = -1f, lastRotationDamper = -1f;
    
    private JointDrive slerpDrive;
    private JointDrive defaultSlerpDrive;
    private Vector3 positionOffset = Vector3.zero;
    private Vector3 targetAnimatedCenterOfMass;
    
    private float lastReadTime;
    private Quaternion defaultTargetRotRelToMuscleInverse = Quaternion.identity;
    private Vector3 targetVelocity;

    public bool ignoreSuperValue = false;

    [Range(0f, 1f)] public float PinWeight = 1f;
    [Range(0f, 1f)] public float MuscleWeight = 1f;
    [Range(0.1f, 8f)] public float PinPower = 0.5f;
    [Range(0f, 100f)] public float PinDistanceFalloff = 1f;
    public float MuscleDamper = 1f;
    public float MuscleSpring = 100f;

    public float MappingWeight = 1f;

    private float _mappingWeight
    {
        get => ignoreSuperValue ? MappingWeight : _copyMotion.MappingWeight;
        set => MappingWeight = value;
    }
    private float _PinWeight => ignoreSuperValue ? PinWeight : _copyMotion.PinWeight;
    private float _MuscleWeight => ignoreSuperValue ? MuscleWeight : _copyMotion.MuscleWeight;
    private float _PinPower => ignoreSuperValue ? PinPower : _copyMotion.PinPower;
    private float _PinDistanceFalloff => ignoreSuperValue ? PinDistanceFalloff : _copyMotion.PinDistanceFalloff;
    private float _MuscleDamper => ignoreSuperValue ? MuscleDamper : _copyMotion.MuscleDamper;
    private float _MuscleSpring => ignoreSuperValue ? MuscleSpring : _copyMotion.MuscleSpring;

    private float defaultPinWeight,
        defaultMuscleWeight,
        defaultPinPower,
        defaultDistanceFallOff;
   
    private Quaternion targetRotationRelative;
    private Transform connectedBodyTransform;

    private Vector3 defaultTargetLocalPosition;
    private Quaternion defaultTargetLocalRotation;

    private Quaternion LocalToJointSpace(Quaternion localRotation) 
        => toJointSpaceInverse * Quaternion.Inverse(localRotation) * toJointSpaceDefault;
    
    private Quaternion LocalRotation => Quaternion.Inverse(ParentRotation) * Transform.rotation;
    private Quaternion TargetLocalRotation => Quaternion.Inverse(TargetParentRotation * toParentSpace ) * target.rotation;
    private Quaternion TargetParentRotation => targetParent == null ? Quaternion.identity : targetParent.rotation;
    private Quaternion ParentRotation
    {
        get
        {
            if (joint == null) return Transform.parent.rotation;
            if (joint.connectedBody != null) return joint.connectedBody.rotation;
            if (Transform.parent == null) return Quaternion.identity;
            return Transform.parent.rotation;
        }
    }

    void Start()
    {
        if (!IsInit)
        {
            IsInit = true;
            defaultTargetLocalRotation = transform.localRotation;
            defaultTargetLocalPosition = transform.localPosition;
            joint = GetComponent<ConfigurableJoint>();
            if (joint == null) return;
            if (joint.connectedBody != null)
            {
                if (joint.connectedBody.transform != null)
                {
                    var transform1 = joint.connectedBody.transform;
                    connectedBodyTarget = transform1;
                    connectedBodyTransform = transform1;
                }            
            }
            
            defaultPinWeight = _PinWeight;
            defaultMuscleWeight = _MuscleWeight;
            defaultPinPower = _PinPower;
            defaultDistanceFallOff = _PinDistanceFalloff;
            var transform2 = transform;
            defaultTargetLocalPosition = transform2.localPosition;
            defaultTargetLocalRotation = transform2.localRotation;
            
            var drive = joint.slerpDrive;
            defaultSlerpDrive.positionSpring = drive.positionSpring;
            defaultSlerpDrive.positionDamper = drive.positionDamper;
            defaultSlerpDrive.maximumForce = drive.maximumForce;
            Transform = joint.transform;
            
            
            _rigidbody = GetComponent<Rigidbody>();
            if (_rigidbody == null) return;

            if(connectedBodyTarget != null){}
            targetParent = connectedBodyTarget != null ? connectedBodyTarget : target.parent;
            defaultLocalRotation = LocalRotation;
            
            Vector3 forward = Vector3.Cross(joint.axis, joint.secondaryAxis).normalized;
            Vector3 upwards = Vector3.Cross(forward, joint.axis).normalized;
            
            var defaultTargetRotRelToMuscle = Quaternion.Inverse(Transform.rotation) * target.rotation;
            defaultTargetRotRelToMuscleInverse = Quaternion.Inverse(defaultTargetRotRelToMuscle);
            
            Quaternion toJointSpace = Quaternion.LookRotation(forward, upwards);
            toJointSpaceInverse = Quaternion.Inverse(toJointSpace);
            toJointSpaceDefault = defaultLocalRotation * toJointSpace;
            
            toParentSpace = Quaternion.Inverse(TargetParentRotation) * ParentRotation;
            localRotationConvert = Quaternion.Inverse(TargetLocalRotation) * LocalRotation;
            if (joint.connectedBody != null)
            {
                if (joint.connectedBody.transform != null)
                {
                    joint.xMotion = ConfigurableJointMotion.Locked;
                    joint.yMotion = ConfigurableJointMotion.Locked;
                    joint.zMotion = ConfigurableJointMotion.Locked;
                    joint.angularXMotion = ConfigurableJointMotion.Limited;
                    joint.angularYMotion = ConfigurableJointMotion.Limited;
                    joint.angularZMotion = ConfigurableJointMotion.Limited;
                    joint.rotationDriveMode = RotationDriveMode.Slerp;
                }    
            }
            else
            {
                joint.xMotion = ConfigurableJointMotion.Free;
                joint.yMotion = ConfigurableJointMotion.Free;
                joint.zMotion = ConfigurableJointMotion.Free;
                joint.angularXMotion = ConfigurableJointMotion.Free;
                joint.angularYMotion = ConfigurableJointMotion.Free;
                joint.angularZMotion = ConfigurableJointMotion.Free;
            }
            targetRotationRelative = Quaternion.Inverse(_rigidbody.transform.rotation) * target.rotation;
            joint.configuredInWorldSpace = false;
            
            targetAnimatedCenterOfMass = _rigidbody.worldCenterOfMass;
            targetAnimatedRotation = TargetLocalRotation * localRotationConvert;
        }
        Read();
        lastReadTime = Time.time;
    }

    public void ReActivate()
    {
        if (isUsable || !IsActive) return;
        PinWeight = defaultPinWeight;
        MuscleWeight = defaultMuscleWeight;
        PinPower = defaultPinPower;
        PinDistanceFalloff = 0;
        
        transform.localRotation = defaultTargetLocalRotation;
        transform.localPosition = defaultTargetLocalPosition;

        var drive = slerpDrive;
        drive.positionSpring = defaultSlerpDrive.positionSpring;
        drive.positionDamper = defaultSlerpDrive.positionDamper;
        drive.maximumForce = defaultSlerpDrive.maximumForce;
        joint.slerpDrive = slerpDrive;
        isUsable = true;
        IsReActivated = true;

    }

    void DeActivate()
    {
        slerpDrive.positionSpring = 0;
        slerpDrive.maximumForce = float.MaxValue;
        slerpDrive.positionDamper = 0;
        joint.slerpDrive = slerpDrive;
            
        PinWeight = 0;
        MuscleWeight = 0;
        PinPower = 10;
        PinDistanceFalloff = 1000;

        isUsable = false;
    }

    void FixedUpdate()
    {
        if (!isUsable) return;
        if (joint == null) return;
        if (Transform == null) return;
        if (!IsActive)
        {
            DeActivate();
        }
        Read();
        mappingWeightMlp = Mathf.Clamp(mappingWeightMlp, 0f, 1f);
        Rotate(_MuscleDamper,_PinPower, _PinDistanceFalloff);
        
        if (!IsReActivated) return;
        PinDistanceFalloff = defaultDistanceFallOff;
        IsReActivated = false;
    }
    
    private void Read()
    {
        float readDeltaTime = Time.time - lastReadTime;
        lastReadTime = Time.time;

        Vector3 tAM = TransformPointUnscaled(target, defaultTargetRotRelToMuscleInverse * _rigidbody.centerOfMass);

        if (readDeltaTime > 0f)
        {
            targetVelocity = (tAM - targetAnimatedCenterOfMass) / readDeltaTime;
        }

        targetAnimatedCenterOfMass = tAM;
        
        if (joint.connectedBody != null)
        {
            targetAnimatedRotation = TargetLocalRotation * localRotationConvert;
        }
    }

    private void Rotate(float muscleDamper, float pinPow, float pinDistanceFalloff)
    {
        float readDeltaTime = Time.time - lastReadTime;
        lastReadTime = Time.time;
        Vector3 tAM = TransformPointUnscaled(Transform, defaultTargetRotRelToMuscleInverse * _rigidbody.centerOfMass); // Center of mass is unscaled, so can't use Transform.TransformPoint() here
        if (readDeltaTime > 0f)
        {
            targetVelocity = (tAM - targetAnimatedCenterOfMass) / readDeltaTime;
        }

        Pin(_PinWeight, pinPow, pinDistanceFalloff, Time.deltaTime);
        MuscleRotation(_MuscleWeight, _MuscleSpring, muscleDamper);
    }

    private void Pin(float pinWeightMaster, float pinPow, float pinDistanceFalloff, float deltaTime)
    {
        positionOffset = targetAnimatedCenterOfMass - _rigidbody.worldCenterOfMass;
        if (float.IsNaN(positionOffset.x)) positionOffset = Vector3.zero;
        float w = pinWeightMaster * 1f * 1f;
        if (w <= 0f) return;
        w = Mathf.Pow(w, pinPow);
        Pin(_rigidbody, positionOffset, targetVelocity, w, pinDistanceFalloff, deltaTime);
    }

    private static void Pin(Rigidbody r, Vector3 posOffset, Vector3 targetVel, float w, float pinDistanceFalloff, float deltaTime)
    {
        var p = posOffset;
        if (deltaTime > 0f) p /= deltaTime;

        var force = -r.velocity + targetVel + p;
        if (r.useGravity) force -= Physics.gravity * Time.deltaTime;
        force *= w;
        if (pinDistanceFalloff > 0f) force /= 1f + posOffset.sqrMagnitude * pinDistanceFalloff;

        r.AddForce(force, ForceMode.VelocityChange);
    }
    private void MuscleRotation(float muscleWeightMaster, float muscleSpring, float muscleDamper)
    {
        float w = muscleWeightMaster * 1f * muscleSpring * 1f * 10f;

        // If no connection point, don't rotate;
        if (joint.connectedBody == null) w = 0f;
        else if (w > 0f) joint.targetRotation = LocalToJointSpace(targetAnimatedRotation);

        float d = (1f * muscleDamper * 1f) + 0f;

        if (Math.Abs(w - lastJointDriveRotationWeight) < Mathf.Epsilon && Math.Abs(d - lastRotationDamper) < Mathf.Epsilon) return;
        lastJointDriveRotationWeight = w;

        lastRotationDamper = d;
        slerpDrive.positionSpring = w;
        slerpDrive.maximumForce = Mathf.Max(w, d) * 1f;
        slerpDrive.positionDamper = d;

        //joint.slerpDrive = slerpDrive;
    }

    private static Vector3 TransformPointUnscaled(Transform t, Vector3 point) => t.position + t.rotation * point;

    private void LateUpdate()
    {
        if (!isUsable) return;
        if(Transform == null) return;
        if (!IsActive) return;
        _mappingWeight = Mathf.Clamp(_mappingWeight, 0f, 1f);
        float mappingBlend = 1f;
        float mW = _mappingWeight * mappingBlend;
        Map(mW);
    }

    public void Map(float mappingWeightMaster)
    {
        float w = _mappingWeight * mappingWeightMaster * mappingWeightMlp;
        if (w <= 0f) return;

        // rigidbody.position does not work with interpolation
        Vector3 position = transform.position;
        Quaternion rotation = transform.rotation;

        if (w >= 1f)
        {
            Transform.rotation = rotation * targetRotationRelative;

            if (connectedBodyTransform != null)
            {
                // Mapping in local space of the parent
                Vector3 relativePosition = connectedBodyTransform.InverseTransformPoint(position);
                Transform.position = connectedBodyTarget.TransformPoint(relativePosition);
            }
            else
            {
                Transform.position = position;
            }

            return;
        }

        Transform.rotation = Quaternion.Lerp(Transform.rotation, rotation * targetRotationRelative, w);

        if (connectedBodyTransform != null)
        {
            // Mapping in local space of the parent
            Vector3 relativePosition = connectedBodyTransform.InverseTransformPoint(position);
            Transform.position = Vector3.Lerp(Transform.position, connectedBodyTarget.TransformPoint(relativePosition), w);
        }
        else
        {
            Transform.position = Vector3.Lerp(Transform.position, position, w);
        }
    }
}
