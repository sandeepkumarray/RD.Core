using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Drawing;
using Word = Microsoft.Office.Interop.Word;
using System.Reflection;
using System.Globalization;
using iTextSharp.text.html.simpleparser;
using iTextSharp.tool.xml;

namespace RDCore
{
    /// <summary>
    /// iTextHelper Class uses for PDF operations.
    /// </summary>
    public static class iTextPDFHelper
    {
        static iTextPDFHelper()
        {
            documents = new List<PdfReader>();
            elementList = new List<IElement>();
        }

        #region Properties
        private static BaseFont baseFont;
        private static bool enablePagination = false;
        private static readonly List<PdfReader> documents;
        private static int totalPages;
        private static readonly List<IElement> elementList;

        public static List<IElement> ElementList
        {
            get { return iTextPDFHelper.elementList; }
        }
        public static BaseFont BaseFont
        {
            get { return baseFont; }
            set { baseFont = value; }
        }
        public static bool EnablePagination
        {
            get { return enablePagination; }
            set
            {
                enablePagination = value;
                if (value && baseFont == null)
                    baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            }
        }
        public static List<PdfReader> Documents
        {
            get { return documents; }
        }
        #endregion

        #region Public Methods
        #region
        /// <summary>
        /// Adds a PdfReader to the List of Documents
        /// </summary>
        /// <param name="filePath">File Path</param>
        public static void AddDocument(string filePath)
        {
            documents.Add(new PdfReader(filePath));
        }
        #endregion

        #region
        /// <summary>
        /// Adds a PdfReader to the List of Documents
        /// </summary>
        /// <param name="pdfStream"></param>
        public static void AddDocument(Stream pdfStream)
        {
            documents.Add(new PdfReader(pdfStream));
        }
        #endregion

        #region
        /// <summary>
        /// Adds a PdfReader to the List of Documents
        /// </summary>
        /// <param name="pdfContents">PDF file in bytes</param>
        public static void AddDocument(byte[] pdfContents)
        {
            documents.Add(new PdfReader(pdfContents));
        }
        #endregion

        #region
        /// <summary>
        /// Adds a PdfReader to the List of Documents
        /// </summary>
        /// <param name="pdfDocument">PdfReader</param>
        public static void AddDocument(PdfReader pdfDocument)
        {
            documents.Add(pdfDocument);
        }
        #endregion

        #region
        /// <summary>
        /// Merges the files in the Documents List into a File
        /// </summary>
        /// <param name="outputFilename">File Path</param>
        public static void Merge(string outputFilename)
        {
            Merge(new FileStream(outputFilename, FileMode.Create));
        }
        #endregion

        #region
        /// <summary>
        /// Merges the files in the Documents List into a Stream
        /// </summary>
        /// <param name="outputStream">Stream</param>
        public static void Merge(Stream outputStream)
        {
            if (outputStream == null || !outputStream.CanWrite)
                throw new Exception("OutputStream es nulo o no se puede escribir en éste.");

            Document newDocument = null;
            try
            {
                newDocument = new Document();
                PdfWriter pdfWriter = PdfWriter.GetInstance(newDocument, outputStream);

                newDocument.Open();
                PdfContentByte pdfContentByte = pdfWriter.DirectContent;

                if (EnablePagination)
                    documents.ForEach(delegate(PdfReader doc)
                    {
                        totalPages += doc.NumberOfPages;
                    });

                int currentPage = 1;
                foreach (PdfReader pdfReader in documents)
                {
                    for (int page = 1; page <= pdfReader.NumberOfPages; page++)
                    {
                        newDocument.NewPage();
                        PdfImportedPage importedPage = pdfWriter.GetImportedPage(pdfReader, page);
                        pdfContentByte.AddTemplate(importedPage, 0, 0);

                        if (EnablePagination)
                        {
                            pdfContentByte.BeginText();
                            pdfContentByte.SetFontAndSize(baseFont, 9);
                            pdfContentByte.ShowTextAligned(PdfContentByte.ALIGN_CENTER,
                                string.Format("{0} de {1}", currentPage++, totalPages), 520, 5, 0);
                            pdfContentByte.EndText();
                        }
                    }
                }
            }
            finally
            {
                outputStream.Flush();
                if (newDocument != null)
                    newDocument.Close();
                outputStream.Close();
            }
        }
        #endregion

