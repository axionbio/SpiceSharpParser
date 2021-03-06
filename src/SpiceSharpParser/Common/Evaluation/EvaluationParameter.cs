﻿using SpiceSharp;
using System;

namespace SpiceSharpParser.Common.Evaluation
{
    /// <summary>
    /// An parameter that triggers re-evaluation when changed.
    /// </summary>
    public class EvaluationParameter : Parameter<double>
    {
        private double _rawValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="EvaluationParameter"/> class.
        /// </summary>
        /// <param name="context">An expression context.</param>
        /// <param name="parameterName">A parameter name.</param>
        public EvaluationParameter(EvaluationContext context, string parameterName)
        {
            EvaluationContext = context ?? throw new ArgumentNullException(nameof(context));
            ParameterName = parameterName ?? throw new ArgumentNullException(nameof(parameterName));
        }

        /// <summary>
        /// Gets or sets the value of parameter.
        /// </summary>
        public override double Value
        {
            get => _rawValue;

            set
            {
                _rawValue = value;
                EvaluationContext.SetParameter(ParameterName, value);
            }
        }

        /// <summary>
        /// Gets the evaluator.
        /// </summary>
        protected EvaluationContext EvaluationContext { get; }

        /// <summary>
        /// Gets the parameter name.
        /// </summary>
        protected string ParameterName { get; }

        /// <summary>
        /// Clones the parameter.
        /// </summary>
        /// <returns>
        /// A clone of parameter.
        /// </returns>
        public override Parameter<double> Clone()
        {
            return new EvaluationParameter(EvaluationContext, ParameterName);
        }
    }
}