using SpiceSharpParser.ModelReaders.Netlist.Spice.Readers.Controls.Prints;
using Xunit;

namespace SpiceSharpParser.IntegrationTests.DotStatements
{
    public class PrintTests : BaseTests
    {
        [Fact]
        public void When_InvalidExportForSimulationWithoutFilter_Expect_Reference()
        {
            var parseResult = ParseNetlist(
                "PRINT - Lowpass RC circuit - The capacitor should act like an open circuit",
                "V1 IN 0 10.0",
                "R1 IN OUT 10e3",
                "C1 OUT 0 10e-6",
                ".OP",
                ".PRINT V(OUT) I(C1)",
                ".END");
            RunSimulations(parseResult);

            Assert.Single(parseResult.Prints);
            Assert.Equal("#1 OP", parseResult.Prints[0].Name);
            Assert.Single(parseResult.Prints[0].ColumnNames);
            Assert.Single(parseResult.Prints[0].Rows[0].Columns);
            Assert.Single(parseResult.Prints[0].Rows);
        }

        [Fact]
        public void When_InvalidExportForSimulationWithFilter_Expect_Reference()
        {
            var parseResult = ParseNetlist(
                "PRINT - Lowpass RC circuit - The capacitor should act like an open circuit",
                "V1 IN 0 10.0",
                "R1 IN OUT 10e3",
                "C1 OUT 0 10e-6",
                ".OP",
                ".PRINT OP V(OUT) I(C1)",
                ".END");
            RunSimulations(parseResult);

            Assert.Single(parseResult.Prints);
            Assert.Equal("#1 OP", parseResult.Prints[0].Name);
            Assert.Equal(2, parseResult.Prints[0].ColumnNames.Count);
            Assert.Equal(2, parseResult.Prints[0].Rows[0].Columns.Count);
            Assert.Single(parseResult.Prints[0].Rows);
        }

        [Fact]
        public void When_WriteToCSV_Expect_NoException()
        {
            var parseResult = ParseNetlist(
               "PRINT - The initial voltage on capacitor is 0V. The result should be an exponential converging to dcVoltage.",
                "C1 OUT 0 1e-6",
                "R1 IN OUT 10e3",
                "V1 IN 0 10",
                ".IC V(OUT)=0.0",
                ".TRAN 1e-8 10e-6",
                ".PRINT TRAN",
                ".END");

            RunSimulations(parseResult);
            Assert.Single(parseResult.Prints);
            parseResult.Prints[0].ToCsv("data2.csv", ";", addCsvHeader: true);
        }

        [Fact]
        public void When_PrintTran_Expect_Reference()
        {
            var parseResult = ParseNetlist(
               "PRINT - The initial voltage on capacitor is 0V. The result should be an exponential converging to dcVoltage.",
                "C1 OUT 0 1e-6",
                "R1 IN OUT 10e3",
                "V1 IN 0 10",
                ".IC V(OUT)=0.0",
                ".TRAN 1e-8 10e-6",
                ".PRINT TRAN",
                ".END");

            RunSimulations(parseResult);
            Assert.Single(parseResult.Prints);
            Assert.Equal("#1 TRAN", parseResult.Prints[0].Name);
            Assert.Equal(7, parseResult.Prints[0].ColumnNames.Count);
            Assert.Equal(62, parseResult.Prints[0].Rows.Count);
        }

        [Fact]
        public void When_PrintOpWithoutParametersWithFilter_Expect_Reference()
        {
            var parseResult = ParseNetlist(
                "PRINT - Lowpass RC circuit - The capacitor should act like an open circuit",
                "V1 IN 0 10.0",
                "R1 IN OUT 10e3",
                "C1 OUT 0 10e-6",
                ".OP",
                ".PRINT OP",
                ".END");

            RunSimulations(parseResult);
            Assert.Single(parseResult.Prints);
            Assert.Equal("#1 OP", parseResult.Prints[0].Name);
            Assert.Equal(6, parseResult.Prints[0].ColumnNames.Count);
            Assert.Single(parseResult.Prints[0].Rows);
        }

        [Fact]
        public void When_PrintOpWithoutFilterWithoutParameters_Expect_Reference()
        {
            var parseResult = ParseNetlist(
                "PRINT - Lowpass RC circuit - The capacitor should act like an open circuit",
                "V1 IN 0 10.0",
                "R1 IN OUT 10e3",
                "C1 OUT 0 10e-6",
                ".OP",
                ".PRINT",
                ".END");

            RunSimulations(parseResult);
            Assert.Single(parseResult.Prints);
            Assert.Equal("#1 OP", parseResult.Prints[0].Name);
            Assert.Equal(5, parseResult.Prints[0].ColumnNames.Count);
            Assert.Single(parseResult.Prints[0].Rows);
        }

