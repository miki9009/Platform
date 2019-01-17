
using System;
using System.Collections.Generic;

namespace Objectives
{
    [Serializable]
    public class ObjectiveSequence
    {
        public List<Sequence> sequences = new List<Sequence>();
    }

    [Serializable]
    public class Sequence
    {
        public List<CollectionObjective> objectives = new List<CollectionObjective>();
    }
}