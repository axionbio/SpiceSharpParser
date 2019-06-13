﻿using System.Collections.Generic;

namespace SpiceSharpParser.Common.Evaluation
{
    public class ExpressionParserContext
    {
        public ExpressionParserContext()
            : this(string.Empty, false)
        {
        }

        public ExpressionParserContext(string name, bool caseSensitiveFunctions)
        {
            Name = name;
            Functions = new Dictionary<string, IFunction>(StringComparerProvider.Get(caseSensitiveFunctions));
        }

        public ExpressionParserContext(string name, Dictionary<string, IFunction> functions)
        {
            Name = name;
            Functions = functions;
        }

        public ExpressionParserContext(Dictionary<string, IFunction> functions) : this(string.Empty, functions)
        {
        }

        /// <summary>
        /// Gets or sets custom functions.
        /// </summary>
        public Dictionary<string, IFunction> Functions { get; protected set; }

        /// <summary>
        /// Gets or sets the name of parser context.
        /// </summary>
        public string Name { get; protected set; }
    }
}
