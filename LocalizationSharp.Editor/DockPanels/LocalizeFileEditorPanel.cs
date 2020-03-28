using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LocalizationSharp.Core;
using WeifenLuo.WinFormsUI.Docking;

namespace LocalizationSharp.Editor.DockPanels
{
    public partial class LocalizeFileEditorPanel : DockContent
    {
        private ILocalizeFile _file;

        public LocalizeFileEditorPanel(ILocalizeFile file)
        {
            InitializeComponent();

            _file = file;

            LoadFromFile();
        }

        public void LoadFromFile()
        {
            Text = $"LocalizeFile {_file.CultureInfo.Name}";

            foreach (KeyValuePair<string, ILocalizeContent<object>> pair in _file)
            {
                dataGridView1.Rows.Add(pair.Key);
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            DataGridViewRow row = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex];
            DataGridViewTextBoxCell cell = (DataGridViewTextBoxCell) row.Cells[0];

            string value = (string) cell.Value;
            if (value != null)
                LocalizeContentEditorPanel.ShowPanel(DockPanel, _file[value]);
        }
    }
}