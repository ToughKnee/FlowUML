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
        private List<AbstractBuilder<Method>> _abstractMethodBuilders;
        // TODO: Make a method that converts this list into the abstract list which other classes need
        private List<MethodBuilder> _methodBuilders = new();
        /// <summary>
        /// This lets other methods access the current built method, which is 
        /// created by the "csharpFile" rule
        /// </summary>
        private MethodBuilder _currentMethodBuilder;
        /// <summary>
        /// Everytime we visit a namespace, this is filled with the namespace if there was any
        /// </summary>
        private string? _currentNamespace = null;
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
            _abstractMethodBuilders = builders;
        }

        private void FillAbstractBuilders()
        {
            foreach(var builder in _methodBuilders)
            {
                _abstractMethodBuilders.Add(builder);
            }
        }
        public IParseTree? GetRuleNodeInChildren(string ruleName, IParseTree parentRule, int initialOffset = 0)
        {
            string suffix = "Context";
            string childRuleName = "";

            // Iterates through all the children and compares if the rule of the child is equal to the ruleName
            for (int j = initialOffset; j < parentRule.ChildCount; j++)
            {
                childRuleName = parentRule.GetChild(j).GetType().ToString();
                childRuleName = childRuleName.Substring(childRuleName.LastIndexOf('+') + 1);
                if (childRuleName.Equals(ruleName + suffix, StringComparison.OrdinalIgnoreCase)) return parentRule.GetChild(j);
            }
            return null;
        }

        public override string VisitProperty([NotNull] CSharpGrammarParser.PropertyContext context)
        {
            int childrenOffset = (context.children.Count > 5) ? (1) : (0);
            properties.Add((context.children[2 + childrenOffset].GetText(), context.children[3 + childrenOffset].GetText()));
            return context.GetChild(3).GetText() + separator + context.GetChild(4).GetText();
        }

        // Main rule that represents the root of all the code in the current file,
        // to ensure we are always starting somewhere
        public override string VisitCSharpFile([NotNull] CSharpGrammarParser.CSharpFileContext context)
        {
            //===========================  Get the current namespace if there is any to add it to the classes and methods present in this file
            var fileNamespacesNode = GetRuleNodeInChildren("fileNamespaces", context);
            int namespaceAbscenseTrigger = 0;
            int namespacesCount = (fileNamespacesNode != null) ? (fileNamespacesNode.ChildCount) : (0);
            if (fileNamespacesNode == null)
            {
                namespaceAbscenseTrigger = -1;
            }

            //===========================  Navigate through all the namespaces in the file to get their classes
            // If there are no namespaces, just navigate through the class rules using the "namespaceAbscenseTrigger"
            IParseTree classDeclarationsNode;
            for (int j = namespaceAbscenseTrigger; j < namespacesCount; j++)
            {
                // Set the namespace we are in currently if we there is one
                if (j != -1)
                {
                    _currentNamespace = Visit(fileNamespacesNode.GetChild(j));
                    classDeclarationsNode = GetRuleNodeInChildren("classDeclarations", fileNamespacesNode.GetChild(j));
                }
                else
                {
                    classDeclarationsNode = GetRuleNodeInChildren("classDeclarations", context);
                }

                // TODO: Make a ClassEntityBuilder and after this loop finishes, add the current class being built to all the MethodBuilders

                // Navigate through the classes in this namespace or file
                int classDeclarationsCount = (classDeclarationsNode != null) ? (fileNamespacesNode.ChildCount) : (0);
                for (int i = 0; i < classDeclarationsCount; i++)
                {
                    // TODO: Make a ClassEntityBuilder and after this loop finishes, add the current class being built to all the MethodBuilders
                    Visit(GetRuleNodeInChildren("classDeclaration", classDeclarationsNode, i));
                }
            }

            FillAbstractBuilders();
            return base.VisitCSharpFile(context);
        }
        public override string VisitFileNamespace([NotNull] CSharpGrammarParser.FileNamespaceContext context)
        {
            var namespaceIdentifierNode = GetRuleNodeInChildren("namespaceIdentifier", context);
            if(namespaceIdentifierNode != null)
            {
                return namespaceIdentifierNode.GetText();
            }
            return null;
        }
        public override string VisitClassDeclaration([NotNull] CSharpGrammarParser.ClassDeclarationContext context)
        {
            var classBodyContentNode = GetRuleNodeInChildren("classBodyContent", context);
            IParseTree classBodyContentChild;
            // TODO: Fill in info for the ClassEntityBuilder
  
            // Pass through all the classContents and get the info for each content
            if(classBodyContentNode != null)
            {
                for (int j = 1; j < classBodyContentNode.ChildCount - 1; j++)
                {
                    // We want to look for the "method" rule, so we do this and later check if it is a method rule node
                    classBodyContentChild = classBodyContentNode.GetChild(j);
                    var methodNode = GetRuleNodeInChildren("method", classBodyContentChild);
                    // If this is a method rule node, we start using the builder
                    if(methodNode != null)
                    {
                        // Building methods info
                        _currentMethodBuilder = new MethodBuilder();
                        _methodBuilders.Add(_currentMethodBuilder);
                        var classIdentifierNode = GetRuleNodeInChildren("identifier", context);
                        var methodIdentifierNode = GetRuleNodeInChildren("identifier", methodNode);
                        var returnTypeNode = GetRuleNodeInChildren("advancedTypeName", methodNode);
                        var parameterListNode = GetRuleNodeInChildren("parameterList", methodNode);
                        _currentMethodBuilder.SetOwnerClass(classIdentifierNode.GetText());
                        _currentMethodBuilder.SetName(methodIdentifierNode.GetText());
                        _currentMethodBuilder.SetReturnType(returnTypeNode.GetText());
                        _currentMethodBuilder.SetBelongingNamespace(_currentNamespace);
                        if(parameterListNode != null)
                        {
                            _currentMethodBuilder.SetParameters(Visit(parameterListNode));
                        }
                        // And after filling the available info at this Node, we go to another Node to get more info for the current Method
                        Visit(methodNode);
                    }
                }
            }
            return base.VisitClassDeclaration(context);
        }
        public override string VisitMethod([NotNull] CSharpGrammarParser.MethodContext context)
        {
            // Useful variables
            int childrenOffset = (context.children.Count > 8) ? (1) : (0);
            var parameterNode = GetRuleNodeInChildren("parameterList", context);
            var methodBodyNode = GetRuleNodeInChildren("methodBodyContent", context);

            // Getting the parameters and storing them into the InstancesDictionary if any
            if(parameterNode != null)
            {
                for (int j = 0; j < parameterNode.ChildCount; j += 2)
                {
                    string[] parameters = Visit(parameterNode.GetChild(j)).Split("-");
                    // TODO: Refactor
                    InstancesDictionaryManager.instance.AddAssignation(new Instance(parameters[1]), new Instance(parameters[0]));
                }
            }

            // Getting inside the "methodBodyContent" to get the variables and storing them into the InstancesDictionary
            for (int j = 1; j < methodBodyNode.ChildCount-1; j++)
            {
                // Differentiates between "functionCall"s which just has 2 children at most
                if(methodBodyNode.GetChild(j).GetChild(0).ChildCount > 2)
                {
                    string[] parameter = Visit(methodBodyNode.GetChild(j).GetChild(0)).Split("-");
                    // TODO: Refactor
                    InstancesDictionaryManager.instance.AddAssignation(new Instance(parameter[0]), new Instance(parameter[1]));
                }
            }
            // TODO: Make the Callsites
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
            var expressionNode = GetRuleNodeInChildren("expression", context);
            // TODO: Make a way to use the "Visit" method to make the things that are expressions like
            // "methodCall" return the function that was called as the string that appears in code
            var methodNode = GetRuleNodeInChildren("methodCall", expressionNode);
            string methodCallString = methodNode.GetText();

            // Gets the Assignee and the Assigner and returns them
            return context.GetChild(1).GetText() + separator + methodCallString;
        }
    }
}
