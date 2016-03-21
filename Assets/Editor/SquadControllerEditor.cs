using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(SquadController))]
public class SquadControllerEditor : Editor
{
    SquadController sc;
    
    void OnEnable()
    {
        sc = (SquadController)target;
    }

    void OnSceneGUI()
    {
        //Radius handle for detection range
        sc.detectionRange = Handles.RadiusHandle(Quaternion.identity, sc.transform.position, sc.detectionRange);
    }
}
