import pygame, NodeGrid, Heap

class PathFinding:
    def __init__(self, target, start, grid):
        self.target = target
        self.start = start

        self.grid = grid

    # stepType variable determines how the function runs. False = immediate path, True = show workings
    def findPath(self, displayStep = False, surface = None, gridDraw = None):
        bugCheck1 = 0
        bugOut = False
        # Create open and closed set
        openSet = Heap.Heap(self.grid.get_gridSize())
        closedSet = []
        openSet.Add(self.start)

        # For step-by-step type
        if displayStep == True:
            clock = pygame.time.Clock()
            stepTime = 0

        #print("Finding Path")

        while openSet.get_Count() > 0:
            bugCheck1 += 1
            # Remove node from openSet and add to closedSet
            currentNode = openSet.RemoveFirst()
            closedSet.append(currentNode)

            # If target node is reached, then exit loop
            if currentNode == self.target or bugOut == True:
                if bugOut == True:
                    print("Too many iterations. Current path is as follows. Number of iterations: " + str(bugCheck1))
                    print("Closed Set: " + str(len(closedSet)))
                else:
                    #print("Path found!")
                    #print("Closed Set: " + str(len(closedSet)))
                    self.grid.drawPath(self.retracePath(self.start, self.target))
                return

            # Check neighbouring nodes
            bugCheck2 = 0
            neighbours = self.grid.getNeighbours(currentNode)
            for n in range(len(neighbours)):
                neighbour = neighbours[n]

                # Bug Checking
                bugCheck2 += 1
                #Sprint("Iteration: " + str(bugCheck1) + " | Checking node no.: " + str(n))
                currentNodeGrid = currentNode.gridX, currentNode.gridY
                neighbourGrid = neighbour.gridX, neighbour.gridY
                #print("Current node: " + str(currentNodeGrid) + " | Neighbour node: " + str(neighbourGrid))

                # Skip checking if node is not walkable, or if the node has been checked before
                if neighbour.type == 3 or neighbour in closedSet:
                    #print("Neighbour is current node or is in closed set")
                    continue

                # Calculate the new distance from the start node to the neighbouring node of the current node. Only add the neighbour node to the
                # open set for consideration if the new distance from the neighbouring node to the start node is smaller than before, or if it has not
                # yet been considered before. This ignores nodes that have already been considered thanks to the if statement above.
                newCostToNeighbour = currentNode.gCost + self.getDistance(currentNode, neighbour)
                #print("New cost to neighbour: " + str(newCostToNeighbour))
                if newCostToNeighbour < neighbour.gCost or openSet.Contains(neighbour) == False:
                    #print ("Calculating Costs")
                    # Recalculate costs of neighbour node based on current node. We'll also parent the node to this one, so we can use the parent list to
                    # backtrace our path to the start node.
                    neighbour.gCost = newCostToNeighbour
                    neighbour.hCost = self.getDistance(neighbour, self.target)
                    neighbour.parent = currentNode

                    if openSet.Contains(neighbour) == False:
                        #print("Not yet in open set. Appending...")
                        openSet.Add(neighbour)
                    else:
                        openSet.UpdateItem(neighbour)
                if bugCheck2 > 50:
                    print("Too many neighbour iterations: " + str(bugCheck2))
                    break

            # Draw step-by-step
            if displayStep == True:
                while stepTime < 100:
                    self.grid.drawSets (openSet, closedSet)
                    gridDraw.drawGrid(surface)
                    pygame.display.update()

                    stepTime += clock.get_rawtime()
                    clock.tick()

                    for event in pygame.event.get():
                        if event.type == pygame.QUIT:
                            return -1
                stepTime = 0

            if bugCheck1 > 10000:
                print("Too many iterations! " + str(bugCheck))
                bugOut = True

        print("No path found! Cannot reach target!")

    def retracePath (self, startNode, targetNode):
        path = []
        currentNode = targetNode

        while currentNode != startNode:
            path.append(currentNode)
            currentNode = currentNode.parent

        path.reverse()
        return path

    def getDistance (self, nodeA, nodeB):
        distX = abs(nodeA.gridX - nodeB.gridX)
        distY = abs(nodeA.gridY - nodeB.gridY)

        if distX > distY:
            return 14 * distY + 10 * (distX - distY);
        return 14 * distX + 10 * (distY - distX);