using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[CustomEditor(typeof(XRSocketInteractorWithName))]
public class SocketinteractorNameEditor : Editor
{

    public override void OnInspectorGUI()
    {
        XRSocketInteractorWithName off = (XRSocketInteractorWithName)target;

        DrawDefaultInspector();
    }
}
