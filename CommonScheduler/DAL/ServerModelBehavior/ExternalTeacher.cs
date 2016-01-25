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

        public ExternalTeacher GetExternalTeacherByIdWithLocalSearch(int externalTeacherId)
        {
            var externalTeachersLocal = from externalTeacher in context.ExternalTeacher.Local
                                        where externalTeacher.ID == externalTeacherId
                                        select externalTeacher;

            ExternalTeacher t = externalTeachersLocal.FirstOrDefault();

            if (t != null)
            {
                return t;
            }

            var externalTeachers = from externalTeacher in context.ExternalTeacher
                                   where externalTeacher.ID == externalTeacherId
                                   select externalTeacher;

            return externalTeachers.FirstOrDefault();
        }

        public void DeleteExternalTeacherForClasses(Classes classes)
        {
            var externalTeachers = from externalTeacher in context.ExternalTeacher
                                   where externalTeacher.ID == classes.EXTERNALTEACHER_ID
                                   select externalTeacher;

            foreach (ExternalTeacher ex in externalTeachers)
            {
                context.ExternalTeacher.Remove(ex);
            }
        }
    }
}
