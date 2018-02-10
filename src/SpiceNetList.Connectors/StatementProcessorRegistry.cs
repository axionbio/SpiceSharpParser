﻿using SpiceNetlist.SpiceObjects;
using SpiceNetlist.SpiceSharpConnector.Processors;
using System;
using System.Collections.Generic;

namespace SpiceNetlist.SpiceSharpConnector
{
    public class StatementProcessorRegistry
    {
        Dictionary<Type, StatementProcessor> Processors = new Dictionary<Type, StatementProcessor>();

        public StatementProcessorRegistry()
        {
            Processors[typeof(Component)] = new ComponentProcessor();
            Processors[typeof(Model)] = new ModelProcessor();
            Processors[typeof(Control)] = new ControlProcessor();
            Processors[typeof(SubCircuit)] = new SubcircuitProcessor();
        }

        public void Init()
        {
            foreach (var processor in Processors.Values)
            {
                processor.Init();
            }
        }

        internal StatementProcessor GetProcessor(Type statementType)
        {
            return Processors[statementType];
        }

        internal bool Supports(Type statementType)
        {
            return Processors.ContainsKey(statementType);
        }
    }
}
