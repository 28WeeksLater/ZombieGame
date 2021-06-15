using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[CustomEditor(typeof(XROffsetGrabInteractable))]
public class OffsetInteractableEditor : Editor
{

    public override void OnInspectorGUI()
    {
        XROffsetGrabInteractable off = (XROffsetGrabInteractable)target;

        DrawDefaultInspector();
    }
}