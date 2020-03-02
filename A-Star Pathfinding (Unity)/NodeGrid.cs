using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGrid : MonoBehaviour
{
    public LayerMask unwalkableMask;
    // Defines the size of the grid in the world
    public Vector2 gridWorldSize;
    // Defines the size of each node
    public float nodeRadius;
    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    // Only draw path nodes
    //public bool pathNodeOnly = false;
    public bool displayGridGizmos = false;

    private void Awake()
    {
        // How many nodes can we fit into the grid (based on nodeRadius)?
        nodeDiameter = nodeRadius * 2;
        // We don't want any half or partial nodes within the grid, so we keep things as integers
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();

    }

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    void CreateGrid()
    {
        // In the Start function, we have found out how many nodes our grid should have, determined by the size of the grid and the diameter of the nodes.
        // Now we create the grid by first declaring a new grid made up of the number of nodes as determined by gridSizeX and gridSizeY.
        grid = new Node[gridSizeX, gridSizeY];
        // Get the bottom-left corner of the world
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        // Looping through the nodes, we'll figure out the positions that each node will be representing, and checking if they are on walkable ground by checking
        // if the worldPoint is inside an obstacle. We can use the Physics.CheckSphere function to create a collision detection field with the size of the node,
        // and seeing if it collides with the collision field of an obstacle with the Unwalkable layer mask. We then create the node by passing in both the worldPoint, 
        // and whether it can be walked on or not.
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeX; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask);
                grid[x, y] = new Node(walkable, worldPoint, x ,y);
            }
        }
    }

    // Get the neighbouring nodes of a given node
    public List<Node> GetNeighbours(Node node)
    {
        // Stores the list of neighbouring nodes
        List<Node> neighbours = new List<Node>();

        // -1 to 1; because the neighbours will only ever be (at most) the 8 nodes surrounding the current node, which are ever only 1 unit apart.
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                // Skip the node in the middle (since it's the node being considered
                if (x == 0 && y == 0)
                    continue;

                // Find the neighbouring nodes' coordinates based on the focused node
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                // Check to make sure that the node is within the grid space, and not beyond
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public Node NodeFromWorldPoint (Vector3 worldPosition)
    {
        // We need to convert the coordinates of a given worldPosition into grid coordinates, which can be done if we change the coordinates to percentages,
        // where the far left is 0%, the center is 50% and the far right is 100% (represented by 0.0 to 1.0). If we use these percentages and multiply them by
        // the size of our grid, we can get the grid coordinates of the worldPosition.
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    //public List<Node> path;

    // Visualize grid
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        // PathRequest Update: We now want the NodeGrid class to have nothing to do with the path itself, so we are now removing
        // all references to the path
        //if (pathNodeOnly)
        //{
        //    if (path != null)
        //    {
        //        foreach (Node n in path)
        //        {
        //            Gizmos.color = Color.cyan;
        //            Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
        //        }
        //    }
        //}
        //else
        //{
            if (grid != null && displayGridGizmos)
            {
                foreach (Node n in grid)
                {
                    Gizmos.color = (n.walkable) ? Color.white : Color.red;
                    //if (path != null)
                    //{
                    //    if (path.Contains(n))
                    //        Gizmos.color = Color.cyan;
                    //}
                    // We take a little bit off the actual size of the node diameter, so when the cube is rendered, there will be a gap to show the individual nodes,
                    // rather than the edges of the cubes merging together to form one giant flat board
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
                }
            }
        //}
    }
}
