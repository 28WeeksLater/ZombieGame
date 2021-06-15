using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[CustomEditor(typeof(XRSocketInteractor))]
public class XRSocketInteractorEditor : Editor
{

    public override void OnInspectorGUI()
    {
        XRSocketInteractor soc = (XRSocketInteractor)target;

        DrawDefaultInspector();
    }
}
