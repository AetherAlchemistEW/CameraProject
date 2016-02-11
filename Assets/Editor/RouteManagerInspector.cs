using UnityEngine;
using System.Collections;
using UnityEditor;

//Custom inspector for the route manager
[CustomEditor(typeof(RouteManager))]
public class RouteManagerInspector : Editor
{
    //keep track of which node is selected
    int selectedNode;
    bool displayNodeSection;

    public override void OnInspectorGUI()
    {
        RouteManager rm = (RouteManager)target;
        //Colour section
        rm.nodeColour = EditorGUILayout.ColorField("Node Color", rm.nodeColour);
        rm.pathColour = EditorGUILayout.ColorField("Path Color", rm.pathColour);
        //Show Nodes
        rm.showRoute = EditorGUILayout.Toggle("Show Route?", rm.showRoute);
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
                rm.nodes[i] = EditorGUILayout.Vector3Field("", rm.nodes[i]);
                if (GUILayout.Button("-"))
                {
                    rm.nodes.Remove(rm.nodes[i]);
                }
                EditorGUILayout.EndHorizontal();
            }
            //Add a new node
            if (GUILayout.Button("+"))
            {
                rm.nodes.Add(rm.nodes[rm.nodes.Count - 1]);
            }
        }
    }

    void OnSceneGUI()
    {
        RouteManager rm = (RouteManager)target;
        if (rm.showRoute)
        {
            //Run through all nodes and compare their position to the current click position (if there is one)
            for (int i = 0; i < rm.nodes.Count; i++)
            {
                Event e = Event.current;
                if (e.type == EventType.MouseDown)
                {
                    Vector2 guiPoint = HandleUtility.WorldToGUIPoint(rm.nodes[i]);
                    //If the click is within 20 pixels, this is the new selected node.
                    if(Vector2.Distance(guiPoint, e.mousePosition) < 20)
                    {
                        selectedNode = i;
                    }
                }
            }
            //Draw handles for the selected node and connect them to the appropriate variable
            rm.nodes[selectedNode] = Handles.PositionHandle(rm.nodes[selectedNode], Quaternion.identity);
        }
    }
}
