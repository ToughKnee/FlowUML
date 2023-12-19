using Moq;
using Xunit.Abstractions;

namespace Domain.Tests.FileTests
{
    public class FileReaderTests
    {
        private static string currentDirectoryPath = "..\\..\\..\\";
        private readonly ITestOutputHelper output;
        public FileReaderTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void ReadContentsOfAFile_CSharpFile_ReadSuccesfully()
        {
            string cSharpFilePath = currentDirectoryPath + "TestFileSystem\\Domain\\TeamAggregate\\Team.cs";
            string classEntityText = File.ReadAllText(cSharpFilePath);
            output.WriteLine("Writing all text from given file...");
            output.WriteLine(classEntityText);
        }


        [Theory]
        [InlineData("TestFileSystem", 32)]
        public void TraverseFileSystem_ReadOnlyCSharpFiles_ReturnListWithContentsOfAllCSharpFiles(string path, int countOfCodeFiles)
        {
            // Arrange
            string destinationPath = currentDirectoryPath + path;
            var mockClassAnalysis = new Mock<IClassAnalysis>();
            var testedFileReader = new FileReader(mockClassAnalysis.Object);

            // Act
            testedFileReader.ReadAndAnalyzeCSharpFiles(destinationPath);

            // Assert
            mockClassAnalysis.Verify(c => c.AnalyzeCode(It.IsAny<string>()), Times.Exactly(countOfCodeFiles));
        }
    }
}