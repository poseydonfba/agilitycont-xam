using Prism.Mvvm;
using SQLite;
using System;

namespace AgilityContXam.Models
{
    public class Usuario : BindableBase
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Chave { get; set; }
        public int Ativo { get; set; }
        public int Tipo { get; set; }

        private string _foto;
        public string Foto
        {
            get => _foto;
            set => SetProperty(ref _foto, value);
        }
    }
}
