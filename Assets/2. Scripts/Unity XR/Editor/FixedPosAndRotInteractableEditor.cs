using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FixedPosAndRotInteractable))]
public class FixedPosAndRotInteractableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        FixedPosAndRotInteractable off = (FixedPosAndRotInteractable)target;

        DrawDefaultInspector();
    }
    
}
