using AgilityContXam.Interfaces;
using AgilityContXam.iOS;
using System.IO;
using Xamarin.Essentials;

[assembly: Xamarin.Forms.Dependency(typeof(FileService))]
namespace AgilityContXam.iOS
{
    public class FileService : IFileService
    {
        public void CreateFolder(string folderName)
        {
            string path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), AppInfo.Name, folderName);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }
}