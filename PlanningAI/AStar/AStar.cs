using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanningAI.AStar
{
    public enum AStarResult
    {
        Working,
        Found,
        Failed
    }

    public class AStar<T> where T : INode<T>
    {
        private readonly List<InternalNode<T>> _openList = new List<InternalNode<T>>();
        private readonly Dictionary<T, InternalNode<T>> _internalNodes = new Dictionary<T, InternalNode<T>>(); 
        private InternalNode<T> _goalNode; 

        public T Start { get; set; }
        public T Goal { get; set; }

        private void Reset()
        {
            _openList.Clear();
            _internalNodes.Clear();
        }

        public AStarResult RunToCompletion(T start, T goal)
        {
            AStarResult result;

            Setup(start, goal);
            for (;;)
            {
                result = Step();
                if (result != AStarResult.Working)
                    break;
            }

            return result;
        }

        public void Setup(T start, T goal)
        {
            Reset();
            Start = start;
            Goal = goal;
        }

        public IEnumerable<T> TraverseFromGoal()
        {
            var node = _goalNode;
            yield return node.Data;

            while (node.Parent != null)
            {
                node = node.Parent;
                yield return node.Data;
            }
        }

        public AStarResult Step()
        {
            if (_openList.Count == 0)
            {
                return AStarResult.Failed;
            }

            var parent = _openList.Min();
            _openList.Remove(parent);
            if (Equals(parent.Data, Goal))
            {
                _goalNode = parent;
                return AStarResult.Found;
            }

            parent.Closed = true;

            foreach (var child in parent.Data.Neighbors)
            {
                var cost = parent.GivenCost + child.TraversalCost;
                var childNode = GetInternal(child);

                if (childNode.Open || childNode.Closed)
                {
                    if (childNode.GivenCost <= cost)
                        continue;

                    var heuristic = childNode.TotalCost - childNode.GivenCost;
                    childNode.TotalCost = cost + heuristic;

                    if (childNode.Closed)
                    {
                        childNode.Closed = false;
                        _openList.Add(childNode);
                    }
                }
                else
                {
                    var heuristic = child.Heuristic;
                    childNode.TotalCost = cost + heuristic;
                }

                childNode.Parent = parent;
                childNode.GivenCost = cost;
                childNode.Open = true;
            }

            return AStarResult.Working;
        }

        private InternalNode<T> GetInternal(T node)
        {
            if (!_internalNodes.ContainsKey(node))
            {
                _internalNodes[node] = new InternalNode<T>();
            }

            return _internalNodes[node];
        }
    }
}
