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
using LocalizationSharp.Files;
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

        private void folderDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "ローカリゼーションフォルダを作成するフォルダを選択してください。";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                _files.ForEach(el => el.Close());
                _files.Clear();

                _path = $"{dialog.SelectedPath}\\LocalizeFiles";

                Directory.CreateDirectory(_path);
                _openedLocalizationManager = new LocalizationManager(_path, CultureInfo.CurrentCulture);

                JsonLocalizeFile file = new JsonLocalizeFile(CultureInfo.CurrentCulture);
                file.Save(_path);

                _openedLocalizationManager.LoadFile($"{_path}\\{CultureInfo.CurrentCulture.Name}.json_lang", ".json_lang");

                foreach (ILocalizeFile localizeFile in _openedLocalizationManager.GetFiles())
                {
                    void LoadFunc()
                    {
                        LocalizeFileEditorPanel panel = new LocalizeFileEditorPanel(localizeFile);
                        _files.Add(panel);
                        panel.Show(dockPanel1, DockState.Document);
                    }


                    LoadFunc();
                }
            }
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
                        MessageBox.Show(ex.ToString(), "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        MessageBox.Show("exeまたは、dllを参照してください。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);

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
            if (_openedLocalizationManager != null)
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
}