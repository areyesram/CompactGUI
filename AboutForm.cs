using System;
using System.Reflection;
using System.Windows.Forms;

namespace Aryes
{
    internal partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            Text = $"About {AssemblyTitle}";
            labelProductName.Text = AssemblyProduct;
            labelVersion.Text = $"Version {AssemblyVersion}";
            labelCopyright.Text = AssemblyCopyright;
            labelCompanyName.Text = AssemblyCompany;
            textBoxDescription.Text = AssemblyDescription;
        }

        public string AssemblyTitle =>
            GetAttribute<AssemblyTitleAttribute>(o => o.Title)
            ??
            System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);

        public string AssemblyVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public string AssemblyDescription => GetAttribute<AssemblyDescriptionAttribute>(o => o.Description);

        public string AssemblyProduct => GetAttribute<AssemblyProductAttribute>(o => o.Product);

        public string AssemblyCopyright => GetAttribute<AssemblyCopyrightAttribute>(o => o.Copyright);

        public string AssemblyCompany => GetAttribute<AssemblyCompanyAttribute>(o => o.Company);

        private static string GetAttribute<T>(Func<T, string> fn)
        {
            var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(T), false);
            return attributes.Length != 0 ? fn((T)attributes[0]) : string.Empty;
        }
    }
}