﻿using System.Collections.Generic;
using SpiceNetlist.SpiceObjects;
using SpiceNetlist.SpiceSharpConnector.Processors.Common;
using SpiceSharp;
using SpiceSharp.Circuits;

namespace SpiceNetlist.SpiceSharpConnector.Processors
{
    public abstract class EntityGenerator : IGenerator
    {
        public string TypeName => string.Join(".", GetGeneratedTypes());

        public abstract Entity Generate(Identifier id, string originalName, string type, ParameterCollection parameters, ProcessingContext context);

        public abstract List<string> GetGeneratedTypes();
    }
}
