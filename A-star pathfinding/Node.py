class Node:
    def __init__(self, pos, gridX, gridY):
        self.type = 0       # Determines type of node (0 = walkable, 1 = target, 2 = start, 3 = obstacle, 4 = path, 5 = open, 6 = closed)
        self.worldPosition = pos
        self.gridX = gridX
        self.gridY = gridY
        self.gCost = 0
        self.hCost = 0
        self.parent = None
        self.heapIndex = 0

    def fCost(self):
        return self.gCost + self.hCost

    def get_heapIndex(self):
        return self.heapIndex

    def set_heapIndex(self, value):
        self.heapIndex = value

    def CompareTo(self, node):
        # Returns 1 if cost is lower than compared node, or -1 if cost is higher
        compare = self.fCost() - node.fCost()
        if compare == 0:
            compare = self.hCost - node.hCost

        if compare > 0:
            compare = -1
        elif compare < 0:
            compare = 1

        return compare