        [Fact]
        public void When_PrintOpWithoutFilter_Expect_Reference()
        {
            var parseResult = ParseNetlist(
                "PRINT - Lowpass RC circuit - The capacitor should act like an open circuit",
                "V1 IN 0 10.0",
                "R1 IN OUT 10e3",
                "C1 OUT 0 10e-6",
                ".OP",
                ".PRINT V(OUT) I(V1)",
                ".END");
            RunSimulations(parseResult);

            Assert.Single(parseResult.Prints);
            Assert.Equal("#1 OP", parseResult.Prints[0].Name);
            Assert.Equal(2, parseResult.Prints[0].ColumnNames.Count);
            Assert.Equal("V(OUT)", parseResult.Prints[0].ColumnNames[0]);
            Assert.Equal("I(V1)", parseResult.Prints[0].ColumnNames[1]);
            Assert.Single(parseResult.Prints[0].Rows);
        }

        [Fact]
        public void When_PrintOpWithoutArgumentsWithoutFilter_Expect_Reference()
        {
            var parseResult = ParseNetlist(
                "PRINT - Lowpass RC circuit - The capacitor should act like an open circuit",
                "V1 IN 0 10.0",
                "R1 IN OUT 10e3",
                "C1 OUT 0 10e-6",
                ".OP",
                ".PRINT",
                ".END");
            RunSimulations(parseResult);

            Assert.Single( parseResult.Prints);
            Assert.Equal("#1 OP", parseResult.Prints[0].Name);
            Assert.Equal(5, parseResult.Prints[0].ColumnNames.Count);

            Assert.Equal("I(V1)", parseResult.Prints[0].ColumnNames[0]);
            Assert.Equal("I(R1)", parseResult.Prints[0].ColumnNames[1]);
            Assert.Equal("V(IN)", parseResult.Prints[0].ColumnNames[2]);
            Assert.Equal("V(0)", parseResult.Prints[0].ColumnNames[3]);
            Assert.Equal("V(OUT)", parseResult.Prints[0].ColumnNames[4]);

            Assert.Single(parseResult.Prints[0].Rows);
        }

        [Fact]
        public void When_PrintOPWithFilter_Expect_Reference()
        {
            var parseResult = ParseNetlist(
                "PRINT - Lowpass RC circuit - The capacitor should act like an open circuit",
                "V1 IN 0 10.0",
                "R1 IN OUT 10e3",
                "C1 OUT 0 10e-6",
                ".OP",
                ".PRINT OP V(OUT) I(V1)",
                ".END");
            RunSimulations(parseResult);

            Assert.Single(parseResult.Prints);
            Assert.Equal("#1 OP", parseResult.Prints[0].Name);
            Assert.Equal(2, parseResult.Prints[0].ColumnNames.Count);
            Assert.Single(parseResult.Prints[0].Rows);
        }

        [Fact]
        public void When_PrintDcWithoutFilter_Expect_Reference()
        {
            var parseResult = ParseNetlist(
              "PRINT - DC Sweep - Current",
              "I1 0 in 0",
              "R1 in 0 10",
              ".DC I1 -10 10 1e-3",
              ".PRINT V(in) I(R1)",
              ".END");
            RunSimulations(parseResult);

            Assert.Single(parseResult.Prints);
            Assert.Equal("#1 DC", parseResult.Prints[0].Name);
            Assert.Equal(3, parseResult.Prints[0].ColumnNames.Count);
            Assert.Equal(20001, parseResult.Prints[0].Rows.Count);
        }

        [Fact]
        public void When_PrintDcWithFilter_Expect_Reference()
        {
            var parseResult = ParseNetlist(
              "PRINT - DC Sweep - Current",
              "I1 0 in 0",
              "R1 in 0 10",
              ".DC I1 -10 10 1e-3",
              ".PRINT DC V(in) I(R1)",
              ".END");
            RunSimulations(parseResult);

            Assert.Single(parseResult.Prints);
            Assert.Equal("#1 DC", parseResult.Prints[0].Name);
            Assert.Equal(3, parseResult.Prints[0].ColumnNames.Count);
            Assert.Equal(20001, parseResult.Prints[0].Rows.Count);
        }

        [Fact]
        public void When_LetIsUsedInPrint_Expect_Reference()
        {
            var parseResult = ParseNetlist(
              "PRINT - DC Sweep - Current",
              "I1 0 in 0",
              "R1 in 0 10",
              ".DC I1 -10 10 1e-3",
              ".PRINT DC V(in) I(R1) V_in_db",
              ".LET V_in_db {log10(V(in))*2}",
              ".END");
            RunSimulations(parseResult);

            Assert.Single(parseResult.Prints);
            Assert.Equal("#1 DC", parseResult.Prints[0].Name);
            Assert.Equal(4, parseResult.Prints[0].ColumnNames.Count);
            Assert.Equal(20001, parseResult.Prints[0].Rows.Count);
        }
    }
}