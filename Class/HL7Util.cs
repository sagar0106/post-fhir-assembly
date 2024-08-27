using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using System.ComponentModel;
using System.Reflection;
using System.Data;
using Microsoft.VisualBasic.FileIO;

namespace PostDnFhirR4Api.Class
{
    public class HL7Util
    {
        public string Hl7WithoutPDF { get; set; }

        #region Constructor
        public HL7Util() { }
        public HL7Util(string HL7Content)
        {
            try
            {
                RemovePDFFromHL7(HL7Content);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        /// <summary>
        /// Read all lines of HL7 data and find HL7Segment 
        /// Look at position HL7SegmentPosition
        /// </summary>
        /// <param name="HL7Data">Data content of file</param>
        /// <param name="HL7Segment">HL7 find Segment</param>
        /// <param name="HL7SegmentPosition">Position to find</param>
        /// <returns>Value if match found</returns>
        /// <remarks></remarks>
        public string GetHL7Value(string HL7Data, string HL7Segment, int HL7SegmentPosition, int HL7SegmentSubPosition = 0)
        {
            try
            {
                // '\v'  =  ((char)11).ToString()
                var v = HL7Data.Replace("\v", string.Empty).Replace("MSH|^~", "MSH||^~").Replace("\r\n", "\r").Replace("\n", "\r").Split('\r');
                v = v.Where(x => x.ToUpper().StartsWith(HL7Segment.ToUpper())).ToArray();
                if (v.Length > 0)
                {
                    foreach (string _strHL7Line in v)
                    {
                        v = _strHL7Line.Split('|');
                        if (v.Length > HL7SegmentPosition)
                        {
                            if (HL7SegmentSubPosition > 0)
                            {
                                v = v[HL7SegmentPosition].Trim().Split('^');
                                if (v.Length >= HL7SegmentSubPosition)
                                    return v[HL7SegmentSubPosition - 1].Trim();
                            }
                            else
                            {
                                return v[HL7SegmentPosition].Trim();
                            }
                        }
                    }

                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///  To get specified HL7 vale
        /// </summary>
        /// <param name="HL7Data">Data content of file</param>
        /// <param name="HL7Segment">HL7 find Segment</param>
        /// <param name="SegmentPosition">Index of the Matching HL7 segment to return</param>
        /// <param name="FieldPosition">Index of the field</param>
        /// <param name="ComponentPosition">Index of the Component</param>
        /// <param name="SubComponentPosition">Index of the Sub-Component</param>
        /// <param name="RepeatFieldPosition">Index of RepeatField</param>
        /// <returns>Value if match found</returns>
        /// <remarks></remarks>
        public string GetHL7Value(string HL7Data, string HL7Segment, int SegmentPosition, int FieldPosition, int ComponentPosition, int SubComponentPosition, int RepeatFieldPosition)
        {
            try
            {
                StringBuilder hl7FileContent = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(Hl7WithoutPDF))
                {
                    hl7FileContent.Append(Hl7WithoutPDF);
                }
                else
                {
                    hl7FileContent.Append(HL7Data);
                }
                if (string.IsNullOrWhiteSpace(hl7FileContent.ToString()))
                {
                    return string.Empty;
                }
                hl7FileContent.Replace("\v", string.Empty);
                hl7FileContent.Replace("MSH|^~", "MSH||^~");
                hl7FileContent.Replace("\r\n", "\r");
                hl7FileContent.Replace("\n", "\r");
                var v = hl7FileContent.ToString().Split('\r').Where(x => x.ToUpper().StartsWith(HL7Segment.ToUpper())).ToArray();
                //--To return value of matching segment at particular index-----
                if (SegmentPosition > 1)
                {
                    if (v.Length >= SegmentPosition)
                    {
                        v = new[] { v[SegmentPosition - 1] };
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                //---------------------------------------------------------------
                string Hl7Value = string.Empty;
                if (v.Length > 0)
                {
                    foreach (string _strHL7Line in v)
                    {
                        //-------------------Get Field Value---------
                        v = _strHL7Line.Split('|');
                        if (v.Length > FieldPosition)
                        {
                            Hl7Value = v[FieldPosition].Trim();
                            if (RepeatFieldPosition > 0)
                            {
                                //--To Get particular RepeatField Value
                                v = Hl7Value.Split('~');
                                if (v.Length >= RepeatFieldPosition)
                                {
                                    Hl7Value = v[Convert.ToInt32(RepeatFieldPosition) - 1].Trim();
                                }
                                else
                                {
                                    return string.Empty;
                                }
                            }
                            if (ComponentPosition > 0)
                            {
                                //-------------------Get Component Value---------
                                v = Hl7Value.Split('^');
                                if (v.Length >= ComponentPosition)
                                {
                                    if (SubComponentPosition > 0)
                                    {
                                        //-------------------Get SubComponent Value---------
                                        v = v[ComponentPosition - 1].Trim().Split('&');
                                        if (v.Length >= SubComponentPosition)
                                        {
                                            return v[SubComponentPosition - 1].Trim();
                                        }
                                    }
                                    else
                                    {
                                        return v[ComponentPosition - 1].Trim();
                                    }
                                }
                            }
                            else
                            {
                                return Hl7Value;
                            }
                        }
                    }

                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// To get specified HL7 vale
        /// </summary>
        /// <param name="HL7Data">Data content of file</param>
        /// <param name="HL7SegmentWithPosition">HL7 find segment with position</param>
        /// <returns>Value if match found</returns>
        /// <remarks></remarks>
        public string GetHL7Value(string HL7Data, string HL7SegmentWithPosition)
        {
            try
            {
                return ExtractHL7Value(HL7Data, HL7SegmentWithPosition);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// To get HL7 value of matching segment at particular index
        /// </summary>
        /// <param name="HL7Data">Data content of file</param>
        /// <param name="HL7SegmentWithPosition">HL7 find segment with position</param>
        /// <param name="SegmentIndex">Index of the Matching HL7 segment to return</param>
        /// <returns>Value if match found</returns>
        /// <remarks></remarks>
        public string GetHL7ValueWithSegmentIndex(string HL7Data, string HL7SegmentWithPosition, int SegmentIndex)
        {
            try
            {
                return ExtractHL7Value(HL7Data, HL7SegmentWithPosition, SegmentIndex);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// To get specified HL7 vale
        /// </summary>       
        /// <param name="HL7SegmentWithPosition">HL7 find segment with position</param>
        /// <returns>Value if match found</returns>
        /// <remarks></remarks>
        public string GetHL7Value(string HL7SegmentWithPosition)
        {
            try
            {
                return ExtractHL7Value(null, HL7SegmentWithPosition);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string ExtractHL7Value(string HL7Data, string HL7SegmentWithPosition, int SegmentIndex = 0)
        {
            try
            {
                string[] sPositions = HL7SegmentWithPosition.Trim().Substring(3).Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                int iRepeatFieldPosition = 0;
                int iFieldPosition = 0;
                bool isRepeatField = sPositions[0].Contains("[");
                if (isRepeatField)
                {
                    int startPosition = sPositions[0].IndexOf("[");
                    int endPosition = sPositions[0].IndexOf("]");
                    int lenth = endPosition - startPosition;
                    iRepeatFieldPosition = Convert.ToInt32(sPositions[0].Substring(startPosition + 1, lenth - 1));
                    iFieldPosition = Convert.ToInt32(sPositions[0].Substring(0, startPosition));
                }
                else
                {
                    iFieldPosition = Convert.ToInt32(sPositions[0]);
                }
                int iComponentPosition = sPositions.Length > 1 ? Convert.ToInt32(sPositions[1]) : 0;
                int iSubComponentPosition = sPositions.Length > 2 ? Convert.ToInt32(sPositions[2]) : 0;
                return GetHL7Value(HL7Data, HL7SegmentWithPosition.Substring(0, 3), SegmentIndex, iFieldPosition, iComponentPosition, iSubComponentPosition, iRepeatFieldPosition);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// To get matching HL7 segments
        /// </summary>
        /// <param name="HL7Data">HL7 Content</param>
        /// <param name="segmentsName">List of the HL7 segments to return</param>
        /// <returns>list of the matching HL7 segments</returns>
        public List<string> GetMatchingHL7Segments(string HL7Data, List<string> segmentsName)
        {
            List<string> lstSegments = new List<string>();
            try
            {
                if (segmentsName != null && segmentsName.Count > 0)
                {
                    string pattern = string.Format(@"^({0})", string.Join("|", segmentsName.ConvertAll(x => x.ToUpper())));
                    Regex regExp = new Regex(pattern);
                    lstSegments = GetListFromHL7String(HL7Data).Where(x => regExp.IsMatch(x)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstSegments;
        }

        /// <summary>
        /// Read all lines of HL7 data and find HL7Segment 
        /// Look at position HL7SegmentPosition
        /// </summary>
        /// <param name="HL7Data">Data content of file</param>
        /// <param name="HL7Segment">HL7 find Segment</param>
        /// <param name="HL7SegmentPosition">Position to find</param>
        /// <returns>Value if match found</returns>
        /// <remarks></remarks>
        public string SetHL7Value(string HL7Data, string HL7Segment, int HL7SegmentPosition, string HL7ReplaceValue, int HL7SegmentSubPosition = 0)
        {
            string[] arLines = null;
            string[] arLineItems = null;
            string[] arLineSubItems = null;
            try
            {
                arLines = HL7Data.Replace("\v", string.Empty).Replace("MSH|^~", "MSH||^~").Replace("\r\n", "\r").Replace("\n", "\r").Split('\r');
                var v = arLines.Where(x => x.ToUpper().StartsWith(HL7Segment.ToUpper())).ToArray();
                if (v.Length > 0)
                {
                    foreach (string _strLine in v)
                    {
                        arLineItems = _strLine.Split('|');
                        if (arLineItems.Length > HL7SegmentPosition)
                        {
                            if (HL7SegmentSubPosition > 0)
                            {
                                arLineSubItems = arLineItems[HL7SegmentPosition].Trim().Split('^');
                                if (arLineSubItems.Length >= HL7SegmentSubPosition)
                                {
                                    arLineSubItems[HL7SegmentSubPosition - 1] = HL7ReplaceValue;
                                    arLineItems[HL7SegmentPosition] = string.Join("^", arLineSubItems);
                                    arLines[Array.IndexOf(arLines, _strLine)] = string.Join("|", arLineItems);
                                }
                            }
                            else
                            {
                                arLineItems[HL7SegmentPosition] = HL7ReplaceValue;
                                arLines[Array.IndexOf(arLines, _strLine)] = string.Join("|", arLineItems);
                            }
                        }
                    }

                }
                return string.Join("\r\n", arLines).Replace("MSH||^~", "MSH|^~");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Read all lines of HL7 data and find HL7Segment 
        /// Look at position HL7SegmentPosition
        /// </summary>
        /// <param name="HL7Data">Data content of file</param>
        /// <param name="HL7SegmentWithPosition">HL7 find Segment</param>       
        /// <returns>Value if match found</returns>
        /// <remarks></remarks>
        public string SetHL7Value(string HL7Data, string HL7SegmentWithPosition, string HL7ReplaceValue)
        {
            try
            {
                var v = HL7SegmentWithPosition.Substring(3).Split('.');
                int iPos = Convert.ToInt32(v[0]);
                int iSub = v.Length > 1 ? Convert.ToInt32(v[1]) : 0;
                return SetHL7Value(HL7Data, HL7SegmentWithPosition.Substring(0, 3), iPos, HL7ReplaceValue, iSub);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Generate File Name From Pattern
        /// </summary>
        /// <param name="sContents">HL7 file Content</param>
        /// <param name="HL7FilePattern">File Rename Pattern</param>
        /// <param name="FileExtension">Specify the File extension to be used in generated file name</param>
        /// <param name="FileName">File Name with extension</param>
        /// <returns>File name with extension</returns>
        /// <remarks>Every Pattern starts with "~" ~DT generates current date; ~. specifies file extension e.g. ~.HD7. File extension is generated from FileExtension if provided else ~. is considered and finally if both earlier mentioned are not present FileName extension is used.</remarks>
        public string GenerateFileNameFromPattern(string sContents, string HL7FilePattern, string FileExtension, string FileName)
        {
            try
            {
                // Remove File path consider only file name for process
                FileName = Path.GetFileName(FileName);
                HL7FilePattern = HL7FilePattern.Trim();
                if (!string.IsNullOrWhiteSpace(Hl7WithoutPDF))
                {
                    sContents = Hl7WithoutPDF;
                }
                //if (!sContents.Replace("\v", "").StartsWith("MSH"))
                //{
                //    // Return same name if file is not valid HL7 content
                //    return FileName;
                //}
                //else if (HL7FilePattern.Trim().Length == 0)
                if (HL7FilePattern.Length == 0 || string.IsNullOrWhiteSpace(sContents))
                {
                    // As no pattern specified therefore return same file name
                    //return FileName;
                    return Path.GetFileNameWithoutExtension(FileName) + GetFileExtension(FileName, FileExtension);
                }

                Int16 iCount = 0;
                //Dim sContents As String = General.ByteArrayToStr(Contents)
                // 1_+~MSH3+_+~MSH3+~DT
                //string result = "";
                StringBuilder result = new StringBuilder();
                string[] Patterns = HL7FilePattern.Split('+');
                string Pattern = string.Empty;
                string uPattern = string.Empty;
                string dotReplacement = "#@$LK$@#";
                foreach (string _strPattern in Patterns)
                {
                    iCount++;
                    Pattern = _strPattern.Trim();
                    uPattern = Pattern.ToUpper();
                    if (iCount == 2 && result.Length == 0)
                    {
                        throw new Exception("Assembly - First element of rename pattern \"" + Patterns[0] + "\" not available");
                    }
                    // MSH,PID,ORC,PV1
                    if (Pattern.StartsWith("~"))
                    {
                        //Make changes as per discussion with ronak on call date on 16/02/2017, Task# LKTRAN-2331
                        if (uPattern.StartsWith("~DTIME") || uPattern.StartsWith("~DT"))
                        {
                            result.Append(DateTime.Now.ToString("yyyyMMddHHmmssfff"));
                            continue;
                        }
                        else if (uPattern.StartsWith("~DATE("))
                        {
                            result.Append(DateTime.Now.ToString(Pattern.Replace('(', '|').Replace(')', '|').Split('|')[1]));
                            continue;
                        }
                        else if (Pattern.StartsWith("~.") & FileExtension.Length == 0)
                        {
                            result.Append(Pattern.Replace("~", ""));
                            continue;
                        }
                        // Get  XXXn.n Read number 9.9 ex. ~MSH9 OR ~MSH9.1
                        if (Pattern.Length > 4 && !Pattern.Contains("~."))
                        {
                            string segmentValue = GetHL7Value(sContents, Pattern.Substring(1));
                            if (segmentValue.Contains("."))
                            {
                                segmentValue = segmentValue.Replace(".", dotReplacement);
                            }
                            result.Append(segmentValue);
                        }
                    }
                    else if (Pattern.StartsWith("*"))
                    {
                        result.Append(Path.GetFileNameWithoutExtension(FileName));
                    }
                    else
                    {
                        result.Append(Pattern);
                    }
                }
                //If rename pattern contains file extension then consider it otherwise consider configured file extension or default extension
                if (string.IsNullOrWhiteSpace(Path.GetExtension(result.ToString())))
                {
                    result.Append(GetFileExtension(FileName, FileExtension));
                }
                if (result.Length == 0)
                {
                    result.Append(FileName);
                }
                else if (result.ToString().Contains(dotReplacement))
                {
                    result.Replace(dotReplacement, ".");
                }
                //Remove invalid file name character with empty string from file name.
                string finalFileName = Regex.Replace(result.ToString(), @"[\\/?:*""><|]+", string.Empty);
                return finalFileName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Generate File Name From Pattern
        /// </summary>       
        /// <param name="HL7FilePattern">File Rename Pattern</param>
        /// <param name="FileExtension">Specify the File extension to be used in generated file name</param>
        /// <param name="FileName">File Name with extension</param>
        /// <returns>File name with extension</returns>
        /// <remarks>Every Pattern starts with "~" ~DT generates current date; ~. specifies file extension e.g. ~.HD7. File extension is generated from FileExtension if provided else ~. is considered and finally if both earlier mentioned are not present FileName extension is used.</remarks>
        public string GenerateFileNameFromPattern(string HL7FilePattern, string FileExtension, string FileName)
        {
            try
            {
                return GenerateFileNameFromPattern(null, HL7FilePattern, FileExtension, FileName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get file extension. When File extension is *.* then return original file extension else file extension
        /// </summary>
        /// <param name="FileName">Original file name</param>
        /// <param name="FileExt">Fix extension</param>
        /// <returns>File extension with dot e.g. GetFileExtension("abc.pdf", "*.dat") = ".dat" Or GetFileExtension("abc.pdf", "*.*") = ".pdf"</returns>
        /// <remarks></remarks>
        public string GetFileExtension(string FileName, string FileExt)
        {
            try
            {
                // *.* OR *.txt
                if (string.IsNullOrWhiteSpace(FileExt))
                {
                    return Path.GetExtension(FileName);
                }
                string[] sFileExt = Convert.ToString("." + FileExt).Split('.');
                if (sFileExt[sFileExt.Length - 1].StartsWith("*"))
                {
                    return Path.GetExtension(FileName);
                }
                else
                {
                    return "." + sFileExt[sFileExt.Length - 1];
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// To convert a string to a byte array
        /// </summary>
        /// <param name="str">String for conversion</param>
        /// <returns>Converted Array of Bytes</returns>
        /// <remarks></remarks>
        public byte[] StrToByteArray(string str)
        {
            try
            {
                if (str == null)
                {
                    str = string.Empty;
                }
                return Encoding.ASCII.GetBytes(str);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// To convert a byte array to a string
        /// </summary>
        /// <param name="dBytes">Array of Bytes</param>
        /// <returns>Converted String</returns>
        /// <remarks></remarks>
        public string ByteArrayToStr(byte[] dBytes)
        {
            try
            {
                if (dBytes == null) return string.Empty;
                return Encoding.ASCII.GetString(dBytes);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Prepares file name with numbers File_Name_n.txt e.g. GetNextFileName("C:\", "File_Name.txt") = File_Name_1.txt
        /// </summary>
        /// <param name="FolderName">Name of Folder to check file existance</param>
        /// <param name="FileName">File name</param>
        /// <returns>File name String with next available number</returns>
        /// <remarks>e.g. GetFileName("C:\", "Test999.dat") will return if exists Test999_1.dat else Test999.dat</remarks>
        public string GetNextFileName(string FolderName, string FileName)
        {
            try
            {
                if (!File.Exists(Path.Combine(FolderName, FileName)))
                    return FileName;
                // We have existing file so get next
                string FileNameOnly = Path.GetFileNameWithoutExtension(FileName);
                string FileExt = Path.GetExtension(FileName);
                long NextNumber = 1;
                // Get next number
                do
                {
                    string CurFileName = FileNameOnly + "_" + NextNumber.ToString() + FileExt;
                    if (!File.Exists(Path.Combine(FolderName, CurFileName)))
                        return CurFileName;
                    NextNumber += 1;
                } while (true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Returns Data or default value
        /// </summary>
        /// <param name="oValue">Value to check if DBNull</param>
        /// <param name="DefaultValue">Default value</param>
        /// <returns>If Value is not DBNull oValue is returned otherwise DefaultValue is returned</returns>
        /// <remarks></remarks>
        public object DBNullToDefault(object oValue, object DefaultValue)
        {
            try
            {
                if (oValue == System.DBNull.Value || oValue == null)
                {
                    return DefaultValue;
                }
                else
                {
                    return oValue;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Check for Field or create
        /// </summary>
        /// <param name="toDataSet">DataSet</param>
        /// <param name="tcField">Name of Field</param>
        /// <param name="oType">DataType</param>
        /// <param name="oDefaultValue">Default value</param>
        /// <param name="tlUpdated">Flag for field is updated</param>
        /// <remarks>Looks in table 0 for existance of field if not create field</remarks>
        public void CheckField(ref DataSet toDataSet, string tcField, System.Type oType, object oDefaultValue, ref bool tlUpdated)
        {
            try
            {
                if (toDataSet.Tables.Count == 0)
                    toDataSet.Tables.Add("oTable");
                if (toDataSet.Tables[0].Columns[tcField] == null)
                {
                    //Add the column
                    DataColumn oColumn = new DataColumn(tcField, oType);

                    oColumn.DefaultValue = oDefaultValue;
                    toDataSet.Tables[0].Columns.Add(oColumn);
                    tlUpdated = true;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Return Error Message Object to Reflect Error on LKTransfer Dashboard
        /// </summary>
        /// <param name="_strErrorMessage">Error Message</param>
        /// <param name="_strDebugLog">Debuglog</param>
        /// <param name="_FileContent">File Content</param>       
        /// <remarks>Build and return error message object to feflect error on LKTransfer Dashboard</remarks>
        public object BuildErrorMessage(string _strErrorMessage, string _strDebugLog, byte[] _FileContent)
        {
            return new
            {
                Status = 3,
                ErrorMessage = _strErrorMessage,
                DebugLog = _strDebugLog,
                Body = _FileContent
            };
        }

        /// <summary>
        /// Return Error Message Object to Reflect Error on LKTransfer Dashboard
        /// </summary>
        /// <param name="_strErrorMessage">Error Message</param>
        /// <param name="_strDebugLog">Debuglog</param>
        /// <param name="_FileContent">File Content</param>       
        /// <remarks>Build and return error message object to reflect error on LKTransfer Dashboard</remarks>
        public object BuildErrorMessage(string _strErrorMessage, string _strDebugLog, byte[] _FileContent, int _intStatus)
        {
            return new
            {
                Status = _intStatus,
                ErrorMessage = _strErrorMessage,
                DebugLog = _strDebugLog,
                Body = _FileContent
            };
        }

        /// <summary>
        /// Build Assembly Response with error details 
        /// </summary>
        /// <param name="strErrorMessage"></param>
        /// <param name="strDebugLog"></param>
        /// <param name="FileContent"></param>
        /// <param name="intStatus"></param>
        /// <returns></returns>
        public ResultDetails BuildResultDetails(string strErrorMessage, string strDebugLog, byte[] FileContent, int intStatus)
        {
            ResultDetails objResultDetails = new ResultDetails();
            objResultDetails.ErrorMessage = strErrorMessage;
            objResultDetails.DebugLog = strDebugLog;
            objResultDetails.Body = FileContent;
            objResultDetails.Status = intStatus;
            return objResultDetails;
        }

        /// <summary>
        /// Build assembly response object with no error details 
        /// </summary>
        /// <param name="debugLog"></param>
        /// <param name="fileContent"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public ResultDetails BuildResultDetails(string debugLog, byte[] fileContent, int status)
        {
            var resultDetails = new ResultDetails();
            resultDetails.DebugLog = debugLog;
            resultDetails.Body = fileContent;
            resultDetails.Status = status;
            return resultDetails;
        }

        /// <summary>
        /// Returns a ResultDetails set with the specified error message and file status if provided. If file status is not provided it is set as Error by default.
        /// </summary>
        /// <param name="resultDetails">The ResultDetails to modify.</param>
        /// <param name="errorMessage">The error message</param>
        /// <param name="status">File status</param>
        /// <returns></returns>
        public ResultDetails ErrorResultDetails(ResultDetails resultDetails, string errorMessage, int status = ResultDetails.LKTransferResponseStatus.Error)
        {
            resultDetails.ErrorMessage = errorMessage;
            resultDetails.Status = status;
            return resultDetails;
        }

        /// <summary>
        /// Returns a ResultDetails set with the specified filter message and file status to filter.
        /// </summary>
        /// <param name="resultDetails">The ResultDetails to modify.</param>
        /// <param name="filterMessage">The filter message</param>        
        /// <returns></returns>
        public ResultDetails FilterResultDetails(ResultDetails resultDetails, string filterMessage)
        {
            resultDetails.ErrorMessage = filterMessage;
            resultDetails.Status = ResultDetails.LKTransferResponseStatus.FilteredMessage;
            return resultDetails;
        }


        /// <summary>
        /// To Show or Hide Assembly Configuration Property
        /// </summary>
        /// <param name="ContainerClass">Class containing property, e.g this</param>
        /// <param name="PropertyName">Name of the Property</param>
        /// <param name="isBrowsable">Set True to Show, False to hide</param>
        public void SetBrowsableProperty(object ContainerClass, string PropertyName, bool isBrowsable)
        {
            try
            {
                PropertyDescriptor descriptor = TypeDescriptor.GetProperties(ContainerClass.GetType())[PropertyName];
                BrowsableAttribute attribute = (BrowsableAttribute)descriptor.Attributes[typeof(BrowsableAttribute)];
                FieldInfo fieldToChange = attribute.GetType().GetField("browsable", BindingFlags.NonPublic | BindingFlags.Instance);
                fieldToChange.SetValue(attribute, isBrowsable);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// To set configuration property as ReadOnly
        /// </summary>
        /// <param name="ContainerClass">Class containing property, e.g this</param>
        /// <param name="PropertyName">Name of the Property</param>
        /// <param name="isReadOnly">Set True to make ReadOnly, False to make Editable</param>
        public void SetReadOnlyProperty(object ContainerClass, string PropertyName, bool isReadOnly)
        {
            try
            {
                PropertyDescriptor descriptor = TypeDescriptor.GetProperties(ContainerClass.GetType())[PropertyName];
                ReadOnlyAttribute attribute = (ReadOnlyAttribute)descriptor.Attributes[typeof(ReadOnlyAttribute)];
                FieldInfo fieldToChange = attribute.GetType().GetField("isReadOnly", BindingFlags.NonPublic | BindingFlags.Instance);
                fieldToChange.SetValue(attribute, isReadOnly);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Generate list of files from file search pattern
        /// </summary>
        /// <param name="FilesFolder">Path of the file containing folder</param>
        /// <param name="FileSearchPattern">Pattern to search files</param>
        /// <returns></returns>
        public List<string> GenerateFileListFromFileSearchPattern(string FilesFolder, string FileSearchPattern, bool CheckSubFolders = false)
        {
            List<string> lstFiles = new List<string>();
            try
            {
                if (string.IsNullOrWhiteSpace(FileSearchPattern.Trim()))
                {
                    FileSearchPattern = "*.*";
                }
                foreach (string pattern in FileSearchPattern.Split(';'))
                {
                    lstFiles.AddRange(Directory.GetFiles(FilesFolder, pattern, CheckSubFolders ? System.IO.SearchOption.AllDirectories : System.IO.SearchOption.TopDirectoryOnly));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstFiles.Distinct().ToList();
        }

        /// <summary>
        /// Check configuration folder and Create folder.
        /// </summary>
        /// <param name="ConfigFileName">Config File Path</param>
        /// <remarks>If doesn't exists create</remarks>       
        public void CheckConfigFolder(string ConfigFileName)
        {
            try
            {
                string XMLFile = ConfigFileName;
                if (File.Exists(XMLFile))
                    return;
                if (Directory.Exists(Path.GetDirectoryName(XMLFile)) == false)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(XMLFile));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// Extracts Base64 embedded PDF content from the specified HL7.
        /// </summary>
        /// <param name="HL7Content">HL7 file content</param>       
        /// <returns>List of PDF bytes, List of Base64 embedded PDF Content, string HL7ContentWithoutPDF</returns>
        public Tuple<List<byte[]>, List<string>, string> ExtractPDFFromHL7(string HL7Content)
        {
            var lstPDFBytes = new List<byte[]>();
            var lstBase64PDFContent = new List<string>();
            string HL7ContentWithoutPDF = String.Empty;
            try
            {
                string PDFContentPosition = String.Empty;
                string PDFContent = String.Empty;
                var _sb = new StringBuilder();
                var regex = new Regex(@"^((OBX\|[0-9]*\|ED\|)|(ZEF\|[0-9]*\|)|(ZTE\|[0-9]*\|ED\|))");
                List<string> lstAllSegments = GetListFromHL7String(HL7Content);
                List<string> lstFilteredSegments = lstAllSegments.Where(x => regex.IsMatch(x)).ToList();
                var lstPDFSegments = new List<string>();
                foreach (string segment in lstFilteredSegments)
                {
                    lstPDFSegments.Add(segment);

                    //Get PDF content position if not yet available, to be used for subsequent segments
                    if (String.IsNullOrWhiteSpace(PDFContentPosition))
                    {
                        PDFContentPosition = GetPDFContentPosition(segment);
                    }

                    if (!String.IsNullOrWhiteSpace(PDFContentPosition))
                    {
                        PDFContent = GetHL7Value(segment, PDFContentPosition);

                        //If we've already found valid PDF content and a subsequent filtered segment also has valid PDF content then it is another PDF file, save PDF data in lists before overwriting.
                        if (!String.IsNullOrWhiteSpace(_sb.ToString()) && ValidatePDFData(PDFContent))
                        {
                            //Add PDF bytes and Base64 embedded string content to list before overwriting StringBuilder with next valid PDF. 
                            lstPDFBytes.Add(Convert.FromBase64String(_sb.ToString().Split('|')[0]));
                            lstBase64PDFContent.Add(_sb.ToString());
                            _sb.Clear();
                        }

                        _sb.Append(PDFContent);
                    }
                }

                //Add PDF bytes and Base64 embedded string content to list
                if (!String.IsNullOrWhiteSpace(_sb.ToString()))
                {
                    lstPDFBytes.Add(Convert.FromBase64String(_sb.ToString().Split('|')[0]));
                    lstBase64PDFContent.Add(_sb.ToString());
                    _sb.Clear();
                }

                //--Set default null values if no valid pdf content was found.--
                if (lstPDFBytes.Count == 0)
                {
                    lstPDFBytes.Add(null); //Caller is responsible for checking if Tuple.Item1[i] is not null

                }
                if (lstBase64PDFContent.Count == 0)
                {
                    lstBase64PDFContent.Add(null); //Caller is responsible for checking if String.IsNullOrWhiteSpace(Tuple.Item2[i])
                }
                //-------------------------------------------------

                //Remove PDF content from original HL7 content
                if (lstPDFSegments.Count > 0)
                {
                    lstPDFSegments.ForEach(x => lstAllSegments.Remove(x));
                }

                HL7ContentWithoutPDF = String.Join(Environment.NewLine, lstAllSegments);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return new Tuple<List<byte[]>, List<string>, string>(lstPDFBytes, lstBase64PDFContent, HL7ContentWithoutPDF);
        }

        /// <summary>
        /// Extracts Base64 embedded PDF content from the specified HL7.
        /// </summary>
        /// <param name="HL7Content">HL7 file content</param>       
        /// <returns>List of PDF bytes, List of Base64 embedded PDF Content, string HL7ContentWithoutPDF</returns>
        private void RemovePDFFromHL7(string HL7Content)
        {
            try
            {
                var regex = new Regex(@"^((OBX\|[0-9]*\|ED\|)|(ZEF\|[0-9]*\|)|(ZTE\|[0-9]*\|ED\|))");
                List<string> lstAllSegments = GetListFromHL7String(HL7Content);
                List<string> lstFilteredSegments = lstAllSegments.Where(x => regex.IsMatch(x)).ToList();
                //Remove PDF content from original HL7 content
                if (lstFilteredSegments.Count > 0)
                {
                    lstFilteredSegments.ForEach(x => lstAllSegments.Remove(x));
                }
                Hl7WithoutPDF = String.Join(Environment.NewLine, lstAllSegments);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Return position of EmbeddedPDFContent
        /// </summary>
        /// <param name="segment">HL7 segment</param>
        /// <returns></returns>
        private string GetPDFContentPosition(string segment)
        {
            string PDFContentPosition = string.Empty;
            try
            {
                string[] fields = segment.Split('|');
                foreach (string field in fields.Skip(2).Where(x => !string.IsNullOrWhiteSpace(x)))
                {
                    string[] components = field.Split('^');
                    foreach (string component in components.Where(x => !string.IsNullOrWhiteSpace(x)))
                    {
                        if (component == "ED")
                            continue;

                        if (ValidatePDFData(component))
                        {
                            PDFContentPosition = segment.Substring(0, 3) + Array.IndexOf(fields, field).ToString() + (components.Length > 1 ? "." + (Array.IndexOf(components, component) + 1).ToString() : string.Empty);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return PDFContentPosition;
        }

        /// <summary>
        /// Check if input string is PDF or not
        /// </summary>
        /// <param name="Data">PDF data</param>
        /// <returns></returns>
        public bool ValidatePDFData(string Data)
        {
            bool isValidPDF = false;
            try
            {
                int iLen = 100;
                if (Data.Length < 100)
                    iLen = Data.Length;
                Data = Data.Substring(0, iLen);
                byte[] arrayPDFByte = Convert.FromBase64String(Data);
                if (Encoding.ASCII.GetString(arrayPDFByte).ToUpper().Contains("%PDF"))
                    isValidPDF = true;
            }
            catch
            {
                isValidPDF = false;
            }
            return isValidPDF;
        }

        /// <summary>
        /// Generate List from HL7string
        /// </summary>
        /// <param name="HL7Content">HL7 file content</param>
        /// <returns></returns>
        public List<string> GetListFromHL7String(string HL7Content)
        {
            List<string> _lstSegment = new List<string>();
            try
            {
                _lstSegment = HL7Content.Replace("\v", string.Empty).Split(new string[] { "\r", "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _lstSegment;
        }

        /// <summary>
        /// Convert Date to Specified Format
        /// </summary>
        /// <param name="Date">Value of the Date</param>
        /// <param name="DateFormat">Expected output date format</param>
        /// <returns></returns>
        public string ConvertDateFormat(string Date, string DateFormat)
        {
            string formattedDate = Date;
            try
            {
                if (!string.IsNullOrWhiteSpace(Date))
                {
                    string[] Validformats = { "yyyyMMdd", "yyyyMMddHHmm", "yyyyMMddHHmmss", "MM/dd/yyyy" };
                    DateTime dateTime = new DateTime();
                    if (DateTime.TryParseExact(Date, Validformats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AdjustToUniversal, out dateTime))
                    {
                        formattedDate = dateTime.ToString(DateFormat);
                    }
                }
            }
            catch (Exception) { }
            return formattedDate;
        }

        /// <summary>
        /// Extracts Broken Line Base64 embedded PDF content from the specified HL7.
        /// </summary>
        /// <param name="HL7Content">HL7 file content</param>       
        /// <returns></returns>
        public Tuple<List<byte[]>, List<string>, string> ExtractPDFWithBrokenLineFromHL7(string HL7Content)
        {
            List<byte[]> lstPDFByte = new List<byte[]>();
            List<string> lstBase64PDFContent = new List<string>();
            string HL7ContentWithoutPDF = string.Empty;
            try
            {
                string PDFContentPosition = string.Empty;
                string PDFContent = string.Empty;
                string BrokenLine = string.Empty;
                string Base64PDFContent = string.Empty;
                int indexBrokenLine = 0;
                StringBuilder _sb = new StringBuilder();
                var regex = new Regex(@"^((OBX\|[0-9]*\|ED\|)|(ZEF\|[0-9]*\|)|(ZTE\|[0-9]*\|ED\|))");
                List<string> lstAllSegment = GetListFromHL7String(HL7Content);
                List<string> lstPDFSegemnt = new List<string>();
                List<string> lstFilteredSegment = lstAllSegment.Where(x => regex.IsMatch(x)).ToList();
                foreach (string segment in lstFilteredSegment)
                {
                    lstPDFSegemnt.Add(segment);
                    //Get PDF content position if not yet available, to be used for subsequent segments
                    if (string.IsNullOrWhiteSpace(PDFContentPosition))
                    {
                        PDFContentPosition = GetPDFContentPosition(segment);
                    }
                    if (!string.IsNullOrWhiteSpace(PDFContentPosition))
                    {
                        PDFContent = GetHL7Value(segment, PDFContentPosition);
                        //If we've already found valid PDF content and a subsequent filtered segment also has valid PDF content then it is another PDF file, save PDF data in lists before overwriting.
                        if (!string.IsNullOrWhiteSpace(_sb.ToString()) && ValidatePDFData(PDFContent))
                        {
                            //Add PDF bytes and Base64 embedded string content to list before overwriting StringBuilder with next valid PDF. 
                            lstPDFByte.Add(Convert.FromBase64String(_sb.ToString().Split('|')[0]));
                            lstBase64PDFContent.Add(_sb.ToString());
                            _sb.Clear();
                        }
                        //-------------------------------------------------------
                        _sb.Append(PDFContent);
                        //---To Consider broken line in PDF content------
                        indexBrokenLine = lstAllSegment.IndexOf(segment) + 1;
                        if (lstAllSegment.Count > indexBrokenLine)
                        {
                            BrokenLine = lstAllSegment[indexBrokenLine];
                            if (!string.IsNullOrWhiteSpace(BrokenLine) && !regex.IsMatch(BrokenLine) && BrokenLine.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Length == 1)
                            {
                                _sb.Append(BrokenLine);
                                lstPDFSegemnt.Add(BrokenLine);
                            }
                        }
                        //------------------------------------------------    
                    }
                }
                //Add PDF bytes and Base64 embedded string content to list
                if (!string.IsNullOrWhiteSpace(_sb.ToString()))
                {
                    lstPDFByte.Add(Convert.FromBase64String(_sb.ToString().Split('|')[0]));
                    lstBase64PDFContent.Add(_sb.ToString());
                    _sb.Clear();
                }
                //--Set default null values if no valid pdf content was found.--
                if (lstPDFByte.Count == 0)
                {
                    lstPDFByte.Add(null);
                }
                if (lstBase64PDFContent.Count == 0)
                {
                    lstBase64PDFContent.Add(null);
                }
                //-------------------------------------------
                //--Remove PDF content from original HL7 content----
                if (lstPDFSegemnt.Count > 0)
                {
                    lstPDFSegemnt.ForEach(x => lstAllSegment.Remove(x));
                }
                //--------------------------------------------------              
                HL7ContentWithoutPDF = string.Join(Environment.NewLine, lstAllSegment);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return new Tuple<List<byte[]>, List<string>, string>(lstPDFByte, lstBase64PDFContent, HL7ContentWithoutPDF);
        }

        /// <summary>
        /// This method will Move file to error folder
        /// </summary>
        /// <param name="SourceFolderLocation">Path of the original input file folder</param>
        /// <param name="ErrorFolderLocation">Path of the error folder</param>
        /// <param name="FileName">Name of the original input file</param>
        /// <param name="ErrorMessage">Error Message</param>        
        /// <returns></returns>
        public void MoveFileToErrorFolder(string SourceFolderLocation, string ErrorFolderLocation, string FileName, string ErrorMessage)
        {
            try
            {
                string SourceFilePath = Path.Combine(SourceFolderLocation, FileName);
                string DestinationFilePath = Path.Combine(ErrorFolderLocation, GetNextFileName(ErrorFolderLocation, Path.GetFileName(FileName)));
                File.Move(SourceFilePath, DestinationFilePath);
                string ErrorFilePath = Path.Combine(ErrorFolderLocation, Path.GetFileNameWithoutExtension(DestinationFilePath) + ".err");
                File.AppendAllText(ErrorFilePath, ErrorMessage);
            }
            catch (Exception ex)
            {
                throw new Exception("MoveFileToErrorFolder() Exception: " + ex.Message);
            }
        }

        /// <summary>
        /// This method will Move file to error folder
        /// </summary>
        /// <param name="SourceFolderLocation">Path of the original input file folder</param>
        /// <param name="ErrorFolderLocation">Path of the error folder</param>
        /// <param name="FileName">Name of the original input file</param>
        /// <param name="ErrorMessage">Error Message</param>
        /// <param name="bWriteErrorFile">Flag which identifies whether or not to write error file in folder</param>
        /// <returns></returns>
        public void MoveFileToErrorFolder(string SourceFolderLocation, string ErrorFolderLocation, string FileName, string ErrorMessage, bool bWriteErrorFile)
        {
            try
            {
                string SourceFilePath = Path.Combine(SourceFolderLocation, FileName);
                string DestinationFilePath = Path.Combine(ErrorFolderLocation, GetNextFileName(ErrorFolderLocation, Path.GetFileName(FileName)));
                File.Move(SourceFilePath, DestinationFilePath);
                if (bWriteErrorFile)
                {
                    string ErrorFilePath = Path.Combine(ErrorFolderLocation, Path.GetFileNameWithoutExtension(DestinationFilePath) + ".err");
                    File.AppendAllText(ErrorFilePath, ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("MoveFileToErrorFolder() Exception: " + ex.Message);
            }
        }

        /// <summary>
        /// Return Datatable from Delimited Text File
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="FileDelimiter"></param>
        /// <returns></returns>
        public DataTable GetDataTableFromDelimitedTextFile(string filePath, string[] FileDelimiter = null)
        {
            DataTable csvData = new DataTable();
            try
            {
                using (TextFieldParser csvReader = new TextFieldParser(filePath))
                {
                    if (FileDelimiter == null)
                    {
                        csvReader.SetDelimiters(new string[] { "," });
                    }
                    else
                    {
                        csvReader.SetDelimiters(FileDelimiter);
                    }
                    csvReader.HasFieldsEnclosedInQuotes = true;
                    //read column names
                    string[] colFields = csvReader.ReadFields();
                    foreach (string column in colFields)
                    {
                        DataColumn datacolumn = new DataColumn(column);
                        csvData.Columns.Add(datacolumn);
                    }
                    while (!csvReader.EndOfData)
                    {
                        string[] fieldData = csvReader.ReadFields();
                        csvData.Rows.Add(fieldData);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetDataTabletFromCSVFile() Exception: " + ex.Message);
            }
            return csvData;
        }

        /// <summary>
        /// Return Datatable from Delimited Text File Content (For PostDownLoad Assembly)
        /// </summary>
        /// <param name="fileBytes"></param>
        /// <param name="FileDelimiter"></param>
        /// <returns></returns>
        public DataTable GetDataTableFromDelimitedTextFileContent(byte[] fileBytes, string[] FileDelimiter = null)
        {
            DataTable csvData = new DataTable();
            try
            {
                using (MemoryStream stream = new MemoryStream(fileBytes))
                {
                    using (TextFieldParser csvReader = new TextFieldParser(stream))
                    {
                        if (FileDelimiter == null)
                        {
                            csvReader.SetDelimiters(new string[] { "," });
                        }
                        else
                        {
                            csvReader.SetDelimiters(FileDelimiter);
                        }
                        csvReader.HasFieldsEnclosedInQuotes = true;
                        //read column names
                        string[] colFields = csvReader.ReadFields();
                        foreach (string column in colFields)
                        {
                            DataColumn datacolumn = new DataColumn(column);
                            csvData.Columns.Add(datacolumn);
                        }
                        while (!csvReader.EndOfData)
                        {
                            string[] fieldData = csvReader.ReadFields();
                            csvData.Rows.Add(fieldData);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("GetDataTabletFromCSVFile() Exception: " + ex.Message);
            }
            return csvData;
        }

        /// <summary>
        /// This method will Write file to error folder (For PostDownload Assembly)
        /// </summary>
        /// <param name="fileBytes">File content in Byte Array</param>
        /// <param name="ErrorFolderLocation">Path of the error folder</param>
        /// <param name="FileName">Name of the original input file</param>
        /// <param name="ErrorMessage">Error Message</param>
        /// <param name="bWriteErrorFile">Flag which identifies whether or not to write error file in folder</param>
        /// <returns></returns>
        public void WriteFileToErrorFolder(byte[] fileBytes, string ErrorFolderLocation, string FileName, string ErrorMessage, bool bWriteErrorFile = true)
        {
            try
            {
                string DestinationFilePath = Path.Combine(ErrorFolderLocation, GetNextFileName(ErrorFolderLocation, Path.GetFileName(FileName)));
                File.WriteAllBytes(DestinationFilePath, fileBytes);
                if (bWriteErrorFile)
                {
                    string ErrorFilePath = Path.Combine(ErrorFolderLocation, Path.GetFileNameWithoutExtension(DestinationFilePath) + ".err");
                    File.AppendAllText(ErrorFilePath, ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("WriteFileToErrorFolder() Exception: " + ex.Message);
            }
        }

        public string WriteFileToErrorFolderOrCreate(byte[] fileBytes, string errorFolderLocation, string fileName, string errorMessage, bool writeErrorFile = true)
        {
            string errorWritingFile = "";

            try
            {
                string destinationFilePath = Path.Combine(errorFolderLocation, GetNextFileName(errorFolderLocation, Path.GetFileName(fileName)));

                if (!Directory.Exists(errorFolderLocation))
                {
                    Directory.CreateDirectory(errorFolderLocation);
                }

                File.WriteAllBytes(destinationFilePath, fileBytes);

                if (writeErrorFile)
                {
                    string errorFilePath = Path.Combine(errorFolderLocation, Path.GetFileNameWithoutExtension(destinationFilePath) + ".err");
                    File.AppendAllText(errorFilePath, errorMessage);
                }
            }
            catch (Exception ex)
            {
                errorWritingFile = ex.ToString();
            }

            return errorWritingFile;
        }
    }

    #region ResultDetails
    public class ResultDetails
    {
        public int Status { get; set; } = LKTransferResponseStatus.Success;
        public string ErrorMessage { get; set; } = string.Empty;
        public string DebugLog { get; set; } = string.Empty;
        public byte[] Body { get; set; } = null;

        public class LKTransferResponseStatus
        {
            /// <summary>
            /// Marks file as success/delivered 
            /// </summary>
            public const int Success = 2;

            /// <summary>
            /// Marks file as errored/moves file to error bucket in LKTransfer dashboard 
            /// </summary>
            public const int Error = 3;

            /// <summary>
            /// Marks file as "transient-error". Use ONLY when retrying the file will result in a success 
            /// Marking a file as "transient-error" causes LKTransfer to hold the message queue and keep retrying the message indefinitely until a non-transient response is returned.
            /// </summary>
            public const int TransientError = 4;

            /// <summary>
            /// Marks file as filtered message
            /// </summary>
            public const int FilteredMessage = 9;
        }
    }
    #endregion
}
