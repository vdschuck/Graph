using System;
using System.Collections.Generic;

namespace Graph
{
    public interface IGraphNode<T>
    {
        bool AddNeighbor(GraphNode<T> neighbor);
        bool RemoveNeighbor(GraphNode<T> neighbor);
        bool AddPrevious(GraphNode<T> previous);
    }

    public class GraphNode<T> : IGraphNode<T>
    {
        public T Value { get; }

        public List<GraphNode<T>> Previous;

        public List<GraphNode<T>> Neighbors;    


        public GraphNode(T value)
        {
            Value = value;
            Previous = new List<GraphNode<T>>();
            Neighbors = new List<GraphNode<T>>();            
        }  

        public bool AddNeighbor(GraphNode<T> neighbor)
        {
            if (Neighbors.Contains(neighbor))
            {
                return false;
            }
            else
            {
                Neighbors.Add(neighbor);
                return true;
            }
        }

        public bool AddPrevious(GraphNode<T> previous)
        {
            if (Previous.Contains(previous))
            {
                return false;
            }
            else
            {
                Previous.Add(previous);
                return true;
            }
        }

        public bool RemoveNeighbor(GraphNode<T> neighbor)
        {
            return Neighbors.Remove(neighbor);
        }       
    }
}
