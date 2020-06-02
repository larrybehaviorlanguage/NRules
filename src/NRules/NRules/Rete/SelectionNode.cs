using NRules.RuleModel;
using NRules.Utilities;

namespace NRules.Rete
{
    internal class SelectionNode : AlphaNode
    {
        private readonly ILhsFactExpression<BoolObj> _compiledExpression;

        public ExpressionElement ExpressionElement { get; }

        public SelectionNode(ExpressionElement expressionElement, ILhsFactExpression<BoolObj> compiledExpression)
        {
            ExpressionElement = expressionElement;
            _compiledExpression = compiledExpression;
        }

        public override bool IsSatisfiedBy(IExecutionContext context, Fact fact)
        {
            try
            {
                return _compiledExpression.Invoke(context, NodeInfo, fact).GetValueAndReturnToPool();
            }
            catch (ExpressionEvaluationException e)
            {
                if (!e.IsHandled)
                {
                    throw new RuleLhsExpressionEvaluationException(
                        "Failed to evaluate condition", e.Expression.ToString(), e.InnerException);
                }

                return false;
            }
        }

        public override void Accept<TContext>(TContext context, ReteNodeVisitor<TContext> visitor)
        {
            visitor.VisitSelectionNode(context, this);
        }
    }
}