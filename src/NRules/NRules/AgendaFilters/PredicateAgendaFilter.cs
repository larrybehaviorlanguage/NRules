using System.Collections.Generic;
using NRules.Utilities;

namespace NRules.AgendaFilters
{
    internal class PredicateAgendaFilter : IAgendaFilter
    {
        private readonly List<IActivationExpression<BoolObj>> _conditions;

        public PredicateAgendaFilter(IEnumerable<IActivationExpression<BoolObj>> conditions)
        {
            _conditions = new List<IActivationExpression<BoolObj>>(conditions);
        }

        public bool Accept(AgendaContext context, Activation activation)
        {
            foreach (var condition in _conditions)
            {
                if (!condition.Invoke(context, activation).GetValueAndReturnToPool()) return false;
            }
            return true;
        }
    }
}