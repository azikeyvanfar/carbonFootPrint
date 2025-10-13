namespace ContractorBackend.Common.Helpers
{
    //public static class ConfigurationHelper
    //{
    //    /// <summary>
    //    /// Build Configuration based on appsettings.json inside Target Project 
    //    /// </summary>
    //    /// <param name="targetDirectoryName">Target Project Directory Name</param>
    //    /// <returns> IConfiguration interface</returns>
    //    public static IConfiguration? BuildConfiguration(string targetDirectoryName)
    //    {
    //        var solutionDir = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
    //        var projectDirs = Directory.GetDirectories(solutionDir);

    //        var targetDirectory = projectDirs.FirstOrDefault(dir => Path.GetFileName(dir) == targetDirectoryName);
    //        var projectAppsettingsPath = Path.Combine(targetDirectory, "appsettings.json");

    //        var builder = new ConfigurationBuilder()
    //            //.SetBasePath(Directory.GetCurrentDirectory())
    //            .SetBasePath(targetDirectory)
    //            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    //            //.AddJsonFile("appsettings.development.json", optional: true, reloadOnChange: true)
    //            ;
    //        return builder.Build();
    //    }
    //}
}
