using System.Collections;
using System.Collections.Generic;

namespace PlanningAI.AStar
{
    public interface INode<T> where T : INode<T>
    {
        float TraversalCost { get; }
        float Heuristic { get; }

        IEnumerable<T> Neighbors { get; }
    }
}

