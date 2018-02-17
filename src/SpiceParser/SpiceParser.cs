﻿using System.Collections.Generic;
using NLexer;
using SpiceGrammar;
using SpiceLexer;

namespace SpiceParser
{
    public class SpiceParser
    {
        /// <summary>
        /// Generates a parse tree for SPICE grammar
        /// </summary>
        /// <param name="tokens">An array of tokens</param>
        /// <param name="rootSymbol">A root symbol of parse tree</param>
        /// <returns>
        /// A parse tree
        /// </returns>
        public ParseTreeNonTerminalNode GetParseTree(SpiceToken[] tokens, string rootSymbol = SpiceGrammarSymbol.START)
        {
            if (tokens == null)
            {
                throw new System.ArgumentNullException(nameof(tokens));
            }

            var stack = new Stack<ParseTreeNode>();

            var root = CreateNonTerminalNode(rootSymbol, null);
            stack.Push(root);

            int currentTokenIndex = 0;

            while (stack.Count > 0)
            {
                var currentNode = stack.Pop();
                if (currentNode is ParseTreeNonTerminalNode ntn)
                {
                    switch (ntn.Name)
                    {
                        case SpiceGrammarSymbol.START:
                            ProcessStartNode(stack, ntn, tokens, currentTokenIndex);
                            break;
                        case SpiceGrammarSymbol.STATEMENTS:
                            ProcessStatements(stack, ntn, tokens, currentTokenIndex);
                            break;
                        case SpiceGrammarSymbol.STATEMENT:
                            ProcessStatement(stack, ntn, tokens, currentTokenIndex);
                            break;
                        case SpiceGrammarSymbol.COMMENT_LINE:
                            ProcessCommentLine(stack, ntn, tokens, currentTokenIndex);
                            break;
                        case SpiceGrammarSymbol.SUBCKT:
                            ProcessSubckt(stack, ntn, tokens, currentTokenIndex);
                            break;
                        case SpiceGrammarSymbol.SUBCKT_ENDING:
                            ProcessSubcktEnding(stack, ntn, tokens, currentTokenIndex);
                            break;
                        case SpiceGrammarSymbol.COMPONENT:
                            ProcessComponent(stack, ntn, tokens, currentTokenIndex);
                            break;
                        case SpiceGrammarSymbol.CONTROL:
                            ProcessControl(stack, ntn, tokens, currentTokenIndex);
                            break;
                        case SpiceGrammarSymbol.MODEL:
                            ProcessModel(stack, ntn, tokens, currentTokenIndex);
                            break;
                        case SpiceGrammarSymbol.PARAMETERS:
                            ProcessParameters(stack, ntn, tokens, currentTokenIndex);
                            break;
                        case SpiceGrammarSymbol.PARAMETER:
                            ProcessParameter(stack, ntn, tokens, currentTokenIndex);
                            break;
                        case SpiceGrammarSymbol.PARAMETER_SINGLE:
                            ProcessParameterSingle(stack, ntn, tokens, currentTokenIndex);
                            break;
                        case SpiceGrammarSymbol.PARAMETER_SINGLE_SEQUENCE:
                            ProcessParameterSingleSequence(stack, ntn, tokens, currentTokenIndex);
                            break;
                        case SpiceGrammarSymbol.PARAMETER_SINGLE_SEQUENCE_CONTINUE:
                            ProcessParameterSingleSequenceContinue(stack, ntn, tokens, currentTokenIndex);
                            break;
                        case SpiceGrammarSymbol.PARAMETER_BRACKET:
                            ProcessParameterBracket(stack, ntn, tokens, currentTokenIndex);
                            break;
                        case SpiceGrammarSymbol.PARAMETER_BRACKET_CONTENT:
                            ProcessParameterBracketContent(stack, ntn, tokens, currentTokenIndex);
                            break;
                        case SpiceGrammarSymbol.PARAMETER_EQUAL:
                            ProcessParameterEqual(stack, ntn, tokens, currentTokenIndex);
                            break;
                        case SpiceGrammarSymbol.PARAMETER_EQUAL_SINGLE:
                            ProcessParameterEqualSingle(stack, ntn, tokens, currentTokenIndex);
                            break;
                        case SpiceGrammarSymbol.PARAMETER_EQUAL_SEQUANCE:
                            ProcessParameterEqualSequence(stack, ntn, tokens, currentTokenIndex);
                            break;
                        case SpiceGrammarSymbol.PARAMETER_EQUAL_SEQUANCE_CONTINUE:
                            ProcessParameterEqualSequenceContinue(stack, ntn, tokens, currentTokenIndex);
                            break;
                        case SpiceGrammarSymbol.VECTOR:
                            ProcessVector(stack, ntn, tokens, currentTokenIndex);
                            break;
                        case SpiceGrammarSymbol.VECTOR_CONTINUE:
                            ProcessVectorContinue(stack, ntn, tokens, currentTokenIndex);
                            break;
                        case SpiceGrammarSymbol.NEW_LINE_OR_EOF:
                            ProcessNewLineOrEOF(stack, ntn, tokens, currentTokenIndex);
                            break;
                    }
                }

                if (currentNode is ParseTreeTerminalNode tn)
                {
                    if (tn.Token.SpiceTokenType == tokens[currentTokenIndex].SpiceTokenType
                        && (tn.Token.Lexem == null || tn.Token.Lexem == tokens[currentTokenIndex].Lexem))
                    {
                        tn.Token.UpdateLexem(tokens[currentTokenIndex].Lexem);
                        currentTokenIndex++;
                    }
                    else
                    {
                        throw new ParseException("Unexpected token: " + tokens[currentTokenIndex].Lexem + ", expected token type: " + tn.Token.SpiceTokenType + " line=" + tokens[currentTokenIndex].LineNumber, tokens[currentTokenIndex].LineNumber);
                    }
                }
            }

            return root;
        }

