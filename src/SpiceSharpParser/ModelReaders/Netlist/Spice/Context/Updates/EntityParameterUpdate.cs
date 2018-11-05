﻿using SpiceSharpParser.Common.Evaluation;

namespace SpiceSharpParser.ModelReaders.Netlist.Spice.Context
{
    public abstract class EntityParameterUpdate
    {
        public string ParameterName { get; set; }

        public abstract double GetValue(IEvaluator evaluator, ExpressionContext context);
    }
}