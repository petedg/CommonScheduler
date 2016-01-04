using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonScheduler.DAL
{
    public partial class ExternalTeacher
    {
        private serverDBEntities context;

        public ExternalTeacher(serverDBEntities context)
        {
            this.context = context;
        }

        public ExternalTeacher GetExternalTeacherById(int externalTeacherId)
        {
            var externalTeachers = from externalTeacher in context.ExternalTeacher
                                   where externalTeacher.ID == externalTeacherId
                                   select externalTeacher;

            return externalTeachers.FirstOrDefault();
        }
    }
}
