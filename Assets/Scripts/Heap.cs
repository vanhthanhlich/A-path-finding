using System;

public class Heap<T> where T : HeapItem<T>
{
    private T[] Items;
    private int currentItemCount;

    public int Count { get { return currentItemCount; } }
    public Heap(int MaxSize)
    {
        currentItemCount = 0;
        Items = new T[MaxSize];
    }

    public bool Contains(T item) => Equals(Items[item.Index], item);

    public T get()
    {
        T curr = Items[0];
        for(int i = 1; i < currentItemCount; i++)
        {
            if (curr.CompareTo(Items[i]) < 0) curr = Items[i];
        }
        return curr;
    }
    public void Add(T item)
    {
        if (!Contains(item))
        {
            item.Index = currentItemCount;
            Items[currentItemCount++] = item;
        }
        else
        {
           Items[item.Index] = item;
        }

        SortUp(item);
    }

    public T RemoveFirst()
    {
        T FirstItem = Items[0];
        Swap(Items[0], Items[--currentItemCount]);

        SortDown(Items[0]);

        return FirstItem;
    }

    private void SortDown(T item)
    {
        if (currentItemCount == 0) return;

        while (true)
        {

            int LeftIndex = (item.Index * 2) + 1;
            int RightIndex = (item.Index * 2) + 2;

            if (LeftIndex >= currentItemCount) return;

            T swapItem = Items[LeftIndex];
            if (RightIndex < currentItemCount && swapItem.CompareTo(Items[RightIndex]) < 0)
            {
                swapItem = Items[RightIndex];
            }

            if (swapItem.CompareTo(item) > 0)
            {
                Swap(swapItem, item);
            }
            else break;
        }
    }

    private void SortUp(T item)
    {
        if (currentItemCount == 0) return;

        while (item.Index != 0)
        {
            T parentItem = Items[(item.Index - 1) / 2];

            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            else break;

        }
    }

    private void Swap(T itemA, T itemB) // ?, ref ao vai ca dai
    {
        Items[itemA.Index] = itemB;
        Items[itemB.Index] = itemA;

        (itemA.Index, itemB.Index) = (itemB.Index, itemA.Index);
    }

}

public interface HeapItem<T> : IComparable<T>
{
    public int Index { get; set; }
}
