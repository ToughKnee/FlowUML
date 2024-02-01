using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Domain.CodeInfo;
using Domain.CodeInfo.InstanceDefinitions;
using Infrastructure.ANTLR.CSharp;
using Infrastructure.Builders;
using Infrastructure.Mediators;

namespace Infrastructure.Antlr
{
    public class CSharpVisitor : CSharpGrammarBaseVisitor<string?>
    {
        /// <summary>
        /// Mediator that receives the data from the localVariables and such to manage them
        /// and define the Domain classes
        /// </summary>
        private IMediator _mediator;
        /// <summary>
        /// Method builders that are added up according to the method declarations in this file
        /// </summary>
        private List<MethodBuilder> _methodBuilders = new();
        /// <summary>
        /// Class builders that are added up according to the class declarations in this file
        /// </summary>
        private List<ClassEntityBuilder> _classBuilders = new();
        /// <summary>
        /// This lets other methods access the current built method, which is 
        /// created by the "csharpFile" rule
        /// </summary>
        private MethodBuilder _currentMethodBuilder;
        /// <summary>
        /// The current class builder for other methods to fill in with class info
        /// </summary>
        private ClassEntityBuilder _currentClassBuilder;
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

        public CSharpVisitor(IMediator mediator)
        {
            _mediator = mediator;
        }

