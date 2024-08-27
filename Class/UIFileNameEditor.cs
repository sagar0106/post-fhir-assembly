using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace PostDnFhirR4Api.Class
{
    class UIFileNameEditor : System.Drawing.Design.UITypeEditor
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

            FileDialog fileDlg = default(FileDialog);
            if (context.PropertyDescriptor.Attributes[typeof(SaveFileAttribute)] == null)
            {    
                fileDlg = new OpenFileDialog();
            }
            else
            {
                fileDlg = new SaveFileDialog();
            }
            fileDlg.Title = "Select " + context.PropertyDescriptor.DisplayName;
            fileDlg.FileName =Convert.ToString(value);
            FileDialogFilterAttribute filterAtt =((FileDialogFilterAttribute)context.PropertyDescriptor.Attributes[typeof(FileDialogFilterAttribute)]);            
            if ((filterAtt != null))
                fileDlg.Filter = filterAtt.Filter;
            if (fileDlg.ShowDialog() == DialogResult.OK)
            {
                value = fileDlg.FileName;
            }
            fileDlg.Dispose();
            return value;
        }
    }
    #region " Filter attribute "
    [AttributeUsage(AttributeTargets.Property)]
    public class FileDialogFilterAttribute : Attribute
    {

        private string _filter;
        public string Filter
        {
            get { return this._filter; }
        }

        public FileDialogFilterAttribute(string filter): base()
        {
            this._filter = filter;
        }
    }
    #endregion
   
    [AttributeUsage(AttributeTargets.Property)]
    public class SaveFileAttribute : Attribute
    {
    }
}
