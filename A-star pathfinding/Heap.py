import Node

class Heap:
    def __init__(self, heapSize):
        self.items = [None] * heapSize
        self.currentItemCount = 0

    def Add (self, item):
        item.heapIndex = self.currentItemCount
        self.items[self.currentItemCount] = item
        self.SortUp(item)
        self.currentItemCount += 1

    def RemoveFirst (self):      
        firstItem = self.items[0];
        self.currentItemCount -= 1;

        self.items[0] = self.items[self.currentItemCount];
        self.items[0].set_heapIndex(0);
        self.SortDown(self.items[0]);
        return firstItem;

    def SortUp (self, item = Node.Node(None, None, None)):
        parentIndex = int((item.get_heapIndex() - 1) / 2)

        while True:
            parentItem = self.items[parentIndex]
            if (item.CompareTo(parentItem) > 0):
                self.Swap(item, parentItem);
            else:
                break
            parentIndex = int((item.get_heapIndex() - 1) / 2)

    def SortDown (self, item = Node.Node(None, None, None)):
        while True:
            childIndexL = item.get_heapIndex() * 2 + 1
            childIndexR = item.get_heapIndex() * 2 + 2
            swapIndex = 0

            if childIndexL < self.currentItemCount:
                swapIndex = childIndexL
                if childIndexR < self.currentItemCount:
                    if self.items[childIndexL].CompareTo(self.items[childIndexR]) < 0:
                        swapIndex = childIndexR

                if item.CompareTo(self.items[swapIndex]) < 0:
                    self.Swap(item, self.items[swapIndex])
                else:
                    return
            else:
                return

    def Swap (self, itemA = Node.Node(None, None, None), itemB = Node.Node(None, None, None)):
        self.items[itemA.get_heapIndex()] = itemB
        self.items[itemB.get_heapIndex()] = itemA
        itemAIndex = itemA.get_heapIndex()
        itemA.set_heapIndex(itemB.get_heapIndex())
        itemB.set_heapIndex(itemAIndex)

    def UpdateItem (self, item):
        self.SortUp(item)

    def Contains (self, item = Node.Node(None, None, None)):
        return self.items[item.get_heapIndex()] == item

    def get_Count(self):
        return self.currentItemCount

    def get_Heap(self):
        return self.items