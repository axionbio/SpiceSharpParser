using System;
using Xunit;

namespace SpiceSharpParser.IntegrationTests.DotStatements
{
    public class DCTests : BaseTests
    {
        [Fact]
        public void DCCurrentSweep()
        {
            var netlist = ParseNetlist(
                "DC Sweep - Current",
                "I1 0 in 0",
                "R1 in 0 10",
                ".DC I1 -10 10 1e-3",
                ".SAVE V(in)",
                ".END");

            var exports = RunDCSimulation(netlist, "V(in)");

            // Get references
            Func<double, double> reference = sweep => sweep * 10.0;
            EqualsWithTol(exports, reference);
        }

        [Fact]
        public void DCVoltageSweep()
        {
            var netlist = ParseNetlist(
                "DC Sweep - Voltage",
                "V1 in 0 0",
                "R1 in 0 10",
                ".DC V1 -10 10 1e-3",
                ".SAVE V(in)",
                ".END");

            var exports = RunDCSimulation(netlist, "V(in)");

            // Get references
            Func<double, double> reference = sweep => sweep;
            EqualsWithTol(exports, reference);
        }
    }
}