        private void ProcessSubcktEnding(Stack<ParseTreeNode> stack, ParseTreeNonTerminalNode current, SpiceToken[] tokens, int currentTokenIndex)
        {
            var currentToken = tokens[currentTokenIndex];
            var nextToken = tokens[currentTokenIndex + 1];

            if (currentToken.Is(SpiceTokenType.ENDS))
            {
                if (nextToken.Is(SpiceTokenType.WORD))
                {
                    stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.NEW_LINE_OR_EOF, current));
                    stack.Push(CreateTerminalNode(SpiceTokenType.WORD, current));
                    stack.Push(CreateTerminalNode(SpiceTokenType.ENDS, current));
                }
                else
                {
                    stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.NEW_LINE_OR_EOF, current));
                    stack.Push(CreateTerminalNode(SpiceTokenType.ENDS, current));
                }
            }
            else
            {
                throw new ParseException("Error during parsing subcircuit. Expected .ENDS. Unexpected token: '" + currentToken.Lexem + "'" + " line=" + currentToken.LineNumber, currentToken.LineNumber);
            }
        }

        private void ProcessNewLineOrEOF(Stack<ParseTreeNode> stack, ParseTreeNonTerminalNode currentNode, SpiceToken[] tokens, int currentTokenIndex)
        {
            var currentToken = tokens[currentTokenIndex];
            if (currentToken.Is(SpiceTokenType.EOF))
            {
                stack.Push(CreateTerminalNode(SpiceTokenType.EOF, currentNode));
            }

            if (currentToken.Is(SpiceTokenType.NEWLINE))
            {
                stack.Push(CreateTerminalNode(SpiceTokenType.NEWLINE, currentNode));
            }
        }

        private void ProcessStartNode(Stack<ParseTreeNode> stack, ParseTreeNonTerminalNode currentNode, SpiceToken[] tokens, int currentTokenIndex)
        {
            stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.STATEMENTS, currentNode));
            stack.Push(CreateTerminalNode(SpiceTokenType.TITLE, currentNode));
        }

        private void ProcessStatements(Stack<ParseTreeNode> stack, ParseTreeNonTerminalNode statementsNode, SpiceToken[] tokens, int currentTokenIndex)
        {
            var currentToken = tokens[currentTokenIndex];

            if (currentToken.Is(SpiceTokenType.DOT)
                || currentToken.Is(SpiceTokenType.WORD)
                || currentToken.Is(SpiceTokenType.ASTERIKS))
            {
                stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.STATEMENTS, statementsNode));
                stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.STATEMENT, statementsNode));
            }
            else if (currentToken.Is(SpiceTokenType.NEWLINE))
            {
                stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.STATEMENTS, statementsNode));
                stack.Push(CreateTerminalNode(SpiceTokenType.NEWLINE, statementsNode));
            }
            else if (currentToken.Is(SpiceTokenType.END))
            {
                stack.Push(CreateTerminalNode(SpiceTokenType.END, statementsNode));
            }
            else if (currentToken.Is(SpiceTokenType.EOF))
            {
                // follow - do nothing
            }
            else if (currentToken.Is(SpiceTokenType.ENDS))
            {
                // follow - do nothing
            }
            else
            {
                throw new ParseException("Error during parsing statements. Unexpected token: '" + currentToken.Lexem + "'" + " line=" + currentToken.LineNumber, currentToken.LineNumber);
            }
        }

        private void ProcessStatement(Stack<ParseTreeNode> stack, ParseTreeNonTerminalNode current, SpiceToken[] tokens, int currentTokenIndex)
        {
            var currentToken = tokens[currentTokenIndex];
            var nextToken = tokens[currentTokenIndex + 1];

            if (currentToken.Is(SpiceTokenType.WORD))
            {
                stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.COMPONENT, current));
            }
            else if (currentToken.Is(SpiceTokenType.DOT))
            {
                if (nextToken.Is(SpiceTokenType.WORD))
                {
                    if (nextToken.Equal("subckt", true))
                    {
                        stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.SUBCKT, current));
                    }
                    else if (nextToken.Equal("model", true))
                    {
                        stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.MODEL, current));
                    }
                    else
                    {
                        stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.CONTROL, current));
                    }
                }
                else
                {
                    throw new ParseException("Error during parsing a statement. Unexpected token: '" + currentToken.Lexem + "'" + " line=" + currentToken.LineNumber, currentToken.LineNumber);
                }
            }
            else if (currentToken.Is(SpiceTokenType.ASTERIKS))
            {
                stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.COMMENT_LINE, current));
            }
        }

        private void ProcessVector(Stack<ParseTreeNode> stack, ParseTreeNonTerminalNode current, SpiceToken[] tokens, int currentTokenIndex)
        {
            stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.VECTOR_CONTINUE, current));
            stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.PARAMETER_SINGLE, current));
            stack.Push(CreateTerminalNode(SpiceTokenType.COMMA, current, ","));
            stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.PARAMETER_SINGLE, current));
        }

        private void ProcessVectorContinue(Stack<ParseTreeNode> stack, ParseTreeNonTerminalNode current, SpiceToken[] tokens, int currentTokenIndex)
        {
            if (currentTokenIndex >= tokens.Length) return; // empty

            var currentToken = tokens[currentTokenIndex];

            if (currentToken.Is(SpiceTokenType.DELIMITER) && currentToken.Lexem == ")")
            {
                // follow
            }
            else
            {
                stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.VECTOR_CONTINUE, current));
                stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.PARAMETER_SINGLE, current));
                stack.Push(CreateTerminalNode(SpiceTokenType.COMMA, current, ","));
            }
        }

        private void ProcessCommentLine(Stack<ParseTreeNode> stack, ParseTreeNonTerminalNode current, SpiceToken[] tokens, int currentTokenIndex)
        {
            var currentToken = tokens[currentTokenIndex];
            var nextToken = tokens[currentTokenIndex + 1];

            if (currentToken.Is(SpiceTokenType.ASTERIKS)
                && (nextToken.Is(SpiceTokenType.COMMENT)
                || nextToken.Is(SpiceTokenType.NEWLINE)
                || nextToken.Is(SpiceTokenType.EOF)))
            {
                stack.Push(CreateTerminalNode(nextToken.SpiceTokenType, current, nextToken.Lexem));
                stack.Push(CreateTerminalNode(currentToken.SpiceTokenType, current, currentToken.Lexem));
            }
            else
            {
                throw new ParseException("Error during parsing a comment. Unexpected token: '" + currentToken.Lexem + "'" + " line=" + currentToken.LineNumber, currentToken.LineNumber);
            }
        }

        private void ProcessSubckt(Stack<ParseTreeNode> stack, ParseTreeNonTerminalNode current, SpiceToken[] tokens, int currentTokenIndex)
        {
            var currentToken = tokens[currentTokenIndex];
            var nextToken = tokens[currentTokenIndex + 1];

            if (currentToken.Is(SpiceTokenType.DOT)
                && nextToken.Is(SpiceTokenType.WORD)
                && nextToken.Equal("subckt", true))
            {
                stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.SUBCKT_ENDING, current));
                stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.STATEMENTS, current));
                stack.Push(CreateTerminalNode(SpiceTokenType.NEWLINE, current));
                stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.PARAMETERS, current));
                stack.Push(CreateTerminalNode(SpiceTokenType.WORD, current));
                stack.Push(CreateTerminalNode(nextToken.SpiceTokenType, current, nextToken.Lexem));
                stack.Push(CreateTerminalNode(currentToken.SpiceTokenType, current, currentToken.Lexem));
            }
            else
            {
                throw new ParseException("Error during parsing a subcircuit. Unexpected token: '" + currentToken.Lexem + "'" + " line=" + currentToken.LineNumber, currentToken.LineNumber);
            }
        }

        private void ProcessParameters(Stack<ParseTreeNode> stack, ParseTreeNonTerminalNode current, SpiceToken[] tokens, int currentTokenIndex)
        {
            var currentToken = tokens[currentTokenIndex];

            if (currentToken.Is(SpiceTokenType.WORD)
                || currentToken.Is(SpiceTokenType.VALUE)
                || currentToken.Is(SpiceTokenType.STRING)
                || currentToken.Is(SpiceTokenType.IDENTIFIER)
                || currentToken.Is(SpiceTokenType.REFERENCE)
                || currentToken.Is(SpiceTokenType.EXPRESSION))
            {
                stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.PARAMETERS, current));
                stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.PARAMETER, current));
            }
            else if (currentToken.Is(SpiceTokenType.EOF))
            {
                // follow - do nothing
            }
            else if (currentToken.Is(SpiceTokenType.NEWLINE))
            {
                // follow - do nothing
            }
            else if (currentToken.Is(SpiceTokenType.DELIMITER) && currentToken.Lexem == ")")
            {
                // follow - do nothing
            }
            else
            {
                throw new ParseException("Error during parsing parameters. Unexpected token: '" + currentToken.Lexem + "'" + " line=" + currentToken.LineNumber, currentToken.LineNumber);
            }
        }

        private void ProcessParameterEqual(Stack<ParseTreeNode> stack, ParseTreeNonTerminalNode current, SpiceToken[] tokens, int currentTokenIndex)
        {
            var currentToken = tokens[currentTokenIndex];
            var nextToken = tokens[currentTokenIndex + 1];

            if (currentToken.Is(SpiceTokenType.WORD))
            {
                if (nextToken.Is(SpiceTokenType.EQUAL))
                {
                    stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.PARAMETER_EQUAL_SINGLE, current));
                }
                else if (nextToken.Is(SpiceTokenType.DELIMITER) && nextToken.Equal("(", true))
                {
                    if ((tokens.Length > currentTokenIndex + 4) && tokens[currentTokenIndex + 3].Lexem == ")" && tokens[currentTokenIndex + 4].Lexem == "=")
                    {
                        stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.PARAMETER_SINGLE, current));
                        stack.Push(CreateTerminalNode(SpiceTokenType.EQUAL, current));
                        stack.Push(CreateTerminalNode(SpiceTokenType.DELIMITER, current, ")"));
                        stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.PARAMETER_SINGLE, current));
                        stack.Push(CreateTerminalNode(SpiceTokenType.DELIMITER, current, "("));
                        stack.Push(CreateTerminalNode(SpiceTokenType.WORD, current));
                    }

                    if ((tokens.Length > currentTokenIndex + 6) && tokens[currentTokenIndex + 5].Lexem == ")" && tokens[currentTokenIndex + 6].Lexem == "=")
                    {
                        stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.PARAMETER_SINGLE, current));
                        stack.Push(CreateTerminalNode(SpiceTokenType.EQUAL, current));
                        stack.Push(CreateTerminalNode(SpiceTokenType.DELIMITER, current, ")"));
                        stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.PARAMETER_SINGLE, current));
                        stack.Push(CreateTerminalNode(SpiceTokenType.COMMA, current, ","));
                        stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.PARAMETER_SINGLE, current));
                        stack.Push(CreateTerminalNode(SpiceTokenType.DELIMITER, current, "("));
                        stack.Push(CreateTerminalNode(SpiceTokenType.WORD, current));
                    }
                }
            }
        }

        private void ProcessParameterEqualSequence(Stack<ParseTreeNode> stack, ParseTreeNonTerminalNode current, SpiceToken[] tokens, int currentTokenIndex)
        {
            stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.PARAMETER_EQUAL_SEQUANCE_CONTINUE, current));
            stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.PARAMETER_EQUAL_SINGLE, current));
        }

        private void ProcessParameterEqualSequenceContinue(Stack<ParseTreeNode> stack, ParseTreeNonTerminalNode current, SpiceToken[] tokens, int currentTokenIndex)
        {
            var currentToken = tokens[currentTokenIndex];

            if (currentToken.Is(SpiceTokenType.DELIMITER) && currentToken.Lexem == ")")
            {
                // follow
            }
            else
            {
                stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.PARAMETER_EQUAL_SEQUANCE_CONTINUE, current));
                stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.PARAMETER_EQUAL_SINGLE, current));
            }
        }


        private void ProcessParameterSingleSequence(Stack<ParseTreeNode> stack, ParseTreeNonTerminalNode current, SpiceToken[] tokens, int currentTokenIndex)
        {
            stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.PARAMETER_SINGLE_SEQUENCE_CONTINUE, current));
            stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.PARAMETER_SINGLE, current));
        }

        private void ProcessParameterSingleSequenceContinue(Stack<ParseTreeNode> stack, ParseTreeNonTerminalNode current, SpiceToken[] tokens, int currentTokenIndex)
        {
            var currentToken = tokens[currentTokenIndex];

            if (currentToken.Is(SpiceTokenType.DELIMITER) && currentToken.Lexem == ")")
            {
                // follow
            }
            else
            {
                stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.PARAMETER_SINGLE_SEQUENCE_CONTINUE, current));
                stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.PARAMETER_SINGLE, current));
            }
        }


        private void ProcessParameterEqualSingle(Stack<ParseTreeNode> stack, ParseTreeNonTerminalNode current, SpiceToken[] tokens, int currentTokenIndex)
        {
            stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.PARAMETER_SINGLE, current));
            stack.Push(CreateTerminalNode(SpiceTokenType.EQUAL, current));
            stack.Push(CreateTerminalNode(SpiceTokenType.WORD, current));
        }

        private void ProcessParameterBracketContent(Stack<ParseTreeNode> stack, ParseTreeNonTerminalNode current, SpiceToken[] tokens, int currentTokenIndex)
        {
            stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.PARAMETERS, current));
        }

        private void ProcessParameter(Stack<ParseTreeNode> stack, ParseTreeNonTerminalNode current, SpiceToken[] tokens, int currentTokenIndex)
        {
            var currentToken = tokens[currentTokenIndex];
            var nextToken = tokens[currentTokenIndex + 1];

            if (nextToken.Is(SpiceTokenType.COMMA))
            {
                stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.VECTOR, current));
            }
            else
            {
                if (currentToken.Is(SpiceTokenType.WORD))
                {
                    if (nextToken.Is(SpiceTokenType.EQUAL))
                    {
                        stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.PARAMETER_EQUAL, current));
                    }
                    else if (nextToken.Is(SpiceTokenType.DELIMITER) && nextToken.Equal("(", true))
                    {
                        if (((tokens.Length > currentTokenIndex + 4) && tokens[currentTokenIndex + 3].Lexem == ")" && tokens[currentTokenIndex + 4].Lexem == "=")
                            || ((tokens.Length > currentTokenIndex + 6) && tokens[currentTokenIndex + 5].Lexem == ")" && tokens[currentTokenIndex + 6].Lexem == "="))
                        {
                            stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.PARAMETER_EQUAL, current));
                        }
                        else
                        {
                            stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.PARAMETER_BRACKET, current));
                        }
                    }
                    else
                    {
                        stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.PARAMETER_SINGLE, current));
                    }
                }
                else
                {
                    if (currentToken.Is(SpiceTokenType.VALUE)
                        || currentToken.Is(SpiceTokenType.STRING)
                        || currentToken.Is(SpiceTokenType.IDENTIFIER)
                        || currentToken.Is(SpiceTokenType.REFERENCE)
                        || currentToken.Is(SpiceTokenType.EXPRESSION))
                    {
                        stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.PARAMETER_SINGLE, current));
                    }
                    else
                    {
                        throw new ParseException("Error during parsing a paremeter. Unexpected token: '" + currentToken.Lexem + "'" + " line=" + currentToken.LineNumber, currentToken.LineNumber);
                    }
                }
            }
        }

        private void ProcessParameterBracket(Stack<ParseTreeNode> stack, ParseTreeNonTerminalNode current, SpiceToken[] tokens, int currentTokenIndex)
        {
            stack.Push(CreateTerminalNode(SpiceTokenType.DELIMITER, current, ")"));
            stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.PARAMETER_BRACKET_CONTENT, current));
            stack.Push(CreateTerminalNode(SpiceTokenType.DELIMITER, current, "("));
            stack.Push(CreateTerminalNode(SpiceTokenType.WORD, current));
        }

        private void ProcessParameterSingle(Stack<ParseTreeNode> stack, ParseTreeNonTerminalNode current, SpiceToken[] tokens, int currentTokenIndex)
        {
            var currentToken = tokens[currentTokenIndex];

            if (currentToken.Is(SpiceTokenType.WORD)
                || currentToken.Is(SpiceTokenType.VALUE)
                || currentToken.Is(SpiceTokenType.STRING)
                || currentToken.Is(SpiceTokenType.IDENTIFIER)
                || currentToken.Is(SpiceTokenType.REFERENCE)
                || currentToken.Is(SpiceTokenType.EXPRESSION))
            {
                stack.Push(CreateTerminalNode(currentToken.SpiceTokenType, current));
            }
            else
            {
                throw new ParseException("Error during parsing a paremeter. Unexpected token: '" + currentToken.Lexem + "'" + " line=" + currentToken.LineNumber, currentToken.LineNumber);
            }
        }

        private void ProcessModel(Stack<ParseTreeNode> stack, ParseTreeNonTerminalNode current, SpiceToken[] tokens, int currentTokenIndex)
        {
            var currentToken = tokens[currentTokenIndex];
            var nextToken = tokens[currentTokenIndex + 1];
            var nextNextToken = tokens[currentTokenIndex + 2];

            if (currentToken.Is(SpiceTokenType.DOT)
                && nextToken.Is(SpiceTokenType.WORD)
                && nextToken.Equal("model", true)
                && (nextNextToken.Is(SpiceTokenType.WORD) || nextNextToken.Is(SpiceTokenType.IDENTIFIER)))
            {
                stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.NEW_LINE_OR_EOF, current));
                stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.PARAMETERS, current));
                stack.Push(CreateTerminalNode(nextNextToken.SpiceTokenType, current));
                stack.Push(CreateTerminalNode(nextToken.SpiceTokenType, current));
                stack.Push(CreateTerminalNode(currentToken.SpiceTokenType, current));
            }
            else
            {
                throw new ParseException("Error during parsing a model. Unexpected token: '" + currentToken.Lexem + "'" + " line=" + currentToken.LineNumber, currentToken.LineNumber);
            }
        }

        private void ProcessControl(Stack<ParseTreeNode> stack, ParseTreeNonTerminalNode current, SpiceToken[] tokens, int currentTokenIndex)
        {
            var currentToken = tokens[currentTokenIndex];
            var nextToken = tokens[currentTokenIndex + 1];

            if (currentToken.Is(SpiceTokenType.DOT) && nextToken.Is(SpiceTokenType.WORD))
            {
                stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.NEW_LINE_OR_EOF, current));
                stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.PARAMETERS, current));
                stack.Push(CreateTerminalNode(nextToken.SpiceTokenType, current));
                stack.Push(CreateTerminalNode(currentToken.SpiceTokenType, current));
            }
            else
            {
                throw new ParseException("Error during parsing a control. Unexpected token: '" + currentToken.Lexem + "'" + " line=" + currentToken.LineNumber, currentToken.LineNumber);
            }
        }

        private void ProcessComponent(Stack<ParseTreeNode> stack, ParseTreeNonTerminalNode current, SpiceToken[] tokens, int currentTokenIndex)
        {
            var currentToken = tokens[currentTokenIndex];

            if (currentToken.Is(SpiceTokenType.WORD))
            {
                stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.NEW_LINE_OR_EOF, current));
                stack.Push(CreateNonTerminalNode(SpiceGrammarSymbol.PARAMETERS, current));
                stack.Push(CreateTerminalNode(currentToken.SpiceTokenType, current));
            }
            else
            {
                throw new ParseException("Error during parsing a component. Unexpected token: '" + currentToken.Lexem + "'" + " line=" + currentToken.LineNumber, currentToken.LineNumber);
            }
        }

        private ParseTreeNonTerminalNode CreateNonTerminalNode(string symbolName, ParseTreeNonTerminalNode currentNode)
        {
            var node = new ParseTreeNonTerminalNode(currentNode, symbolName);
            if (currentNode != null)
            {
                currentNode.Children.Insert(0, node);
            }

            return node;
        }

        private ParseTreeTerminalNode CreateTerminalNode(SpiceTokenType tokenType, ParseTreeNonTerminalNode currentNode, string tokenValue = null)
        {
            var node = new ParseTreeTerminalNode(new SpiceToken(tokenType, tokenValue, 0), currentNode);
            currentNode.Children.Insert(0, node);
            return node;
        }
    }
}
