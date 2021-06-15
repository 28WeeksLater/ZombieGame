using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[CustomEditor(typeof(TwoHandGrabInteractable))]
public class XRTwoHandGrabInteractableEditor : Editor
{

    public override void OnInspectorGUI()
    {
        TwoHandGrabInteractable off = (TwoHandGrabInteractable)target;

        DrawDefaultInspector();
    }
}