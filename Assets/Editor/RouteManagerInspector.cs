using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

//Custom inspector for the route manager
[CustomEditor(typeof(RouteManager))]
public class RouteManagerInspector : Editor
{
    //keep track of which node is selected
    int selectedNode;
    bool displayNodeSection;
    RouteManager rm;
    List<SerializedProperty> nodes;

    void OnEnable()
    {
        rm = (RouteManager)target;

        //nodes = serializedObject.FindProperty("nodes");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (Selection.activeGameObject == rm)
        {
            selectedNode = -1;
        }
        //Colour section
        rm.nodeColour = EditorGUILayout.ColorField("Node Color", rm.nodeColour);
        rm.pathColour = EditorGUILayout.ColorField("Path Color", rm.pathColour);
        //Show Nodes
        rm.showRoute = EditorGUILayout.Toggle("Show Route?", rm.showRoute);
        rm.segmentCount = EditorGUILayout.IntSlider("Curve Detail", rm.segmentCount, 0, 20);
        rm.drawLines = EditorGUILayout.Toggle("Draw Calculations", rm.drawLines);
        EditorGUILayout.Separator();

        //Node control section
        displayNodeSection = EditorGUILayout.Foldout(displayNodeSection, "Node Controls");
        if (displayNodeSection)
        {
            EditorGUILayout.HelpBox("Node Controls. Control selection, manual positioning, and add and remove nodes", MessageType.Info);
            //For each node draw a checkbox for selection, the Vector3 field, and a button to remove the element
            for (int i = 0; i < rm.nodes.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                //If we toggle the button, set selected node accordingly, 
                //if the selected is the current node display toggle as true, otherwise false.
                if (EditorGUILayout.Toggle(i.ToString(), selectedNode == i ? true : false))
                {
                    selectedNode = i;
                }
                EditorGUILayout.BeginVertical();
                rm.nodes[i].pathNode = EditorGUILayout.Vector3Field("path", rm.nodes[i].pathNode);
                //Control nodes
                if (i != 0)//First node needs no previous handle
                {
                    rm.nodes[i].controlNodeP = EditorGUILayout.Vector3Field("control Prev", rm.nodes[i].controlNodeP);
                }
                if (i != rm.nodes.Count - 1)//Last node needs no next handle
                {
                    rm.nodes[i].controlNodeN = EditorGUILayout.Vector3Field("control Next", rm.nodes[i].controlNodeN);
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();
                //Buttons
                if (GUILayout.Button("-"))
                {
                    rm.nodes.Remove(rm.nodes[i]); //Remove node
                }
                if (i != 0)
                {
                    if (GUILayout.Button("0"))
                    {
                        rm.nodes[i].controlNodeP = rm.nodes[i].pathNode + Vector3.left * 2; //Previous Node Reset
                    }
                }
                if (i != rm.nodes.Count - 1)
                {
                    if (GUILayout.Button("0"))
                    {
                        rm.nodes[i].controlNodeN = rm.nodes[i].pathNode + Vector3.left * -2; //Next Node Reset
                    }
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Separator();
            }
            //Add a new node
            if (GUILayout.Button("+"))
            {
                rm.nodes.Add(new Node(rm.nodes[rm.nodes.Count - 1].pathNode, Vector3.left * 2, Vector3.left * -2));
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    void OnSceneGUI()
    {
        serializedObject.Update();

        if (rm.showRoute)
        {
            //Run through all nodes and compare their position to the current click position (if there is one)
            for (int i = 0; i < rm.nodes.Count; i++)
            {
                Event e = Event.current;
                if (e.type == EventType.MouseDown)
                {
                    Vector2 guiPoint = HandleUtility.WorldToGUIPoint(rm.nodes[i].pathNode);
                    //If the click is within 20 pixels, this is the new selected node.
                    if(Vector2.Distance(guiPoint, e.mousePosition) < 10)
                    {
                        selectedNode = i;
                    }
                }
            }
            //Draw handles for the selected node and connect them to the appropriate variable
            if (selectedNode >= 0)
            {
                //Position Handle for movement
                rm.nodes[selectedNode].pathNode = Handles.PositionHandle(rm.nodes[selectedNode].pathNode, Quaternion.identity);
                //FreemoveHandle for control node
                //determine handle size...
                Handles.color = Color.cyan;
                //Freemove control node handles
                if (selectedNode != 0) //First node needs no previous handle
                {
                    rm.nodes[selectedNode].controlNodeP = Handles.FreeMoveHandle(rm.nodes[selectedNode].controlNodeP, Quaternion.identity, 0.5f, Vector3.one, Handles.CubeCap);
                    Handles.DrawLine(rm.nodes[selectedNode].pathNode, rm.nodes[selectedNode].controlNodeP);
                }
                if (selectedNode != rm.nodes.Count - 1) // Last node doesn't need a next handle
                {
                    rm.nodes[selectedNode].controlNodeN = Handles.FreeMoveHandle(rm.nodes[selectedNode].controlNodeN, Quaternion.identity, 0.5f, Vector3.one, Handles.CubeCap);
                    Handles.DrawLine(rm.nodes[selectedNode].pathNode, rm.nodes[selectedNode].controlNodeN);
                }
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
