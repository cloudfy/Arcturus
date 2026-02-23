using Arcturus.DevHost.Hosting.Abstracts;
using System.Xml.Linq;

namespace Arcturus.DevHost.Hosting.Internals;

internal static class IProjectMetadataExtensions
{
    internal static string GetAssemblyName(this IProjectMetadata projectMetadata)
    {
        try
        {
            if (!File.Exists(projectMetadata.ProjectPath))
            {
                return projectMetadata.ProjectName;
            }

            var doc = XDocument.Load(projectMetadata.ProjectPath);

            // Look for <AssemblyName> element in the project file
            var assemblyName = doc.Descendants("AssemblyName").FirstOrDefault()?.Value;

            // If AssemblyName is not specified, use the project name (without extension)
            return assemblyName ?? Path.GetFileNameWithoutExtension(projectMetadata.ProjectPath);
        }
        catch
        {
            // Fallback to project name if anything goes wrong
            return projectMetadata.ProjectName;
        }
    }
}
