using System;
using System.Data;
using System.Reflection;
using System.Windows.Forms;

namespace TestAssembly
{
    public partial class frmConfigureAssemblies : Form
    {
        #region " Declarations "

        private object oProperties;
        private Type oType;

        // Flag for oProperties is changed or not means IsDirty
        private bool IsDirty = false;

        private System.Data.DataRow _oRowConfig = null;

        // Const for oTable DataColumn
        private const string c_ID = "ID";

        private const string c_AssemblyName = "AssemblyName";
        private const string c_AssemblyFile = "AssemblyFile";
        private const string c_ProcessType = "ProcessType";
        private const string c_ExecuteOrder = "ExecuteOrder";
        internal System.Data.DataTable oTable;
        internal System.Data.DataView oDataView;
        private string _Filename;
        private string _fileContent;

        #endregion " Declarations "

        #region " Exposed Property "

        /// <summary>
        /// Main Configuration Row
        /// </summary>
        /// <value>DataRow</value>
        /// <returns></returns>
        /// <remarks></remarks>
        public System.Data.DataRow oRowConfig
        {
            get { return _oRowConfig; }
            set { _oRowConfig = value; }
        }

        #endregion " Exposed Property "

        public frmConfigureAssemblies()
        {
            InitializeComponent();
        }

        #region " Control Events "

        /// <summary>
        /// Handle form Closing event and Save main configuration.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void frmConfigureAssemblies_Load(object sender, EventArgs e)
        {
            FillControls();
        }

        /// <summary>
        /// Handle form Load event and Fill controls on form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void frmConfigureAssemblies_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveMainConfig();
        }

        /// <summary>
        /// Handle Click event of Button Cancel and Close form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// Add asembly entry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void btnAddAssembly_Click(object sender, EventArgs e)
        {
            string lcAssembly = this.AddAssembly();
            if (lcAssembly.Length > 0)
            {
                System.Data.DataRowView drv = oDataView.AddNew();
                drv[c_ID] = oTable.Rows.Count;
                string AssemblyName = System.IO.Path.GetFileNameWithoutExtension(lcAssembly);
                drv[c_AssemblyName] = AssemblyName;
                drv[c_AssemblyFile] = lcAssembly;
                drv[c_ExecuteOrder] = oDataView.Count;
                drv[c_ProcessType] = ddlProcessType.SelectedItem.ToString();
                drv.EndEdit();
                //Me.lstAssembly.Items.Add(lcAssembly)
                lstAssembly.SelectedValue = int.Parse(drv[c_ID].ToString());
            }
        }

        /// <summary>
        /// Handle SelectedIndexChanged event of ListView lvwAssembly and Show Properties
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void ddlProcessType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (lstAssembly.SelectedItem == null)
                    return;
                btnDown.Enabled = lstAssembly.SelectedIndex != (lstAssembly.Items.Count - 1);
                btnUp.Enabled = lstAssembly.SelectedIndex != 0;
                btnRemoveAssembly.Enabled = lstAssembly.SelectedItem != null;
                //Load Assembly file
                this.ShowProperties((DataRowView)lstAssembly.SelectedItem);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// Handle Click event of Button Up and move execute order up.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void btnUp_Click(object sender, EventArgs e)
        {
            this.MoveUp();
            btnDown.Enabled = lstAssembly.SelectedIndex != (lstAssembly.Items.Count - 1);
            btnUp.Enabled = lstAssembly.SelectedIndex != 0;
        }

        /// <summary>
        /// Handle Click event of Button Down and move execute order down.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void btnDown_Click(object sender, EventArgs e)
        {
            this.MoveDown();
            btnDown.Enabled = lstAssembly.SelectedIndex != (lstAssembly.Items.Count - 1);
            btnUp.Enabled = lstAssembly.SelectedIndex != 0;
        }

        /// <summary>
        /// Remove an assembly entry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void btnRemoveAssembly_Click(object sender, EventArgs e)
        {
            // Remove Record from Table
            if (lstAssembly.SelectedIndex == -1)
                return;
            int iID = int.Parse(lstAssembly.SelectedValue.ToString());
            foreach (System.Data.DataRowView oRow in oDataView)
            {
                if (int.Parse(oRow[c_ID].ToString()) == iID)
                {
                    // Delete Configuration file for this Assembly
                    string AccountNo = _oRowConfig["UserID"].ToString();
                    string InterfaceType = _oRowConfig["InterfaceType"].ToString();
                    string ProcessType = oRow[c_ProcessType].ToString();
                    string AssemblyFile = oRow[c_AssemblyFile].ToString();
                    DeleteProcessAssemblyConfigFile(ProcessType, AccountNo, InterfaceType, AssemblyFile);
                    // Delete Config Record
                    oRow.Delete();
                    break; // TODO: might not be correct. Was : Exit For
                }
            }
        }

