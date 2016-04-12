using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlanningAI.AStar
{
    public class InternalNode<T> where T : INode<T>
    {
        public InternalNode<T> Parent { get; set; } 

        public T Data { get; set; }
        public float GivenCost { get; set; } = 0;
        public float TotalCost { get; set; } = 0;

        public bool Open { get; set; } = false;
        public bool Closed { get; set; } = false;
    }
}
