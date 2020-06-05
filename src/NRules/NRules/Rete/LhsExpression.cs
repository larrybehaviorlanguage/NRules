﻿using System;
using System.Linq.Expressions;
using NRules.Utilities;

namespace NRules.Rete
{
    internal interface ILhsExpression<out TResult>
    {
        TResult Invoke(IExecutionContext context, NodeDebugInfo nodeInfo, Tuple tuple, Fact fact);
    }

    internal interface ILhsFactExpression<out TResult> : ILhsExpression<TResult>
    {
        TResult Invoke(IExecutionContext context, NodeDebugInfo nodeInfo, Fact fact);
    }

    internal interface ILhsTupleExpression<out TResult> : ILhsExpression<TResult>
    {
        TResult Invoke(IExecutionContext context, NodeDebugInfo nodeInfo, Tuple tuple);
    }

    internal sealed class LhsExpression<TResult> : ILhsExpression<TResult>
    {
        private readonly LambdaExpression _expression;
        private readonly Func<Tuple, Fact, TResult> _compiledExpression;
        private readonly IArgumentMap _argumentMap;

        public LhsExpression(LambdaExpression expression, Func<Tuple, Fact, TResult> compiledExpression, IArgumentMap argumentMap)
        {
            _expression = expression;
            _compiledExpression = compiledExpression;
            _argumentMap = argumentMap;
        }

        public TResult Invoke(IExecutionContext context, NodeDebugInfo nodeInfo, Tuple tuple, Fact fact)
        {
            Exception exception = null;
            TResult result = default;
            try
            {
                result = _compiledExpression(tuple, fact);
                return result;
            }
            catch (Exception e)
            {
                exception = e;
                bool isHandled = false;
                context.EventAggregator.RaiseLhsExpressionFailed(context.Session, e, _expression, _argumentMap, tuple, fact, nodeInfo, ref isHandled);
                throw new ExpressionEvaluationException(e, _expression, isHandled);
            }
            finally
            {
                if (context.EventAggregator.TraceEnabled)
                {
                    object eventResult;

                    // Extract the bool result out of BoolObj results since these
                    // to prevent accessing the event results after the BoolObj has been returned to the pool
                    if (result is BoolObj boolObj)
                    {
                        eventResult = boolObj.Value;
                    }
                    else
                    {
                        eventResult = result;
                    }
                    context.EventAggregator.RaiseLhsExpressionEvaluated(context.Session, exception, _expression, _argumentMap, eventResult, tuple, fact, nodeInfo);
                }
            }
        }
    }

    internal sealed class LhsFactExpression<TResult> : ILhsFactExpression<TResult>
    {
        private readonly LambdaExpression _expression;
        private readonly Func<Fact, TResult> _compiledExpression;
        private readonly IArgumentMap _argumentMap;

        public LhsFactExpression(LambdaExpression expression, Func<Fact, TResult> compiledExpression, IArgumentMap argumentMap)
        {
            _expression = expression;
            _compiledExpression = compiledExpression;
            _argumentMap = argumentMap;
        }

        public TResult Invoke(IExecutionContext context, NodeDebugInfo nodeInfo, Fact fact)
        {
            return Invoke(context, nodeInfo, null, fact);
        }

        public TResult Invoke(IExecutionContext context, NodeDebugInfo nodeInfo, Tuple tuple, Fact fact)
        {
            Exception exception = null;
            TResult result = default;
            try
            {
                result = _compiledExpression(fact);
                return result;
            }
            catch (Exception e)
            {
                exception = e;
                bool isHandled = false;
                context.EventAggregator.RaiseLhsExpressionFailed(context.Session, e, _expression, _argumentMap, tuple, fact, nodeInfo, ref isHandled);
                throw new ExpressionEvaluationException(e, _expression, isHandled);
            }
            finally
            {
                if (context.EventAggregator.TraceEnabled)
                {
                    object eventResult;
                    if (result is BoolObj boolObj)
                    {
                        eventResult = boolObj.Value;
                    }
                    else
                    {
                        eventResult = result;
                    }
                    context.EventAggregator.RaiseLhsExpressionEvaluated(context.Session, exception, _expression, _argumentMap, eventResult, tuple, fact, nodeInfo);
                }
            }
        }
    }

    internal sealed class LhsTupleExpression<TResult> : ILhsTupleExpression<TResult>
    {
        private readonly LambdaExpression _expression;
        private readonly Func<Tuple, TResult> _compiledExpression;
        private readonly IArgumentMap _argumentMap;

        public LhsTupleExpression(LambdaExpression expression, Func<Tuple, TResult> compiledExpression, IArgumentMap argumentMap)
        {
            _expression = expression;
            _compiledExpression = compiledExpression;
            _argumentMap = argumentMap;
        }

        public TResult Invoke(IExecutionContext context, NodeDebugInfo nodeInfo, Tuple tuple)
        {
            return Invoke(context, nodeInfo, tuple, null);
        }

        public TResult Invoke(IExecutionContext context, NodeDebugInfo nodeInfo, Tuple tuple, Fact fact)
        {
            Exception exception = null;
            TResult result = default;
            try
            {
                result = _compiledExpression(tuple);
                return result;
            }
            catch (Exception e)
            {
                exception = e;
                bool isHandled = false;
                context.EventAggregator.RaiseLhsExpressionFailed(context.Session, e, _expression, _argumentMap, tuple, fact, nodeInfo, ref isHandled);
                throw new ExpressionEvaluationException(e, _expression, isHandled);
            }
            finally
            {
                if (context.EventAggregator.TraceEnabled)
                {
                    object eventResult;
                    if (result is BoolObj boolObj)
                    {
                        eventResult = boolObj.Value;
                    }
                    else
                    {
                        eventResult = result;
                    }
                    context.EventAggregator.RaiseLhsExpressionEvaluated(context.Session, exception, _expression, _argumentMap, eventResult, tuple, fact, nodeInfo);
                }
            }
        }
    }
}