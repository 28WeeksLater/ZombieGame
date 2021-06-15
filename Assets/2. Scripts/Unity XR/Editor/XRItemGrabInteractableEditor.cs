using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[CustomEditor(typeof(XRItemGrabInteractable))]
public class XRItemGrabInteractableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        XRItemGrabInteractable off = (XRItemGrabInteractable)target;

        DrawDefaultInspector();
    }
}
