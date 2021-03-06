﻿using SpiceSharp.Components;
using SpiceSharpParser.ModelReaders.Netlist.Spice.Context;
using SpiceSharpParser.ModelReaders.Netlist.Spice.Custom;
using SpiceSharpParser.Models.Netlist.Spice.Objects;

namespace SpiceSharpParser.ModelReaders.Netlist.Spice.Readers.EntityGenerators.Models
{
    public class SwitchModelGenerator : ModelGenerator
    {
        public override SpiceSharp.Components.Model Generate(string id, string type, ParameterCollection parameters, ICircuitContext context)
        {
            SpiceSharp.Components.Model model = null;

            switch (type.ToLower())
            {
                case "sw": model = new VoltageSwitchModel(id); break;
                case "csw": model = new CurrentSwitchModel(id); break;
                case "vswitch": model = new VSwitchModel(id); break;
                case "iswitch": model = new ISwitchModel(id); break;
            }

            if (model != null)
            {
                SetParameters(context, model, parameters);
            }

            return model;
        }
    }
}