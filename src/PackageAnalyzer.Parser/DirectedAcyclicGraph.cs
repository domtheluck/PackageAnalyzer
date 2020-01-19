// ***********************************************************************
// Copyright (c) 2019 Dominik Lachance
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;

namespace PackageAnalyzer.Parser
{
    /// <summary>
    /// Used to order a list of nodes based on their dependencies.
    /// </summary>
    /// <typeparam name="T">Node type.</typeparam>
    internal class DirectedAcyclicGraph<T>
    {
        #region Private Members

        private readonly IList<T> _nodes = new List<T>();
        private readonly IDictionary<int, List<int>> _edges = new Dictionary<int, List<int>>();

        #endregion

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

                foreach ((int key, List<int> value) in edges)
                {
                    if (!value.Contains(indexFrom))
                    {
                        continue;
                    }

                    value.Remove(indexFrom);

                    if (value.Count == 0)
                    {
                        nodesWithoutIncomingEdges.Add(key);
                    }
                }
            }

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
    }
}