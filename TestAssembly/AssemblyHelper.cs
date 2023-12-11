using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace TestAssembly
{
    public class AssemblyHelper
    {
        /// <summary>
        /// Ensure that File name provided is valid Process Assembly. Constrain Assembly must have methods Execute, Save, Restore.
        /// </summary>
        /// <param name="AssemblyFileName">Assembly File name</param>
        /// <returns>True if valid</returns>
        /// <remarks></remarks>
        public static bool isValidAssembly(string AssemblyFileName)
        {
            bool isExecute = false;
            bool isSave = false;
            bool isRestore = false;
            System.Reflection.MethodInfo oMethodInfo = null;
            try
            {
                if (AssemblyFileName.Length == 0)
                    return false;
                if (System.IO.File.Exists(AssemblyFileName) == false)
                    return false;
                System.Reflection.Assembly oAssembly = System.Reflection.Assembly.LoadFrom(AssemblyFileName);
                Type[] types = oAssembly.GetTypes();
                foreach (Type oType in types)
                {
                    if (isExecute == false)
                    {
                        oMethodInfo = oType.GetMethod("Execute");
                        if ((oMethodInfo != null))
                            isExecute = true;
                    }
                    if (isSave == false)
                    {
                        oMethodInfo = oType.GetMethod("Save");
                        if ((oMethodInfo != null))
                            isSave = true;
                    }
                    if (isRestore == false)
                    {
                        oMethodInfo = oType.GetMethod("Restore");
                        if ((oMethodInfo != null))
                            isRestore = true;
                    }
                }
                return isExecute & isSave & isRestore;
            }
            catch (Exception ex)
            {
                throw ex;
                return false;
            }
        }

        /// <summary>
        /// Get valid Process Assembly. Constrain Assembly must have methods Execute, Save, Restore.
        /// </summary>
        /// <param name="AssemblyFileName">Assembly File name</param>
        /// <returns>Valid Object</returns>
        /// <remarks></remarks>
        public static object GetValidType(string AssemblyFileName)
        {
            bool isExecute = false;
            bool isSave = false;
            bool isRestore = false;
            System.Reflection.MethodInfo oMethodInfo = null;
            try
            {
                System.Reflection.Assembly oAssembly = System.Reflection.Assembly.LoadFrom(AssemblyFileName);

                Type[] types = oAssembly.GetTypes();
                foreach (Type oType in types)
                {
                    if (oType.IsAbstract == true || oType.IsInterface)
                        continue;
                    if (isExecute == false)
                    {
                        oMethodInfo = oType.GetMethod("Execute");
                        if ((oMethodInfo != null))
                            isExecute = true;
                    }
                    if (isSave == false)
                    {
                        oMethodInfo = oType.GetMethod("Save");
                        if ((oMethodInfo != null))
                            isSave = true;
                    }
                    if (isRestore == false)
                    {
                        oMethodInfo = oType.GetMethod("Restore");
                        if ((oMethodInfo != null))
                            isRestore = true;
                    }
                    if (isExecute & isSave & isRestore)
                    {
                        oMethodInfo = oType.GetMethod("Save");
                        if ((oMethodInfo != null))
                        {
                            string typeName = oType.FullName;
                            object oLateBound = oAssembly.CreateInstance(typeName);
                            return oLateBound;
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static Type GetValidAssemblyType(string AssemblyFileName)
        {
            bool isExecute = false;
            bool isSave = false;
            bool isRestore = false;
            System.Reflection.MethodInfo oMethodInfo = null;
            try
            {
                System.Reflection.Assembly oAssembly = System.Reflection.Assembly.LoadFrom(AssemblyFileName);

                Type[] types = oAssembly.GetTypes();
                foreach (Type oType in types)
                {
                    if (oType.IsAbstract == true || oType.IsInterface)
                        continue;
                    if (isExecute == false)
                    {
                        oMethodInfo = oType.GetMethod("Execute");
                        if ((oMethodInfo != null))
                            isExecute = true;
                    }
                    if (isSave == false)
                    {
                        oMethodInfo = oType.GetMethod("Save");
                        if ((oMethodInfo != null))
                            isSave = true;
                    }
                    if (isRestore == false)
                    {
                        oMethodInfo = oType.GetMethod("Restore");
                        if ((oMethodInfo != null))
                            isRestore = true;
                    }
                    if (isExecute & isSave & isRestore)
                    {
                        return oType;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Get assembly information string
        /// </summary>
        /// <param name="AssemblyFileName"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static string GetAssemblyInformation(string AssemblyFileName)
        {
            try
            {
                Assembly oAssembly = System.Reflection.Assembly.LoadFrom(AssemblyFileName);
                AssemblyDescriptionAttribute oDescAttr = (AssemblyDescriptionAttribute)AssemblyDescriptionAttribute.GetCustomAttribute(oAssembly, typeof(AssemblyDescriptionAttribute));
                AssemblyTitleAttribute oTitleAttr = (AssemblyTitleAttribute)AssemblyTitleAttribute.GetCustomAttribute(oAssembly, typeof(AssemblyTitleAttribute));
                FileVersionInfo oVersionInfo = FileVersionInfo.GetVersionInfo(AssemblyFileName);
                string lcString = "" + oTitleAttr.Title + "\r\n";
                lcString += "" + oDescAttr.Description + "\r\n";
                //lcString += "Major Version: " & vi.ProductMajorPart().ToString()
                //lcString += "Minor Version: " & vi.ProductMinorPart().ToString()
                //lcString += "Build Number: " & vi.ProductBuildPart().ToString()
                lcString += "Version: " + oVersionInfo.ProductVersion + "\r\n";
                lcString += oVersionInfo.CompanyName;
                return lcString;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        /// <summary>
        /// Show OpenFileDialog for DLL, EXE, All files and return selected file
        /// </summary>
        /// <returns>Valid file name</returns>
        /// <remarks></remarks>
        public static string GetAssemblyFileName(string sFileName)
        {
            try
            {
                OpenFileDialog oOpenFileDialog = new OpenFileDialog();
                // Open File Dialog
                oOpenFileDialog.FileName = sFileName;
                oOpenFileDialog.CheckFileExists = true;
                oOpenFileDialog.Filter = "DLL Files|*.dll;|Executable Files|*.exe;|All Files|*.*;";
                oOpenFileDialog.InitialDirectory = Application.StartupPath;
                oOpenFileDialog.RestoreDirectory = true;
                oOpenFileDialog.SupportMultiDottedExtensions = true;
                oOpenFileDialog.Title = "Select Assembly File name";
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