        #region
        /// <summary>
        /// To set the value of the PDF Form field 
        /// </summary>
        /// <param name="pdfFormFields">AcroFields</param>
        /// <param name="ctrlname">string</param>
        /// <param name="ctrlvalue">string</param>
        /// <param name="ischeckbox">bool</param>
        /// <param name="istextbox">bool</param>
        /// <param name="minLength">int</param>
        /// <param name="maxLength">int</param>
        public static void GenericClass(AcroFields pdfFormFields, string ctrlname, string ctrlvalue, bool ischeckbox, bool istextbox, int minLength, int maxLength)
        {
            if (ischeckbox == true)
            {
                pdfFormFields.SetField(ctrlname, ctrlvalue);
            }
            if (istextbox == true)
            {
                pdfFormFields.SetField(ctrlname, ctrlvalue);
            }
            else
            {
                if (ctrlvalue.Length != 0)
                {
                    int j = 0;
                    for (int i = minLength; i <= maxLength; i++)
                    {
                        if (j <= ctrlvalue.Length - 1 && pdfFormFields.GetField(i.ToString()) != null)
                        {
                            pdfFormFields.SetField(i.ToString(), ctrlvalue[j].ToString());

                            j++;
                        }
                    }
                }
            }

        }
        #endregion

        #region
        /// <summary>
        /// Setting Meta Information or PDF Document Properties
        /// </summary>
        /// <param name="outputDoc">Document</param>
        /// <param name="Title">string</param>
        /// <param name="Subject">string</param>
        /// <param name="Keywords">string</param>
        /// <param name="Creator">string</param>
        /// <param name="Author">string</param>
        /// <param name="HeaderName">string</param>
        /// <param name="HeaderContent">string</param>
        /// <returns></returns>
        public static Document SetMetaData(Document outputDoc, string Title, string Subject, string Keywords, string Creator, string Author, string HeaderName, string HeaderContent)
        {
            if (outputDoc != null)
            {
                outputDoc.AddTitle(Title);
                outputDoc.AddSubject(Subject);
                outputDoc.AddKeywords(Keywords);
                outputDoc.AddCreator(Creator);
                outputDoc.AddAuthor(Author);
                outputDoc.AddHeader(HeaderName, HeaderContent);
            }
            return outputDoc;
        }
        #endregion

        #region
        /// <summary>
        /// Adds an Paragraph Element to pdf document.
        /// </summary>
        /// <param name="outputDoc"></param>
        /// <param name="pintParagraph"></param>
        /// <returns></returns>
        public static Document AddParagraph(Document outputDoc, IElement pintIElement)
        {
            if (outputDoc != null)
            {
                // NOTE: When we want to insert text, then we've to do it through creating paragraph
                outputDoc.Add(pintIElement);
            }
            return outputDoc;
        }
        #endregion

        #region
        /// <summary>
        /// Set Background color of pdf Document.
        /// </summary>
        /// <param name="outputDoc"></param>
        /// <param name="pintColor"></param>
        /// <returns></returns>
        public static Document SetBackgroundColor(Document outputDoc, Color pintColor)
        {
            if (outputDoc != null)
            {
                iTextSharp.text.Rectangle rec = new iTextSharp.text.Rectangle(outputDoc.PageSize);
                rec.BackgroundColor = new BaseColor(pintColor);
                //outputDoc.SetPageSize(rec);
                outputDoc = null;
                outputDoc = new Document(rec);
            }
            return outputDoc;
        }
        #endregion

