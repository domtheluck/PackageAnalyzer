using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PackageAnalyzer.Parser
{
    internal class DirectedAcyclicGraph<T>
    {
        #region Public Methods

        public void AddEdge(T item, T dependency)
        {
            int itemIndex = AddNode(item);
            int dependencyIndex = AddNode(dependency);

            _edges[itemIndex].Add(dependencyIndex);
        }

        public int AddNode(T item)
        {
            if (!_nodes.Contains(item))
            {
                _nodes.Add(item);
            }

            int index = IndexOf(item);

            if (!_edges.ContainsKey(index))

            {
                _edges.Add(index, new List<int>());
            }

            return index;
        }

        public T[] Sort()
        {
            IList<T> nodes = new List<T>(_nodes);
            IDictionary<int, List<int>> edges = new Dictionary<int, List<int>>(_edges);

            List<T> sortedElements = new List<T>(nodes.Count);
            List<int> nodesWithoutIncomingEdges = new List<int>(nodes.Count);

            nodesWithoutIncomingEdges.AddRange(edges.Keys.Where(index => edges[index].Count == 0));

            while (nodesWithoutIncomingEdges.Count != 0)

            {
                T node = nodes[nodesWithoutIncomingEdges[0]];

                nodesWithoutIncomingEdges.RemoveAt(0);

                sortedElements.Add(node);

                int indexFrom = nodes.IndexOf(node);

                foreach (KeyValuePair<int, List<int>> item in edges)
                {
                    if (!item.Value.Contains(indexFrom))
                    {
                        continue;
                    }

                    item.Value.Remove(indexFrom);

                    if (item.Value.Count == 0)
                    {
                        nodesWithoutIncomingEdges.Add(item.Key);
                    }
                }
            }

            // if graph has edges then
            // output error message (graph has at least one cycle)
            // else 
            // output message (proposed topologically sorted order: L)


            if (edges.Values.Any(item => item.Count != 0))
            {
                throw new ApplicationException("Circular dependency detected!");
            }

            return sortedElements.ToArray();
        }

        private int IndexOf(T item)

        {
            return _nodes.IndexOf(item);
        }

        #endregion

        #region Private Members

        private readonly IList<T> _nodes = new List<T>();
        private readonly IDictionary<int, List<int>> _edges = new Dictionary<int, List<int>>();

        #endregion

        //#region Operations


        //public T[] Sort()

        //{

        //    IList<T> Nodes = new List<T>(nodes_);

        //    IDictionary<Int32, List<Int32>> Edges = new Dictionary<Int32, List<Int32>>(edges_);


        //    // L ← Empty list that will contain the sorted elements

        //    // S ← Set of all nodes with no incoming edges


        //    List<T> L = new List<T>(Nodes.Count);

        //    List<Int32> S = new List<int>(Nodes.Count);

        //    foreach (Int32 index in Edges.Keys)

        //        if (Edges[index].Count == 0)

        //            S.Add(index);


        //    // while S is non-empty do

        //    // remove a node n from S

        //    // insert n into L

        //    // for each node m with an edge e from n to m do

        //    // remove edge e from the graph

        //    // if m has no other incoming edges then

        //    // insert m into S


        //    while (S.Count != 0)

        //    {

        //        T node = Nodes[S[0]];

        //        S.RemoveAt(0);


        //        L.Add(node);


        //        Int32 index_from = Nodes.IndexOf(node);


        //        foreach (KeyValuePair<Int32, List<Int32>> item in Edges)

        //        {

        //            if (item.Value.Contains(index_from))

        //            {

        //                item.Value.Remove(index_from);

        //                if (item.Value.Count == 0)

        //                    S.Add(item.Key);

        //            }

        //        }

        //    }


        //    // if graph has edges then

        //    // output error message (graph has at least one cycle)

        //    // else 

        //    // output message (proposed topologically sorted order: L)


        //    foreach (List<Int32> item in Edges.Values)

        //        if (item.Count != 0)

        //            throw new ApplicationException("Circular dependency detected!");


        //    return L.ToArray();

        //}


        //public T[] Sort(bool reverse)

        //{

        //    T[] array = Sort();

        //    if (reverse)

        //        Array.Reverse(array);

        //    return array;

        //}


        //#endregion


        //#region Implementation


        


        //#endregion
    }
}