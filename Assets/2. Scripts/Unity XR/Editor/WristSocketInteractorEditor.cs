using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[CustomEditor(typeof(WristSocket))]
public class WristSocketInteractorEditor : Editor
{

    public override void OnInspectorGUI()
    {
        WristSocket off = (WristSocket)target;

        DrawDefaultInspector();
    }
}
