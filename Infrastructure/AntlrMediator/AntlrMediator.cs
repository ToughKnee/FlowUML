using Domain.Mediators;

namespace Infrastructure.Mediators
{
    public class AntlrMediator : IMediator
    {
        public void ReceiveMethodInfo(string returnType, string name, List<string> parameters)
        {
            var test = returnType + name + parameters;
        }
    }
}
