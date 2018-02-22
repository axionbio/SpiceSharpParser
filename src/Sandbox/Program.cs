﻿namespace Sandbox
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using SpiceLexer;
    using SpiceNetlist.SpiceSharpConnector;
    using SpiceParser;
    using SpiceSharp.Parser.Readers;
    using SpiceSharp.Simulations;

    public class Program
    {
        public static void Main(string[] args)
        {
            StringBuilder st = new StringBuilder();
            st.Append(@"mosamp2 - mos amplifier - transient
.options acct abstol=10n  vntol=10n
.tran 0.1us 10us
m1  15 15  1 32 m w=88.9u  l=25.4u
m2   1  1  2 32 m w=12.7u  l=266.7u
m3   2  2 30 32 m w=88.9u  l=25.4u
m4  15  5  4 32 m w=12.7u  l=106.7u
m5   4  4 30 32 m w=88.9u  l=12.7u
m6  15 15  5 32 m w=44.5u  l=25.4u
m7   5 20  8 32 m w=482.6u l=12.7u
m8   8  2 30 32 m w=88.9u  l=25.4u
m9  15 15  6 32 m w=44.5u  l=25.4u
m10  6 21  8 32 m w=482.6u l=12.7u
m11 15  6  7 32 m w=12.7u  l=106.7u
m12  7  4 30 32 m w=88.9u  l=12.7u
m13 15 10  9 32 m w=139.7u l=12.7u
m14  9 11 30 32 m w=139.7u l=12.7u
m15 15 15 12 32 m w=12.7u  l=207.8u
m16 12 12 11 32 m w=54.1u  l=12.7u
m17 11 11 30 32 m w=54.1u  l=12.7u
m18 15 15 10 32 m w=12.7u  l=45.2u
m19 10 12 13 32 m w=270.5u l=12.7u
m20 13  7 30 32 m w=270.5u l=12.7u
m21 15 10 14 32 m w=254u   l=12.7u
m22 14 11 30 32 m w=241.3u l=12.7u
m23 15 20 16 32 m w=19u    l=38.1u
m24 16 14 30 32 m w=406.4u l=12.7u
m25 15 15 20 32 m w=38.1u  l=42.7u
m26 20 16 30 32 m w=381u   l=25.4u
m27 20 15 66 32 m w=22.9u  l=7.6u
cc 7 9 40pf
cl 66 0 70pf
vin 21 0 pulse(0 5 1ns 1ns 1ns 5us 10us)
vccp 15 0 dc +15
vddn 30 0 dc -15
vb 32 0 dc -20
.model m nmos(nsub=2.2e15 uo=575 ucrit=49k uexp=0.1 tox=0.11u xj=2.95u
+   level=2 cgso=1.5n cgdo=1.5n cbd=4.5f cbs=4.5f ld=2.4485u nss=3.2e10
+   kp=2e-5 phi=0.6 )
.print tran v(20) v(66)
.plot  tran v(20) v(66)
.save v(20)
.end 


");
            var tokensStr = st.ToString();

            var s0 = new Stopwatch();
            s0.Start();
            SpiceLexer lexer = new SpiceLexer(new SpiceLexerOptions() { HasTitle = true });
            var tokensEnumerable = lexer.GetTokens(tokensStr);
            var tokens = tokensEnumerable.ToArray();
            Console.WriteLine("Lexer: " + s0.ElapsedMilliseconds + "ms");

            var s1 = new Stopwatch();
            s1.Start();
            var parseTree = new SpiceParser().GetParseTree(tokens);
            Console.WriteLine("Parse tree generated: " + s1.ElapsedMilliseconds + "ms");

            var s2 = new Stopwatch();
            s2.Start();
            var eval = new ParseTreeEvaluator();
            var netlist = eval.Evaluate(parseTree) as SpiceNetlist.Netlist;
            Console.WriteLine("Translating to Netlist Object Model:" + s2.ElapsedMilliseconds + "ms");

            var s3 = new Stopwatch();
            s3.Start();
            var connector = new Connector();
            var n = connector.Translate(netlist);
            Console.WriteLine("Translating  Netlist Object Model to SpiceSharp: " + s3.ElapsedMilliseconds + "ms");

            Console.WriteLine("Warning: " + n.Warnings.Count);

            Console.WriteLine("Done");

            var export = n.Exports[0];

            n.Simulations[0].OnExportSimulationData += (object sender, ExportDataEventArgs e) => {
                Console.WriteLine(export.Extract());
            };
            n.Simulations[0].Run(n.Circuit);
        }
    }
}