        #region
        /// <summary>
        /// Converts MS Doc type to other formats.
        /// </summary>
        /// <param name="WordDocFilePath"></param>
        /// <param name="OutPutFileFormat"></param>
        public static void ConvertWord(string WordDocFilePath, WordFormat OutPutFileFormat)
        {
            Word.Application WordApp = new Word.Application();
            Object FilePath = WordDocFilePath;
            Object ReadOnly = true;
            Object Missing = Type.Missing; //This will be passed when ever we don’t want to pass value

            //Open document from file
            Word.Document WordDoc = WordApp.Documents.Open(ref FilePath, ref Missing, ref ReadOnly, ref Missing, ref Missing,
                ref Missing, ref Missing, ref Missing, ref Missing, ref Missing, ref Missing, ref Missing, ref Missing,
                ref Missing, ref Missing, ref Missing);

            WordDoc.SaveAs2(FilePath + DictionaryLib.GetExtention(OutPutFileFormat), (Word.WdSaveFormat)OutPutFileFormat, ref Missing, ref Missing, ref Missing, ref Missing,
                ref Missing, ref Missing, ref Missing, ref Missing, ref Missing, ref Missing, ref Missing, ref Missing, ref Missing,
                ref Missing, ref Missing);

            Object SaveChanges = false;
            WordDoc.Close(ref SaveChanges, ref Missing, ref Missing);

            //Quit the word application
            WordApp.Quit(ref Missing, ref Missing, ref Missing);
        }
        #endregion

        #region
        /// <summary>
        /// Converts Pdf to MS Doc type.
        /// </summary>
        /// <param name="FileFullPath"></param>
        /// <param name="OutPutFileFullPath"></param>
        public static void PDFToWord(string FileFullPath, string OutPutFileFullPath)
        {
            iTextSharp.text.pdf.parser.ITextExtractionStrategy strategy = new iTextSharp.text.pdf.parser.LocationTextExtractionStrategy();
            PdfReader pdfReader = new PdfReader(FileFullPath);
            for (int page = 1; page <= pdfReader.NumberOfPages; page++)
            {
                string Pagestring;

                Pagestring = iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(pdfReader, page, strategy);

                FileStream richTextBox1fs = new FileStream(OutPutFileFullPath, FileMode.Create, FileAccess.Write, FileShare.None);
                StreamWriter sw = new StreamWriter(richTextBox1fs);
                sw.WriteLine(Pagestring);
                sw.Flush();
                sw.Close();
            }
            //AcroPDDoc pdfd = new AcroPDDoc();
            //pdfd.Open(FileFullPath);
            //Object jsObj = pdfd.GetJSObject();
            //Type jsType = pdfd.GetType();
            ////have to use acrobat javascript api because, acrobat
            //object[] saveAsParam = { "newFile.doc", "com.adobe.acrobat.doc", "", false, false };
            //jsType.InvokeMember("saveAs", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, jsObj, saveAsParam, CultureInfo.InvariantCulture);
        }
        #endregion

        #region "HTMLToPDF"
        /// <summary>
        /// Converts HTML string to pdf document.
        /// </summary>
        /// <param name="HTMLString"></param>
        /// <param name="OutPutFileFullPath"></param>
        public static void HTMLToPDF(string HTMLString, string OutPutFileFullPath)
        {
            FileStream OutPutFileFS = new FileStream(OutPutFileFullPath, FileMode.Create, FileAccess.Write, FileShare.None);
            Document doc = iTextPDFHelper.SetBackgroundColor(new Document(), System.Drawing.Color.White);
            PdfWriter writer = PdfWriter.GetInstance(doc, OutPutFileFS);
            XMLWorkerHelper XMLWorkerHelperobj = XMLWorkerHelper.GetInstance();
            StringReader sr = new StringReader(HTMLString);
            XMLWorkerHelperobj.ParseXHtml(writer, doc, sr);
        }
        #endregion

        #region "AddParagraph2"
        /// <summary>
        /// Extended version of AddParagraph to avoid passing the Document object.
        /// </summary>
        /// <param name="ParagraphContent"></param>
        /// <param name="FontFamily"></param>
        /// <param name="FontSize"></param>
        /// <param name="FontColor"></param>
        public static void AddParagraph2(String ParagraphContent, String FontFamily, float FontSize, Color? FontColor = null)
        {
            //baseFont = BaseFont.CreateFont(TextFont.Name, BaseFont.TIMES_ROMAN, BaseFont.NOT_EMBEDDED);
            iTextSharp.text.Font ltFont;
            BaseColor basecolor;

            if (FontColor != null)
            {
                basecolor = new BaseColor((Color)FontColor);
                ltFont = FontFactory.GetFont(FontFamily, FontSize, basecolor);
            }
            else
                ltFont = FontFactory.GetFont(FontFamily, FontSize);

            Paragraph ltParagraph = new Paragraph(ParagraphContent, ltFont);
            ElementList.Add(ltParagraph);
        }
        #endregion

