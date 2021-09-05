using PdfSharpCore.Pdf;

namespace AgilityContXam.Interfaces
{
	public interface IPdfSave
	{
		void Save(PdfDocument doc, string fileName);
	}
}
