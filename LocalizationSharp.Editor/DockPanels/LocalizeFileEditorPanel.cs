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
using LocalizationSharp.Contents;
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
                dataGridView1.Rows.Add(pair.Key, pair.Value, "変更");
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            DataGridViewRow row = dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex];
            DataGridViewTextBoxCell cell = (DataGridViewTextBoxCell) row.Cells[0];

            string value = (string) cell.Value;
            if (value != null)
                LocalizeContentEditorPanel.ShowPanel(DockPanel, value, (ILocalizeContent<object>) row.Cells[1].Value);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
            DataGridViewTextBoxCell cell = (DataGridViewTextBoxCell) row.Cells[0];

            string value = (string) cell.Value;
            if (value != null && e.ColumnIndex == 2)
            {
                CreateContentDialog dialog = new CreateContentDialog(row.Cells[1].Value as ILocalizeContent<object>);
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    row.Cells[1].Value = dialog.Content;

                    dataGridView1_SelectionChanged(this, EventArgs.Empty);
                }
            }
        }

        private void dataGridView1_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells[1].Value = new LocalizeTextContent();
        }

        public void Save()
        {
            _file.Clear();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells[0].Value is string key)
                    _file[key] = row.Cells[1].Value as ILocalizeContent<object>;
            }
        }
    }
}