using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Domain.CodeInfo;
using Domain.CodeInfo.InstanceDefinitions;
using Domain.CodeInfo.MethodSystem;
using Infrastructure.ANTLR.CSharp;
using Infrastructure.Builders;
using Infrastructure.Mediators;

namespace Infrastructure.Antlr
{
    public class CSharpVisitor : CSharpGrammarBaseVisitor<string?>
    {
        /// <summary>
        /// Class that stores the actual type of a method call return type when a previous node 
        /// like local variable or expression node have the actual type of a MethodCall 
        /// but the method call node isn't able to get that type
        /// Uses a Stack to be able to store multiple actual types
        /// </summary>
        private class MethodActualTypeContainer
        {
            /// <summary>
            /// This is the method's signature which this type is intended to
            /// when the methodCall node is looking for its actual type
            /// </summary>
            private string _methodNameIdentifier;
            private string _actualType;
            public static Stack<MethodActualTypeContainer> _previousActualInstance = new();

            public MethodActualTypeContainer(string actualType, string methodName)
            {
                PushActualType(actualType, methodName);
            }
            public void PushActualType(string actualType, string methodName)
            {
                _actualType = actualType;
                _methodNameIdentifier = methodName;
                MethodActualTypeContainer._previousActualInstance.Push(this);
            }
            public static string PopActualType()
            {
                return MethodActualTypeContainer._previousActualInstance.Pop()._actualType;
            }
            public static bool CheckForActualMethodType(string methodSignature)
            {
                if(_previousActualInstance.Count == 0) 
                    return false;
                var currentMethodType = _previousActualInstance.Peek();
                if(currentMethodType._methodNameIdentifier == methodSignature)
                    return true;
                else
                    return false;
            }
        }
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
        /// <summary>
        /// Stack that is used mainly by the method "VisitMethodCall" whenever there are other nested methodCalls, 
        /// things like parameters that are methodCalls too, or chained methodCalls like 
        /// "MyMethodCall().propertyChain.chainedMethodCall()", where "propertyChain.chainedMethodCall()" would be 
        /// the chained methodCall of "MyMethodCall()"
        /// An element is added whenever a methodCall has been visited, and retrieved by the methodCall that owns the
        /// other methodCall because it was a parameter or a chained methodCall
        /// This can be thought of as pre order tree traversal
        /// This is important for the mediator because it contains the methodCalls found in a expression, 
        /// this is sent to the mediator and cleared to keep doing the same process
        /// </summary>
        private Stack<MethodInstanceBuilder> _methodInstanceBuildersStack = new();

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
        public override string VisitAdvancedTypeName([NotNull] CSharpGrammarParser.AdvancedTypeNameContext context)
        {
            return "";
        }
        public override string VisitProperty([NotNull] CSharpGrammarParser.PropertyContext context)
        {
            var identifierNode = GetRuleNodeInChildren("identifier", context);
            var typeNode = GetRuleNodeInChildren("type", context);
            var propertyTypeString = typeNode.GetText().Replace("?", "");
            _currentClassBuilder.AddProperty(propertyTypeString, identifierNode.GetText());
            return propertyTypeString + separator + identifierNode.GetText();
        }
        /// <summary>
        /// Returns the identifiers of all the namespaces in a string separated by hyphens
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override string VisitUsingDirectives([NotNull] CSharpGrammarParser.UsingDirectivesContext context)
        {
            string usedNamespaces = "";
            for (int j = 0; j < context.ChildCount; j++)
            {
                var namespaceIdentifierNode = GetRuleNodeInChildren("namespaceIdentifier", context.GetChild(0), j);
                usedNamespaces += namespaceIdentifierNode.GetText() + "-";
            }
            usedNamespaces = usedNamespaces.Substring(0, usedNamespaces.Length - 1);
            return usedNamespaces;
        }

        // Main rule that represents the root of all the code in the current file,
        // to ensure we are always starting somewhere
        public override string VisitCSharpFile([NotNull] CSharpGrammarParser.CSharpFileContext context)
        {
            // Fill the _currentlyUsedNamespaces list
            var usingDirectivesNode = GetRuleNodeInChildren("usingDirectives", context);
            if(usingDirectivesNode != null)
            {
                List<string> usedNamespaces = Visit(usingDirectivesNode).Split("-").ToList();
                _mediator.ReceiveUsedNamespaces(usedNamespaces);
            }

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
                    classDeclarationsNode = GetRuleNodeInChildren("classDeclarations", fileNamespacesNode.GetChild(j));
                }
                else
                {
                    classDeclarationsNode = GetRuleNodeInChildren("classDeclarations", context);
                }
                _mediator.ReceiveNamespace(_currentNamespace);

