namespace AgilityContXam.Models
{
    public class LoginBindingModel : BindModelBase
    {
        private string _userName;
        private string _password;

        public string Username
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }
    }
}
