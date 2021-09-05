using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AgilityContXam.Models
{
    public class BindModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetProperty<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(property, value))
                return;

            property = value;
            OnPropertyChanged(propertyName);
        }
    }
}
