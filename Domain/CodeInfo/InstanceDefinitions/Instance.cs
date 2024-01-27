namespace Domain.CodeInfo.InstanceDefinitions
{
    public class Instance : AbstractInstance
    {
        public Instance(string name, ClassEntity? implementation) : base(name, implementation)
        {
        }
        public Instance(string name) : base(name)
        {
        }
        public Instance() : base()
        {
        }

    }
}