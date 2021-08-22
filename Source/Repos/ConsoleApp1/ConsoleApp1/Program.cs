using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfiumViewer;
using System.Drawing.Printing;
using System.Configuration;



namespace PDFPrint
{
    class Program
    {

        public static string filename;
        public static int copies = 1;

        static void Main(string[] args)
        { 

            string printer = "Canon iP110 series";
            string paperName = string.Empty;
            
            string Watch_folder = ConfigurationManager.AppSettings[0];

            filename = string.Empty;


            for (; ; )
            {
                try
                {

                    Watch_Directory(Watch_folder);

                    if (filename != string.Empty)
                    {
                        // Create the printer settings for our printer
                        var printerSettings = new PrinterSettings
                        {
                            PrinterName = printer,
                            Copies = (short)copies,
                        };

                        // Create our page settings for the paper size selected
                        var pageSettings = new PageSettings(printerSettings)
                        {
                            Margins = new Margins(0, 0, 0, 0),
                        };
                        foreach (PaperSize paperSize in printerSettings.PaperSizes)
                        {
                            if (paperSize.PaperName == paperName)
                            {
                                pageSettings.PaperSize = paperSize;
                                break;
                            }
                        }

                        // Now print the PDF document
                        using (var document = PdfDocument.Load(filename))
                        {
                            using (var printDocument = document.CreatePrintDocument())
                            {
                                printDocument.PrinterSettings = printerSettings;
                                printDocument.DefaultPageSettings = pageSettings;
                                printDocument.PrintController = new StandardPrintController();
                                printDocument.Print();
                            }
                        }

                        fileDel(Watch_folder);
                        filename = string.Empty;
                    }
                }
                catch (Exception ex)
                {


                    Error_log(ex.ToString());
                    throw ex;
                }
            }
        }

        private static bool Watch_Directory(string DirectoryPath)
        {
            bool ret = false;

            string[] filecount;

            filecount = new string[100];

            filecount = System.IO.Directory.GetFiles(DirectoryPath);

            foreach (string path in filecount)
            {
                if (path.Contains("JOB"))
                {
                    ret = Set_read_file(path);
                }
            }



            return ret;
        }

        private static bool Set_read_file(string file)
        {
            bool ret = false;
            List<string> Result = new List<string>();

            StreamReader sr = new StreamReader(file, Encoding.GetEncoding("Shift_JIS"));
            try
            {
                while (sr.Peek() >= 0)
                {
                    Result.Add(sr.ReadLine());
                }

                sr.Close();

                filename = Result[0];
                copies = Int32.Parse(Result[1]);
            }
            catch (Exception ex)
            {

            }

            ret = true;




            return ret;

        }

        private static void fileDel(string directry_path)
        {
            string[] filecount;

            filecount = new string[1000];

            filecount = System.IO.Directory.GetFiles(directry_path);

            foreach (string path in filecount)
            {
                if (path.Contains("JOB"))
                {
                    System.IO.File.Delete(path);
                }
            }
        }

        private static void  Error_log(string ex)
        {

            string Path = @"C:\Data\" + DateTime.Now.ToString("yyyy:MM:dd_HH:mm:ss") + ".txt";

            StreamWriter sw = new StreamWriter(Path);
            sw.WriteLine(ex);

            sw.Close();
        }
    }
}
