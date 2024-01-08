namespace Domain
{
    public class ClassAnalysis : IClassAnalysis
    {
        // Read specific things from the file
        // First, getting the name of the classes present in the file and all the methods with the return types
        // Add those methods to the InstancesMap 
        // SECONDARY: CREATE a ClassEntity out of each class
        public void AnalyzeCode(string rawCodeFile)
        {
            // Create a ClassEntity class out of a class definition

            // Replace any 'this' with the name of the class, cover the exception when the 'this' is used in a C# Extension Method
            //rawCodeFile.Replace("this", nameOf)
        }
    }
}