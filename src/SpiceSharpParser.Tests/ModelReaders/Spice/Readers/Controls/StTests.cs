﻿using NSubstitute;
using SpiceSharpParser.ModelReaders.Netlist.Spice.Context;
using SpiceSharpParser.Models.Netlist.Spice.Objects;
using SpiceSharpParser.Models.Netlist.Spice.Objects.Parameters;
using Xunit;
using SpiceSharpParser.ModelReaders.Netlist.Spice.Readers.Controls;
using System.Linq;
using SpiceSharp.Simulations;
using SpiceSharpParser.ModelReaders.Netlist.Spice;
using SpiceSharpParser.Common.Evaluation;

namespace SpiceSharpParser.Tests.ModelReaders.Spice.Readers.Controls.Simulations
{
    public class StTests
    {
        [Fact]
        public void LinDefault()
        {
            // prepare
            var control = new Control()
            {
                Name = "st",
                Parameters = new ParameterCollection()
                {
                    new WordParameter("v1"),
                    new ValueParameter("1"),
                    new ValueParameter("5"),
                    new ValueParameter("1"),
                }
            };

            var evaluator = Substitute.For<IEvaluator>();
            evaluator.EvaluateDouble("1").Returns(1.0);
            evaluator.EvaluateDouble("5").Returns(5.0);

            var resultService = new ResultService(
                new SpiceNetlistReaderResult(new SpiceSharp.Circuit(), "title"));

            var readingContext = new ReadingContext(
                string.Empty,
                Substitute.For<ISimulationsParameters>(),
                new EvaluatorsContainer(evaluator, new FunctionFactory()),
                resultService,
                new MainCircuitNodeNameGenerator(new string[] { }, true),
                new ObjectNameGenerator(string.Empty),
                new ObjectNameGenerator(string.Empty),
                null,
                null,
                new SpiceNetlistCaseSensitivitySettings());

            // act
            var stControl = new StControl();
            stControl.Read(control, readingContext);

            // assert
            Assert.Single(resultService.SimulationConfiguration.ParameterSweeps);
            Assert.True(resultService.SimulationConfiguration.ParameterSweeps[0].Sweep is LinearSweep);
            Assert.Equal(4, ((LinearSweep)resultService.SimulationConfiguration.ParameterSweeps[0].Sweep).Points.Count());
        }

        [Fact]
        public void Lin()
        {
            // prepare
            var control = new Control()
            {
                Name = "st",
                Parameters = new ParameterCollection()
                {
                    new WordParameter("LIN"),
                    new WordParameter("v1"),
                    new ValueParameter("1"),
                    new ValueParameter("5"),
                    new ValueParameter("1"),
                }
            };

            var evaluator = Substitute.For<IEvaluator>();
            evaluator.EvaluateDouble("1").Returns(1.0);
            evaluator.EvaluateDouble("5").Returns(5.0);

            var resultService = new ResultService(
                new SpiceNetlistReaderResult(new SpiceSharp.Circuit(), "title"));

            var readingContext = new ReadingContext(
                string.Empty,
                Substitute.For<ISimulationsParameters>(),
                new EvaluatorsContainer(evaluator, new FunctionFactory()),
                resultService,
                new MainCircuitNodeNameGenerator(new string[] { }, true),
                new ObjectNameGenerator(string.Empty),
                new ObjectNameGenerator(string.Empty),
                null,
                null,
                new SpiceNetlistCaseSensitivitySettings());

            // act
            var stControl = new StControl();
            stControl.Read(control, readingContext);

            // assert
            Assert.Single(resultService.SimulationConfiguration.ParameterSweeps);
            Assert.True(resultService.SimulationConfiguration.ParameterSweeps[0].Sweep is LinearSweep);
            Assert.Equal(4, ((LinearSweep)resultService.SimulationConfiguration.ParameterSweeps[0].Sweep).Points.Count());
        }