                // Navigate through the classes available in this namespace or file
                int classDeclarationsCount = (classDeclarationsNode != null) ? (classDeclarationsNode.ChildCount) : (0);
                for (int i = 0; i < classDeclarationsCount; i++)
                {
                    _currentClassBuilder = new ClassEntityBuilder();
                    _classBuilders.Add(_currentClassBuilder);
                    _currentClassBuilder.SetNamespace(_currentNamespace);
                    var classDeclarationNode = GetRuleNodeInChildren("classDeclaration", classDeclarationsNode, i);

                    string classNameAndInheritance = GetRuleNodeInChildren("identifier", classDeclarationNode).GetText();
                    var classHeritageNode = GetRuleNodeInChildren("classHeritage", classDeclarationNode);
                    // Get the name of the class and their inheritance if they have any, and send it to the mediator
                    if (classHeritageNode != null)
                    {
                        classNameAndInheritance += "," + classHeritageNode.GetText().Substring(1);
                    }

                    _mediator.ReceiveClassNameAndInheritance(classNameAndInheritance);

                    var templateTypeNameNode = GetRuleNodeInChildren("templateTypeName", classDeclarationNode, i);
                    // Look for the typenames this class has and if it does, then add them to the current class builder
                    if (templateTypeNameNode != null)
                    {
                        CustomVisitTemplateTypeName(templateTypeNameNode, true);
                    }

                    // Visit the "classDeclaration"
                    Visit(classDeclarationNode);
                }
            }
            // There is no need to explore the tree again since we did that already
            return "";
        }
        public override string VisitFileNamespace([NotNull] CSharpGrammarParser.FileNamespaceContext context)
        {
            var namespaceIdentifierNode = GetRuleNodeInChildren("namespaceIdentifier", context);
            if(namespaceIdentifierNode != null)
            {
                return namespaceIdentifierNode.GetText();
            }
            return "";
        }
        /// <summary>
        /// This method traverses the templateTypeName node if there are typenames present in the class or
        /// method declaration and sets them to the correspondent current builder
        /// </summary>
        /// <param name="context">The node that has the templateTypeName as a child</param>
        /// <param name="useClassBuilder">This boolean is set according to the context from which this
        /// method was called, if it was called at the class declaration node, then it is true,
        /// if it was called in the method declaration, then false</param>
        /// <returns></returns>
        public string CustomVisitTemplateTypeName(IParseTree context, bool useClassBuilder)
        {
            if (useClassBuilder)
                _currentClassBuilder.SetTypename(Typename.GetTypenameList(context.GetText()));
            else
                _currentMethodBuilder.SetTypename(Typename.GetTypenameList(context.GetText()));
            return "";
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
                        string[] properties = Visit(classContentChild).Split("-");
                        _mediator.ReceiveProperties(properties[0], properties[1]);
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
                        // If the return type is null(meaning also it isn't "void"), then use the identifier of this method as the return type since this is a constructor method
                        else
                        {
                            _currentMethodBuilder.SetReturnType(methodIdentifierText);
                        }
                        if (parameterListNode != null)
                        {
                            parameters = Visit(parameterListNode);
                            _currentMethodBuilder.SetParameters(parameters);
                        }
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

            // Getting inside the "methodBodyContent" to get the variables and methodCalls and pass them to the mediator
            ProcessMethodBodyContent(methodBodyNode);
            
            _mediator.ReceiveMethodAnalysisEnd();
            return "";
        }
        public void ProcessMethodBodyContent(IParseTree methodBodyNode)
        {
            if (methodBodyNode == null)
                return;
            for (int j = 1; j < methodBodyNode.ChildCount - 1; j++)
            {
                // Get the local variable rule if there is
                if (ChildRuleNameIs("localVariableDefinition", methodBodyNode.GetChild(j).GetChild(0), 0))
                {
                    Visit(methodBodyNode.GetChild(j).GetChild(0).GetChild(0));
                }
                else
                {
                    // We visit all the other childs explicitly, in order to end the method analysis after we find the callsites this method made and be able to let the mediator get all the info it needs, because at this point the mediator does not have that kind of info, but will if we visit the children preemptively
                    // Aside from that, if we visit the "statement" node, we start visting all the nodes automatically
                    Visit(methodBodyNode.GetChild(j).GetChild(0));
                }

                // Check if there are methodCalls in the stack, and send them to the mediator, also the methodCall at the bottom of the stack is the first method being called, and the methodCalls that come after it are methodCalls inside the subtree of this MethodCall that are not part of the methodCall chained property nor parameters
                if (_methodInstanceBuildersStack.Count > 0)
                {
                    _mediator.ReceiveMethodCall(_methodInstanceBuildersStack.Cast<AbstractBuilder<AbstractInstance>>().ToList());
                    _methodInstanceBuildersStack.Clear();
                }
            }
        }
        /// <summary>
        /// This method will get the properties of instantiated classes used in a method
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override string VisitAdvancedIdentifier([NotNull] CSharpGrammarParser.AdvancedIdentifierContext context)
        {
            // TODO: Recopile the information of properties and send it to the mediator
            return "";
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
        public override string VisitLocalVariableDefinition([NotNull] CSharpGrammarParser.LocalVariableDefinitionContext context)
        {
            var expressionNode = GetRuleNodeInChildren("expression", context);

            // "Left side" of an assignment
            var identifierNode = GetRuleNodeInChildren("identifier", context);

            string assignerExpression = "";
            string result = "";
            IParseTree expressionChildNode;
            InstanceBuilder instanceBuilder = new(_mediator);
            var listWithBuilders = new List<AbstractBuilder<AbstractInstance>>();

            // If the expressionNode is null, then this is just a variable declaration, and we can get the type
            if(expressionNode == null)
            {
                instanceBuilder.SetCallerClassName(identifierNode.GetText());
                instanceBuilder.SetType(GetRuleNodeInChildren("type", context).GetText());
                listWithBuilders.Add(instanceBuilder);
                _mediator.ReceiveLocalVariableDefinition(identifierNode.GetText(), "", listWithBuilders);
                return assignerExpression;
            }
            // If the expression is a methodCall, visit it and get the info of the assignment
            else if ((expressionChildNode = GetRuleNodeInChildren("methodCall", expressionNode)) != null)
            {
                // If the variable declaration looks like "MyType typeInstance = new();", then we store the type in the property for the methodCall node to set the methodInstance
                if (expressionNode.GetText().Contains("new("))
                {
                    new MethodActualTypeContainer((GetRuleNodeInChildren("type", context) is not null && GetRuleNodeInChildren("type", context).GetText() != "var") 
                        ? (GetRuleNodeInChildren("type", context).GetText()) : (null), expressionChildNode.GetText());
                }
                // "Right side" of the assignment
                assignerExpression = Visit(expressionNode);

                // Gets the Assignee and the Assigner and returns them, if the assignee is a property of a class, then leave the assignee null
                var identifierText = (identifierNode is not null) ? (identifierNode.GetText()) : ("");
                result = identifierText + separator + assignerExpression;
                listWithBuilders = _methodInstanceBuildersStack.Cast<AbstractBuilder<AbstractInstance>>().ToList();
                _methodInstanceBuildersStack.Clear();
            }
            // If the expression is another identifier of another variable, or field of a class, then get the info and return it
            else if((expressionChildNode = GetRuleNodeInChildren("advancedIdentifier", expressionNode)) is not null)
            {
                // If the assignee is not a simple variable(and instead is a property of a class) then do nothing
                if (identifierNode == null) return "";

                var typeNode = GetRuleNodeInChildren("type", context);
                // If the type of the variable isn't "var", then we set the type
                if (typeNode is not null && typeNode.GetText() != "var")
                {
                    instanceBuilder.SetType(typeNode.GetText());
                }
                listWithBuilders.Add(instanceBuilder);
                instanceBuilder.SetCallerClassName(expressionChildNode.GetText());
                var indexRetrievalInstance = ProcessIndexRetrieval(expressionNode);
                instanceBuilder.SetIndexRetrievalInstance(indexRetrievalInstance);
                result = identifierNode.GetText() + separator + expressionChildNode.GetText();
            }
            // TODO: Visit other localVariableDefinition nodes chained, the things like "var thing1 = 1, thing2 = 2"

            // Sending the info to the mediator
            if (result.Contains("-"))
            {
                string[] assignmentValues = result.Split("-");
                // If there are arithmetic operations expressions, and there is no immediate method call, then return and do nothing
                if (GetRuleNodeInChildren("arithmeticOperations", expressionNode) is not null && GetRuleNodeInChildren("methodCall", expressionNode) is null)
                    return assignerExpression;
                _mediator.ReceiveLocalVariableDefinition(assignmentValues[0], assignmentValues[1], listWithBuilders);
            }

            return assignerExpression;
        }
        /// <summary>
        /// Represents the logic that results in a given value of any type, like a methodCall or a simple math procedure
        /// This normally gets the "right side" of an assignment, which is something that gives a type of something to a variable
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override string VisitExpression([NotNull] CSharpGrammarParser.ExpressionContext context)
        {
            string assignmentText = "";
            var methodCallNode = GetRuleNodeInChildren("methodCall", context);
            var arithmeticOperationsNode = GetRuleNodeInChildren("arithmeticOperations", context);
            
            // Get the full text of the assignment 
            if(methodCallNode != null)
            {
                var typeCasterNode = GetRuleNodeInChildren("typeCaster", context);

                // Check if there are explicit type casters in this method call, and set the data for the method call node to receive it
                if (typeCasterNode is not null)
                {
                    new MethodActualTypeContainer(typeCasterNode.GetChild(1).GetText(), methodCallNode.GetText());
                }

                assignmentText = Visit(methodCallNode);
            }
            else
            {
                assignmentText = context.GetText();
            }

            //===========================  Visit whichever expression here to traverse the entire tree to find other methodCalls inside complex rules like ternaryOperators, including also the arithmetic operations node
            if (methodCallNode == null)
                base.VisitExpression(context);
            else if(arithmeticOperationsNode != null)
            {
                for (int i = 1; i < context.ChildCount; i++)
                {
                    arithmeticOperationsNode = GetRuleNodeInChildren("arithmeticOperations", context, i);
                    Visit(arithmeticOperationsNode);
                }
            }
            //=====

            return assignmentText;
        }
        private AbstractInstance? ProcessIndexRetrieval(IParseTree context, MethodInstanceBuilder ownerMethodInstanceBuilder = null)
        {
            var indexRetrievalNode = GetRuleNodeInChildren("indexRetrieval", context);
            if (indexRetrievalNode is null) return null;
            AbstractInstance chainedInstance;
            string resultInstanceName = indexRetrievalNode.GetChild(1).GetText(); // We get the second child because that child will always contain the contents of the brackets indexer
            var indexRetrievalInstanceResult = new Instance(resultInstanceName);
            indexRetrievalInstanceResult.kind = KindOfInstance.IsIndexRetrievalInstance;
            //if (methodInstanceBuilder != null) methodInstanceBuilder.SetMethodKind(KindOfInstance.IsIndexRetrievalInstance);

            var expressionChainNode = GetRuleNodeInChildren("expressionChain", indexRetrievalNode);
            // If there is an expressionChain in the indexRetrieval node, then process it
            if (expressionChainNode != null)
            {
                // Get the chained instance to later add it to the generated result instance
                var chainedInstanceObject = ProcessExpressionChain(indexRetrievalNode);
                if(chainedInstanceObject is MethodInstanceBuilder)
                {
                    var chainedInstanceBuilder = (MethodInstanceBuilder)chainedInstanceObject;
                    chainedInstance = ((MethodInstanceBuilder)chainedInstanceBuilder).Build();
                }
                else chainedInstance = (Instance)chainedInstanceObject;

                indexRetrievalInstanceResult.chainedInstance = chainedInstance;
            }
            // TODO: At some point process also other IndexRetrieval nodes
            return indexRetrievalInstanceResult;
        }

        private object? ProcessExpressionChain(IParseTree context)
        {
            var expressionChainNode = GetRuleNodeInChildren("expressionChain", context);
            if(expressionChainNode == null) return null; 

            // Process the index retrieval if there is any
            var indexRetrievalInstance = ProcessIndexRetrieval(expressionChainNode);

            // If there is another methodCall chained, then visit that too, which will put the chained methodCall into the Stack of MethodInstanceBuilders and be retrieved after the visited chained methodCall finishes being processed and then set it to the methodInstanceBuilder
            if (expressionChainNode != null && ChildRuleNameIs("methodCall", expressionChainNode, 1))
            {
                Visit(expressionChainNode.GetChild(1));
                return _methodInstanceBuildersStack.Pop();
            }
            // If its a normal instance of an object, then get the string and add it to the methodInstanceBuilder
            // TODO: Move the logic of the Instance creation to the MthodInstancBuilder to reduce coupling
            else if (expressionChainNode != null && ChildRuleNameIs("advancedIdentifier", expressionChainNode, 1))
            {
                var processedChainInstance = new Instance(expressionChainNode.GetChild(1).GetText());
                processedChainInstance.kind = KindOfInstance.IsPropertyFromInheritanceOrInThisClass;
                processedChainInstance.indexRetrievedInstance = indexRetrievalInstance;
                return processedChainInstance;
            }

            return null;
        }
        /// <summary>
        /// Gets the info of this methodCall like the method's signature to later
        /// create the callsites made inside the method that made this methodCall 
        /// and send this info to the mediator
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override string VisitMethodCall([NotNull] CSharpGrammarParser.MethodCallContext context)
        {
            // Create a new methodInstanceBuilder
            MethodInstanceBuilder methodInstanceBuilder = new(this._mediator);

            //===========================  Getting the components of the method called
            bool isConstructor = GetRuleNodeInChildren("new", context) != null;
            string completeFunctionString = context.GetText(), namespaceAndClass = "", methodName = "";

            // Removing the new keyword from the actual method call
            if(isConstructor)
            {
                completeFunctionString = completeFunctionString.Remove(completeFunctionString.IndexOf("new"), 3);
            }
            int firstParenthesesIndex = completeFunctionString.IndexOf('(');
            List<object> parameterList = new();
            if(firstParenthesesIndex > 0)
            {
                string callerComponent = completeFunctionString.Substring(0, firstParenthesesIndex-1);
                var lastPeriodIndex = callerComponent.LastIndexOf('.');
                if (completeFunctionString.LastIndexOf('(') < lastPeriodIndex)
                {
                    lastPeriodIndex = -1;
                }
                methodName = completeFunctionString.Substring(
                    (lastPeriodIndex != -1) ? (lastPeriodIndex + 1) : (0)
                    );
                namespaceAndClass = (lastPeriodIndex != -1) ? (completeFunctionString.Substring(0, lastPeriodIndex)) : ("");
                var openParenIndex = methodName.IndexOf('(');
                var closeParenIndex = methodName.LastIndexOf(')');
                methodName = methodName.Substring(0, openParenIndex);
            }
            // If there are no parentheses, then this must be a custom constructor of a class or a collection data initializer
            else
            {
                methodName = completeFunctionString;
                // TODO: Finish the logic to handle this case
                throw new NotImplementedException();
            }
            //=====
            // If the method name is the "new" keyword then get the type of the variable and set this method call as a constructor
            if(methodName == "new")
            {
                methodName = MethodActualTypeContainer.PopActualType();
                isConstructor = true;
            }
            // If this wasn't the "new()" constructor but the property containing the type of the method call isn't null, then there is a type caster and we make the methodInstanceBuilder set this type
            else if(MethodActualTypeContainer.CheckForActualMethodType(context.GetText()))
            {
                methodInstanceBuilder.SetType(MethodActualTypeContainer.PopActualType());
            }

            // Get the argumentList to visit all the arguments, if they are methodCalls then visit them and remove them from the _methodCallDataList and move them to the parameter list, if they are normal variables then add them to the parameterList normally
            var argumentListNode = GetRuleNodeInChildren("argumentList", context);
            int argumentListNodeChildrenCount = (argumentListNode is not null) ? (argumentListNode.ChildCount) : (0);
            for (int i = 0; i < argumentListNodeChildrenCount; i++)
            {
                string expressionString = Visit(argumentListNode.GetChild(i));

                // If the visited child node was a methodCall, then we remove it from the Stack of method instance builders and move it to the parameterList
                if (!String.IsNullOrEmpty(expressionString) && GetRuleNodeInChildren("methodCall", argumentListNode.GetChild(i)) != null)
                {
                    parameterList.Add(_methodInstanceBuildersStack.Pop());
                }
                // If it is a normal variable, then add it to the parameterList
                else if(!String.IsNullOrEmpty(expressionString))
                {
                    parameterList.Add(expressionString);
                }
            }
            methodInstanceBuilder.SetLinkedMethodBuilder(_currentMethodBuilder);

            // Visit the "expressionChain" to get the properties or method calls that are chained to the result of this method call
            var chainedInstance = ProcessExpressionChain(context);
            if(chainedInstance is Instance) methodInstanceBuilder.SetNormalInstanceChainedInstance((Instance) chainedInstance);
            else if(chainedInstance is MethodInstanceBuilder) methodInstanceBuilder.SetMethodCallChainedInstance((MethodInstanceBuilder)chainedInstance);

            var indexRetrievalInstance = ProcessIndexRetrieval(context);
            methodInstanceBuilder.SetIndexRetrievalInstance(indexRetrievalInstance);

            // Set the remaining data of the methodCall to be managed by the methodInstanceBuilder
            methodInstanceBuilder.SetMethodName(methodName);
            methodInstanceBuilder.SetCallerClassName(namespaceAndClass);
            methodInstanceBuilder.SetParameters(parameterList);
            if(isConstructor) methodInstanceBuilder.SetMethodKind(KindOfInstance.IsConstructor);
            _methodInstanceBuildersStack.Push(methodInstanceBuilder);

            return completeFunctionString;
        }
    }
}
