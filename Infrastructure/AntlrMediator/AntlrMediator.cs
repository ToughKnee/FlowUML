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
    }
}   
