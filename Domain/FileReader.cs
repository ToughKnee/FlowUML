namespace Domain
{
    // TODO: Add a software pattern to switch between languages
    public class FileReader
    {
        private IClassAnalysis _classAnalysis;

        public FileReader() { }

        public FileReader(IClassAnalysis classAnalysis)
        {
            _classAnalysis = classAnalysis;
        }

        public void ReadAndAnalyzeCSharpFiles(string path)
        {
            string destinationPath = path;
            List<string> filesInCurrentDirectory = new();
            List<string> directoriesToGo = Directory.EnumerateDirectories(destinationPath).ToList();

            while (directoriesToGo.Count > 0)
            {
                // Getting into the next directory and inserting the next directories
                var nextDirectory = directoriesToGo[0];
                directoriesToGo.RemoveAt(0);
                var newDirectoriesToGo = Directory.EnumerateDirectories(nextDirectory).ToList();

                // If there are more directories, insert them
                if (newDirectoriesToGo.Count > 0)
                {
                    directoriesToGo.InsertRange(0, newDirectoriesToGo.AsEnumerable());
                }

                // Read the files in the current directory if any
                List<string> newFilesInCurrentDirectory = Directory.EnumerateFiles(nextDirectory).ToList();

                // Perform analysius only on the objective code files
                for (global::System.Int32 i = 0; i < newFilesInCurrentDirectory.Count; ++i)
                {
                    var file = newFilesInCurrentDirectory[i];
                    if (!file.EndsWith(".cs"))
                    {
                        newFilesInCurrentDirectory.Remove(file);
                        --i;
                    }
                    // TODO: Check if there is a need for 'newFilesInCurrentDirectory' and save the code files instead of analysing them right away
                    else
                    {
                        _classAnalysis.AnalyzeCode(File.ReadAllText(file));
                    }
                }

                //============  TODO: Decide if the analysis of the file is done in the moment, thus only using the hot files list which is constantly depleted and more effective, or put everything into a big list and do the analysis later as another batch
                // Add the new elements to the total files found list
                filesInCurrentDirectory.AddRange(newFilesInCurrentDirectory.AsEnumerable());
            }

        }
    }
}
