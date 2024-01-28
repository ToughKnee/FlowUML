using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Domain.CodeInfo;
using Domain.CodeInfo.InstanceDefinitions;
using Infrastructure.ANTLR.CSharp;
using Infrastructure.Builders;

namespace Infrastructure.Antlr
{
    public class CSharpVisitor : CSharpGrammarBaseVisitor<string?>
    {
        /// <summary>
        /// This visitor creates a MethodBuilder and it visits a method rule,
        /// and fills it with the information it finds to later be used by someone else
        /// and make all the necessary information for the domain classes from the builders
        /// </summary>
        private List<AbstractBuilder<Method>> _methodBuilders;
        /// <summary>
        /// This is used to separate multiple children to be returned, as an example, 
        /// the method VisitProperty should return the type and the name of the property
        /// and it should look like this: "string-name", if the property was something like
        /// "public string name"
        /// </summary>
        private readonly string separator = "-";
        public List<(string, string)> properties = new List<(string, string)>();
        public List<(string, string)> methods = new List<(string, string)>();

        public CSharpVisitor(List<AbstractBuilder<Method>> builders)
        {
            _methodBuilders = builders;
        }

        public int GetRuleIndexInChildren(string ruleName, IParseTree parentRule, int initialOffset = 0)
        {
            string suffix = "Context";
            string childRuleName = "";

            // Iterates through all the children and compares if the rule of the child is equal to the ruleName
            for (int j = initialOffset; j < parentRule.ChildCount; j++)
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

        public override string VisitClassDeclaration([NotNull] CSharpGrammarParser.ClassDeclarationContext context)
        {
            int classBodyContentIndex = GetRuleIndexInChildren("classBodyContent", context);
            int methodIndex = 0;
            var classBodyContentNode = context.children[classBodyContentIndex];
            IParseTree classBodyContentChild;
  
            // Pass through all the classContents and get the info for each content
            for (int j = 1; j < classBodyContentNode.ChildCount - 1; j++)
            {
                // We want to look for the "method" rule, so we do this and later check if it is a method rule node
                classBodyContentChild = classBodyContentNode.GetChild(j);
                methodIndex = GetRuleIndexInChildren("method", classBodyContentChild);
                // If this is a method rule node, we start using the builder
                if(methodIndex > -1)
                {
                    // Building methods info
                    MethodBuilder methodBuilder = new MethodBuilder();
                    _methodBuilders.Add((AbstractBuilder<Method>) methodBuilder);
                    int classIdentifierIndex = GetRuleIndexInChildren("identifier", context);
                    int methodIdentifierIndex = GetRuleIndexInChildren("identifier", classBodyContentChild.GetChild(methodIndex));
                    int returnTypeIndex = GetRuleIndexInChildren("advancedTypeName", classBodyContentChild.GetChild(methodIndex));
                    int parameterListIndex = GetRuleIndexInChildren("parameterList", classBodyContentChild.GetChild(methodIndex));
                    methodBuilder.SetOwnerClass(context.children[classIdentifierIndex].GetText());
                    methodBuilder.SetName(classBodyContentChild.GetChild(methodIndex).GetChild(methodIdentifierIndex).GetText());
                    methodBuilder.SetReturnType(classBodyContentChild.GetChild(methodIndex).GetChild(returnTypeIndex).GetText());
                    if(parameterListIndex > -1)
                    {
                        methodBuilder.SetParameters(Visit(classBodyContentChild.GetChild(methodIndex).GetChild(parameterListIndex)));
                    }
                    Visit(classBodyContentChild.GetChild(methodIndex));

                }
            }
            return base.VisitClassDeclaration(context);
        }
        public override string VisitMethod([NotNull] CSharpGrammarParser.MethodContext context)
        {
            // Useful variables
            int childrenOffset = (context.children.Count > 8) ? (1) : (0);
            int parameterIndex = GetRuleIndexInChildren("parameterList", context);
            int methodBodyIndex = GetRuleIndexInChildren("methodBodyContent", context);

            // Getting the parameters and storing them into the InstancesDictionary if any
            if(parameterIndex > -1)
            {
                for (int j = 0; j < context.children[parameterIndex].ChildCount; j += 2)
                {
                    string[] parameters = Visit(context.children[parameterIndex].GetChild(j)).Split("-");
                    // TODO: Refactor
                    InstancesDictionaryManager.instance.AddAssignation(new Instance(parameters[0]), new Instance(parameters[1]));
                }
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

            methods.Add((context.children[2 + childrenOffset].GetText(), context.children[3 + childrenOffset].GetText()));
            return base.VisitMethod(context);
        }

        public override string VisitParameterList([NotNull] CSharpGrammarParser.ParameterListContext context)
        {
            string result = "";
            // Getting the parameters and storing them into the InstancesDictionary if any
            if (context.ChildCount > 0)
            {
                for (int j = 0; j < context.ChildCount; j += 2)
                {
                    string[] parameters = Visit(context.GetChild(j)).Split("-");
                    result += parameters[0] + ",";
                }
                result = result.Substring(0, result.Length - 1);
            }
            return result;
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
