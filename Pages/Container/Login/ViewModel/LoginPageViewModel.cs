using Record_System.BackEnd.ObjectPropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Record_System.Pages.Container.Login.ViewModel
{
    public class LoginPageViewModel : BindableBase
    {
        public LoginPageViewModel()
        {
            IsLoginIn = false;
        }

        private bool _isLoginIn;

        public bool IsLoginIn
        {
            get => _isLoginIn;

            set => SetProperty(ref _isLoginIn, value);
        }
    }
}
