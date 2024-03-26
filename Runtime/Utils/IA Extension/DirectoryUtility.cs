namespace IA.Utils
{
    public static class DirectoryUtility
    {
        public static void CreateDirectoryIfNotExists(string _assetPath)
        {
            // Check if the directory exists, create if not
            string directoryPath = System.IO.Path.GetDirectoryName(_assetPath);

            if (!System.IO.Directory.Exists(directoryPath))
            {
                System.IO.Directory.CreateDirectory(directoryPath);
            }
        }
    }
}
