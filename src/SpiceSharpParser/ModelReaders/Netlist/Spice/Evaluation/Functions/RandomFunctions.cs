﻿using System;
using SpiceSharpParser.Common.Evaluation;

namespace SpiceSharpParser.ModelReaders.Netlist.Spice.Evaluation.Functions
{
    public class RandomFunctions
    {
        /// <summary>
        /// Get a mc() function.
        /// </summary>
        /// <returns>
        /// A new instance of random mc function.
        /// </returns>
        public static Function CreateMc()
        {
            Function function = new Function();
            function.Name = "mc";
            function.VirtualParameters = false;
            function.ArgumentsCount = 2;

            function.DoubleArgsLogic = (image, args, evaluator, context) =>
            {
                if (args.Length != 2)
                {
                    throw new Exception("mc() expects two arguments");
                }

                Random random = context.Randomizer.GetRandom(context.Seed);
                double x = args[0];
                double tol = args[1];

                double min = x - (tol * x);
                double randomChange = random.NextDouble() * 2.0 * tol * x;

                return min + randomChange;
            };

            return function;
        }

        /// <summary>
        /// Get a gauss() function.
        /// </summary>
        /// <returns>
        /// A new instance of random gauss function.
        /// </returns>
        public static Function CreateGauss()
        {
            Function function = new Function();
            function.Name = "gauss";
            function.VirtualParameters = false;
            function.ArgumentsCount = 1;

            function.DoubleArgsLogic = (image, args, evaluator, context) =>
            {
                if (args.Length != 1)
                {
                    throw new Exception("gauss() expects one argument");
                }

                Random random = context.Randomizer.GetRandom(context.Seed);

                double p1 = 1 - random.NextDouble();
                double p2 = 1 - random.NextDouble();

                double std = Math.Sqrt(-2.0 * Math.Log(p1)) * Math.Sin(2.0 * Math.PI * p2);
                return (double)args[0] * std;
            };

            return function;
        }

        /// <summary>
        /// Get a random() function. It generates number between 0.0 and 1.0 (uniform distribution).
        /// </summary>
        /// <returns>
        /// A new instance of random function.
        /// </returns>
        public static Function CreateRandom()
        {
            Function function = new Function();
            function.Name = "random";
            function.VirtualParameters = false;
            function.ArgumentsCount = 0;

            function.DoubleArgsLogic = (image, args, evaluator, context) =>
            {
                if (args.Length != 0)
                {
                    throw new Exception("random() expects no arguments");
                }

                Random random = context.Randomizer.GetRandom(context.Seed);
                return random.NextDouble();
            };

            return function;
        }

        /// <summary>
        /// Get a flat() function. It generates number between -x and +x.
        /// </summary>
        /// <returns>
        /// A new instance of random function.
        /// </returns>
        public static Function CreateFlat()
        {
            Function function = new Function();
            function.Name = "flat";
            function.VirtualParameters = false;
            function.ArgumentsCount = 1;

            function.DoubleArgsLogic = (image, args, evaluator, context) =>
            {
                if (args.Length != 1)
                {
                    throw new ArgumentException("flat() function expects one argument");
                }

                Random random = context.Randomizer.GetRandom(context.Seed);

                double x = (double)args[0];

                return (random.NextDouble() * 2.0 * x) - x;
            };

            return function;
        }
    }
}
