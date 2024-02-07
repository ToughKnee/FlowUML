using Domain.CodeInfo;
using Domain.CodeInfo.InstanceDefinitions;
using Moq;
using Xunit.Abstractions;

namespace Domain.Tests
{
    public class InstancesTests
    {
        private readonly ITestOutputHelper output;
        public InstancesTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        // [Theory]
        // [InlineData("class1")]
        // public void InstancesDictionary_FindsAMatch_RenamesInstanceName(string resultName)
        // {
        //     // Arrange
        //     var instance1 = new Instance("name1");
        //     var classEntity1 = new ClassEntity("myInstance");
        //     var method1 = new Method(classEntity1, "GetName");
        //     var callsite1 = new Callsite(method1);
        //     var instancer = new MethodInstance(callsite1);
        //     // var name1 = myInstance.GetName();
        //     InstancesDictionaryManager.instance.AddAssignment(instance1, instancer);

        //     // Act

        //     // Assert

        // }



    }
}