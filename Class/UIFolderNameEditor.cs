using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System;

namespace PostDnFhirR4Api.Class
{
    public class UIFolderNameEditor : System.Drawing.Design.UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if ((context != null) && (context.Instance != null))
            {
                return UITypeEditorEditStyle.Modal;
            }
            return UITypeEditorEditStyle.None;
        }
        [RefreshProperties(RefreshProperties.All)]
        public override Object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, Object value)
        {
            if (context == null || provider == null || context.Instance == null)
            {
                return base.EditValue(provider, value);
            }

            FolderBrowserDialog folderDlg = default(FolderBrowserDialog);
            // FileDialog
            folderDlg = new FolderBrowserDialog();

            folderDlg.Description = "Select " + context.PropertyDescriptor.DisplayName;
            folderDlg.SelectedPath = Convert.ToString(value);
            if (folderDlg.ShowDialog() == DialogResult.OK)
            {
                value = folderDlg.SelectedPath;
            }
            folderDlg.Dispose();
            return value;
        }
    }
}
