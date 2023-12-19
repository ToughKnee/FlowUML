namespace Domain
{
    public interface IClassAnalysis
    {
        /// <summary>
        /// Analyzes the code to get insight and information from it,
        /// creating ClassEntities and the mapping of variables, parameters
        /// and methods with return types, basically the entrypoint
        /// </summary>
        /// <param name="rawCodeFile">Raw string containing al the contents in a code file,
        /// received from a file reader</param>
        public void AnalyzeCode(string rawCodeFile);
    }
}