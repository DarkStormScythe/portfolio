using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heap<T> where T : IHeapItem<T>
{
    T[] items;
    int currentItemCount;

    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }

    public void Add(T item)
    {
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        SortUp(item);
        currentItemCount++;
    }

    public T RemoveFirst()
    {
        T firstItem = items[0];
        currentItemCount--;

        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return firstItem;
    }

    // Comparing and swapping an item with the lowest index of its children
    void SortDown (T item)
    {
        while (true)
        {
            // Find the heap index of both children
            int childIndexL = item.HeapIndex * 2 + 1;
            int childIndexR = item.HeapIndex * 2 + 2;
            int swapIndex = 0;

            // If the resultant indexes are outside of the current size of the items list, then disregard. Otherwise,
            // set the swapIndex to the left child by default. This swapIndex will determine which child the focused item
            // will be swapping with.
            if (childIndexL < currentItemCount)
            {
                swapIndex = childIndexL;

                if (childIndexR < currentItemCount)
                {
                    // If it turns out that the right child is less than the left child, change the swapIndex to the right child instead.
                    if (items[childIndexL].CompareTo(items[childIndexR]) < 0) {
                        swapIndex = childIndexR;
                    }
                }

                // If the focused item has an index less than the child to be swapped (by now you would have found which child has the lower
                // cost), then swap the item with its child. Otherwise, don't swap and move on
                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(item, items[swapIndex]);
                } 
                else
                {
                    return;
                }
            } 
            else
            {
                return;
            }
        }
    }

    void SortUp (T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;

        while (true)
        {
            T parentItem = items[parentIndex];
            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            } 
            else
            {
                break;
            }
            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    // Swap the index of 2 given items
    void Swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;
        int itemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
    }

    // In case we want to change the priority of an item (like if we want to update the fCost in one of our pathfinding
    // nodes, which would result in a lower priority within the heap, so we'll want to update the position of this node
    // within the heap)

    // In this particular case, there will not be an instance of needing to decrease the priority of any given node, so a
    // method to sort down is unncessary. This may change in another scenario.
    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    public bool Contains(T item)
    {
        // Checks if the heap index of the item being passed in is the same as the item within the items list
        return Equals(items[item.HeapIndex], item);
    }

    public int Count
    {
        get
        {
            return currentItemCount;
        }
    }
}

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex { get; set; }
}