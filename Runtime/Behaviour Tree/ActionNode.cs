
namespace IA.BehaviourTree
{
    public class ActionNode : Node
    {
        private System.Action action;

        public ActionNode(System.Action action)
        {
            this.action = action;
        }

        public override bool Tick()
        {
            action();
            return true;
        }
    }
}