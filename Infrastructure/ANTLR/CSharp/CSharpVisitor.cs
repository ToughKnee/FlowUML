using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Domain.CodeInfo;
using Domain.CodeInfo.InstanceDefinitions;
using Infrastructure.ANTLR.CSharp;
using System;
using static Antlr4.Runtime.Atn.SemanticContext;
using static Infrastructure.ANTLR.CSharp.CSharpGrammarParser;

namespace Infrastructure.Antlr
{
    public class CSharpVisitor : CSharpGrammarBaseVisitor<string?>
    {
        /// <summary>
        /// This is used to separate multiple children to be returned, as an example, 
        /// the method VisitProperty should return the type and the name of the property
        /// and it should look like this: "string-name", if the property was something like
        /// "public string name"
        /// </summary>
        private readonly string separator = "-";
        /// <summary>
        /// This will work like a bunch of waiting lines, where a key is a line, specifically the line for 
        /// "X" thing, which actually is the alias of variables in code, the ones like "Jim = manager.GetName()", 
        /// where "Jim" would be the key and the request to know what is the Type of Jim(This also applies to "manager")
        /// The value is the class that wants to know about the Type of this variable from code, in order
        /// to complete the definition and Signature of the method they hold
        /// </summary>
        /// TODO: DELETEME
        public Dictionary<string, MethodInstance> foundInstances { get; private set; } = new Dictionary<string, MethodInstance>();
        public List<(string, string)> properties = new List<(string, string)>();
        public List<(string, string)> methods = new List<(string, string)>();

        // The reason the variable 'childrenOffset' exists is because if there are attributes(the "[something]"
        // in the property, then this is added as another child and we must rearrange the children according to this

        public int GetRuleIndexInChildren(string ruleName, IList<IParseTree> contextChildren)
        {
            string suffix = "Context";
            string childRuleName = "";

            // Iterates through all the children and compares if the rule of the child is equal to the ruleName
            for (int j = 0; j < contextChildren.Count; j++)
            {
                childRuleName = contextChildren[j].GetType().ToString();
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
            int childrenOffset = (context.children.Count > 8) ? (1) : (0);
            int parameterIndex = GetRuleIndexInChildren("parameterList", context.children);
            int methodBodyIndex = GetRuleIndexInChildren("methodBodyContent", context.children);
            // Getting the parameters and storing them into the InstancesDictionary
            for (int j = 0; j < context.children[parameterIndex].ChildCount; j += 2)
            {
                string[] parameter = Visit(context.children[parameterIndex].GetChild(j)).Split("-");
                // TODO: Refactor
                InstancesDictionaryManager.instance.AddAssignation(new Instance(parameter[0]), new Instance(parameter[1]));
            }
            // Getting the variables and storing them into the InstancesDictionary
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
            int childrenOffset = (context.GetChild(3).ChildCount > 1) ? (1) : (0);
            // Gets the Assignee and the Assigner and returns them
            return context.GetChild(1).GetText() + separator + context.GetChild(3).GetChild(childrenOffset).GetText();
        }
    }
}
