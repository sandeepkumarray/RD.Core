using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Data;
using System.Collections;
using System.Reflection;
using System.Globalization;

namespace RDCore
{
    /// <summary>
    /// XML Utility for XML Files.
    /// </summary>
    public static class XmlUtility
    {
        /// <summary>
        /// It takes the object and Serializes it into an XML
        /// </summary>
        /// <param name="obj">Object</param>
        /// <returns>XML Data</returns>
        public static string ToXml(object obj)
        {
            XmlSerializer s = new XmlSerializer(obj.GetType());
            using (StringWriterWithEncoding writer = new StringWriterWithEncoding(Encoding.UTF8))
            {
                s.Serialize(writer, obj);
                return writer.ToString();
            }
        }

        /// <summary>
        /// It takes the XML data and deserializes it into an object
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="data">XML data</param>
        /// <returns>object</returns>
        public static object FromXml<T>(this TextReader data)
        {
            XmlSerializer s = new XmlSerializer(typeof(T));
            //using (StringReader reader = new StringReader(data))
            //{

            object obj = s.Deserialize(data);
            return (T)obj;
            //}
        }

        /// <summary>
        /// Extends StringWriter class for changing Encoding type.
        /// </summary>
        class StringWriterWithEncoding : StringWriter
        {
            public StringWriterWithEncoding(StringBuilder sb, Encoding encoding)
                : base(sb)
            {
                this.m_Encoding = encoding;
            }

            public StringWriterWithEncoding(Encoding encoding)
                : base()
            {
                this.m_Encoding = encoding;
            }

            private readonly Encoding m_Encoding;
            public override Encoding Encoding
            {
                get
                {
                    return this.m_Encoding;
                }
            }
        }

        /// <summary>
        /// Reading DataSet from an XML file
        /// </summary>
        /// <param name="Path">XML File Path</param>
        /// <param name="XmlReadMode">Read Mode</param>
        /// <returns>Dataset</returns>
        public static DataSet ReadDataSetFromXML(string Path, XmlReadMode XmlReadMode)
        {
            DataSet ObjdataSet = new DataSet();
            ObjdataSet.ReadXml(Path, XmlReadMode);
            return ObjdataSet;
        }

        /// <summary>
        /// Reading DataSet from an XML file
        /// </summary>
        /// <param name="Path">StringReader Path</param>
        /// <param name="XmlReadMode">Read Mode</param>
        /// <returns>DataSet</returns>
        public static DataSet ReadDataSetFromXML(StringReader Path, XmlReadMode XmlReadMode)
        {
            DataSet ObjdataSet = new DataSet();
            ObjdataSet.ReadXml(Path, XmlReadMode);
            return ObjdataSet;
        }

        /// <summary>
        /// Reading DataSet from an XML file
        /// </summary>
        /// <param name="Path">XMLReader</param>
        /// <param name="XmlReadMode">Read Mode</param>
        /// <returns>DataSet</returns>
        public static DataSet ReadDataSetFromXML(XmlReader Path, XmlReadMode XmlReadMode)
        {
            DataSet ObjdataSet = new DataSet();
            ObjdataSet.ReadXml(Path, XmlReadMode);
            return ObjdataSet;
        }

        /// <summary>
        /// Writes the Data from DataSet into a XML File
        /// </summary>
        /// <param name="dataSet">DataSet</param>
        /// <param name="FileName">File Path</param>
        /// <param name="XmlWriteMode">Write Mode</param>
        /// <returns></returns>
        public static bool WriteDataSetToXML(DataSet dataSet,string FileName, XmlWriteMode XmlWriteMode)
        {
            bool isReturn = false;
            try
            {
                dataSet.WriteXml(FileName, XmlWriteMode);
                isReturn = true;
            }
            catch (Exception e)
            {
                isReturn = false;
            }
            return isReturn;
        }

        /// <summary>
        /// Writes the Data from DataSet into a XML File
        /// </summary>
        /// <param name="dataSet">DataSet</param>
        /// <param name="FileName">File Path</param>
        /// <param name="XmlWriteMode">Write Mode</param>
        /// <returns></returns>
        public static bool WriteDataSetToXML(DataSet dataSet, StreamWriter FileName, XmlWriteMode XmlWriteMode)
        {
            bool isReturn = false;
            try
            {
                dataSet.WriteXml(FileName, XmlWriteMode);
                isReturn = true;
            }
            catch (Exception e)
            {
                isReturn = false;
            }
            return isReturn;
        }

        #region "CreateXML - Using Reflection"
        /// <summary>
        /// Read the given entity and returns xml string Using Reflection
        /// </summary>
        /// <param name="psrcObject">Source Object</param>
        /// <param name="HomeTag">Home Tag</param>
        /// <returns>String</returns>
        public static String CreateXML(Object psrcObject,String HomeTag)
        {
            System.Text.StringBuilder xmlString = null;
            String[] strTableName = null;
            String strParameterType = String.Empty;
            try
            {
                xmlString = new System.Text.StringBuilder();
                xmlString.Append("<" + HomeTag + ">");
                //checking whether if it has a collection of entities in a arraylist 
                if (psrcObject.GetType() == typeof(ArrayList))
                {
                    //if it is a collection of entities
                    foreach (ArrayList obj in (ArrayList)psrcObject)
                    {
                        strTableName = obj.ToString().Split(".".ToCharArray());
                        xmlString.Append(String.Format(System.Globalization.CultureInfo.InvariantCulture, "<{0}>", strTableName[strTableName.Length - 1]).ToUpper());
                        ReadProperties(obj, xmlString);
                        xmlString.Append(String.Format(System.Globalization.CultureInfo.InvariantCulture, "</{0}>", strTableName[strTableName.Length - 1]).ToUpper());
                    }
                }
                else
                {
                    //If only one entity
                    strTableName = psrcObject.ToString().Split(".".ToCharArray());
                    xmlString.Append(String.Format(System.Globalization.CultureInfo.InvariantCulture, "<{0}>", strTableName[strTableName.Length - 1]).ToUpper());
                    ReadProperties(psrcObject, xmlString);
                    xmlString.Append(String.Format(System.Globalization.CultureInfo.InvariantCulture, "</{0}>", strTableName[strTableName.Length - 1]).ToUpper());
                }
                xmlString.Append("</" + HomeTag + ">");
            }
            catch
            {
                throw;
            }
            return xmlString.ToString();
        }

