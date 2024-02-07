namespace Domain.CodeInfo.InstanceDefinitions
{
    public class Instance : AbstractInstance
    {
        public Instance(string name, string? implementation) : base(name, implementation)
        {
        }
        public Instance(string name) : base(name)
        {
        }

    }
}