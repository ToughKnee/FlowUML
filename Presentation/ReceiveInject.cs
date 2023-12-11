using Application.UseCases;

namespace Presentation
{
    public class ReceiveInject
    {
        IinjectTest InjectTest { get; set; }

        public ReceiveInject(IinjectTest injectTest)
        {
            InjectTest = injectTest;
        }
        public int GetNumber()
        {
            return InjectTest.GetInt();
        }
    }
}
