using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[CustomEditor(typeof(XRSocketInteractorTag))]
public class SocketinteractorTagEditor : Editor
{

    public override void OnInspectorGUI()
    {
        XRSocketInteractorTag off = (XRSocketInteractorTag)target;

        DrawDefaultInspector();
    }
}
