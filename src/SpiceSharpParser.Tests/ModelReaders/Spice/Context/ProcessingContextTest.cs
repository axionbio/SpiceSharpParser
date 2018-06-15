﻿using NSubstitute;
using SpiceSharpParser.ModelsReaders.Netlist.Spice.Context;
using SpiceSharpParser.ModelsReaders.Netlist.Spice.Evaluation;
using SpiceSharp.Components;
using System.Collections.Generic;
using Xunit;
using SpiceSharp.Simulations;
using SpiceSharpParser.Common;

namespace SpiceSharpParser.Tests.ModelReaders.Spice.Context
{
    public class ReadingContextTest
    {
        [Fact]
        public void SetParameterWithExpressionTest()
        {
            // prepare
            var evaluator = Substitute.For<ISpiceEvaluator>();
            evaluator.EvaluateDouble("a+1").Returns(
                x =>
                {
                    return 1.1;
                });

            var resultService = Substitute.For<IResultService>();
            var context = new ReadingContext(string.Empty, evaluator, resultService, new MainCircuitNodeNameGenerator(new string[] { }), new ObjectNameGenerator(string.Empty));

            // act
            var resistor = new Resistor("R1");
            context.SetEntityParameter(resistor, "resistance", "a+1");

            // assert 
            Assert.Equal(1.1, resistor.ParameterSets.GetParameter("resistance").Value);
        }

        [Fact]
        public void SetParameterCaseTest()
        {
            // prepare
            var evaluator = Substitute.For<ISpiceEvaluator>();
            evaluator.EvaluateDouble("1").Returns(1);

            var resultService = Substitute.For<IResultService>();
            var context = new ReadingContext(string.Empty,
                evaluator,
                resultService,
                new MainCircuitNodeNameGenerator(new string[] { }),
                new ObjectNameGenerator(string.Empty));

            // act
            var resistor = new Resistor("R1");
            context.SetEntityParameter(resistor, "L", "1");

            // assert
            Assert.Equal(1, resistor.ParameterSets.GetParameter("l").Value);
        }

        [Fact]
        public void SetUnkownParameterTest()
        {
            // prepare
            var evaluator = Substitute.For<ISpiceEvaluator>();
            evaluator.EvaluateDouble("1").Returns(1);

            var resultService = Substitute.For<IResultService>();
            var context = new ReadingContext(string.Empty,
                evaluator,
                resultService,
                new MainCircuitNodeNameGenerator(new string[] { }),
                new ObjectNameGenerator(string.Empty));

            // act
            var resistor = new Resistor("R1");
            Assert.False(context.SetEntityParameter(resistor, "uknown", "1"));
        }

        [Fact]
        public void SetNodeSetVoltageTest()
        {
            // prepare
            var evaluator = Substitute.For<ISpiceEvaluator>();
            evaluator.EvaluateDouble("x+1").Returns(3);

            var simulations = new List<Simulation>();
            var simulation = new DC("DC");
            simulations.Add(simulation);

            var resultService = Substitute.For<IResultService>();
            resultService.SimulationConfiguration.Returns(new SimulationConfiguration());
            resultService.Simulations.Returns(simulations);

            var context = new ReadingContext(
                string.Empty,
                evaluator,
                resultService,
                new MainCircuitNodeNameGenerator(new string[] { }),
                new ObjectNameGenerator(string.Empty));

            // act
            context.SetNodeSetVoltage("node1", "x+1");

            // assert
            Assert.Equal(3, simulation.Nodes.NodeSets["node1"]);
        }
    }
}