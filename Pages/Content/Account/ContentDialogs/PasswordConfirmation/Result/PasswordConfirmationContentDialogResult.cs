using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Record_System.Pages.Content.Account.ContentDialogs.PasswordConfirmation.Result
{
    public enum PasswordConfirmationContentDialogResults : byte
    {
        Incorrect = 0,
        Cancel,
        Correct
    }
}
