
namespace IA.BehaviourTree
{
    public class SelectorNode : Node
    {
        private Node[] children;

        public SelectorNode(params Node[] children)
        {
            this.children = children;
        }

        public override bool Tick()
        {
            foreach (Node child in children)
            {
                if (child.Tick())
                {
                    return true;
                }
            }
            return false;
        }
    }
}
