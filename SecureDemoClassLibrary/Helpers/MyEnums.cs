using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureDemoClassLibrary.Helpers
{
    public enum UserClaims
    {
        User_View = 1,
        User_Add,
        User_Edit,
        User_Delete,
        User_ChangePassword
    }

    public enum InvoiceClaims
    {
        Invoice_View = 6,
        Invoice_Add, 
        Invoice_Pending_Edit,
        Invoice_Pending_Delete,
        Invoice_Approve
    }
}
