using CommonScheduler.ContentComponents.GlobalAdmin.Controls;
using CommonScheduler.MenuComponents.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CommonScheduler.Authorization
{
    public class ContentManager
    {
        public ContentType CurrentContentType { get; set; }

        public object PreviousTopContent { get; set; }
        public object PreviousMainContent { get; set; }

        public UIElement getLeftMenuContent()
        {
            if (CurrentContentType == ContentType.MENU)
            {
                return new MenuGridControl();
            }
            else if (CurrentContentType == ContentType.SUPER_ADMIN_MANAGEMENT || CurrentContentType == ContentType.ADMIN_MANAGEMENT)
            {
                return new LeftMenuGridControl();
            }
            else if (CurrentContentType == ContentType.DEFAULT)
            {
                return new LeftMenuGridControl();
            }

            return new Rectangle { Fill = Brushes.LightGray };
        }

        public UIElement getTopMenuContent()
        {
            if (CurrentContentType == ContentType.MENU)
            {
                return new Rectangle { Fill = Brushes.LightGray };
            }
            else if (CurrentContentType == ContentType.SUPER_ADMIN_MANAGEMENT || CurrentContentType == ContentType.ADMIN_MANAGEMENT)
            {
                return new TopMenuGridControl();
            }
            else if (CurrentContentType == ContentType.DEFAULT)
            {
                return new Rectangle { Fill = Brushes.LightGray };
            }


            return new Rectangle { Fill = Brushes.LightGray };
        }

        public UIElement getMainContent()
        {
            if (CurrentContentType == ContentType.MENU)
            {
                return new Rectangle { Fill = Brushes.LightGray };
            }
            else if (CurrentContentType == ContentType.SUPER_ADMIN_MANAGEMENT)
            {
                return new SuperAdminDataGridControl();
            }
            else if (CurrentContentType == ContentType.ADMIN_MANAGEMENT)
            {
                return new AdminDataGridControl();
            }
            else if (CurrentContentType == ContentType.DEFAULT)
            {
                return new Rectangle { Fill = Brushes.LightGray };
            }

            return new Rectangle { Fill = Brushes.LightGray };
        }

        private static ContentManager instance;
        private ContentManager() { }

        public static ContentManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ContentManager();                    
                }
                return instance;
            }
        }
    }
}
