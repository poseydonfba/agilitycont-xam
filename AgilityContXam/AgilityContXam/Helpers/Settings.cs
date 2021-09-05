using AgilityContXam.Models;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;

namespace AgilityContXam.Helpers
{
    public class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                if (CrossSettings.IsSupported)
                    return CrossSettings.Current;

                return null; // or your custom implementation 
            }
        }

        public static string Username
        {
            get => AppSettings.GetValueOrDefault(nameof(Username), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(Username), value);
        }

        public static Guid Id
        {
            get => AppSettings.GetValueOrDefault(nameof(Id), Guid.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(Id), value);
        }

        public static string Nome
        {
            get => AppSettings.GetValueOrDefault(nameof(Nome), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(Nome), value);
        }

        public static string Email
        {
            get => AppSettings.GetValueOrDefault(nameof(Email), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(Email), value);
        }

        public static string Chave
        {
            get => AppSettings.GetValueOrDefault(nameof(Chave), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(Chave), value);
        }

        public static int Ativo
        {
            get => AppSettings.GetValueOrDefault(nameof(Ativo), 0);
            set => AppSettings.AddOrUpdateValue(nameof(Ativo), value);
        }

        public static int Tipo
        {
            get => AppSettings.GetValueOrDefault(nameof(Tipo), 0);
            set => AppSettings.AddOrUpdateValue(nameof(Tipo), value);
        }

        public static string Foto
        {
            get => AppSettings.GetValueOrDefault(nameof(Foto), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(Foto), value);
        }

        public static string AccessToken
        {
            get => AppSettings.GetValueOrDefault(nameof(AccessToken), string.Empty);
            set => AppSettings.AddOrUpdateValue(nameof(AccessToken), value);
        }

        public static DateTime AccessTokenExpirationDate
        {
            get => AppSettings.GetValueOrDefault(nameof(AccessTokenExpirationDate), DateTime.UtcNow);
            set => AppSettings.AddOrUpdateValue(nameof(AccessTokenExpirationDate), value);
        }

        public static Usuario Usuario
        {
            get
            {
                return new Usuario
                {
                    Id = Id,
                    Nome = Nome,
                    Email = Email,
                    Chave = Chave,
                    Ativo = Ativo,
                    Tipo = Tipo,
                    Foto = Foto
                };
            }
        }
    }
}
