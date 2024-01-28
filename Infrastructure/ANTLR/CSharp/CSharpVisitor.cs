using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Domain.Mediators;
using Domain.CodeInfo;
using Domain.CodeInfo.InstanceDefinitions;
using Infrastructure.ANTLR.CSharp;

namespace Infrastructure.Antlr
{
    public class CSharpVisitor : CSharpGrammarBaseVisitor<string?>
    {
        private IMediator _mediator;
        /// <summary>
        /// This is used to separate multiple children to be returned, as an example, 
        /// the method VisitProperty should return the type and the name of the property
        /// and it should look like this: "string-name", if the property was something like
        /// "public string name"
        /// </summary>
        private readonly string separator = "-";
        public List<(string, string)> properties = new List<(string, string)>();
        public List<(string, string)> methods = new List<(string, string)>();

        public CSharpVisitor(IMediator mediator)
        {
            _mediator = mediator;
        }

        public int GetRuleIndexInChildren(string ruleName, IParseTree parentRule)
        {
            string suffix = "Context";
            string childRuleName = "";

            // Iterates through all the children and compares if the rule of the child is equal to the ruleName
            for (int j = 0; j < parentRule.ChildCount; j++)
            {
                childRuleName = parentRule.GetChild(j).GetType().ToString();
                childRuleName = childRuleName.Substring(childRuleName.LastIndexOf('+') + 1);
                if (childRuleName.Equals(ruleName + suffix, StringComparison.OrdinalIgnoreCase)) return j;
            }
            return -1;
        }

        public override string VisitProperty([NotNull] CSharpGrammarParser.PropertyContext context)
        {
            int childrenOffset = (context.children.Count > 5) ? (1) : (0);
            properties.Add((context.children[2 + childrenOffset].GetText(), context.children[3 + childrenOffset].GetText()));
            return context.GetChild(3).GetText() + separator + context.GetChild(4).GetText();
        }

        public override string VisitMethod([NotNull] CSharpGrammarParser.MethodContext context)
        {
            // Useful variables
            int childrenOffset = (context.children.Count > 8) ? (1) : (0);
            int parameterIndex = GetRuleIndexInChildren("parameterList", context);
            int methodBodyIndex = GetRuleIndexInChildren("methodBodyContent", context);

            // Getting the method info and sending it to the mediator
            _mediator.ReceiveMethodInfo("a", "as", new List<string> { "asa" });

            // Getting the parameters and storing them into the InstancesDictionary
            for (int j = 0; j < context.children[parameterIndex].ChildCount; j += 2)
            {
                string[] parameter = Visit(context.children[parameterIndex].GetChild(j)).Split("-");
                // TODO: Refactor
                InstancesDictionaryManager.instance.AddAssignation(new Instance(parameter[0]), new Instance(parameter[1]));
            }

            // Getting inside the "methodBodyContent" to get the variables and storing them into the InstancesDictionary
            for (int j = 1; j < context.children[methodBodyIndex].ChildCount-1; j++)
            {
                // Differentiates between "functionCall"s which just has 2 children at most
                if(context.children[methodBodyIndex].GetChild(j).GetChild(0).ChildCount > 2)
                {
                    string[] parameter = Visit(context.children[methodBodyIndex].GetChild(j).GetChild(0)).Split("-");
                    // TODO: Refactor
                    InstancesDictionaryManager.instance.AddAssignation(new Instance(parameter[0]), new Instance(parameter[1]));
                }
            }

            // TODO: Check what this line is doing
            string parameters = context.children[parameterIndex].GetText();
            methods.Add((context.children[2 + childrenOffset].GetText(), context.children[3 + childrenOffset].GetText()));
            return base.VisitMethod(context);
        }
        public override string VisitParameter([NotNull] CSharpGrammarParser.ParameterContext context)
        {
            return context.GetChild(0).GetText() + separator + context.GetChild(1).GetText();
        }

        public override string VisitLocalVariableDeclaration([NotNull] CSharpGrammarParser.LocalVariableDeclarationContext context)
        {
            // Send the info to the mediator that an "expression" has been spotted and manage it
            int expressionIndex = GetRuleIndexInChildren("expression", context);
            // TODO: Make a way to use the "Visit" method to make the things that are expressions like
            // "methodCall" return the function that was called as the string that appears in code
            int methodIndex = GetRuleIndexInChildren("methodCall", context.children[expressionIndex]);
            string methodCallString = context.GetChild(expressionIndex).GetChild(methodIndex).GetText();

            // Gets the Assignee and the Assigner and returns them
            return context.GetChild(1).GetText() + separator + methodCallString;
        }
    }
}