        #region "CreatePDF"
        /// <summary>
        /// Creates PDF Based on the paragraph list.
        /// </summary>
        /// <param name="OutputFileFullPath"></param>
        /// <param name="BackGroundColor"></param>
        /// <param name="Producer"></param>
        public static void CreatePDF(String OutputFileFullPath, Color? BackGroundColor = null, String Producer = null)
        {
            iTextSharp.text.Rectangle rec;
            Document outputDoc = new Document();
            //FileStream fs = new FileStream(OutputFileFullPath, FileMode.Create, FileAccess.Write, FileShare.None);
            try
            {
                if (BackGroundColor != null)
                {
                    rec = new iTextSharp.text.Rectangle(outputDoc.PageSize);
                    rec.BackgroundColor = new BaseColor((Color)BackGroundColor);
                    outputDoc = new Document(rec);
                }

                if (ElementList.Count > 0)
                {
                    if (outputDoc != null)
                    {
                        using (FileStream fs = new FileStream(OutputFileFullPath, FileMode.Create, FileAccess.Write, FileShare.None))
                        using (outputDoc)
                        using (PdfWriter writer = PdfWriter.GetInstance(outputDoc, fs))
                        {
                            outputDoc.Open();
                            if (Producer != null)
                                writer.Info.Put(new PdfName("Producer"), new PdfString(Producer));

                            foreach (var item in ElementList)
                            {
                                outputDoc.Add(item);
                            }
                            outputDoc.Close();
                        }
                    }
                }
                else
                {
                    baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                    iTextSharp.text.Font ltFont = new iTextSharp.text.Font(baseFont);
                    ltFont.Color = new BaseColor(Color.Red);
                    ltFont.Size = 22f;
                    using (FileStream fs = new FileStream(OutputFileFullPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    using (outputDoc)
                    using (PdfWriter writer = PdfWriter.GetInstance(outputDoc, fs))
                    {
                        outputDoc.Open();
                        outputDoc.Add(new Paragraph("Nothing To Show Here", ltFont));
                        outputDoc.Close();
                    }
                }
            }
            catch
            {
            }
            finally
            {
                iTextPDFHelper.ElementList.Clear();
                outputDoc = null;
            }
        }
        #endregion

        #region "AddWaterMark"
        /// <summary>
        /// Adds WaterMark in the PDF.
        /// </summary>
        /// <param name="InputFileFullPath"></param>
        /// <param name="OutputFileFullPath"></param>
        /// <param name="wWaterMark"></param>
        public static void AddWaterMark(String InputFileFullPath, String OutputFileFullPath, String wWaterMark)
        {
            // Creating watermark on a separate layer
            // Creating iTextSharp.text.pdf.PdfReader object to read the Existing PDF Document produced by 1 no.
            PdfReader reader1 = new PdfReader(InputFileFullPath);
            using (FileStream fs = new FileStream(OutputFileFullPath, FileMode.Create, FileAccess.Write, FileShare.None))
            // Creating iTextSharp.text.pdf.PdfStamper object to write Data from iTextSharp.text.pdf.PdfReader object to FileStream object
            using (PdfStamper stamper = new PdfStamper(reader1, fs))
            {
                // Getting total number of pages of the Existing Document
                int pageCount = reader1.NumberOfPages;

                // Create New Layer for Watermark
                PdfLayer layer = new PdfLayer("WatermarkLayer", stamper.Writer);
                // Loop through each Page
                for (int i = 1; i <= pageCount; i++)
                {
                    // Getting the Page Size
                    iTextSharp.text.Rectangle rect = reader1.GetPageSize(i);

                    // Get the ContentByte object
                    PdfContentByte cb = stamper.GetUnderContent(i);

                    // Tell the cb that the next commands should be "bound" to this new layer
                    cb.BeginLayer(layer);
                    cb.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.EMBEDDED), 50);

                    PdfGState gState = new PdfGState();
                    gState.FillOpacity = 0.25f;
                    cb.SetGState(gState);

                    cb.SetColorFill(BaseColor.GREEN);

                    cb.BeginText();

                    cb.ShowTextAligned(PdfContentByte.ALIGN_CENTER, wWaterMark, rect.Width / 2, rect.Height / 2, 45f);

                    cb.EndText();

                    // Close the layer
                    cb.EndLayer();
                }
            }
        }
        #endregion

