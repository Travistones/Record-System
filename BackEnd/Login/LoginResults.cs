using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Record_System.BackEnd.Login
{
    public enum LoginResults : byte
    {
        Success = 1,
        IncorrectPasswordOrUserName,
        LostConnection
    }
}
