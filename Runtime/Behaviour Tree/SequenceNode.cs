
namespace IA.BehaviourTree
{
    public class SequenceNode : Node
    {
        private Node[] children;

        public SequenceNode(params Node[] children)
        {
            this.children = children;
        }

        public override bool Tick()
        {
            foreach (Node child in children)
            {
                if (!child.Tick())
                {
                    return false;
                }
            }
            return true;
        }
    }
}