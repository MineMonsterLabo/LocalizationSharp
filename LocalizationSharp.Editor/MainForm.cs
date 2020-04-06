using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
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
        private string _path;

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

                _path = dialog.SelectedPath;
                _openedLocalizationManager = new LocalizationManager(_path, CultureInfo.CurrentCulture);
                foreach (ILocalizeFile localizeFile in _openedLocalizationManager.GetFiles())
                {
                    void LoadFunc()
                    {
                        LocalizeFileEditorPanel panel = new LocalizeFileEditorPanel(localizeFile);
                        _files.Add(panel);
                        panel.Show(dockPanel1, DockState.Document);
                    }

                    try
                    {
                        LoadFunc();
                    }
                    catch (FileNotFoundException ex)
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

                        LoadFunc();
                    }
                }
            }
        }

        private void saveSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (IDockContent content in dockPanel1.Contents)
            {
                if (content is LocalizeFileEditorPanel fileEditorPanel)
                {
                    fileEditorPanel.Save();
                }
            }

            foreach (ILocalizeFile file in _openedLocalizationManager.GetFiles())
            {
                file.Save(_path);
            }
        }
    }
}