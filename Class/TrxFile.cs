using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostDnLKCloudFhirR4Api.Class
{
    class TrxFile
    {
        // Properties
        internal byte[] Content;
        internal string FileName;
        internal string FileFolder;
        internal int Length;
        internal long TrxFileID;
        public TrxFile(System.Data.DataRow oRow)
        {
            try
            {
                FillData(oRow);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void FillData(System.Data.DataRow oRow)
        {
            ///'''''''''''''''''''''''''''''''''''''''
            // Variable     Field Name of oRowTrxFile
            // FileFolder   SourceLocation
            // FileName     FileName
            // Content      Delivered
            // Length       Bytes
            // TrxFileID    TrxFileID
            ///'''''''''''''''''''''''''''''''''''''''
            // Get data from oRow
            //General _objGeneral = new General();
            HL7UtilV2.HL7Util _objHL7Util = new HL7UtilV2.HL7Util();
            try
            {
                FileFolder = oRow["SourceLocation"].ToString();
                FileName = oRow["FileName"].ToString();
                Content = (byte[])(_objHL7Util.DBNullToDefault(oRow["Delivered"], null));
                Length = System.Convert.ToInt32("0" + _objHL7Util.DBNullToDefault(oRow["Bytes"], 0).ToString());
                TrxFileID = Int64.Parse(oRow["TrxFileID"].ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _objHL7Util = null;
            }
        }
    }
}
