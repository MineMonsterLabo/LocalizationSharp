using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LocalizationSharp.Core;
using WeifenLuo.WinFormsUI.Docking;

namespace LocalizationSharp.Editor.DockPanels
{
    public partial class LocalizeContentEditorPanel : DockContent
    {
        private static LocalizeContentEditorPanel _panel;

        public static void ShowPanel(DockPanel panel, ILocalizeContent<object> content)
        {
            if (_panel == null)
            {
                _panel = new LocalizeContentEditorPanel();
                _panel.Show(panel, DockState.DockRight);
            }

            _panel.SelectContent(content);
        }

        public LocalizeContentEditorPanel()
        {
            InitializeComponent();
        }

        private void SelectContent(ILocalizeContent<object> content)
        {
            propertyGrid1.SelectedObject = content;
        }
    }
}