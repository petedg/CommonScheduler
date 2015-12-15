using CommonScheduler.MenuComponents.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace CommonScheduler.Authorization
{
    public class ContentManager
    {
        public ContentType CurrentContentType { get; set; }

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
