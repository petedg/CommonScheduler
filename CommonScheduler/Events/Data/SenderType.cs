using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonScheduler.Events.Data
{
    public enum SenderType
    {
        SUPER_ADMIN_MANAGEMENT_BUTTON,
        ADMIN_MANAGEMENT_BUTTON,
        SEMESTER_MANAGEMENT_BUTTON,
        DEPARTMENT_MANAGEMENT_BUTTON,
        LOCATION_MANAGEMENT_BUTTON,
        MAJOR_MANAGEMENT_BUTTON,

        SAVE_BUTTON,
        CANCEL_BUTTON,
        EDIT_ROLE_BUTTON,
        EDIT_HOLIDAYS_BUTTON,

        CLOSE_CONTENT,
        LOGOUT
    }
}
