using System.Collections.Generic;

/// <summary>
/// Hanlder to Connect to the Drone
/// @author Karsten Siedentopp
/// @date 04.09.2015
/// </summary>
class MaxBuffer<T> : Queue<T>
{
    private int? maxCapacity { get; set; }

    public MaxBuffer() { maxCapacity = null; }
    public MaxBuffer(int capacity) { maxCapacity = capacity; }

    public void Add(T newElement)
    {
        if (this.Count == (maxCapacity ?? -1)) this.Dequeue(); // no limit if maxCapacity = null
        this.Enqueue(newElement);
    }
}