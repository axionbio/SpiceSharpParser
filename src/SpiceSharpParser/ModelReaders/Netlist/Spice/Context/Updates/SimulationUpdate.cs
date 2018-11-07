﻿using SpiceSharp.Simulations;
using SpiceSharpParser.Common.Evaluation;
using System;

namespace SpiceSharpParser.ModelReaders.Netlist.Spice.Context
{
    public class SimulationUpdate
    {
        public Simulation Simulation { get; set; }

        public Action<BaseSimulation, ISimulationEvaluators, SimulationExpressionContexts> Update { get; set; }
    }
}