        #endregion

        #region "ReadProperties"
        /// <summary>
        /// Determining the type of property using reflection
        /// </summary>
        /// <param name="psrcObject">Source Object</param>
        /// <param name="pxmlString">Stringbuilder object</param>
        static void ReadProperties(Object psrcObject, StringBuilder pxmlString)
        {

            Type myType;
            String pName = String.Empty;
            String pValue = String.Empty;
            Object objChildList = null;
            Boolean blnAssign;
            try
            {
                myType = psrcObject.GetType();

                foreach (PropertyInfo myPropertyInfo in myType.GetProperties())
                {
                    pName = myPropertyInfo.Name.ToUpper(System.Globalization.CultureInfo.InvariantCulture);
                    if (!myPropertyInfo.PropertyType.IsGenericType)
                    {
                        if (myPropertyInfo.GetValue(psrcObject, null) != null)
                            pValue = myPropertyInfo.GetValue(psrcObject, null).ToString();
                        else
                            pValue = null;

                        blnAssign = false;
                        if (myPropertyInfo.PropertyType == typeof(Decimal))
                        {
                            if (pValue != System.Decimal.MinValue.ToString(System.Globalization.CultureInfo.InvariantCulture))
                            {
                                blnAssign = true;
                            }
                        }
                        else if (myPropertyInfo.PropertyType == typeof(Int32))
                        {
                            if (pValue != int.MinValue.ToString(System.Globalization.CultureInfo.InvariantCulture))
                            {
                                blnAssign = true;
                            }
                        }
                        else if (myPropertyInfo.PropertyType == typeof(String))
                        {
                            if (pValue != null)
                            {
                                pValue = pValue.Replace("&", "&amp;");
                                blnAssign = true;
                            }
                        }
                        else if (myPropertyInfo.PropertyType == typeof(DateTime))
                        {
                            if (pValue != System.DateTime.MinValue.ToString(System.Globalization.CultureInfo.InvariantCulture))
                            {
                                blnAssign = true;
                            }
                        }
                        else if (myPropertyInfo.PropertyType == typeof(Boolean))
                        {
                            blnAssign = true;
                        }
                        else if (myPropertyInfo.PropertyType == typeof(Int64))
                        {
                            if (pValue != Int64.MinValue.ToString(System.Globalization.CultureInfo.InvariantCulture))
                            {
                                blnAssign = true;
                            }
                        }
                        else if (myPropertyInfo.PropertyType == typeof(Byte[]))
                        {
                            if (pValue != null)
                            {
                                blnAssign = true;
                            }
                        }
                        else if (myPropertyInfo.PropertyType.IsEnum)
                        {
                            blnAssign = true;
                        }
                        else if (myPropertyInfo.PropertyType == typeof(Double))
                        {
                            if (pValue != double.MinValue.ToString(CultureInfo.InvariantCulture))
                            {
                                if (Convert.ToDouble(pValue) > 0)
                                {
                                    blnAssign = true;
                                }
                            }
                        }
                        //else if (myPropertyInfo.PropertyType.BaseType == typeof(BaseDTO))
                        //{
                        //    StringBuilder sb = new StringBuilder(string.Empty);
                        //    //object myProperty = (object)myPropertyInfo;
                        //    ReadProperties(myPropertyInfo, sb);
                        //    pValue = sb.ToString();
                        //    //ReadProperties(myPropertyInfo.GetValue(
                        //}
                        if (blnAssign)
                        {
                            pxmlString.Append(String.Format(System.Globalization.CultureInfo.InvariantCulture, "<{0}>{1}</{0}>", pName, pValue));
                        }
                        blnAssign = false;
                    }
                    else
                    {
                        objChildList = myPropertyInfo.GetValue(psrcObject, null);
                        objChildList = (IList)objChildList;
                        pxmlString.Append(String.Format(System.Globalization.CultureInfo.InvariantCulture, "<{0}>", pName + "_Child"));
                        if (objChildList != null)
                        {
                            foreach (Object obj1 in (IList)objChildList)
                            {
                                pxmlString.Append(String.Format(System.Globalization.CultureInfo.InvariantCulture, "<{0}>", pName));
                                ReadProperties(obj1, pxmlString);
                                pxmlString.Append(String.Format(System.Globalization.CultureInfo.InvariantCulture, "</{0}>", pName));
                            }
                        }
                        pxmlString.Append(String.Format(System.Globalization.CultureInfo.InvariantCulture, "</{0}>", pName + "_Child"));
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {

                myType = null;
                objChildList = null;
            }


        }
        #endregion
    }
}
