using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Pathfinding : MonoBehaviour {

    PathRequestManager requestManager;
    NodeGrid grid;

    private void Awake() {
        requestManager = GetComponent<PathRequestManager>();
        grid = GetComponent<NodeGrid>();
    }

    public void StartFindPath(Vector3 startPos, Vector3 endPos) {
        StartCoroutine(FindPath(startPos, endPos));
    }

    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos) {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        // First, we'll need to know which node we will be starting from, and which node we will be going to
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        // Check if the start and target nodes are walkable (since all would be useless if either are in places that are supposed to
        // be inaccessible)
        if (startNode.walkable && targetNode.walkable) {
            // We need an open set and closed set of nodes to tell us which nodes have been considered and which will need
            // to be considered.

            // The Open Set contains nodes that are next to the nodes that have been considered, which are within the Closed Set.
            // When the node has been thoroughly considered, we will move this node into the closed set.
            // When first initialized, the closed set will be empty, and the open set will only have the starting node within to be
            // considered.
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            // While there are nodes to be considered, we keep the first node in mind while we scan through the list of nodes after it.
            // If we find a node that has a lower overall cost than the current node, we will shift the focus to this new node instead.
            // If that node has the same overall cost as the focused node, we look at the herustic cost of both nodes, and focus on the
            // one with the lower hCost instead. Once done, we need to shift the previously focused node to the Closed Set, and remove
            // it from the Open Set.
            while (openSet.Count > 0) {
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                // If we have reached the target node, we can leave the loop
                if (currentNode == targetNode) {
                    sw.Stop();
                    print("Path found in: " + sw.ElapsedMilliseconds + "ms");
                    pathSuccess = true;
                    break;
                }

                // Check each neighbouring node
                foreach (Node neighbour in grid.GetNeighbours(currentNode)) {
                    // Skip the checking for the focused node if it is unwalkable, or if it has already been considered before.
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                        continue;

                    // Calculate the new distance from the start node to the neighbouring node of the current node. Only add the neighbour node to the
                    // open set for consideration if the new distance from the neighbouring node to the start node is smaller than before, or if it has not
                    // yet been considered before. This ignores nodes that have already been considered thanks to the if statement above.
                    int newMoveCost2Neighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.weight;
                    if (newMoveCost2Neighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
                        // Recalculate costs of neighbour node based on current node. We'll also parent the node to this one, so we can use the parent list to
                        // backtrace our path to the start node.
                        neighbour.gCost = newMoveCost2Neighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour)) {
                            openSet.Add(neighbour);
                        } else {
                            openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }
            // If loop is completed without finding a path, we can conclude that the target cannot be reached.
            if (!pathSuccess) {
                UnityEngine.Debug.Log("No path found. Target cannot be reached");
            }
        }
        yield return null;
        if (pathSuccess) {
            waypoints = RetracePath(startNode, targetNode);
        }
        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    // Backtracing of the resultant path based on the parent nodes
    // Beginning with the target (end) node, we'll find the parents of each node in succession, until we reach the
    // starting node. We'll then reverse the list (so that it begins at the start node rather than the end node) and
    // send it to the Grid class for visualization
    Vector3[] RetracePath(Node startNode, Node targetNode) {
        List<Node> path = new List<Node>();
        Node currentNode = targetNode;

        while (currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        // PathRequest Update: instead of simply returning the path List, we will instead simplfy the path first, and return
        // the waypoints instead.
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;


        // For the visualization of the path
        // PathRequest Update: No need for grid to have reference to path
        //grid.path = path;
    }

    // PathRequest Update: This method streamlines the path by removing node points between two points in a straight line.
    // This leaves only the node points that require the changing of direction to face the next node
    Vector3[] SimplifyPath (List<Node> path) {
        List<Vector3> waypoints = new List<Vector3>();
        // Stores direction of the previous vector. We use direction here because we want to figure out if the node we're looking
        // at is in fact looking in a different direction than the previous node. That will tell us if this node requires the path
        // follower to change direction. If not, we can remove the node to simplify the path
        Vector2 directionOld = Vector2.zero;

        // Since this loop requires looking at 2 nodes at once, we either need to start late or end early. With a for loop, it is
        // generally easier to start later than it is to end earlier.
        for (int i = 1; i < path.Count; i++) {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if (directionNew != directionOld) {
                waypoints.Add(path[i].worldPosition);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }

    int GetDistance(Node nodeA, Node nodeB) {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}
