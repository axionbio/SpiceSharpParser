﻿using SpiceSharpBehavioral.Parsers;
using SpiceSharpParser.Common.Evaluation.Expressions;
using System;
using System.Collections.Generic;

namespace SpiceSharpParser.Common.Evaluation.Functions
{
    public class ExpressionFunction : Function<double, double>, IDerivativeFunction<double, double>
    {
        public ExpressionFunction(string name, List<string> arguments, string expression)
        {
            Name = name;
            ArgumentsCount = arguments.Count;
            Arguments = arguments;
            Expression = expression;
        }

        public List<string> Arguments { get; }

        public string Expression { get; }

        public override double Logic(string image, double[] args, EvaluationContext context)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var childContext = context.CreateChildContext(string.Empty, false);
            for (var i = 0; i < Arguments.Count; i++)
            {
                childContext.SetParameter(Arguments[i], args[i]);
            }

            var @value = childContext.Evaluate(Expression);
            return @value;
        }

        public Derivatives<Func<double>> Derivative(string image, FunctionFoundEventArgs<Derivatives<Func<double>>> args, EvaluationContext context)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var childContext = context.CreateChildContext(string.Empty, false);
            for (var i = 0; i < Arguments.Count; i++)
            {
                var iLocal = i;
                childContext.SetParameter(Arguments[i], args[iLocal][0]());
                childContext.Arguments.Add(Arguments[i], new FunctionExpression(args[iLocal][0]));
            }

            var childParser = childContext.GetDeriveParser();
            var parseResult = childParser.Parse(Expression);
            return parseResult;
        }
    }
}