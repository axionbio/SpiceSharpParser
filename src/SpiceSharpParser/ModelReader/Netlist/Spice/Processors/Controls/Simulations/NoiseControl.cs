﻿using SpiceSharp;
using SpiceSharp.Simulations;
using SpiceSharpParser.Model.Netlist.Spice.Objects;
using SpiceSharpParser.Model.Netlist.Spice.Objects.Parameters;
using SpiceSharpParser.ModelReader.Netlist.Spice.Context;
using SpiceSharpParser.ModelReader.Netlist.Spice.Exceptions;

namespace SpiceSharpParser.ModelReader.Netlist.Spice.Processors.Controls.Simulations
{
    /// <summary>
    /// Processes .NOISE <see cref="Control"/> from spice netlist object model.
    /// </summary>
    public class NoiseControl : SimulationControl
    {
        public override string TypeName => "noise";

        /// <summary>
        /// Processes <see cref="Control"/> statement and modifies the context
        /// </summary>
        /// <param name="statement">A statement to process</param>
        /// <param name="context">A context to modify</param>
        public override void Process(Control statement, IProcessingContext context)
        {
            CreateSimulations(statement, context, CreateNoiseSimulation);
        }

        private Noise CreateNoiseSimulation(string name, Control statement, IProcessingContext context)
        {
            Noise noise = null;

            // Check parameter count
            switch (statement.Parameters.Count)
            {
                case 0: throw new WrongParametersCountException("Output expected for .NOISE");
                case 1: throw new WrongParametersCountException("Source expected");
                case 2: throw new WrongParametersCountException("Step type expected");
                case 3: throw new WrongParametersCountException("Number of points expected");
                case 4: throw new WrongParametersCountException("Starting frequency expected");
                case 5: throw new WrongParametersCountException("Stopping frequency expected");
                case 6: break;
                case 7: break;
                default:
                    throw new WrongParametersCountException("Too many parameters");
            }

            string type = statement.Parameters.GetString(2);
            var numberSteps = context.ParseDouble(statement.Parameters.GetString(3));
            var start = context.ParseDouble(statement.Parameters.GetString(4));
            var stop = context.ParseDouble(statement.Parameters.GetString(5));

            Sweep<double> sweep;

            switch (type)
            {
                case "lin": sweep = new LinearSweep(start, stop, (int)numberSteps); break;
                case "oct": sweep = new OctaveSweep(start, stop, (int)numberSteps); break;
                case "dec": sweep = new DecadeSweep(start, stop, (int)numberSteps); break;
                default:
                    throw new WrongParameterException("LIN, DEC or OCT expected");
            }

            // The first parameters needs to specify the output voltage
            if (statement.Parameters[0] is BracketParameter bracket)
            {
                if (bracket.Name.ToLower() == "v")
                {
                    switch (bracket.Parameters.Count)
                    {
                        // V(A, B) - V(vector)
                        // V(A) - V(singleParameter)
                        case 1:
                            if (bracket.Parameters[0] is VectorParameter v && v.Elements.Count == 2)
                            {
                                var output = new StringIdentifier(v.Elements[0].Image);
                                var reference = new StringIdentifier(v.Elements[1].Image);
                                var input = new StringIdentifier(statement.Parameters[2].Image);
                                noise = new Noise(name, output, reference, input, sweep);
                            }
                            else if (bracket.Parameters[0] is SingleParameter s)
                            {
                                var output = new StringIdentifier(s.Image);
                                var input = new StringIdentifier(statement.Parameters[1].Image);
                                noise = new Noise(name, output, input, sweep);
                            }

                            break;
                        default:
                            throw new WrongParameterException("1 or 2 nodes expected");
                    }
                }
                else
                {
                    throw new WrongParameterException("Invalid output");
                }
            }
            else
            {
                throw new WrongParameterException("Invalid output");
            }

            context.Result.AddSimulation(noise);

            return noise;
        }
    }
}