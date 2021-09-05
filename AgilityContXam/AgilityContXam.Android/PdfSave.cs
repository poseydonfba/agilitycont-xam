using AgilityContXam.Droid;
using AgilityContXam.Interfaces;
using PdfSharpCore.Pdf;
using Xamarin.Essentials;

[assembly: Xamarin.Forms.Dependency(typeof(PdfSave))]
namespace AgilityContXam.Droid
{
	public class PdfSave : IPdfSave
	{
		public void Save(PdfDocument doc, string fileName)
		{
            string path = System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, AppInfo.Name, "Pdf", fileName);

            doc.Save(path);
			doc.Close();

			global::Xamarin.Forms.Application.Current.MainPage.DisplayAlert(
				title: "Sucesso",
				message: $"Seu PDF foi gerado e salvo em @ {path}",
				cancel: "OK");
		}
	}
}
