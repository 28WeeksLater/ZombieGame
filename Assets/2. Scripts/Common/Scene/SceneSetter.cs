using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSetter: MonoBehaviour
{
    [SerializeField]
    private Material skybox;

    [SerializeField]
    private Light sun;

    private void Awake()
    {
        RenderSettings.skybox = skybox;
        RenderSettings.sun = sun;
    }
}
