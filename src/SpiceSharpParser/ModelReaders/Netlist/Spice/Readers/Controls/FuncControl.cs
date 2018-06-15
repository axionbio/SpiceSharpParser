﻿using System.Linq;
using SpiceSharpParser.Models.Netlist.Spice.Objects;
using SpiceSharpParser.ModelsReaders.Netlist.Spice.Context;
using SpiceSharpParser.ModelsReaders.Netlist.Spice.Exceptions;

namespace SpiceSharpParser.ModelsReaders.Netlist.Spice.Readers.Controls
{
    /// <summary>
    /// Reades .FUNC <see cref="Control"/> from spice netlist object model.
    /// </summary>
    public class FuncControl : BaseControl
    {
        public override string SpiceCommandName => "func";

        /// <summary>
        /// Reades <see cref="Control"/> statement and modifies the context
        /// </summary>
        /// <param name="statement">A statement to process</param>
        /// <param name="context">A context to modify</param>
        public override void Read(Control statement, IReadingContext context)
        {
            if (statement.Parameters == null)
            {
                throw new System.ArgumentNullException(nameof(statement.Parameters));
            }

            for (var i = 0; i < statement.Parameters.Count; i++)
            {
                var param = statement.Parameters[i];

                if (param is Models.Netlist.Spice.Objects.Parameters.AssignmentParameter assigmentParameter)
                {
                    if (!assigmentParameter.HasFunctionSyntax)
                    {
                        throw new System.Exception("User function needs to be a function");
                    }

                    context.Evaluator.AddCustomFunction(assigmentParameter.Name, assigmentParameter.Arguments, assigmentParameter.Value);
                    break;
                }
                else
                {
                    if (param is Models.Netlist.Spice.Objects.Parameters.BracketParameter bracketParameter)
                    {
                        context.Evaluator.AddCustomFunction(
                            bracketParameter.Name,
                            bracketParameter.Parameters.ToList().Select(p => p.Image).ToList(), // TODO: improve it please
                            statement.Parameters[i + 1].Image);
                        break;
                    }
                    else
                    {
                        throw new WrongParameterTypeException("Unsupported syntax for .FUNC");
                    }
                }
            }
        }
    }
}