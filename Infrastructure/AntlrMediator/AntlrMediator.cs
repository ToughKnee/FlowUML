using Domain.CodeInfo;
using Infrastructure.Builders;

namespace Infrastructure.Mediators
{
    public class AntlrMediator : IMediator
    {
        public void ReceiveClassEntityBuilder(List<AbstractBuilder<ClassEntity>> builders)
        {
            throw new NotImplementedException();
        }
        public void ReceiveMethodBuilder(List<AbstractBuilder<Method>> builders)
        {
            throw new NotImplementedException();
        }
        
        public void ReceiveMethodDeclaration(string belongingNamespace, string ownerClass, string name, string parametersType, string returnType)
        {
            throw new NotImplementedException();
        }

        public void ReceiveNamespace(string? belongingNamespace)
        {
            throw new NotImplementedException();
        }
        public void ReceiveClassName(string className)
        {
            throw new NotImplementedException();
        }
        public void ReceiveParameters(string type, string identifier)
        {
            throw new NotImplementedException();
        }
        public void ReceiveLocalVariableDeclaration(string assignee, string assigner)
        {
            throw new NotImplementedException();
        }
        public void ReceiveMethodAnalysisEnd()
        {
            throw new NotImplementedException();
        }

        public void ReceiveMethodCall(string calledClassName, string calledMethodName, List<string>? calledParameters, MethodBuilder linkedMethodBuilder)
        {
            throw new NotImplementedException();
        }

        public void ReceiveUsedNamespaces(List<string>? usedNamespaces)
        {
            throw new NotImplementedException();
        }
    }
}   
