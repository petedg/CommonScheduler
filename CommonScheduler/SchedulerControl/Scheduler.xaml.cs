using CommonScheduler.ContentComponents.Admin.Windows;
using CommonScheduler.DAL;
using CommonScheduler.Events.CustomEventArgs;
using CommonScheduler.Exporting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Xps.Packaging;

namespace CommonScheduler.SchedulerControl
{
    /// <summary>
    /// Logika interakcji dla klasy Scheduler.xaml
    /// </summary>
    public partial class Scheduler : UserControl
    {
        private serverDBEntities context;
        private Classes classesBehavior;

        public object Group { get; set; }
        public Week Week { get; set; }
        private SchedulerGroupType schedulerGroupType;
        private List<Classes> classesList;

        private Rectangle rect = new Rectangle { Fill = Brushes.LightGray };

        public Scheduler(serverDBEntities context, object group, Week week)
        {
            InitializeComponent();
    
            this.context = context;
            this.classesBehavior = new Classes(context);

            this.Group = group;
            this.Week = week;

            int groupId = 0;

            if (group.GetType() == typeof(Group) || group.GetType().BaseType == typeof(Group))
            {
                groupId = ((Group)group).ID;
                classesList = classesBehavior.GetListForGroup((Group) group, Week);
                schedulerGroupType = SchedulerGroupType.GROUP;
            }
            else if (group.GetType() == typeof(Subgroup) || group.GetType().BaseType == typeof(Subgroup))
            {
                Subgroup subgroup = (Subgroup)group;
                if (subgroup.SUBGROUP_ID == null)
                {
                    classesList = classesBehavior.GetListForSubgroup_S1(subgroup, Week);
                    schedulerGroupType = SchedulerGroupType.SUBGROUP_S1;
                }
                else
                {
                    classesList = classesBehavior.GetListForSubgroup_S2(subgroup, Week);
                    schedulerGroupType = SchedulerGroupType.SUBGROUP_S2;
                }

                groupId = ((Subgroup)group).ID;
            }
                
            grid.Children.Add(new SchedulerGrid(context, schedulerGroupType, groupId, classesList));
        }

        public PngBitmapEncoder CreateImgFile()
        {
            ((SchedulerGrid)grid.Children[0]).PrepareImageExport(1039, 768);

            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(1024, 768, 96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(grid.Children[0]);
            PngBitmapEncoder pngImage = new PngBitmapEncoder();
            pngImage.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

            UIElement temp = grid.Children[0];
            grid.Children.Clear();
            grid.Children.Add(temp);
            return pngImage;
        }

        public void CreatePdfFile(string fileName)
        {
            Scheduler ccc = new Scheduler(context, Group, Week);
            ccc.Width = 1055;
            ccc.Height = 750;
            ((SchedulerGrid)ccc.grid.Children[0]).IsExport = true;

            FixedDocument fixedDoc = SchedulerExport.CreateXpsFile(ccc, 1055, 750);

            //string pdfPath = System.IO.Path.GetTempPath() + "scheduler_pdf_tmp.pdf";// Path to place PDF file

            string xpsPath = System.IO.Path.GetTempPath() + "scheduler_xps_tmp.xps";            
            using(XpsDocument doc = new XpsDocument(xpsPath, FileAccess.Write))
              XpsDocument.CreateXpsDocumentWriter(doc).Write(fixedDoc);

            fileName = fileName.Replace(" ", "_");

            using (PdfSharp.Xps.XpsModel.XpsDocument pdfXpsDoc = PdfSharp.Xps.XpsModel.XpsDocument.Open(xpsPath))
            {
                PdfSharp.Xps.XpsConverter.Convert(pdfXpsDoc, fileName, 0);
            } 

            //Process process = Process.Start(@"gxps.exe", "-sDEVICE=pdfwrite -sOutputFile=" + fileName + " " + xpsPath);
            //process.WaitForExit();                  

            //ZAPIS
            //XpsDocument xpsd = new XpsDocument(filename, FileAccess.ReadWrite);
            //System.Windows.Xps.XpsDocumentWriter xw = XpsDocument.CreateXpsDocumentWriter(xpsd);
            //xw.Write(fixedDoc);
            //xpsd.Close();
        }
    }
}
