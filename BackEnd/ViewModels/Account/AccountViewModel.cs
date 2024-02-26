using Record_System.BackEnd.ObjectPropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Record_System.BackEnd.ViewModels.Account
{
    public class AccountViewModel : BindableBase
    {
        public AccountViewModel()
        {
            Account = new();
        }

        private Record_System.BackEnd.User.Account _account;

        public Record_System.BackEnd.User.Account Account
        {
            get => _account;

            set
            {
                SetProperty(ref _account, value);
            }
        }
    }
}
