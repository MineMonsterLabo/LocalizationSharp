using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LocalizationSharp.Core;
using LocalizationSharp.Editor.DockPanels;
using WeifenLuo.WinFormsUI.Docking;

namespace LocalizationSharp.Editor
{
    public partial class MainForm : Form
    {
        private LocalizationManager _openedLocalizationManager;
        private List<LocalizeFileEditorPanel> _files = new List<LocalizeFileEditorPanel>();

        public MainForm()
        {
            InitializeComponent();

            PatchController.EnableAll = true;
        }

        private void loadLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "ローカリゼーションファイルのあるフォルダを選択してください";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _files.ForEach(el => el.Close());
                _files.Clear();

                string path = dialog.SelectedPath;
                _openedLocalizationManager = new LocalizationManager(path, CultureInfo.CurrentCulture);
                foreach (ILocalizeFile localizeFile in _openedLocalizationManager.GetFiles())
                {
                    LocalizeFileEditorPanel panel = new LocalizeFileEditorPanel(localizeFile);
                    _files.Add(panel);
                    panel.Show(dockPanel1, DockState.Document);
                }
            }
        }

        private void saveSToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
    }
}