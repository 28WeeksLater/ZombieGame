using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.XR.Interaction.Toolkit;

[CustomEditor(typeof(RopeInteractable))]
public class RopeInteractableEditor : Editor
{

    public override void OnInspectorGUI()
    {
        RopeInteractable off = (RopeInteractable)target;

        DrawDefaultInspector();
    }
}
