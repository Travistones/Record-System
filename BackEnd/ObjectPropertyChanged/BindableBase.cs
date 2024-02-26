using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Record_System.BackEnd.ObjectPropertyChanged
{
    public class BindableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T origionalValue, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (Equals(origionalValue, newValue))
                return false;

            origionalValue = newValue;

            RaisePropertyChanged(propertyName);

            return true;
        }
    }
}
