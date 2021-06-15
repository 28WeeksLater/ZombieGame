using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BloodParticleSystem : MonoBehaviour
{

    public BloodData bloodData;
    public Vector3 direction;

    public void Create(Collision other, Transform target)
    {
        var blood = BloodPool.Instance.GetBlood();

        var instance = blood[0];
        instance.transform.position = other.contacts[0].point;
        instance.transform.rotation = Quaternion.Euler(other.contacts[0].normal);
        var attachBloodInstance = blood[1];
        
        var bloodT = attachBloodInstance.transform;
        bloodT.position = other.contacts[0].point;
        bloodT.localRotation = Quaternion.identity;
        bloodT.localScale = Vector3.one * Random.Range(0.15f, 0.3f);
        bloodT.LookAt(other.contacts[0].point + other.contacts[0].normal, direction);
        bloodT.Rotate(90, 0, 0);
        bloodT.transform.parent = target;
        
        blood[0].SetActive(true);
        blood[1].SetActive(true);
        StartCoroutine(DestroyBlood(blood));
    }
    
    public void Create(Vector3 point, Quaternion dir, Transform target)
    {
        var blood = BloodPool.Instance.GetBlood();
        
        var instance = blood[0];
        instance.transform.position = point;
        instance.transform.rotation = dir;
        
        var attachBloodInstance = blood[1];
        
        var bloodT = attachBloodInstance.transform;
        bloodT.position = point;
        bloodT.localRotation = Quaternion.identity;
        bloodT.localScale = Vector3.one * Random.Range(0.15f, 0.3f);
        bloodT.LookAt(point + dir.normalized.eulerAngles, direction);
        bloodT.Rotate(90, 0, 0);
        bloodT.transform.parent = target;

        blood[0].SetActive(true);
        blood[1].SetActive(true);
        StartCoroutine(DestroyBlood(blood));
    }

    IEnumerator DestroyBlood(GameObject[] blood)
    {
        yield return new WaitForSeconds(0.3f);
        BloodPool.Instance.ReturnBlood(blood);
    }
}
