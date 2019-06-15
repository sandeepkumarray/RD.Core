using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace RDCore
{
    public static class DictionaryLib
    {
        static DictionaryLib()
        {
            WordFormatDictionary = new Dictionary<string, string>();


            WordFormatDictionary.Add(WordFormat.wdFormatDocument97.ToString(), ".doc");
            //WordFormatDictionary.Add(WordFormat.wdFormatDocument.ToString(), ".idoc");
            WordFormatDictionary.Add(WordFormat.wdFormatTemplate97.ToString(), ".tpl");
            //WordFormatDictionary.Add(WordFormat.wdFormatTemplate.ToString(), ".tpl");
            WordFormatDictionary.Add(WordFormat.wdFormatText.ToString(), ".txt");
            WordFormatDictionary.Add(WordFormat.wdFormatTextLineBreaks.ToString(), ".txt");
            WordFormatDictionary.Add(WordFormat.wdFormatDOSText.ToString(), ".txt");
            WordFormatDictionary.Add(WordFormat.wdFormatDOSTextLineBreaks.ToString(), ".txt");
            WordFormatDictionary.Add(WordFormat.wdFormatRTF.ToString(), ".rtf");
            WordFormatDictionary.Add(WordFormat.wdFormatUnicodeText.ToString(), ".rtf");
            //WordFormatDictionary.Add(WordFormat.wdFormatEncodedText.ToString(), ".rtf");
            WordFormatDictionary.Add(WordFormat.wdFormatHTML.ToString(), ".html");
            WordFormatDictionary.Add(WordFormat.wdFormatWebArchive.ToString(), ".html");
            WordFormatDictionary.Add(WordFormat.wdFormatFilteredHTML.ToString(), ".html");
            WordFormatDictionary.Add(WordFormat.wdFormatXML.ToString(), ".xml");
            WordFormatDictionary.Add(WordFormat.wdFormatXMLDocument.ToString(), ".xml");
            WordFormatDictionary.Add(WordFormat.wdFormatXMLDocumentMacroEnabled.ToString(), ".xml");
            WordFormatDictionary.Add(WordFormat.wdFormatXMLTemplate.ToString(), ".xml");
            WordFormatDictionary.Add(WordFormat.wdFormatXMLTemplateMacroEnabled.ToString(), ".xml");
            WordFormatDictionary.Add(WordFormat.wdFormatDocumentDefault.ToString(), ".docx");
            WordFormatDictionary.Add(WordFormat.wdFormatPDF.ToString(), ".pdf");
            WordFormatDictionary.Add(WordFormat.wdFormatXPS.ToString(), ".xps");
            WordFormatDictionary.Add(WordFormat.wdFormatFlatXML.ToString(), ".xml");
            WordFormatDictionary.Add(WordFormat.wdFormatFlatXMLMacroEnabled.ToString(), ".xml");
            WordFormatDictionary.Add(WordFormat.wdFormatFlatXMLTemplate.ToString(), ".xml");
            WordFormatDictionary.Add(WordFormat.wdFormatFlatXMLTemplateMacroEnabled.ToString(), ".xml");
            WordFormatDictionary.Add(WordFormat.wdFormatOpenDocumentText.ToString(), ".odt");

        }

        public static Dictionary<string, string> WordFormatDictionary;

        public static string GetExtention(WordFormat WordFormatType)
        {
            string result = string.Empty;
            foreach (var i in WordFormatDictionary)
            {
                if (i.Key.ToString().Contains(WordFormatType.ToString()))
                {
                    result = i.Value;
                }
            }
            return result;
        }
    }
}