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
        private const int MaxSteps = 300;
        private int _steps;
        private readonly List<InternalNode<T>> _openList = new List<InternalNode<T>>();
        private readonly Dictionary<T, InternalNode<T>> _internalNodes = new Dictionary<T, InternalNode<T>>(); 
        private InternalNode<T> _goalNode;

        public T Start { get; set; }
        public T Goal { get; set; }

        private void Reset()
        {
            _steps = 0;
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
            var startInt = GetInternal(Start);

            startInt.TotalCost = Start.GetHeuristic(Goal);
            startInt.Open = true;
            _openList.Add(startInt);
        }

        public IEnumerable<T> TraverseFromGoal()
        {
            var node = _goalNode;
            yield return node.Data;

            while (!Equals(node.Data, Start))
            {
                node = node.Parent;
                yield return node.Data;
            }
        }

        public AStarResult Step()
        {
            if (_steps > MaxSteps)
            {
                return AStarResult.Failed;
            }

            _steps += 1;
            if (_openList.Count == 0)
            {
                return AStarResult.Failed;
            }

            var parent = _openList.Min();
            parent.Closed = true;

            _openList.Remove(parent);
            if (parent.Data.Equivalent(Goal))
            {
                _goalNode = parent;
                return AStarResult.Found;
            }

            parent.Closed = true;

            foreach (var child in parent.Data.Neighbors)
            {
                if (child == null)
                    continue;

                var cost = parent.GivenCost + child.GetTraversalCost(parent.Data);
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
                    var heuristic = child.GetHeuristic(Goal);
                    childNode.TotalCost = cost + heuristic;
                    _openList.Add(childNode);
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
                _internalNodes[node] = new InternalNode<T>
                {
                    Data = node
                };
            }

            return _internalNodes[node];
        }
    }
}
