using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CommonScheduler.DAL;

namespace CommonScheduler
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // insert
            using (var db = new shedulerDBEntities())
            {
                var users = db.Set<GlobalUser>();
                users.Add(new GlobalUser { ID = 2, NAME="adam", SURNAME="Małysz", ID_CREATED=1, DATE_CREATED=DateTime.Now, DATE_MODIFIED=null, LOGIN="amalysz", PASSWORD="ppp", ROLE_ID=1 });

                db.SaveChanges();
            }
        }
    }
}
