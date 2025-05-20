﻿using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public Vector2 gridWorldSize = new Vector2(10, 10); // Size of the world in Unity units
    public float nodeRadius = 0.5f;                      // Half the size of each node
    public LayerMask unwalkableMask;                    // Layer that marks obstacles

    Node[,] grid; // 2D array to store the grid

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    // Create the grid
    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];

        // Bottom-left corner of the grid in world space
        Vector3 worldBottomLeft = transform.position - 
            Vector3.right * gridWorldSize.x / 2 - 
            Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                // Calculate the world position of this node
                Vector3 worldPoint = worldBottomLeft + 
                                     Vector3.right * (x * nodeDiameter + nodeRadius) +
                                     Vector3.forward * (y * nodeDiameter + nodeRadius);

                // Check if this position is walkable
                bool walkable = !Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask);
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    // Optional: Draw the grid in the editor
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        // Draw the overall grid boundary
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        // If we haven't created the grid yet, show the predicted grid layout
        if (grid == null)
        {
            float nodeDiameter = nodeRadius * 2;
            int gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
            int gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

            Vector3 worldBottomLeft = transform.position - 
                Vector3.right * gridWorldSize.x / 2 - 
                Vector3.forward * gridWorldSize.y / 2;

            // Draw predicted grid layout
            Gizmos.color = new Color(1, 1, 1, 0.3f); // Semi-transparent white
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    Vector3 worldPoint = worldBottomLeft + 
                        Vector3.right * (x * nodeDiameter + nodeRadius) +
                        Vector3.forward * (y * nodeDiameter + nodeRadius);
                    Gizmos.DrawWireCube(worldPoint, Vector3.one * (nodeDiameter - 0.1f));
                }
            }
        }
        else
        {
            // Draw the actual grid nodes
            foreach (Node node in grid)
            {
                // Walkable nodes are white, obstacles are red
                Gizmos.color = node.walkable ? 
                    new Color(1, 1, 1, 0.3f) : // Semi-transparent white
                    new Color(1, 0, 0, 0.3f);  // Semi-transparent red

                // Draw filled cubes for each node
                Gizmos.DrawCube(node.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
                
                // Draw wire frame for better visibility
                Gizmos.color = node.walkable ? 
                    new Color(1, 1, 1, 0.9f) : // More opaque white
                    new Color(1, 0, 0, 0.9f);  // More opaque red
                Gizmos.DrawWireCube(node.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
            }
        }
    }
#endif
}