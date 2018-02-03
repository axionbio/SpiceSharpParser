﻿using System;
using SpiceNetlist.Connectors.SpiceSharpConnector.Expressions;
using SpiceNetlist.SpiceObjects;
using SpiceSharp.IntegrationMethods;

namespace SpiceNetlist.Connectors.SpiceSharpConnector.Processors.Controls
{
    class OptionControl : SingleControlProcessor
    {
        public override void Process(Control statement, NetList netlist)
        {
            foreach (var param in statement.Parameters.Values)
            {
                if (param is SpiceNetlist.SpiceObjects.Parameters.AssignmentParameter a)
                {
                    string name = a.Name.ToLower();
                    string value = a.Value;

                    switch (name)
                    {
                        case "abstol":
                            if (netlist.BaseConfiguration == null) netlist.BaseConfiguration = new SpiceSharp.Simulations.BaseConfiguration();
                            netlist.BaseConfiguration.AbsoluteTolerance = spiceExpressionParser.Parse(value); break;
                        case "reltol":
                            if (netlist.BaseConfiguration == null) netlist.BaseConfiguration = new SpiceSharp.Simulations.BaseConfiguration();
                            netlist.BaseConfiguration.AbsoluteTolerance = spiceExpressionParser.Parse(value); break;
                        case "gmin":
                            if (netlist.BaseConfiguration == null) netlist.BaseConfiguration = new SpiceSharp.Simulations.BaseConfiguration();
                            netlist.BaseConfiguration.Gmin = spiceExpressionParser.Parse(value); break;
                        case "itl1":
                            if (netlist.BaseConfiguration == null) netlist.BaseConfiguration = new SpiceSharp.Simulations.BaseConfiguration();
                            netlist.BaseConfiguration.DCMaxIterations = (int)spiceExpressionParser.Parse(value); break;
                        case "itl2":
                            if (netlist.DCConfiguration == null) netlist.DCConfiguration = new SpiceSharp.Simulations.DCConfiguration();
                            netlist.DCConfiguration.SweepMaxIterations = (int)spiceExpressionParser.Parse(value); break;

                        case "itl4":
                            if (netlist.TimeConfiguration == null) netlist.TimeConfiguration = new SpiceSharp.Simulations.TimeConfiguration();
                            netlist.TimeConfiguration.TranMaxIterations = (int)spiceExpressionParser.Parse(value); break;

                        case "temp":
                            // TODO: Set current temperature
                            break;

                        case "tnom":
                            // TODO: Set nominal temperature
                            break;

                        case "method":
                            switch (value.ToLower())
                            {
                                case "trap":
                                case "trapezoidal":
                                    if (netlist.TimeConfiguration == null) netlist.TimeConfiguration = new SpiceSharp.Simulations.TimeConfiguration();
                                    netlist.TimeConfiguration.Method = new Trapezoidal();
                                    break;
                            }
                            break;

                        default:
                            throw new Exception();
                    }
                }

                if (param is SpiceObjects.Parameters.WordParameter w)
                {
                    if (w.RawValue.ToLower() == "keepopinfo")
                    {
                        if (netlist.FrequencyConfiguration == null) netlist.FrequencyConfiguration = new SpiceSharp.Simulations.FrequencyConfiguration();
                        netlist.FrequencyConfiguration.KeepOpInfo = true;
                    }
                }
            }
        }
    }
}