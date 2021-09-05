using Prism.Mvvm;

namespace AgilityContXam.Models
{
    public class AlterarFotoBindingModel : BindableBase
    {
        private string _foto;
        public string Foto
        {
            get => _foto;
            set => SetProperty(ref _foto, value);
        }

    }
}
