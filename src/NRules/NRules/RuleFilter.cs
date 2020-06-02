using System.Collections.Generic;
using NRules.AgendaFilters;
using NRules.Utilities;

namespace NRules
{
    internal interface IRuleFilter
    {
        IEnumerable<IActivationExpression<BoolObj>> Conditions { get; }
        IEnumerable<IActivationExpression<object>> KeySelectors { get; }
    }

    internal class RuleFilter : IRuleFilter
    {
        private readonly List<IActivationExpression<BoolObj>> _conditions;
        private readonly List<IActivationExpression<object>> _keySelectors;

        public RuleFilter(IEnumerable<IActivationExpression<BoolObj>> conditions, IEnumerable<IActivationExpression<object>> keySelectors)
        {
            _conditions = new List<IActivationExpression<BoolObj>>(conditions);
            _keySelectors = new List<IActivationExpression<object>>(keySelectors);
        }

        public IEnumerable<IActivationExpression<BoolObj>> Conditions => _conditions;
        public IEnumerable<IActivationExpression<object>> KeySelectors => _keySelectors;
    }
}
