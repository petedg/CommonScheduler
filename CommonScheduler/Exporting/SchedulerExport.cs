using CommonScheduler.DAL;
using CommonScheduler.SchedulerControl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Xps.Packaging;

namespace CommonScheduler.Exporting
{
    public static class SchedulerExport
    {
        public static FixedDocument CreateXpsFile(UIElement control, double width, double height)
        {
            FixedDocument fixedDoc = new FixedDocument();
            PageContent pageContent = new PageContent();
            FixedPage fixedPage = new FixedPage();

            fixedPage.Width = width;
            fixedPage.Height = height;

            //Create first page of document
            fixedPage.Children.Add(control);
            ((System.Windows.Markup.IAddChild)pageContent).AddChild(fixedPage);
            fixedDoc.Pages.Add(pageContent);
            //Create any other required pages here

            //View the document
            //documentViewer1.Document = fixedDoc;
           
            return fixedDoc;
        }

        public static string CreateTemporaryPdfFile(serverDBEntities context, Group group, Week week)
        {
            Scheduler ccc = new Scheduler(context, group, week);
            ccc.Width = 1055;
            ccc.Height = 750;
            ((SchedulerGrid)ccc.grid.Children[0]).IsExport = true;

            FixedDocument fixedDoc = CreateXpsFile(ccc, 1055, 750);

            //string pdfPath = System.IO.Path.GetTempPath() + "scheduler_pdf_tmp.pdf";// Path to place PDF file

            string xpsPath = System.IO.Path.GetTempPath() + "scheduler_xps_tmp.xps";
            using (XpsDocument doc = new XpsDocument(xpsPath, FileAccess.Write))
                XpsDocument.CreateXpsDocumentWriter(doc).Write(fixedDoc);

            string pdfPath = System.IO.Path.GetTempPath() + "scheduler_pdf_tmp.pdf";

            using (PdfSharp.Xps.XpsModel.XpsDocument pdfXpsDoc = PdfSharp.Xps.XpsModel.XpsDocument.Open(xpsPath))
            {
                PdfSharp.Xps.XpsConverter.Convert(pdfXpsDoc, pdfPath, 0);
            }           

            //Process process = Process.Start(@"gxps.exe", "-sDEVICE=pdfwrite -sOutputFile=" + fileName + " " + xpsPath);
            //process.WaitForExit();

            //ZAPIS
            //XpsDocument xpsd = new XpsDocument(filename, FileAccess.ReadWrite);
            //System.Windows.Xps.XpsDocumentWriter xw = XpsDocument.CreateXpsDocumentWriter(xpsd);
            //xw.Write(fixedDoc);
            //xpsd.Close();

            return pdfPath;
        }

        public static PngBitmapEncoder CreatePngFile(serverDBEntities context, Group group, Week week)
        {
            Scheduler ccc = new Scheduler(context, group, week);
            ((SchedulerGrid)ccc.grid.Children[0]).PrepareImageExport_v2(1024, 768);

            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(1024, 768, 96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(ccc.grid.Children[0]);
            PngBitmapEncoder pngImage = new PngBitmapEncoder();
            pngImage.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

            return pngImage;
        }
    }
}