        public List<AbstractBuilder<Method>> GetMethodBuilders()
        {
            List<AbstractBuilder<Method>> abstractMethodBuilders = new();
            foreach (var builder in _methodBuilders)
            {
                abstractMethodBuilders.Add(builder);
            }
            return abstractMethodBuilders;
        }
        public List<AbstractBuilder<ClassEntity>> GetClassBuilders()
        {
            List<AbstractBuilder<ClassEntity>> abstractClassEntityBuilders = new();
            foreach (var builder in _classBuilders)
            {
                abstractClassEntityBuilders.Add(builder);
            }
            return abstractClassEntityBuilders;
        }
        public bool ChildRuleNameIs(string ruleNameToCheck, IParseTree parentRule, int childIndex)
        {
            string suffix = "Context";
            string childRuleName = parentRule.GetChild(childIndex).GetType().ToString();
            childRuleName = childRuleName.Substring(childRuleName.LastIndexOf('+') + 1);
            if (childRuleName.Equals(ruleNameToCheck + suffix, StringComparison.OrdinalIgnoreCase)) return true;
            return false;
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
            var identifierNode = GetRuleNodeInChildren("identifier", context);
            var typeNode = GetRuleNodeInChildren("type", context);
            _currentClassBuilder.AddProperty(typeNode.GetText(), identifierNode.GetText());
            return typeNode.GetText() + separator + identifierNode.GetText();
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
                // Set the namespace we are in currently if there is one
                if (j != -1)
                {
                    _currentNamespace = Visit(fileNamespacesNode.GetChild(j));
                    _mediator.ReceiveNamespace(_currentNamespace);
                    classDeclarationsNode = GetRuleNodeInChildren("classDeclarations", fileNamespacesNode.GetChild(j));
                }
                else
                {
                    classDeclarationsNode = GetRuleNodeInChildren("classDeclarations", context);
                }

                // Navigate through the classes available in this namespace or file
                int classDeclarationsCount = (classDeclarationsNode != null) ? (classDeclarationsNode.ChildCount) : (0);
                for (int i = 0; i < classDeclarationsCount; i++)
                {
                    _currentClassBuilder = new ClassEntityBuilder();
                    _classBuilders.Add(_currentClassBuilder);
                    _currentClassBuilder.SetNamespace(_currentNamespace);
                    var classDeclarationNode = GetRuleNodeInChildren("classDeclaration", classDeclarationsNode, i);
                    _mediator.ReceiveClassName(GetRuleNodeInChildren("identifier", classDeclarationNode).GetText());
                    // Visit the "classDeclaration" 
                    Visit(classDeclarationNode);
                }
            }
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
        /// <summary>
        /// This handles things like "methods, properties, constructors" and all the things that a class may have
        /// At this point we have defined the namespace(if there was one), and we need to gather data like className, 
        /// properties, methods
        /// At an even deeper level(which will be covered by other visit methods) we will cover the callsites, 
        /// instance declarations and their types to be stored at the instancesDictionary and such
        /// </summary>
        /// <param name="context"></param>
        /// <returns>This returns null because there are no other points of interest 
        /// further down the tree that we have not exlpored after finishing this node</returns>
        public override string VisitClassDeclaration([NotNull] CSharpGrammarParser.ClassDeclarationContext context)
        {
            var classBodyContentNode = GetRuleNodeInChildren("classBodyContent", context);
            IParseTree classContentNode;
            // TODO: Fill in info for the ClassEntityBuilder
            var classIdentifierNode = GetRuleNodeInChildren("identifier", context);
            _currentClassBuilder.SetName(classIdentifierNode.GetText());

            // Pass through all the classContents and get the info for each content
            if (classBodyContentNode != null)
            {
                for (int j = 1; j < classBodyContentNode.ChildCount - 1; j++)
                {
                    classContentNode = classBodyContentNode.GetChild(j);
                    var classContentChild = classContentNode.GetChild(0);
                    // If this is a property rule, we visit the node
                    if (ChildRuleNameIs("property", classContentNode, 0))
                    {
                        // TODO: Call the mediator to make him receive the results of visiting the parameters
                        // and make him add to the instancesDicionary the parameters
                        Visit(classContentChild);
                    }
                    // If this is a method rule node, we create a methodBuilder and start building the method
                    else if (ChildRuleNameIs("method", classContentNode, 0))
                    {
                        // Building methods info
                        _currentMethodBuilder = new MethodBuilder();
                        _methodBuilders.Add(_currentMethodBuilder);
                        _currentClassBuilder.AddMethod(_currentMethodBuilder);
                        string methodIdentifierText = GetRuleNodeInChildren("identifier", classContentChild).GetText();
                        string ownerClassText = classIdentifierNode.GetText();
                        string returnTypeText = "";
                        string parameters = "";
                        var returnTypeNode = GetRuleNodeInChildren("type", classContentChild);
                        var parameterListNode = GetRuleNodeInChildren("parameterList", classContentChild);
                        _currentMethodBuilder.SetBelongingNamespace(_currentNamespace);
                        _currentMethodBuilder.SetName(methodIdentifierText);
                        if (returnTypeNode != null)
                        {
                            returnTypeText = returnTypeNode.GetText();
                            _currentMethodBuilder.SetReturnType(returnTypeText);
                        }
                        if (parameterListNode != null)
                        {
                            parameters = Visit(parameterListNode);
                            _currentMethodBuilder.SetParameters(parameters);
                        }
                        // Send to the mediator the method declaration to put that in the isntancesDictionary
                        _mediator.ReceiveMethodDeclaration(_currentNamespace, ownerClassText, methodIdentifierText, parameters, returnTypeText);
                        // And after filling the available info at this Node, we go to another Node to get more info for the current Method
                        Visit(classContentChild);
                    }
                }
            }
            return "";
        }
        public override string VisitMethod([NotNull] CSharpGrammarParser.MethodContext context)
        {
            // Useful variables
            int childrenOffset = (context.children.Count > 8) ? (1) : (0);
            var parameterNode = GetRuleNodeInChildren("parameterList", context);
            var methodBodyNode = GetRuleNodeInChildren("methodBodyContent", context);

            // Getting the parameters and passing them to the mediator
            if(parameterNode != null)
            {
                for (int j = 0; j < parameterNode.ChildCount; j += 2)
                {
                    string[] parameters = Visit(parameterNode.GetChild(j)).Split("-");
                    _mediator.ReceiveParameters(parameters[0], parameters[1]);
                }
            }

            // Getting inside the "methodBodyContent" to get the variables and pass them to the mediator
            for (int j = 1; j < methodBodyNode.ChildCount-1; j++)
            {
                // Get the local variable rule if there is
                if(ChildRuleNameIs("localVariableDeclaration", methodBodyNode.GetChild(j), 0))
                {
                    // TODO: Make the MethodInstances WTIH their linked Callsites, ALSO, when making the callsites or MEthodInstances also STORE the usings that were used in this file to be able to disambiaguate between classes with the same name
                    string[] parameter = Visit(methodBodyNode.GetChild(j).GetChild(0)).Split("-");
                    _mediator.ReceiveLocalVariableDeclaration(parameter[0], parameter[1]);
                }
                else
                {
                    // We visit all the other childs explicitly, in order to end the method analysis after we find the callsites this method made and be able to let the mediator get all the info it needs, because at this point the mediator does not have that kind of info, but will if we visit the children preemptively before the default implementation
                    Visit(methodBodyNode.GetChild(j).GetChild(0));
                }
            }
            _mediator.ReceiveMethodAnalysisEnd();
            return "";
        }

