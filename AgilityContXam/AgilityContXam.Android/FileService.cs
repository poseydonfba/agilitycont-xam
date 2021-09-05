using AgilityContXam.Droid;
using AgilityContXam.Interfaces;
using System.IO;
using Xamarin.Essentials;

[assembly: Xamarin.Forms.Dependency(typeof(FileService))]
namespace AgilityContXam.Droid
{
    public class FileService : IFileService
    {
        public void CreateFolder(string folderName)
        {
            string path = System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, AppInfo.Name, folderName);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }
}