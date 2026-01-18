using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Transform Player;
    public LayerMask unwalkableMask;    // 벽 LayerMask
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public float nodeRadiusX;
    public float nodeRadiusY;

    public float nodeDiameterX = 1.0f;  // 노드의 가로 길이 = 1
    public float nodeDiameterY = Mathf.Sqrt(2); // 노드의 세로 길이 = 1.414
    Node[,] grid;   // 맵을 노드들로 분할한 2차원 배열 grid

    int gridSizeX, gridSizeY;   // grid의 가로 사이즈와 세로 사이즈
    public int nodeParallel = 10;   // 노드의 평행 이동 비용
    public int nodeDiagonal = 14;   // 노드의 대각선 이동 비용

    public Transform Boundary_TopLeft;
    public Transform Boundary_BottomRight;
    public float boundary_Top;
    public float boundary_Bottom;
    public float boundary_Right;
    public float boundary_Left;
    public Vector3 MiddlePos;

    private void Awake()
    {
        nodeRadiusX = nodeDiameterX * 0.5f;
        nodeRadiusY = nodeDiameterY * 0.5f;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameterX);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameterY);

        CreateGrid();

        boundary_Top = Boundary_TopLeft.position.z;
        boundary_Left= Boundary_TopLeft.position.x;
        boundary_Bottom = Boundary_BottomRight.position.z;
        boundary_Right = Boundary_BottomRight.position.x;
        MiddlePos = new Vector2((boundary_Right + boundary_Left) / 2, (boundary_Top + boundary_Bottom) / 2);
    }

    public int MaxSize
    {
        get { return gridSizeX*gridSizeY; } 
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameterX +nodeRadiusX) + Vector3.forward * (y * nodeDiameterY +nodeRadiusY);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadiusY, unwalkableMask ));
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    public List<Node> GetNeigbours(Node node)
    {
        List<Node> neighbours=new List<Node>();

        // 주변 8개 노드 탐색
        for(int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)   // 자기 자신 스킵
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if(checkX>=0&&checkX<gridSizeX&& checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX,checkY]);
                }
            }
        }

        return neighbours;
    }

    public Node NodeFromWorldPoint(Vector3 worldPos)
    {
        float percentX = (worldPos.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPos.z + gridWorldSize.y / 2) / gridWorldSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    // grid가 맵 안에 있는지 확인하는 함수
    private bool CheckGridInArea(Vector3 gridWorldPos, GameManager.MapArea area)
    {
        if (area == GameManager.MapArea.TopLeft)
        {
            if (gridWorldPos.x < MiddlePos.x && gridWorldPos.x > boundary_Left
            && gridWorldPos.z < boundary_Top && gridWorldPos.z > MiddlePos.y)
                return true;
            else
                return false;
        }
        else if(area== GameManager.MapArea.TopRight)
        {
            if (gridWorldPos.x < boundary_Right && gridWorldPos.x > MiddlePos.x
            && gridWorldPos.z < boundary_Top && gridWorldPos.z > MiddlePos.y)
                return true;
            else
                return false;
        }
        else if(area== GameManager.MapArea.BottomLeft)
        {
            if (gridWorldPos.x < MiddlePos.x && gridWorldPos.x > boundary_Left
            && gridWorldPos.z < MiddlePos.y && gridWorldPos.z > boundary_Bottom)
                return true;
            else
                return false;
        }
        else
        {
            if (gridWorldPos.x < boundary_Right && gridWorldPos.x > MiddlePos.x
            && gridWorldPos.z < MiddlePos.y && gridWorldPos.z > boundary_Bottom)
                return true;
            else
                return false;
        }
    }

    // 걸을 수 있는 노드를 하나 골라 반환하는 함수
    public Vector3 SelectWalkableNode(GameManager.MapArea area)
    {
        int x, y;

        while (true)
        {
            x = Random.Range(0, gridSizeX);
            y = Random.Range(0, gridSizeY);

            if (!CheckGridInArea(grid[x, y].worldPos,area))
                continue;

            if (grid[x, y].walkable == true)
                break;
        }

        return grid[x, y].worldPos;
    }
}
