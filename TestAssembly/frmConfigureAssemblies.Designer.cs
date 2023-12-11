namespace TestAssembly
{
    partial class frmConfigureAssemblies
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.Label7 = new System.Windows.Forms.Label();
            this.Label6 = new System.Windows.Forms.Label();
            this.Label5 = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.Label4 = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.txtDataContent = new System.Windows.Forms.TextBox();
            this.btnLoadDataFile = new System.Windows.Forms.Button();
            this.btnRemoveAssembly = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnExecute = new System.Windows.Forms.Button();
            this.SplitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnAddAssembly = new System.Windows.Forms.Button();
            this.lstAssembly = new System.Windows.Forms.ListBox();
            this.txtAssemblyInfo = new System.Windows.Forms.TextBox();
            this.ddlProcessType = new System.Windows.Forms.ComboBox();
            this.prpPropertiesList = new System.Windows.Forms.PropertyGrid();
            this.GroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer1)).BeginInit();
            this.SplitContainer1.Panel1.SuspendLayout();
            this.SplitContainer1.Panel2.SuspendLayout();
            this.SplitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // GroupBox1
            // 
            this.GroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupBox1.Controls.Add(this.Label1);
            this.GroupBox1.Controls.Add(this.Label7);
            this.GroupBox1.Controls.Add(this.Label6);
            this.GroupBox1.Controls.Add(this.Label5);
            this.GroupBox1.Controls.Add(this.Label3);
            this.GroupBox1.Controls.Add(this.Label4);
            this.GroupBox1.Controls.Add(this.Label2);
            this.GroupBox1.Location = new System.Drawing.Point(20, 6);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Size = new System.Drawing.Size(716, 85);
            this.GroupBox1.TabIndex = 17;
            this.GroupBox1.TabStop = false;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label1.ForeColor = System.Drawing.SystemColors.Desktop;
            this.Label1.Location = new System.Drawing.Point(18, 11);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(656, 14);
            this.Label1.TabIndex = 19;
            this.Label1.Text = "Advanced configuration: Allows specifying pre-processing and post-processing on i" +
    "ncoming and outgoing messages";
            // 
            // Label7
            // 
            this.Label7.AutoSize = true;
            this.Label7.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label7.Location = new System.Drawing.Point(18, 62);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(163, 14);
            this.Label7.TabIndex = 18;
            this.Label7.Text = "3. After Delivery Processing:";
            // 
            // Label6
            // 
            this.Label6.AutoSize = true;
            this.Label6.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label6.Location = new System.Drawing.Point(18, 48);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(156, 14);
            this.Label6.TabIndex = 17;
            this.Label6.Text = "2. After Pickup Processing:";
            // 
            // Label5
            // 
            this.Label5.AutoSize = true;
            this.Label5.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label5.Location = new System.Drawing.Point(18, 34);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(168, 14);
            this.Label5.TabIndex = 16;
            this.Label5.Text = "1. Before Pickup Processing: ";
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label3.Location = new System.Drawing.Point(183, 48);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(509, 14);
            this.Label3.TabIndex = 14;
            this.Label3.Text = "Executed after the files are retrieved but before sent out.  e.g. Change the cont" +
    "ents before sending data";
            // 
            // Label4
            // 
            this.Label4.AutoSize = true;
            this.Label4.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label4.Location = new System.Drawing.Point(183, 62);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(424, 14);
            this.Label4.TabIndex = 15;
            this.Label4.Text = "Executed on delivery after the message is retrived. e.g. Copy the file to multipl" +
    "e folders";
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label2.Location = new System.Drawing.Point(183, 34);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(495, 14);
            this.Label2.TabIndex = 13;
            this.Label2.Text = "Executed before the files are picked up for outgoing messages. e.g. Check file na" +
    "mes and move them";
            // 
            // txtDataContent
            // 
            this.txtDataContent.Location = new System.Drawing.Point(310, 438);
            this.txtDataContent.Name = "txtDataContent";
            this.txtDataContent.Size = new System.Drawing.Size(117, 20);
            this.txtDataContent.TabIndex = 30;
            this.txtDataContent.Text = "Test Data content";
            // 
            // btnLoadDataFile
            // 
            this.btnLoadDataFile.Location = new System.Drawing.Point(433, 436);
            this.btnLoadDataFile.Name = "btnLoadDataFile";
            this.btnLoadDataFile.Size = new System.Drawing.Size(85, 23);
            this.btnLoadDataFile.TabIndex = 29;
            this.btnLoadDataFile.Text = "Load Data File";
            this.btnLoadDataFile.UseVisualStyleBackColor = true;
            this.btnLoadDataFile.Click += new System.EventHandler(this.btnLoadDataFile_Click);
            // 
            // btnRemoveAssembly
            // 
            this.btnRemoveAssembly.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRemoveAssembly.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRemoveAssembly.Location = new System.Drawing.Point(20, 435);
            this.btnRemoveAssembly.Name = "btnRemoveAssembly";
            this.btnRemoveAssembly.Size = new System.Drawing.Size(123, 25);
            this.btnRemoveAssembly.TabIndex = 26;
            this.btnRemoveAssembly.Text = "&Remove Application";
            this.btnRemoveAssembly.UseVisualStyleBackColor = true;
            this.btnRemoveAssembly.Click += new System.EventHandler(this.btnRemoveAssembly_Click);
            // 
            // btnDown
            // 
            this.btnDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDown.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDown.Location = new System.Drawing.Point(227, 435);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(73, 25);
            this.btnDown.TabIndex = 25;
            this.btnDown.Text = "Move &Down";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnUp
            // 
            this.btnUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUp.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUp.Location = new System.Drawing.Point(148, 435);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(73, 25);
            this.btnUp.TabIndex = 24;
            this.btnUp.Text = "Move &Up";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(661, 435);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 25);
            this.btnCancel.TabIndex = 28;
            this.btnCancel.Text = "&Close";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnExecute
            // 
            this.btnExecute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExecute.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExecute.Location = new System.Drawing.Point(524, 435);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(118, 25);
            this.btnExecute.TabIndex = 27;
            this.btnExecute.Text = "Test Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // SplitContainer1
            // 
            this.SplitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SplitContainer1.Location = new System.Drawing.Point(20, 100);
            this.SplitContainer1.Name = "SplitContainer1";
            // 
            // SplitContainer1.Panel1
            // 
            this.SplitContainer1.Panel1.Controls.Add(this.btnAddAssembly);
            this.SplitContainer1.Panel1.Controls.Add(this.lstAssembly);
            this.SplitContainer1.Panel1.Controls.Add(this.txtAssemblyInfo);
            this.SplitContainer1.Panel1.Controls.Add(this.ddlProcessType);
            // 
            // SplitContainer1.Panel2
            // 
            this.SplitContainer1.Panel2.Controls.Add(this.prpPropertiesList);
            this.SplitContainer1.Size = new System.Drawing.Size(716, 328);
            this.SplitContainer1.SplitterDistance = 286;
            this.SplitContainer1.TabIndex = 31;
            // 
            // btnAddAssembly
            // 
            this.btnAddAssembly.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddAssembly.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddAssembly.Location = new System.Drawing.Point(178, 5);
            this.btnAddAssembly.Name = "btnAddAssembly";
            this.btnAddAssembly.Size = new System.Drawing.Size(105, 25);
            this.btnAddAssembly.TabIndex = 0;
            this.btnAddAssembly.Text = "&Add Application...";
            this.btnAddAssembly.UseVisualStyleBackColor = true;
            this.btnAddAssembly.Click += new System.EventHandler(this.btnAddAssembly_Click);
            // 
            // lstAssembly
            // 
            this.lstAssembly.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstAssembly.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstAssembly.FormattingEnabled = true;
            this.lstAssembly.IntegralHeight = false;
            this.lstAssembly.ItemHeight = 14;
            this.lstAssembly.Location = new System.Drawing.Point(3, 33);
            this.lstAssembly.Name = "lstAssembly";
            this.lstAssembly.Size = new System.Drawing.Size(280, 239);
            this.lstAssembly.TabIndex = 2;
            this.lstAssembly.SelectedIndexChanged += new System.EventHandler(this.lstAssembly_SelectedIndexChanged);
            // 
            // txtAssemblyInfo
            // 
            this.txtAssemblyInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAssemblyInfo.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAssemblyInfo.ForeColor = System.Drawing.SystemColors.Desktop;
            this.txtAssemblyInfo.Location = new System.Drawing.Point(3, 275);
            this.txtAssemblyInfo.Multiline = true;
            this.txtAssemblyInfo.Name = "txtAssemblyInfo";
            this.txtAssemblyInfo.Size = new System.Drawing.Size(280, 50);
            this.txtAssemblyInfo.TabIndex = 5;
            // 
            // ddlProcessType
            // 
            this.ddlProcessType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ddlProcessType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlProcessType.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ddlProcessType.FormattingEnabled = true;
            this.ddlProcessType.Items.AddRange(new object[] {
            "Before Pickup Processing",
            "After Pickup Processing",
            "After Download Processing"});
            this.ddlProcessType.Location = new System.Drawing.Point(3, 7);
            this.ddlProcessType.Name = "ddlProcessType";
            this.ddlProcessType.Size = new System.Drawing.Size(166, 22);
            this.ddlProcessType.TabIndex = 1;
            this.ddlProcessType.SelectedIndexChanged += new System.EventHandler(this.ddlProcessType_SelectedIndexChanged);
            // 
            // prpPropertiesList
            // 
            this.prpPropertiesList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.prpPropertiesList.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.prpPropertiesList.HelpBackColor = System.Drawing.SystemColors.ControlLightLight;
            this.prpPropertiesList.LineColor = System.Drawing.SystemColors.ControlLight;
            this.prpPropertiesList.Location = new System.Drawing.Point(0, 0);
            this.prpPropertiesList.Name = "prpPropertiesList";
            this.prpPropertiesList.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.prpPropertiesList.Size = new System.Drawing.Size(426, 328);
            this.prpPropertiesList.TabIndex = 7;
            this.prpPropertiesList.ToolbarVisible = false;
            this.prpPropertiesList.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.prpPropertiesList_PropertyValueChanged);
            this.prpPropertiesList.Leave += new System.EventHandler(this.prpPropertiesList_Leave);
            // 
            // frmConfigureAssemblies
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(757, 470);
            this.Controls.Add(this.SplitContainer1);
            this.Controls.Add(this.txtDataContent);
            this.Controls.Add(this.btnLoadDataFile);
            this.Controls.Add(this.btnRemoveAssembly);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.GroupBox1);
            this.Font = new System.Drawing.Font("Arial", 8.25F);
            this.Name = "frmConfigureAssemblies";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Advanced Configuration";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmConfigureAssemblies_FormClosing);
            this.Load += new System.EventHandler(this.frmConfigureAssemblies_Load);
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox1.PerformLayout();
            this.SplitContainer1.Panel1.ResumeLayout(false);
            this.SplitContainer1.Panel1.PerformLayout();
            this.SplitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer1)).EndInit();
            this.SplitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.GroupBox GroupBox1;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.Label Label7;
        internal System.Windows.Forms.Label Label6;
        internal System.Windows.Forms.Label Label5;
        internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.Label Label4;
        internal System.Windows.Forms.Label Label2;
        internal System.Windows.Forms.TextBox txtDataContent;
        internal System.Windows.Forms.Button btnLoadDataFile;
        internal System.Windows.Forms.Button btnRemoveAssembly;
        internal System.Windows.Forms.Button btnDown;
        internal System.Windows.Forms.Button btnUp;
        internal System.Windows.Forms.Button btnCancel;
        internal System.Windows.Forms.Button btnExecute;
        internal System.Windows.Forms.SplitContainer SplitContainer1;
        internal System.Windows.Forms.Button btnAddAssembly;
        internal System.Windows.Forms.ListBox lstAssembly;
        internal System.Windows.Forms.TextBox txtAssemblyInfo;
        internal System.Windows.Forms.ComboBox ddlProcessType;
        internal System.Windows.Forms.PropertyGrid prpPropertiesList;
    }
}

