namespace Domain.CodeInfo.InstanceDefinitions
{
    public class Instance : AbstractInstance
    {
        public Instance(string name, string? implementation) : base(name, new StringWrapper(implementation))
        {
        }
        public Instance(string name) : base(name)
        {
        }
        // TODO: Make data in this class which makes this able to manage the COLLECTIONS, and when this Instance is assigning the value of another Instance like "var newInst = thisInst[0]", then there is code here that addresses that scenario where we can just make the reftype.data be the type of the elements contained
    }
}