        /// <summary>
        /// Save Properties of assembly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void prpPropertiesList_Leave(object sender, EventArgs e)
        {
            try
            {
                if (IsDirty)
                {
                    oType.InvokeMember("Save", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public, null, oProperties, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error during Save", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Update IsDirty flag as property value is changed.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void prpPropertiesList_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            IsDirty = true;
        }

        /// <summary>
        /// Call the Execute method of an Assembly for test
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void btnExecute_Click(object sender, EventArgs e)
        {
            DataTable objTable = new DataTable("TestTable");

            objTable.Columns.Add("SourceLocation", Type.GetType("System.String"));
            objTable.Columns.Add("FileName", Type.GetType("System.String"));
            objTable.Columns.Add("Delivered", Type.GetType("System.Object"));
            objTable.Columns.Add("Bytes", Type.GetType("System.Int32"));
            objTable.Columns.Add("TrxFileID", Type.GetType("System.Int32"));

            DataRow objRow = objTable.NewRow();

            objRow["SourceLocation"] = "";
            objRow["FileName"] = _Filename;
            //"abc.txt"
            objRow["Delivered"] = StrToByteArray(_fileContent);
            objRow["Bytes"] = "File Data".Length;
            objRow["TrxFileID"] = 101;

            try
            {
                if (_oRowConfig != null && oProperties != null)
                {
                    if (oType != null && oProperties != null)
                    {
                        if (ddlProcessType.Text.Equals("Before Pickup Processing"))
                        {
                            oType.InvokeMember("Execute", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public, null, oProperties, new object[] { _oRowConfig });
                        }
                        else
                        {
                            oType.InvokeMember("Execute", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public, null, oProperties, new object[] { _oRowConfig, objRow });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error executing \"Execute\"", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }

            MessageBox.Show("Assembly execution complete.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void btnLoadDataFile_Click(object sender, EventArgs e)
        {
            try
            {
                string sFileName = General.GetFileName("");
                _Filename = sFileName;
                if (sFileName.Length > 0)
                {
                    _fileContent = System.IO.File.ReadAllText(sFileName);
                    txtDataContent.Text = System.IO.File.ReadAllText(_fileContent);
                }
            }
            catch (Exception ex)
            {
            }
        }

        #endregion " Control Events "

        #region " Local Function Sub "

        /// <summary>
        /// Fill Controls on Form
        /// </summary>
        /// <remarks></remarks>
        private void FillControls()
        {
            try
            {
                _oRowConfig = General.GetDataRows(1)[0];
                if (_oRowConfig == null)
                {
                    MessageBox.Show("No Configuration Record is provided therefore Can't Show any Data.", "Configuration Error", MessageBoxButtons.OK);
                    this.Close();
                    //Exit Sub
                }
                // Prepare Table to hold Temp Data
                oTable = CreateTable();
                // Before Pickup Processing
                InsertRows(_oRowConfig[General._cPickupPreProcesses].ToString(), General._cBeforePickupProcessing);
                // After Pickup Processing
                InsertRows(_oRowConfig[General._cPickupPostProcesses].ToString(), General._cAfterPickupProcessing);
                // After Download Delivery Processing
                InsertRows(_oRowConfig[General._cDeliveryPostDownloadProcesses].ToString(), General._cAfterDownloadDeliveryProcessing);

                //Me.lstAssembly.BeginUpdate()

                this.oDataView = oTable.DefaultView;
                this.oDataView.Sort = "ExecuteOrder";

                this.lstAssembly.DataSource = null;
                lstAssembly.Items.Clear();
                lstAssembly.DataSource = this.oDataView;
                lstAssembly.DisplayMember = "AssemblyName";
                // "DisplayItem"
                lstAssembly.ValueMember = "ID";
                // "ValueItem"
                //Me.ApplyFilter()
                ddlProcessType.SelectedIndex = 0;
                txtDataContent.Text = "MSH|^~\\&|SHIEL|SHIEL|H_Dx|999994|200801221330||ORU^R01|SH9082422|D|2.3|" + "\r\n" + "PID|||||Mccrae^Tina||19610423|F|||1520 CARROLL ST.APT.1-K^^Brooklyn^NY^11213||7187713284||||||079600370||||||||||||KADU039213|MRN1256884|" + "\r\n" + "IN1|1||0||||||||||||||0|||||||||||||||||||||||||||" + "\r\n" + "ORC|RE|55965-20080122-1|4273342||CM||||200801220001|715^Tirona^Mila||-2^John,Jr^Weber|825004^Cont Adm-Bk-ADU-VNS|" + "\r\n" + "OBR|1|55965-20080122-1||90204^Prothrombin Time^^PROTIME|||200801221100|||||||200801221330||-2^John,Jr^Weber|||||||||F||||||||||||||||" + "\r\n" + "OBX|1|NM|2204^Prothrombin Time^^PROTIME||15.9|Seconds|9.9 - 12.9|H|||F|||200801221316||9995^^AutoApprover||||||200801221330|" + "\r\n" + "NTE|1||Reference range reflects Non-Anticoagulated patients|" + "\r\n" + "OBX|2|NM|2102^INR^^INR||1.90|Ratio|2.0 - 3.0|L|||F|||200801221316||9995^^AutoApprover||||||200801221330|" + "\r\n" + "NTE|1||Recommended therapeutic range: 2.0 - 3.0 Acute M.I., prophylaxis & treatment of venous thrombosis, pulmonary embolism, tissue heart valve, atrial fibrillation, valvular heart disease, prevention of systemic embolism. 2.5 - 3.5 Mechanical Heart Valve.|" + "\r\n" + "NTE|2||An INR reference interval of 0.8-1.3 is applicable to patients not receiving anti-coagulant medication.|";

                //Me.lstAssembly.EndUpdate()
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// Apply Row Filter
        /// </summary>
        /// <remarks>ListView displays oDataView rows</remarks>
        private void ApplyFilter()
        {
            try
            {
                this.oDataView.RowFilter = "ProcessType = '" + this.ddlProcessType.SelectedItem.ToString().Trim() + "'";
            }
            catch (Exception ex)
            {
                //
            }
        }

        /// <summary>
        /// Add new process assembly to List
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private string AddAssembly()
        {
            string _LoadingAssembly = "";
            try
            {
                _LoadingAssembly = AssemblyHelper.GetAssemblyFileName("");
                if (_LoadingAssembly.Length == 0)
                {
                    prpPropertiesList.SelectedObject = null;
                    //Me.DialogResult = Windows.Forms.DialogResult.Cancel
                    //Me.Close()
                    return "";
                }

                if (AssemblyHelper.isValidAssembly(_LoadingAssembly) == false)
                {
                    MessageBox.Show("The file selected is not valid. Please specify valid assembly file.", "Invalid Assembly", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                    _LoadingAssembly = "";
                    //break; // TODO: might not be correct. Was : Exit Try
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error :", MessageBoxButtons.OK, MessageBoxIcon.Stop, MessageBoxDefaultButton.Button1);
            }
            return _LoadingAssembly;
        }

        /// <summary>
        /// Show Properties of an assembly
        /// </summary>
        /// <param name="oRow">Assembly information Row</param>
        /// <remarks></remarks>
        private void ShowProperties(DataRowView oRow)
        {
            string AssemblyName = oRow[c_AssemblyFile].ToString();
            string ProcessType = oRow[c_ProcessType].ToString();
            string AccountNo = _oRowConfig["UserID"].ToString();
            string InterfaceType = _oRowConfig["InterfaceType"].ToString();

            try
            {
                oType = AssemblyHelper.GetValidAssemblyType(AssemblyName);
                if (oType != null)
                {
                    oProperties = Activator.CreateInstance(oType);
                    PropertyInfo ConfigFile = oType.GetProperty("ConfigFileName");
                    ConfigFile.SetValue(oProperties, General.GetProcessAssemblyConfigFileName(ProcessType, AccountNo, InterfaceType, AssemblyName), null);
                }
            }
            catch (Exception ex)
            {
            }
            if (oProperties != null)
            {
                txtAssemblyInfo.Text = AssemblyHelper.GetAssemblyInformation(AssemblyName);
            }

            prpPropertiesList.SelectedObject = oProperties;
            prpPropertiesList.PropertySort = PropertySort.CategorizedAlphabetical;
            prpPropertiesList.ExpandAllGridItems();
            IsDirty = false;
            // Reset flag
        }

        /// <summary>
        /// Create Temp Table to hold assembly information
        /// </summary>
        /// <returns>Empty Table</returns>
        /// <remarks></remarks>
        private System.Data.DataTable CreateTable()
        {
            System.Data.DataTable oTable = new System.Data.DataTable("AssemblyList");
            try
            {
                var _with1 = oTable.Columns;
                System.Data.DataColumn oColumn = null;
                oColumn = new System.Data.DataColumn(c_ID, typeof(int));
                oColumn.DefaultValue = 0;
                _with1.Add(oColumn);
                oColumn = new System.Data.DataColumn(c_AssemblyName, typeof(string));
                oColumn.DefaultValue = "";
                _with1.Add(oColumn);
                oColumn = new System.Data.DataColumn(c_AssemblyFile, typeof(string));
                oColumn.DefaultValue = "";
                _with1.Add(oColumn);
                oColumn = new System.Data.DataColumn(c_ProcessType, typeof(string));
                oColumn.DefaultValue = "";
                _with1.Add(oColumn);
                oColumn = new System.Data.DataColumn(c_ExecuteOrder, typeof(int));
                oColumn.DefaultValue = 0;
                _with1.Add(oColumn);
            }
            catch (Exception ex)
            {
            }
            return oTable;
        }

        /// <summary>
        /// Insert Rows to Temp table
        /// </summary>
        /// <param name="AssemblyNameCSV">Assembly name value pair in CSV format</param>
        /// <param name="ProcessType">Specifies Type of Process</param>
        /// <returns></returns>
        /// <remarks></remarks>
        private object InsertRows(string AssemblyNameCSV, string ProcessType)
        {
            try
            {
                string[] AssemblyAr = AssemblyNameCSV.Split(';');
                foreach (string sAssembly in AssemblyAr)
                {
                    if (sAssembly.Length > 0)
                    {
                        System.Data.DataRow oRow = oTable.NewRow();
                        oRow[c_ID] = oTable.Rows.Count;
                        oRow[c_AssemblyName] = System.IO.Path.GetFileNameWithoutExtension(sAssembly);
                        oRow[c_AssemblyFile] = sAssembly;
                        oRow[c_ProcessType] = ProcessType;
                        oRow[c_ExecuteOrder] = oTable.Rows.Count;
                        oTable.Rows.Add(oRow);
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return true;
        }

        /// <summary>
        /// Moves Row to Up position
        /// </summary>
        /// <remarks></remarks>
        private void MoveUp()
        {
            this.SuspendLayout();
            // Get a reference to the current row
            int ItemID = int.Parse(Convert.ToString(lstAssembly.SelectedValue));

            DataRowView oRow = null;
            //Dim oLastRow As DataRowView = Nothing
            DataRowView oPrevRow = null;

            foreach (DataRowView oRow_loopVariable in this.oDataView)
            {
                oRow = oRow_loopVariable;
                // Check if the row if the first row. If yes, then exit
                if (oPrevRow == null & int.Parse(oRow["ID"].ToString()) == ItemID)
                {
                    break; // TODO: might not be correct. Was : Exit For
                }

                //If we reach the current item then swap the order values of LastRow and Row
                if (int.Parse(oRow["ID"].ToString()) == ItemID & oPrevRow != null)
                {
                    //We reached the row we wish to change
                    //Swap the ExecuteOrder
                    int liCurrentOrder = int.Parse(oRow["ExecuteOrder"].ToString());
                    lstAssembly.SelectedIndexChanged -= lstAssembly_SelectedIndexChanged;
                    oRow["ExecuteOrder"] = oPrevRow["ExecuteOrder"];
                    oPrevRow["ExecuteOrder"] = liCurrentOrder;
                    this.oDataView.Table.AcceptChanges();
                    lstAssembly.SelectedIndexChanged += lstAssembly_SelectedIndexChanged;
                    this.lstAssembly.SelectedValue = ItemID;
                    break; // TODO: might not be correct. Was : Exit For
                }
                else
                {
                    //Store the last row
                    oPrevRow = oRow;
                }
            }
            this.ResumeLayout();
        }

        /// <summary>
        /// Moves Row to Down position
        /// </summary>
        /// <remarks></remarks>
        private void MoveDown()
        {
            this.SuspendLayout();

            // Get a reference to the current row
            int ItemID = int.Parse(this.lstAssembly.SelectedValue.ToString());

            DataRowView oRow = null;
            DataRowView oSelectedRow = null;

            foreach (DataRowView oRow_loopVariable in this.oDataView)
            {
                oRow = oRow_loopVariable;
                //If we reach the current item then swap the order values of LastRow and Row
                if (oSelectedRow != null)
                {
                    //We reached the row we wish to change
                    //Swap the ExecuteOrder
                    int liCurrentOrder = int.Parse(oRow["ExecuteOrder"].ToString());
                    int liSelectedOrder = int.Parse(oSelectedRow["ExecuteOrder"].ToString());
                    lstAssembly.SelectedIndexChanged -= lstAssembly_SelectedIndexChanged;
                    oSelectedRow["ExecuteOrder"] = liCurrentOrder;
                    oRow["ExecuteOrder"] = liSelectedOrder;
                    // oSelectedRow("ExecuteOrder")
                    this.oDataView.Table.AcceptChanges();
                    this.oDataView.Sort = "ExecuteOrder";
                    lstAssembly.SelectedIndexChanged += lstAssembly_SelectedIndexChanged;
                    this.lstAssembly.SelectedValue = ItemID;
                    break; // TODO: might not be correct. Was : Exit For
                }
                else if (int.Parse(oRow["ID"].ToString()) == ItemID)
                {
                    //Store the Selected row
                    oSelectedRow = oRow;
                }
            }
            this.ResumeLayout();
        }

        /// <summary>
        /// Save Main Configuration file
        /// </summary>
        /// <remarks></remarks>
        private void SaveMainConfig()
        {
            try
            {
                // Get ID of the Row
                int iID = int.Parse(_oRowConfig["ID"].ToString());
                _oRowConfig[General._cPickupPreProcesses] = GetProcessTypeProperty(General._cBeforePickupProcessing);
                _oRowConfig[General._cPickupPostProcesses] = GetProcessTypeProperty(General._cAfterPickupProcessing);
                _oRowConfig[General._cDeliveryPostDownloadProcesses] = GetProcessTypeProperty(General._cAfterDownloadDeliveryProcessing);
                _oRowConfig.Table.WriteXml(General.GetXmlDataFile(), XmlWriteMode.WriteSchema);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Prepares Property of main configuration file ProcessAssemblies
        /// </summary>
        /// <param name="ProcessType">Specifies type of process</param>
        /// <returns>String having list of assemblies in coma separated values.</returns>
        /// <remarks></remarks>
        private string GetProcessTypeProperty(string ProcessType)
        {
            System.Data.DataView oView = oTable.DefaultView;
            string @out = "";
            oView.Sort = c_ExecuteOrder;
            this.oDataView.RowFilter = "ProcessType = '" + ProcessType + "'";
            if (oView.Count == 0)
                return "";
            foreach (System.Data.DataRowView oRow in oView)
            {
                @out += oRow[c_AssemblyFile].ToString() + ";";
            }
            return @out;
        }

        /// <summary>
        /// Remove Process assembly configuration file.
        /// </summary>
        /// <param name="ProcessType">Type of process</param>
        /// <param name="AccountNo">Account #</param>
        /// <param name="InterfaceType">Type of Interface</param>
        /// <param name="AssemblyFileName">Name of an Assembly file</param>
        /// <remarks></remarks>
        private void DeleteProcessAssemblyConfigFile(string ProcessType, string AccountNo, string InterfaceType, string AssemblyFileName)
        {
            try
            {
                string ConfigFile = General.GetProcessAssemblyConfigFileName(ProcessType, AccountNo, InterfaceType, AssemblyFileName);
                if (System.IO.File.Exists(ConfigFile))
                    System.IO.File.Delete(ConfigFile);
            }
            catch (Exception ex)
            {
                //
            }
        }

        /// <summary>
        /// To convert a string to a byte array
        /// </summary>
        /// <param name="str">String for conversion</param>
        /// <returns>Converted Array of Bytes</returns>
        /// <remarks></remarks>
        public static byte[] StrToByteArray(string str)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            try
            {
                if (str == null)
                    return encoding.GetBytes("");
                return encoding.GetBytes(str);
            }
            catch (Exception ex)
            {
                return encoding.GetBytes("");
            }
        }

        #endregion " Local Function Sub "

        /// <summary>
        /// Handle SelectedIndexChanged event of ListView lvwAssembly and Show Properties
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks></remarks>
        private void lstAssembly_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (lstAssembly.SelectedItem == null)
                    return;
                btnDown.Enabled = lstAssembly.SelectedIndex != (lstAssembly.Items.Count - 1);
                btnUp.Enabled = lstAssembly.SelectedIndex != 0;
                btnRemoveAssembly.Enabled = lstAssembly.SelectedItem != null;
                //Load Assembly file
                this.ShowProperties((DataRowView)lstAssembly.SelectedItem);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}