﻿using System;
using SpiceSharp.Simulations;
using SpiceSharpParser.Model.Netlist.Spice.Objects;
using SpiceSharpParser.ModelReader.Netlist.Spice.Context;
using SpiceSharpParser.ModelReader.Netlist.Spice.Exceptions;

namespace SpiceSharpParser.ModelReader.Netlist.Spice.Processors.Controls.Simulations
{
    /// <summary>
    /// Processes .AC <see cref="Control"/> from spice netlist object model.
    /// </summary>
    public class ACControl : SimulationControl
    {
        public override string TypeName => "ac";

        /// <summary>
        /// Processes <see cref="Control"/> statement and modifies the context
        /// </summary>
        /// <param name="statement">A statement to process</param>
        /// <param name="context">A context to modify</param>
        public override void Process(Control statement, IProcessingContext context)
        {
            CreateSimulations(statement, context, CreateACSimulation);
        }

        private AC CreateACSimulation(string name, Control statement, IProcessingContext context)
        {
            switch (statement.Parameters.Count)
            {
                case 0: throw new Exception("LIN, DEC or OCT expected");
                case 1: throw new Exception("Number of points expected");
                case 2: throw new Exception("Starting frequency expected");
                case 3: throw new Exception("Stopping frequency expected");
            }

            AC ac;

            string type = statement.Parameters.GetString(0).ToLower();
            var numberSteps = context.ParseDouble(statement.Parameters.GetString(1));
            var start = context.ParseDouble(statement.Parameters.GetString(2));
            var stop = context.ParseDouble(statement.Parameters.GetString(3));

            switch (type)
            {
                case "lin": ac = new AC(name, new LinearSweep(start, stop, (int)numberSteps)); break;
                case "oct": ac = new AC(name, new OctaveSweep(start, stop, (int)numberSteps)); break;
                case "dec": ac = new AC(name, new DecadeSweep(start, stop, (int)numberSteps)); break;
                default:
                    throw new WrongParameterException("LIN, DEC or OCT expected");
            }

            SetBaseConfiguration(ac.BaseConfiguration, context);
            SetACParameters(ac.FrequencyConfiguration, context);
            context.Result.AddSimulation(ac);

            return ac;
        }

        private void SetACParameters(FrequencyConfiguration frequencyConfiguration, IProcessingContext context)
        {
            if (context.Result.SimulationConfiguration.KeepOpInfo.HasValue)
            {
                frequencyConfiguration.KeepOpInfo = context.Result.SimulationConfiguration.KeepOpInfo.Value;
            }
        }
    }
}