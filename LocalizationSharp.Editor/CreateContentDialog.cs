using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LocalizationSharp.Core;

namespace LocalizationSharp.Editor
{
    public partial class CreateContentDialog : Form
    {
        public ILocalizeContent<object> Content { get; private set; }

        public CreateContentDialog(ILocalizeContent<object> content)
        {
            InitializeComponent();

            FindContentTypes();
            comboBox1.SelectedIndex = 0;

            if (content != null)
            {
                Content = content;
                propertyGrid1.SelectedObject = Content;
            }
        }

        private void loadAssemblyLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog asmDialog = new OpenFileDialog
            {
                Filter = "library files (*.dll;*.exe)|*.dll;*.exe"
            };
            if (asmDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string fileName in asmDialog.FileNames)
                {
                    Assembly.LoadFile(fileName);
                }
            }

            FindContentTypes();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = comboBox1.SelectedIndex;
            if (idx != -1)
            {
                Content = (ILocalizeContent<object>) Activator.CreateInstance((Type) comboBox1.Items[idx]);
                propertyGrid1.SelectedObject = Content;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void FindContentTypes()
        {
            comboBox1.Items.Clear();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (IsGenericsContent(type))
                    {
                        comboBox1.Items.Add(type);
                    }
                }
            }
        }

        private bool IsGenericsContent(Type type)
        {
            foreach (var t in type.GetInterfaces())
            {
                if (t.IsGenericType &&
                    t.GetGenericTypeDefinition() == typeof(ILocalizeContent<>))
                    return true;
            }

            return false;
        }
    }
}