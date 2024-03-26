
namespace IA.BehaviourTree
{
    public class ConditionNode : Node
    {
        private System.Func<bool> condition;

        public ConditionNode(System.Func<bool> condition)
        {
            this.condition = condition;
        }

        public override bool Tick()
        {
            return condition();
        }
    }
}