        [Fact]
        public void Dec()
        {
            // prepare
            var control = new Control()
            {
                Name = "st",
                Parameters = new ParameterCollection()
                {
                    new WordParameter("DEC"),
                    new WordParameter("v1"),
                    new ValueParameter("1"),
                    new ValueParameter("16"),
                    new ValueParameter("1"),
                }
            };

            var evaluator = Substitute.For<IEvaluator>();
            evaluator.EvaluateDouble("1").Returns(1.0);
            evaluator.EvaluateDouble("16").Returns(16);

            var resultService = new ResultService(
                new SpiceNetlistReaderResult(new SpiceSharp.Circuit(), "title"));

            var readingContext = new ReadingContext(
                string.Empty,
                Substitute.For<ISimulationsParameters>(),
                new EvaluatorsContainer(evaluator, new FunctionFactory()),
                resultService,
                new MainCircuitNodeNameGenerator(new string[] { }, true),
                new ObjectNameGenerator(string.Empty),
                new ObjectNameGenerator(string.Empty),
                null,
                null,
                new SpiceNetlistCaseSensitivitySettings());

            // act
            var stControl = new StControl();
            stControl.Read(control, readingContext);

            // assert
            Assert.Single(resultService.SimulationConfiguration.ParameterSweeps);
            Assert.True(resultService.SimulationConfiguration.ParameterSweeps[0].Sweep is DecadeSweep);
            Assert.Equal(2, ((DecadeSweep)resultService.SimulationConfiguration.ParameterSweeps[0].Sweep).Points.Count());
        }

        [Fact]
        public void Oct()
        {
            // prepare
            var control = new Control()
            {
                Name = "st",
                Parameters = new ParameterCollection()
                {
                    new WordParameter("OCT"),
                    new WordParameter("v1"),
                    new ValueParameter("1"),
                    new ValueParameter("16"),
                    new ValueParameter("1"),
                }
            };

            var evaluator = Substitute.For<IEvaluator>();
            evaluator.EvaluateDouble("1").Returns(1.0);
            evaluator.EvaluateDouble("16").Returns(16);

            var resultService = new ResultService(
                new SpiceNetlistReaderResult(new SpiceSharp.Circuit(), "title"));

            var readingContext = new ReadingContext(
                string.Empty,
                Substitute.For<ISimulationsParameters>(),
                new EvaluatorsContainer(evaluator, new FunctionFactory()),
                resultService,
                new MainCircuitNodeNameGenerator(new string[] { }, true),
                new ObjectNameGenerator(string.Empty),
                new ObjectNameGenerator(string.Empty),
                null,
                null,
                new SpiceNetlistCaseSensitivitySettings());

            // act
            var stControl = new StControl();
            stControl.Read(control, readingContext);

            // assert
            Assert.Single(resultService.SimulationConfiguration.ParameterSweeps);
            Assert.True(resultService.SimulationConfiguration.ParameterSweeps[0].Sweep is OctaveSweep);
            Assert.Equal(5, ((OctaveSweep)resultService.SimulationConfiguration.ParameterSweeps[0].Sweep).Points.Count());
        }

        [Fact]
        public void List()
        {
            // prepare
            var control = new Control()
            {
                Name = "st",
                Parameters = new ParameterCollection()
                {
                    new WordParameter("LIST"),
                    new WordParameter("v1"),
                    new ValueParameter("1.0"),
                    new ValueParameter("1.0"),
                    new ValueParameter("1.0"),
                }
            };

            var evaluator = Substitute.For<IEvaluator>();
            evaluator.EvaluateDouble("1.0").Returns(1.0);

            var resultService = new ResultService(
                new SpiceNetlistReaderResult(new SpiceSharp.Circuit(), "title"));

            var readingContext = new ReadingContext(
                string.Empty,
                Substitute.For<ISimulationsParameters>(),
                new EvaluatorsContainer(evaluator, new FunctionFactory()),
                resultService,
                new MainCircuitNodeNameGenerator(new string[] { }, true),
                new ObjectNameGenerator(string.Empty),
                new ObjectNameGenerator(string.Empty),
                null,
                null,
                new SpiceNetlistCaseSensitivitySettings());

            // act
            var stControl = new StControl();
            stControl.Read(control, readingContext);

            // assert
            Assert.Single(resultService.SimulationConfiguration.ParameterSweeps);
            Assert.True(resultService.SimulationConfiguration.ParameterSweeps[0].Sweep is ListSweep);
            Assert.Equal(3, ((ListSweep)resultService.SimulationConfiguration.ParameterSweeps[0].Sweep).Points.Count());
        }
    }
}