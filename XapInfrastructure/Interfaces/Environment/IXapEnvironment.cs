namespace Xap.Infrastructure.Interfaces.Environment {
    public interface IXapEnvironment {
        string EnvironmentName { get; }
        bool IsWebEnvironment { get; }
        string ConfigurationFile { get; }
        string ConfigurationPath { get; }
        string DependencyPath { get; }
        string MapFolderPath(string path);
        string GetAppConfigValue(string keyName);
    }
}
