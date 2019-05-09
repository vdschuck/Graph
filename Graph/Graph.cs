using System.Collections.Generic;
using System.Linq;

namespace Graph
{
    public interface IGraph<T>
    {
        IEnumerable<T> RoutesBetween(T source, T target);
    }

    public class Graph<T> : IGraph<T>
    {
        private readonly IEnumerable<ILink<T>> Links;
        private readonly IList<IGraphNode<T>> Nodes;       
       
        public Graph(IEnumerable<ILink<T>> links)
        {
            Links = links;
            Nodes = new List<IGraphNode<T>>();          

            // Create graph
            AddNodes();
            AddEdges();
        }

        public IEnumerable<T> RoutesBetween(T source, T target)
        {           
            var sourceNode = Search(source);
            var targetNode = Search(target);

            if (sourceNode != null && targetNode != null)
            {
                SearchPrevious(sourceNode, targetNode);

                // Single node may have more than one previous
                foreach (var node in targetNode.Previous)
                {
                    yield return DiscoverRoute(node, targetNode.Value);
                }
            }               
        }

        /// <summary>
        /// From the destination write the possible routes of the graph
        /// </summary>
        /// <param name="previousTarget"></param>
        /// <param name="targetValue"></param>
        /// <returns>type T</returns>
        private T DiscoverRoute(GraphNode<T> previousTarget, T targetValue)
        {            
            var routes = new LinkedList<T>();
            var navigation = new LinkedList<GraphNode<T>>();
            
            routes.AddFirst(targetValue);
            routes.AddFirst(previousTarget.Value);
            navigation.AddFirst(previousTarget);

            while (navigation.Count > 0)
            {
                GraphNode<T> currentNode = navigation.First();
                navigation.RemoveFirst();

                foreach (var node in currentNode.Previous)
                {
                    routes.AddFirst(node.Value);

                    if (node.Previous.Count > 0)
                    {
                        navigation.AddLast(node);
                    }
                }
            }

            return (T)(object) string.Join("-", routes);
        }

        /// <summary>
        /// Walks on the graph and adds to the current node the previous node
        /// </summary>
        /// <param name="sourceNode"></param>
        /// <param name="targetNode"></param>
        private void SearchPrevious(GraphNode<T> sourceNode, GraphNode<T> targetNode)
        {
            var path = new List<GraphNode<T>>();           
            var navigation = new LinkedList<GraphNode<T>>();

            path.Add(sourceNode);
            navigation.AddFirst(sourceNode);

            while (navigation.Count > 0)
            {
                GraphNode<T> currentNode = navigation.First.Value;
                navigation.RemoveFirst();

                foreach (var node in currentNode.Neighbors)
                {
                    if (node.Value.Equals(targetNode.Value))
                    {
                        node.AddPrevious(currentNode);
                        path.Add(node);
                    }
                    else if (path.Contains(node))
                    {
                        continue;
                    }
                    else
                    {
                        node.AddPrevious(currentNode);
                        path.Add(node);                       

                        if (node.Neighbors.Count > 0)
                        {
                            navigation.AddLast(node);
                        }                       
                    }
                }   
            }         
        }

        /// <summary>
        /// Add new node in graph
        /// </summary>
        public void AddNodes()
        {
            foreach(var link in Links)
            {
                if (Search(link.Source) == null)
                {
                    Nodes.Add(new GraphNode<T>(link.Source));
                }

                if(Search(link.Target) == null)
                {
                    Nodes.Add(new GraphNode<T>(link.Target));
                }
            }
        }

        /// <summary>
        /// Makes the connections between nodes
        /// </summary>
        public void AddEdges()
        {
            foreach(var link in Links)
            {
                GraphNode<T> node1 = Search(link.Source);
                GraphNode<T> node2 = Search(link.Target);

                if(node1 != null && node2 != null)
                {
                    if (!node1.Neighbors.Contains(node2))
                    {
                        node1.AddNeighbor(node2);
                    }
                }
            }            
        }

        /// <summary>
        /// Search for a node by value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public GraphNode<T> Search(T value)
        {
            foreach(GraphNode<T> node in Nodes)
            {
                if (node.Value.Equals(value))
                {
                    return node;
                }
            }

            return null;
        }  
    }
}
