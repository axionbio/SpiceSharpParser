﻿using SpiceSharpParser.Common.Evaluation;

namespace SpiceSharpParser.ModelReaders.Netlist.Spice.Evaluation.Functions.Math
{
    public class PwrsFunction : Function<double, double>
    {
        public PwrsFunction()
        {
            Name = "pwrs";
            ArgumentsCount = 2;
        }

        public override double Logic(string image, double[] args, EvaluationContext context)
        {
            double x = args[0];
            double y = args[1];

            return System.Math.Sign(x) * System.Math.Pow(System.Math.Abs(x), y);
        }
    }
}