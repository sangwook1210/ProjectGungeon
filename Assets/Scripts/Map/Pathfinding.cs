using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class Pathfinding : MonoBehaviour
{
    PathRequestManager requestManager;

    Grid grid;

    private void Awake()
    {
        requestManager = GetComponent<PathRequestManager>();
        grid = GetComponent<Grid>();
    }

    public void StartFindPath(Vector3 startPos,  Vector3 targetPos)
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }

    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (!(startNode == targetNode))
        {

            if (startNode.walkable && targetNode.walkable)
            {
                Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
                HashSet<Node> closedSet = new HashSet<Node>();

                openSet.Add(startNode);

                while (openSet.Count > 0)
                {
                    Node currentNode = openSet.RemoveFirst();

                    closedSet.Add(currentNode);

                    if (currentNode == targetNode)
                    {
                        pathSuccess = true;
                        break;
                    }

                    foreach (Node neighbour in grid.GetNeigbours(currentNode))
                    {
                        // 이웃 노드가 벽이거나 closedSet에 들어가 있다면 skip
                        if (!neighbour.walkable || closedSet.Contains(neighbour))
                            continue;

                        int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                        if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                        {
                            neighbour.gCost = newMovementCostToNeighbour;
                            neighbour.hCost = GetDistance(neighbour, targetNode);
                            neighbour.parent = currentNode;

                            if (!openSet.Contains(neighbour))
                                openSet.Add(neighbour);
                            else
                                openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }
            yield return null;
        }
        if (pathSuccess)
        {
            waypoints=RetracePath(startNode, targetNode);
        }

        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path= new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Add(currentNode);

        Vector3[] waypoints=SimplifyPath(path);
        Array.Reverse(waypoints);

        return waypoints;
    }

    Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints= new List<Vector3>();
        Vector2 directionOld = Vector2.zero;


        for (int i = 0; i < path.Count-1; i++)
        {
            Vector2 directionNew = new Vector2(path[i].gridX - path[i+1].gridX, path[i].gridY - path[i+1].gridY);
            Vector3 direction;

            // +,+
            if (directionNew.x > 0 && directionNew.y > 0)
            {
                direction = new Vector3(1, 0, 1);
            }
            // +,0
            else if (directionNew.x > 0 && directionNew.y == 0)
            {
                direction = new Vector3(1, 0, 0);
            }
            // +,-
            else if (directionNew.x > 0 && directionNew.y < 0)
            {
                direction = new Vector3(1, 0, -1);
            }
            // 0,+
            else if (directionNew.x == 0 && directionNew.y > 0)
            {
                direction = new Vector3(0, 0, 1);
            }
            // 0,-
            else if (directionNew.x == 0 && directionNew.y < 0)
            {
                direction = new Vector3(0, 0, -1);
            }
            // -,+
            else if (directionNew.x < 0 && directionNew.y > 0)
            {
                direction = new Vector3(-1, 0, 1);
            }
            // -,0
            else if (directionNew.x < 0 && directionNew.y == 0)
            {
                direction = new Vector3(-1, 0, 0);
            }
            // -,-
            else
            {
                direction = new Vector3(-1, 0, -1);
            }

            waypoints.Add(direction);
        }

        return waypoints.ToArray();
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int disX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int disY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if( disX > disY)
            return grid.nodeDiagonal * disY + grid.nodeParallel * (disX - disY);

        return grid.nodeDiagonal * disX + grid.nodeParallel * (disY - disX);
    }
}
