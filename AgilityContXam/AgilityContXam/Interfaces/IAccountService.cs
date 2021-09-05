using AgilityContXam.Models;
using Fusillade;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgilityContXam.Interfaces
{
    public interface IAccountService
    {
        Task<string> LoginAsync(string userName, string password);

        void LogoutAsync();

        Task<Usuario> GetAccount(Priority priority, bool forceRefresh = false);

        Task<List<MainMenu>> GetMenus();

        Task AlterarFoto(AlterarFotoBindingModel foto);
    }
}
