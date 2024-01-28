using Domain.CodeInfo;
using Infrastructure.Builders;

namespace Infrastructure.Mediators
{
    public class AntlrMediator : IMediator
    {
        public void ReceiveClassEntityBuilder(List<AbstractBuilder<ClassEntity>> builder)
        {
            throw new NotImplementedException();
        }

        public void ReceiveMethodBuilder(List<AbstractBuilder<Method>> builder)
        {
            throw new NotImplementedException();
        }
    }
}
