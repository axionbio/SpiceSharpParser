﻿using System;
using SpiceNetlist.SpiceSharpConnector.Processors;

namespace SpiceNetlist.SpiceSharpConnector
{
    /// <summary>
    /// Registry for <see cref="EntityGenerator"/>s
    /// </summary>
    public class EntityGeneratorRegistry : BaseRegistry<EntityGenerator>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityGeneratorRegistry"/> class.
        /// </summary>
        public EntityGeneratorRegistry()
        {
        }

        /// <summary>
        /// Adds generator to the registry (all generated types)
        /// </summary>
        /// <param name="generator">
        /// A generator to add
        /// </param>
        public override void Add(EntityGenerator generator)
        {
            foreach (var type in generator.GetGeneratedTypes())
            {
                if (ElementsByType.ContainsKey(type))
                {
                    throw new Exception("Conflict in geneators");
                }

                ElementsByType[type] = generator;
            }

            Elements.Add(generator);
        }
    }
}
