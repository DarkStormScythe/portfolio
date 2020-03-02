import pygame, Node

class NodeGrid:
    def __init__(self, winSize, gridSizeX, gridSizeY, nodeSize):
        self.winSize = winSize
        self.gridSizeX = gridSizeX
        self.gridSizeY = gridSizeY
        
        self.nodeSize = nodeSize
        self.nodeGrid = []

    def findWorldTopLeft(self):
        worldCenter = (self.winSize[0] / 2, self.winSize[1] / 2)
        x = worldCenter[0] - ((self.gridSizeX / 2) * self.nodeSize)
        y = worldCenter[1] - ((self.gridSizeY / 2) * self.nodeSize)
        return (x, y)

    def get_gridSize(self):
        return self.gridSizeX * self.gridSizeY

    def createGrid (self):
        for x in range(self.gridSizeX):
            yList = []
            self.nodeGrid.append(yList)
            for y in range(self.gridSizeY):
                self.nodeGrid[x].append(Node.Node((x * self.nodeSize + (self.nodeSize / 2), y * self.nodeSize + (self.nodeSize / 2)), x, y))
                
    def resetGrid (self):
        self.nodeGrid.clear()
        self.createGrid()

    def drawGrid (self, surface):
        # Center the grid to the screen
        # In case the nodes can't be divided evenly across the screen
        # Or if the grid is intentionally smaller than the screen
        worldTopLeft = self.findWorldTopLeft()
        for x in range(len(self.nodeGrid)):
            for y in range(len(self.nodeGrid[x])):
                # Change color depending on situations
                if self.nodeGrid[x][y].type == 0:
                    color = pygame.Color("white")
                elif self.nodeGrid[x][y].type == 1:
                    color = pygame.Color("gold")
                elif self.nodeGrid[x][y].type == 2:
                    color = pygame.Color("blue")
                elif self.nodeGrid[x][y].type == 4:
                    color = pygame.Color("cyan")
                elif self.nodeGrid[x][y].type == 5:
                    color = pygame.Color("green")
                elif self.nodeGrid[x][y].type == 6:
                    color = pygame.Color("red")
                else:
                    color = pygame.Color("black")

                # Draws a white square for each node
                # Find corners of the rect based on grid position
                topX = (self.nodeGrid[x][y].worldPosition[0] - (self.nodeSize / 2)) + worldTopLeft[0]
                topY = (self.nodeGrid[x][y].worldPosition[1] - (self.nodeSize / 2)) + worldTopLeft[1]
                #print(worldTopLeft)
                pygame.draw.rect(surface, color, (topX, topY, self.nodeSize, self.nodeSize), 0)

        # Draw grid lines
        for x in range(self.gridSizeX + 1):
            # Vertical lines
            pygame.draw.line(surface, (0,0,0), ((x * self.nodeSize) + worldTopLeft[0], 0), ((x * self.nodeSize) + worldTopLeft[0], self.winSize[1]), 2)
        for y in range(self.gridSizeY + 1):
            # Horizontal lines
            pygame.draw.line(surface, (0,0,0), (0, (y * self.nodeSize) + worldTopLeft[1]), (self.winSize[0], (y * self.nodeSize) + worldTopLeft[1]), 2)

    def setPoints (self, mousePos, type):
        offset = self.findWorldTopLeft()
        # This tells us where in the grid the mouse is at right now
        #print("Mouse position: " + str(mousePos))
        gridX = int(mousePos[0] - offset[0]) // self.nodeSize
        gridY = int(mousePos[1] - offset[1]) // self.nodeSize
        if gridX >= 0 and gridY >= 0:
            if gridX < len(self.nodeGrid) and gridY < len(self.nodeGrid[gridX]):
                self.nodeGrid[gridX][gridY].type = type

                if self.nodeGrid[gridX][gridY].type == 1 or self.nodeGrid[gridX][gridY].type == 2:
                    return self.nodeGrid[gridX][gridY]
            else:
                print("Outside of play area. Try again")
        else:
            print("Outside of play area. Try again")

    def drawPath (self, path):
        for i in range(len(path)):
            p = path[i]
            if p.type != 1:
                self.nodeGrid[p.gridX][p.gridY].type = 4

    def drawSets (self, openSet, closedSet):
        openHeap = openSet.get_Heap()
        for i in range(len(openHeap)):
            open = openHeap[i]
            if open != None and open.type == 0:
                self.nodeGrid[open.gridX][open.gridY].type = 5

        for i in range(len(closedSet)):
            closed = closedSet[i]
            if closed.type == 0 or closed.type == 5:
                self.nodeGrid[closed.gridX][closed.gridY].type = 6

    def getNeighbours (self, node):
        neighbours = []

        for x in range(-1,2,1):
            for y in range(-1,2,1):
                if x == 0 and y == 0:
                    continue

                # Check if the node is within the boundaries of the world
                checkX = node.gridX + x
                checkY = node.gridY + y

                if checkX >= 0 and checkY >= 0 and checkX < self.gridSizeX and checkY < self.gridSizeY:
                    neighbours.append(self.nodeGrid[checkX][checkY])

        return neighbours