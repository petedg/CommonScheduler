using CommonScheduler.DAL;
using CommonScheduler.SchedulerControl;
using iTextSharp.text;
using iTextSharp.text.pdf;
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

        public static string CreatePdfFile(serverDBEntities context, string fileName, object group, Week week)
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
            
            string outputPdfPath = fileName;
            if (outputPdfPath == null)
            {
                outputPdfPath = System.IO.Path.GetTempPath() + "scheduler_pdf_tmp_desc.pdf";
            }

            using (PdfSharp.Xps.XpsModel.XpsDocument pdfXpsDoc = PdfSharp.Xps.XpsModel.XpsDocument.Open(xpsPath))
            {
                PdfSharp.Xps.XpsConverter.Convert(pdfXpsDoc, pdfPath, 0);
            }

            AppendDescriptionToPdfFile(context, pdfPath, outputPdfPath, group, week);

            //Process process = Process.Start(@"gxps.exe", "-sDEVICE=pdfwrite -sOutputFile=" + fileName + " " + xpsPath);
            //process.WaitForExit();

            //ZAPIS
            //XpsDocument xpsd = new XpsDocument(filename, FileAccess.ReadWrite);
            //System.Windows.Xps.XpsDocumentWriter xw = XpsDocument.CreateXpsDocumentWriter(xpsd);
            //xw.Write(fixedDoc);
            //xpsd.Close();

            return outputPdfPath;
        }

        private static void AppendDescriptionToPdfFile(serverDBEntities context, string pdfPath, string outputPdfPath, object group, Week week)
        {
            // Create output PDF
            Document document = new Document(PageSize.A4.Rotate(), 72, 72, 72, 72);            
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(outputPdfPath, FileMode.Create));
            document.Open();
            PdfContentByte cb = writer.DirectContent;

            // Load existing PDF
            PdfReader reader = new PdfReader(new FileStream(pdfPath, FileMode.Open));
            PdfImportedPage page = writer.GetImportedPage(reader, 1);

            // Copy first page of existing PDF into output PDF
            document.NewPage();
            float scaleX = 0.9f;
            float scaleY = 0.8f;

            float translationX = 64.9375f;
            float translationY = 72f;      
            cb.AddTemplate(page, scaleX, 0, 0, scaleY, translationX, translationY);            

            // Add your new data / text here
            document.SetPageSize(PageSize.A4);
            document.NewPage();
            
            AppendScheduleDescriptionToDocument(document, context, group, week);

            document.Close();
        }

        private static void AppendScheduleDescriptionToDocument(Document document, serverDBEntities context, object group, Week week)
        {
            Classes classesBehavior = new Classes(context);
            Room roomBehavior = new Room(context);
            Teacher teacherBehavior = new Teacher(context);
            ExternalTeacher externalTeacherBehavior = new ExternalTeacher(context);
            SpecialLocation specialLocationBehavior = new SpecialLocation(context);
            DictionaryValue dictionaryValueBehavior = new DictionaryValue(context);
            Location locationBehavior = new Location(context);

            List<Classes> classesList = null;

            if (group.GetType() == typeof(Group) || group.GetType().BaseType == typeof(Group))
            {
                classesList = classesBehavior.GetListForGroup((Group)group, week);
            }
            else if(group.GetType() == typeof(Subgroup) || group.GetType().BaseType == typeof(Subgroup))
            {
                Subgroup subgroup = (Subgroup)group;

                if (subgroup.SUBGROUP_ID == null)
                {
                    classesList = classesBehavior.GetListForSubgroup_S1(subgroup, week);
                }
                else
                {
                    classesList = classesBehavior.GetListForSubgroup_S2(subgroup, week);
                }
            }

            BaseFont bfTimesPL = BaseFont.CreateFont(System.AppDomain.CurrentDomain.BaseDirectory + "Resources\\Fonts\\times.ttf", BaseFont.CP1250, BaseFont.EMBEDDED);
            Font boldHeaderFont = new Font(bfTimesPL, 20f, Font.BOLD);
            Font unitHeaderFont = new Font(bfTimesPL, 14f, Font.BOLD);

            iTextSharp.text.Paragraph p = new iTextSharp.text.Paragraph();
            p.Font = new Font(bfTimesPL, 10f, Font.NORMAL);

            // przedmioty  

            p.AddAll(new List<Chunk> { new Chunk("LEGENDA", boldHeaderFont), Chunk.NEWLINE, Chunk.NEWLINE, new Chunk("Przedmioty:", unitHeaderFont), Chunk.NEWLINE });                       

            foreach (string s in classesList.GroupBy(x => x.SUBJECT_SHORT).Select(x => x.First().SUBJECT_SHORT + " - " + x.First().SUBJECT_NAME))
            {
                p.Add(new Phrase(s));
                p.Add(Chunk.NEWLINE);
            }

            // ---

            // pracownicy 

            p.AddAll(new List<Chunk> { Chunk.NEWLINE, new Chunk("Pracownicy:", unitHeaderFont), Chunk.NEWLINE });            

            foreach (Classes c in classesList.Where(x => x.TEACHER_ID != 3).GroupBy(x => x.TEACHER_ID).Select(x => x.First()))
            {
                Teacher teacher = teacherBehavior.GetTeacherByID(c.TEACHER_ID);

                p.Add(new Phrase(teacher.NAME_SHORT + " - " + dictionaryValueBehavior.GetValue("Stopnie naukowe nauczycieli", teacher.DEGREE_DV_ID) + " " + teacher.NAME + " " + teacher.SURNAME));
                p.Add(Chunk.NEWLINE);                             
            }

            foreach (Classes c in classesList.Where(x => x.TEACHER_ID == 3))
            {
                ExternalTeacher teacher = externalTeacherBehavior.GetExternalTeacherById((int)c.EXTERNALTEACHER_ID);

                p.Add(new Phrase(teacher.NAME_SHORT + " - " + teacher.NAME + " " + teacher.SURNAME));
                p.Add(Chunk.NEWLINE);                 
            }

            // ---

            // sale       

            p.AddAll(new List<Chunk> { Chunk.NEWLINE, new Chunk("Lokalizacje:", unitHeaderFont), Chunk.NEWLINE });

            foreach (Classes c in classesList.Where(x => x.Room_ID != 4).GroupBy(x => x.Room_ID).Select(x => x.First()))
            {
                Room room = roomBehavior.GetRoomById(c.Room_ID);
                Location location = locationBehavior.GetLocationById(room.Location_ID);

                p.Add(new Phrase(room.NUMBER_SHORT + " - Sala " + room.NUMBER + " (" + location.NAME + ", " + location.STREET + " " + location.STREET_NUMBER + " " + location.CITY + ")"));
                p.Add(Chunk.NEWLINE);
            }

            foreach (Classes c in classesList.Where(x => x.Room_ID == 4))
            {
                SpecialLocation location = specialLocationBehavior.GetSpecialLocationById((int)c.SPECIALLOCATION_ID);

                p.Add(new Phrase(location.NAME_SHORT + " - " + location.NAME + " (" + location.STREET + " " + location.STREET_NUMBER + " " + location.CITY + ")"));
                p.Add(Chunk.NEWLINE);
            }

            // ---                    

            document.Add(p);         
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
