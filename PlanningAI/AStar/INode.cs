using System.Collections;
using System.Collections.Generic;

namespace PlanningAI.AStar
{
    public interface INode<T> where T : INode<T>
    {
        float GetTraversalCost(T source);
        float GetHeuristic(T goal);

        IEnumerable<T> Neighbors { get; }

        bool Equivalent(T other);
    }
}