        /// <summary>
        /// Gets the info to later create the callsites made inside a method 
        /// and sends this info to the mediators
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override string VisitMethodCall([NotNull] CSharpGrammarParser.MethodCallContext context)
        {
            //===========================  Getting the components of the method called
            string functionSignature = context.GetText();
            var lastPeriodIndex = functionSignature.LastIndexOf('.');
            Console.WriteLine("lastPeriodIndex: " + lastPeriodIndex.ToString());
            var methodName = functionSignature.Substring(
                (lastPeriodIndex != -1) ? (lastPeriodIndex + 1) : (0)
                );
            var namespaceAndClass = (lastPeriodIndex != -1) ? (functionSignature.Substring(0, lastPeriodIndex)) : ("");
            var openParenIndex = methodName.IndexOf('(');
            var closeParenIndex = methodName.IndexOf(')');
            var parameters = methodName.Substring(openParenIndex + 1, closeParenIndex - openParenIndex - 1).Split(',');
            List<string> parameterList = null;
            if (!String.IsNullOrEmpty(parameters[0]))
            {
                parameterList = new List<string>(parameters);
            }
            methodName = methodName.Substring(0, openParenIndex);

            // Passing the info to the mediator
            _mediator.ReceiveMethodCall(namespaceAndClass, methodName, parameterList, _currentMethodBuilder);

            return functionSignature;
        }

        public override string VisitParameterList([NotNull] CSharpGrammarParser.ParameterListContext context)
        {
            string result = "";
            // Getting the parameters type and returning them separated by comma
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

        /// <summary>
        /// This method is important to know the Callsites ANd also the Instances inside a method, which will identify the callsites
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override string VisitLocalVariableDeclaration([NotNull] CSharpGrammarParser.LocalVariableDeclarationContext context)
        {
            var expressionNode = GetRuleNodeInChildren("expression", context);

            // "Left side" of an assignment
            var identifierNode = GetRuleNodeInChildren("identifier", context);

            var methodNode = GetRuleNodeInChildren("methodCall", expressionNode);
            // "Right side" of the assignment
            string assignerExpression = Visit(expressionNode);
            // Gets the Assignee and the Assigner and returns them
            return identifierNode.GetText() + separator + assignerExpression;
        }
        /// <summary>
        /// This gets the "right side" of an assignment, which is something that gives an implementation of something to a variable
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override string VisitExpression([NotNull] CSharpGrammarParser.ExpressionContext context)
        {
            string assignmentText = "";
            var methodNode = GetRuleNodeInChildren("methodCall", context);
            if(methodNode != null)
            {
                assignmentText = Visit(methodNode) + separator;
            }
            return assignmentText;
        }
    }
}
