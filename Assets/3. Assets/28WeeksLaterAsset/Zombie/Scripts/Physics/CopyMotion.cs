using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[DisallowMultipleComponent]
[ExecuteInEditMode]
public class CopyMotion : MonoBehaviour
{
    public bool isActive = true;
    public Transform physicsTarget;
    public Transform animationTarget;
    public PhysicMaterial  n ;
    
    [Range(0f, 1f)] public float PinWeight = 1f;
    [Range(0f, 1f)] public float MuscleWeight = 1f;
    [Range(1f, 8f)] public float PinPower = 0.5f;
    [Range(0f, 100f)] public float PinDistanceFalloff = 1f;
    public float MuscleDamper = 1f;
    public float MuscleSpring = 100f;
    public float MappingWeight = 1f;

    public List<GameObject> targetGameObjects = new List<GameObject>();
    
    void Start()
    {
        physicsTarget = transform.Find("Physics").Find("Root");
        animationTarget = transform.Find("Animation").Find("Root");
        transform.Find("Animation").GetComponent<Animator>().cullingMode = AnimatorCullingMode.AlwaysAnimate;
        AddScriptToChild(physicsTarget);
    }

    public void ReActivate()
    {
        foreach (var targetGameObject in targetGameObjects)
        {
            targetGameObject.GetComponent<CopyMotionComponent>().ReActivate();
        }
    }

    private void AddScriptToChild(Transform target)
    {
        foreach (Transform child in target)
        {
            var rBody = child.gameObject.GetComponent<Rigidbody>();
            if (rBody != null)
            {
                rBody.interpolation = RigidbodyInterpolation.Interpolate;
                rBody.collisionDetectionMode = CollisionDetectionMode.Continuous;    
                var oJoint = child.gameObject.GetComponent<CharacterJoint>();
                var joint = child.gameObject.GetComponent<ConfigurableJoint>();
                if (joint == null)
                {
                    child.gameObject.AddComponent<ConfigurableJoint>();
                    joint = child.gameObject.GetComponent<ConfigurableJoint>();
                    if (oJoint != null)
                    {
                        joint.connectedBody = oJoint.connectedBody;
                        joint.connectedAnchor = oJoint.connectedAnchor;
                        DestroyImmediate(oJoint);    
                    }
                }
                if (child.gameObject.GetComponent<CopyMotionComponent>() == null)
                {
                    GameObject obj;
                    (obj = child.gameObject).AddComponent<CopyMotionComponent>();
                    var comp = obj.GetComponent<CopyMotionComponent>();
                    comp._copyMotion = this;
                    comp.target = SearchHierarchyForBone(animationTarget,obj.name).transform;
                    targetGameObjects.Add(obj);
                }

            }
            else
            {
                if (child.gameObject.GetComponent<Collider>() == null)
                {
                    DestroyImmediate(child.gameObject.GetComponent<ConfigurableJoint>());
                    DestroyImmediate(child.gameObject.GetComponent<Rigidbody>());
                }
            }
            AddScriptToChild(child.transform);
        }
    }
    
    private void OnDestroy()
    {
        foreach (var obj in targetGameObjects)
        {
            DestroyImmediate(obj.GetComponent<CopyMotionComponent>());
        }
    }
    
    private static Transform SearchHierarchyForBone(Transform current, string name)
    {
        if (current.name == name)
            return current;
        for (int i = 0; i < current.childCount; ++i)
        {
            var child = current.GetChild(i);
            Transform found = SearchHierarchyForBone(child, name);
            if (found != null)
                return found;
        }
        return null;
    }

}