        #region "AddImage"
        /// <summary>
        /// Add Image to Pdf from Image path.
        /// </summary>
        /// <param name="ImagePath"></param>
        /// <param name="AddImageType"></param>
        /// <param name="MaxHeight"></param>
        /// <param name="MaxWidth"></param>
        public static void AddImage(String ImagePath, AddImageType AddImageType = AddImageType.HeightWidth, float MaxHeight = 700, float MaxWidth = 600)
        {
            if (AddImageType == AddImageType.HeightWidth)
                AddImageWithWidthHeight(ImagePath, Convert.ToInt32(MaxHeight), Convert.ToInt32(MaxWidth));
            else if (AddImageType == AddImageType.Size)
                AddImageWithSize(ImagePath, MaxHeight);
        }

        /// <summary>
        /// Add Image to Pdf from Image path.
        /// </summary>
        /// <param name="ImagePath">A Path</param>
        /// <param name="MaxHeight">Max Height Allowed</param>
        /// <param name="MaxWidth">Max Width Allowed</param>
        /// <exception cref="System.ArgumentNullException">ImagePath is null.</exception>
        /// <exception cref="System.UriFormatException">ImagePath is empty.-or- The scheme specified in ImagePath is not correctly formed.</exception>
        private static void AddImageWithWidthHeight(String ImagePath, Int32 MaxHeight = 700, Int32 MaxWidth = 600)
        {
            System.Drawing.Image Image = System.Drawing.Bitmap.FromFile(ImagePath);
            iTextSharp.text.Image pic = iTextSharp.text.Image.GetInstance(Image, System.Drawing.Imaging.ImageFormat.Jpeg);

            if (pic.Height > pic.Width)
            {
                //Maximum height is 800 pixels.
                float percentage = 0.0f;
                percentage = MaxHeight / pic.Height;
                pic.ScalePercent(percentage * 100);
            }
            else
            {
                //Maximum width is 600 pixels.
                float percentage = 0.0f;
                percentage = 600 / pic.Width;
                pic.ScalePercent(percentage * 100);
            }

            pic.Border = iTextSharp.text.Rectangle.BOX;
            pic.BorderColor = iTextSharp.text.BaseColor.GREEN;
            pic.BorderWidth = 3f;

            ElementList.Add(pic);
        }

        /// <summary>
        /// Add Image to Pdf from Image path with Scale size.
        /// </summary>
        /// <param name="ImagePath"></param>
        /// <param name="ImageSize"></param>
        /// <exception cref="System.ArgumentNullException">ImagePath is null.</exception>
        /// <exception cref="System.UriFormatException">ImagePath is empty.-or- The scheme specified in ImagePath is not correctly formed.</exception>
        private static void AddImageWithSize(String ImagePath, float ImageSize = 100f)
        {
            System.Drawing.Image Image = System.Drawing.Bitmap.FromFile(ImagePath);

            iTextSharp.text.Image pic = iTextSharp.text.Image.GetInstance(Image, System.Drawing.Imaging.ImageFormat.Jpeg);

            pic.ScaleAbsolute(ImageSize, ImageSize);
            PdfPTable table = new PdfPTable(1);
            table.AddCell(new PdfPCell(pic));
            table.DefaultCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
            ElementList.Add(table);
        }
        #endregion
        #endregion
    }

    public enum AddImageType
    {
        Size = 0,
        HeightWidth = 1
    }
}
