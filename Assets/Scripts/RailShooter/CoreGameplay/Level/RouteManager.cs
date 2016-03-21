using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;

//Handles the nodes, has path nodes and control nodes
[System.Serializable]
public class Node
{
    [SerializeField]
    public Vector3 pathNode;
    [SerializeField]
    public Vector3 controlNodeP;
    [SerializeField]
    public Vector3 controlNodeN;

    //Constructor
    public Node(Vector3 _pathNode, Vector3 _controlNodeP, Vector3 _controlNodeN)
    {
        pathNode = _pathNode;
        controlNodeP = _controlNodeP;
        controlNodeN = _controlNodeN;
    }
}

public class RouteManager : MonoBehaviour
{
    //VARIABLES
    public bool showRoute;
    public Color nodeColour;
    public Color pathColour;
    [SerializeField]
    public List<Node> nodes;
    public List<Vector3> fullNodes;
    public int segmentCount = 10;
    public bool drawLines;

    void OnEnable()
    {
    }

    //Go through the nodes on the route, draw a sphere at each point and a line between them
    void OnDrawGizmos()
    {
#if UNITY_EDITOR //only run in editor
        if (nodes.Count <= 0)
        {
            nodes = new List<Node>();
            nodes.Add(new Node(transform.position, transform.position + Vector3.left * 2, transform.position + Vector3.right * 2));
        }
        //Full Nodes
        if (fullNodes.Count <= 0)
        {
            fullNodes = new List<Vector3>();
        }

        //Checkbox to handle showing the gizmos
        if (showRoute)
        {
            for(int i = 0; i < nodes.Count; i++)
            {
                //Draw a sphere at the node position
                Gizmos.color = nodeColour;
                Gizmos.DrawWireSphere(nodes[i].pathNode, 0.5f);
                //Make sure we have a next node, draw a line between the current and the next
                if (i+1 < nodes.Count)
                {
                    Gizmos.color = new Color (pathColour.r, pathColour.g, pathColour.b, pathColour.a * 0.5f);
                    Gizmos.DrawLine(nodes[i].pathNode, nodes[i + 1].pathNode);
                    Gizmos.color = pathColour;
                }
            }

            //Determine the curve using node list and calculated control node
            fullNodes = FullNodes();
            for (int i = 0; i < fullNodes.Count; i++)
            {
                //Debug.Log(i);
                Gizmos.color = new Color(nodeColour.r, nodeColour.g, nodeColour.b, 0.2f);
                Gizmos.DrawWireSphere(fullNodes[i], 0.5f);
                if (i + 1 < fullNodes.Count)
                {
                    Gizmos.color = pathColour;
                    Gizmos.DrawLine(fullNodes[i], fullNodes[i + 1]);
                }
            }
        }
#endif
    }

    private List<Vector3> FullNodes()
    {
        //Debug.Log("FullNodes Called");
        List<Vector3> ret = new List<Vector3>();
        //run through all but the end node
        for (int i = 0; i < nodes.Count - 1; i++)
        {
            //Debug.Log("i is " + i);
            List<Vector3> curPoints = BezPoints(i);
            
            for (int j = 0; j <= segmentCount; j++)
            {
                //Debug.Log("j is " + i);
                //time fraction of the step
                float t = j / (float)segmentCount;
                //Debug.Log((float)numberOfSteps / (float)100);
                //u is the inverse fraction, so we can count backwards
                float u = 1 - t;
                //time squared
                float tt = t * t;
                //inverse time squared
                float uu = u * u;
                //time cubed
                float ttt = tt * t;
                //inverse time cubed
                float uuu = uu * u;

                //point starts at inverse time cubed * the first point
                Vector3 p = uuu * curPoints[0]; //first term
                
                //increased by 3 * inverse time squared * time * the second point
                p += 3 * uu * t * curPoints[1]; //second term
                
                //increased by 3 * inverse time * time squared * the third point
                p += 3 * u * tt * curPoints[2]; //third term
                
                //increased by time cubed * the fourth point
                p += ttt * curPoints[3]; //fourth term
                
                if(drawLines)
                {
                    Debug.DrawLine(curPoints[0], p, Color.magenta * new Color(1, 1, 1, 0.1f));
                    Debug.DrawLine(curPoints[1], p, Color.yellow * new Color(1, 1, 1, 0.1f));
                    Debug.DrawLine(curPoints[2], p, Color.white * new Color(1, 1, 1, 0.1f));
                    Debug.DrawLine(curPoints[3], p, Color.green * new Color(1, 1, 1, 0.1f));
                }

                ret.Add(p);
            }
        }
        return ret;
    }

    List<Vector3> BezPoints(int i)
    {
        //Debug.Log("Bez Points Called");
        List<Vector3> points = new List<Vector3>();
        //Get own node position,
        Vector3 p0 = nodes[i].pathNode;
        points.Add(p0);
        //Get my next control node's position,
        Vector3 p1 = nodes[i].controlNodeN;
        points.Add(p1);
        //Get next node's previous control node's position,
        Vector3 p2 = nodes[i+1].controlNodeP;
        points.Add(p2);
        //Get next node position,
        Vector3 p3 = nodes[i+1].pathNode;
        //Debug.Log("Bez Points Complete");
        points.Add(p3);
        return points;
    }
}
