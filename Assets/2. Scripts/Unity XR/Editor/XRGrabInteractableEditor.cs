using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[CustomEditor(typeof(XRGrabInteractable))]
public class XRGrabInteractableEditor : Editor
{

    public override void OnInspectorGUI()
    {
        XRGrabInteractable off = (XRGrabInteractable)target;

        DrawDefaultInspector();
    }
}
