using System;
using System.Data;
using System.Windows.Forms;

namespace TestAssembly
{
    static class General
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmConfigureAssemblies());
        }

        public static string ServiceStatus = "";
        public static string DebugFileName = Application.StartupPath + "\\LKDebug.FLG";

        public static bool DataSetValidated = false;

        // Used in DropDown ddlProcessType
        public const string _cBeforePickupProcessing = "Before Pickup Processing";

        public const string _cAfterPickupProcessing = "After Pickup Processing";

        public const string _cAfterDownloadDeliveryProcessing = "After Download Processing";

        // Column name of ConfigDataXML
        public const string _cPickupPreProcesses = "PickupPreProcesses";

        public const string _cPickupPostProcesses = "PickupPostProcesses";

        public const string _cDeliveryPostDownloadProcesses = "DeliveryPostDownloadProcesses";

        /// <summary>
        /// Checks for file is in use by other application
        /// </summary>
        /// <param name="FileName">Specifies path and file name</param>
        /// <returns>Returns True if file is in use</returns>
        /// <remarks></remarks>
        public static bool FileInUse(string FileName)
        {
            System.IO.FileStream fs = null;
            try
            {
                fs = System.IO.File.Open(FileName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.None);
                return false;
            }
            catch (System.IO.IOException ex)
            {
                return true;
            }
            finally
            {
                if ((fs != null))
                {
                    fs.Close();
                }
            }
        }

        /// <summary>
        /// Get Configuration file name used to Save DataSet in XML
        /// </summary>
        /// <returns>File name with execution path</returns>
        /// <remarks></remarks>
        public static string GetXmlDataFile()
        {
            return System.IO.Path.Combine(Application.StartupPath, "LKTransferConfigData.Xml");
        }

        public static System.Data.DataSet GetConfigDataSet()
        {
            try
            {
                System.Data.DataSet ds = new System.Data.DataSet();
                ds.ReadXml(GetXmlDataFile(), XmlReadMode.Auto);

                //Check if the dataset is validated
                General.ValidateDataSet(ref ds);

                return ds;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static void ValidateDataSet(ref System.Data.DataSet toDataSet)
        {
            if (General.DataSetValidated == true)
            {
                return;
            }

            // Check the dataset for new fields
            bool lUpdatedFields = false;
            General.CheckField(ref toDataSet, "PickupPreProcesses", typeof(string), ref lUpdatedFields);
            General.CheckField(ref toDataSet, "PickupPostProcesses", typeof(string), ref lUpdatedFields);
            General.CheckField(ref toDataSet, "DeliveryPostDownloadProcesses", typeof(string), ref lUpdatedFields);
            General.CheckField(ref toDataSet, "PickupSFTPURL", typeof(string), ref lUpdatedFields);
            General.CheckField(ref toDataSet, "PickupSFTPUserID", typeof(string), ref lUpdatedFields);
            General.CheckField(ref toDataSet, "PickupSFTPPassword", typeof(string), ref lUpdatedFields);
            General.CheckField(ref toDataSet, "PickupSFTPPort", typeof(string), ref lUpdatedFields);
            General.CheckField(ref toDataSet, "PickupSFTPFolder", typeof(string), ref lUpdatedFields);
            General.CheckField(ref toDataSet, "PickupSFTPFileExtension", typeof(string), ref lUpdatedFields);

            General.CheckField(ref toDataSet, "DeliverySFTPURL", typeof(string), ref lUpdatedFields);
            General.CheckField(ref toDataSet, "DeliverySFTPUserID", typeof(string), ref lUpdatedFields);
            General.CheckField(ref toDataSet, "DeliverySFTPPassword", typeof(string), ref lUpdatedFields);
            General.CheckField(ref toDataSet, "DeliverySFTPPort", typeof(string), ref lUpdatedFields);
            General.CheckField(ref toDataSet, "DeliverySFTPFolder", typeof(string), ref lUpdatedFields);
            General.CheckField(ref toDataSet, "DeliverySFTPFileExtension", typeof(string), ref lUpdatedFields);

            //If any fields have been updated, save the dataset first to the file
            if (lUpdatedFields == true)
            {
                toDataSet.Tables[0].WriteXml(General.GetXmlDataFile(), XmlWriteMode.WriteSchema);
            }

            //Set the flag that the dataset is now validated
            General.DataSetValidated = true;
        }

        public static void CheckField(ref System.Data.DataSet toDataSet, string tcField, System.Type dataType, ref bool tlUpdated)
        {
            if (toDataSet.Tables[0].Columns[tcField] == null)
            {
                //Add the column
                System.Data.DataColumn oColumn = new System.Data.DataColumn(tcField, dataType);
                oColumn.DefaultValue = "";
                toDataSet.Tables[0].Columns.Add(oColumn);
                tlUpdated = true;
            }
        }

        public static System.Data.DataRow[] GetDataRows(int ID)
        {
            try
            {
                // Read Data from XML file Create DataSet and Get Table
                System.Data.DataTable oTable = GetConfigDataSet().Tables[0];
                // Filter record
                string strExpr = "ID = " + ID.ToString();
                string strSort = "ID";
                DataRow[] oRow = null;
                // Use the Select method to find ID.
                oRow = oTable.Select(strExpr, strSort);
                return oRow;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static object DBNullToDefault(object oValue, object DefaultValue)
        {
            return (Convert.ToString(oValue).Trim() == string.Empty ? DefaultValue : oValue);
        }

        public static string GetProcessAssemblyConfigFileName(string ProcessType, string AccountNo, string InterfaceType, string AssemblyFileName)
        {
            try
            {
                //Remove Config file from: Current Folder\Pickup Pre-Processing\<AccountNo>_<InterfaceType>_<AssemblyName>.xml
                //Dim AccountNo As String = oDataRow("UserID").ToString
                //Dim InterfaceType As String = oDataRow("InterfaceType").ToString
                string AssemblyName = System.IO.Path.GetFileNameWithoutExtension(AssemblyFileName);
                string ConfigPath = "";

                if (ProcessType == _cBeforePickupProcessing | ProcessType == _cPickupPreProcesses)
                {
                    ConfigPath = Application.StartupPath + "\\Pickup Pre-Processing\\";
                }
                else if (ProcessType == _cAfterPickupProcessing | ProcessType == _cPickupPostProcesses)
                {
                    ConfigPath = Application.StartupPath + "\\Pickup Post-Processing\\";
                }
                else if (ProcessType == _cAfterDownloadDeliveryProcessing | ProcessType == _cDeliveryPostDownloadProcesses)
                {
                    ConfigPath = Application.StartupPath + "\\Delivery PostDownload-Processing\\";
                }
                //Dim ConfigFile As String = System.IO.Path.Combine(ConfigPath, AccountNo + "_" + InterfaceType.Replace(" ", "-") + "_" + AssemblyName + ".xml")
                string ConfigFile = System.IO.Path.Combine(ConfigPath, AccountNo.Trim() + "_" + InterfaceType.Replace(" ", "") + "_" + AssemblyName + ".xml").Trim();
                return ConfigFile;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        /// <summary>
        /// Show OpenFileDialog for All files and return selected file
        /// </summary>
        /// <returns>Valid file name</returns>
        /// <remarks></remarks>
        public static string GetFileName(string sFileName)
        {
            try
            {
                OpenFileDialog oOpenFileDialog = new OpenFileDialog();
                // Open File Dialog
                oOpenFileDialog.FileName = sFileName;
                oOpenFileDialog.CheckFileExists = true;
                oOpenFileDialog.Filter = "All Files|*.*;";
                oOpenFileDialog.InitialDirectory = Application.StartupPath;
                oOpenFileDialog.RestoreDirectory = true;
                oOpenFileDialog.SupportMultiDottedExtensions = true;
                oOpenFileDialog.Title = "Select File";
                if (oOpenFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    return oOpenFileDialog.FileName;
                }
            }
            catch (Exception ex)
            {
            }
            return "";
        }
    }
}