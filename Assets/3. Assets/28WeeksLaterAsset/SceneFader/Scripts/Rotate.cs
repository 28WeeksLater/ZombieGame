using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    private void Update()
    {
        if(SubCamManager.Instance.camOn)
            transform.Rotate(Vector3.right, 10.0f * Time.deltaTime);
    }
}
