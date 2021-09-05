using AgilityContXam.Interfaces;
using AgilityContXam.iOS;
using PdfSharpCore.Pdf;
using Xamarin.Essentials;

[assembly: Xamarin.Forms.Dependency(typeof(PdfSave))]
namespace AgilityContXam.iOS
{
	public class PdfSave : IPdfSave
	{
		public void Save(PdfDocument doc, string fileName)
		{
            string path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), AppInfo.Name, "Pdf", fileName);

            doc.Save(System.IO.Path.GetTempPath() + fileName);

			global::Xamarin.Forms.Application.Current.MainPage.DisplayAlert(
				title: "Sucesso",
				message: $"Seu PDF foi gerado e salvo em @ {path}",
				cancel: "OK");
		}
	}
}
