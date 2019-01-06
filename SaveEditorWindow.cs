using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ch_save_edit
{
    public partial class SaveEditorWindow : Form
    {
        private string data;
        private JToken root;
        public SaveEditorWindow() => InitializeComponent();

        private void DecodeButton_click(object sender, EventArgs e)
        {
            data = JsonUtil.DecodeJson(rawText.Text);
            rawText.Text = data;
            FillTreeView();
        }

        private void EncodeButton_Click(object sender, EventArgs e) => JsonUtil.EncodeJson(rawText.Text);

        private void ValueSetButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(valueTextBox.Text)) return;
            if (root is null) return;
            var node = jsonTree.SelectedNode.Tag;
            if (node is null) return;
            if (!(node is JValue jnode)) return;

            //set actual json objects value
            jnode.Value = valueTextBox.Text;
            //set label of treeview
            jsonTree.SelectedNode.Text = valueTextBox.Text;

            //empty up textbox
            valueTextBox.Text = string.Empty;

            //update rawText
            rawText.Text = root.ToString();
            //update treeview
            jsonTree.Update();
        }
        
        private void FillTreeView()
        {
            if (string.IsNullOrWhiteSpace(rawText.Text)) return;
            if (string.IsNullOrWhiteSpace(data)) return;
            using (var reader = new StringReader(data))
            using (var jsonReader = new JsonTextReader(reader))
            {
                root = JToken.Load(jsonReader);
                DisplayTreeView(root, "Data");
            }
        }

        private void DisplayTreeView(JToken root, string rootName)
        {
            jsonTree.BeginUpdate();
            try
            {
                jsonTree.Nodes.Clear();
                var tNode = jsonTree.Nodes[jsonTree.Nodes.Add(new TreeNode(rootName))];
                tNode.Tag = root;
                AddNode(root, tNode);
            }
            finally
            {
                jsonTree.EndUpdate();
            }
        }

        private void AddNode(JToken token, TreeNode inTreeNode)
        {
            if (token == null)
                return;
            if (token is JValue)
            {
                var childNode = inTreeNode.Nodes[inTreeNode.Nodes.Add(new TreeNode(token.ToString()))];
                childNode.Tag = token;
            }
            else if (token is JObject obj)
            {
                foreach (var property in obj.Properties())
                {
                    var childNode = inTreeNode.Nodes[inTreeNode.Nodes.Add(new TreeNode(property.Name))];
                    childNode.Tag = property;
                    AddNode(property.Value, childNode);
                }
            }
            else if (token is JArray array)
            {
                for (int i = 0; i < array.Count; i++)
                {
                    var childNode = inTreeNode.Nodes[inTreeNode.Nodes.Add(new TreeNode(i.ToString()))];
                    childNode.Tag = array[i];
                    AddNode(array[i], childNode);
                }
            }
            else
            {
                Debug.WriteLine(string.Format("{0} not implemented", token.Type)); // JConstructor, JRaw
            }
        }

        private void JsonTree_AfterSelect(object sender, EventArgs e)
        {
            valueTextBox.Text = jsonTree.SelectedNode.Text;
        }

        private void ImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                openFileDialog.Filter = "json files (*.json)|*.json|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Text = "ch_save_edit - " + openFileDialog.FileName;
                    rawText.Text = File.ReadAllText(openFileDialog.FileName);
                    if (JsonUtil.IsEncoded(rawText.Text))
                    {
                        rawText.Text = JsonUtil.DecodeJson(rawText.Text);
                        data = rawText.Text;
                        FillTreeView();
                    } else
                    {
                        data = rawText.Text;
                        FillTreeView();
                    }
                }
            }
        }
        private void ExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                //setup
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                saveFileDialog.Filter = "json files (*.json)|*.json|All files (*.*)|*.*";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.Title = "Save ch-save-file";

                //get file path
                saveFileDialog.ShowDialog();
                if (string.IsNullOrWhiteSpace(saveFileDialog.FileName)) return;
                //save
                File.WriteAllText(saveFileDialog.FileName, rawText.Text);
            }
        }
        private void EncodeToolStripMenuItem_Click(object sender, EventArgs e) => EncodeButton_Click(null, null);
        private void DecodeToolStripMenuItem_Click(object sender, EventArgs e) => DecodeButton_click(null, null);
        private void ImportSaveToolStripMenuItem_Click(object sender, EventArgs e) => MessageBox.Show(
                @"Open the clicker heroes settings and click save.\n
                Close the save window and paste your clipboard content into the textfield\n
                that says 'raw data:'\n
                Hit decode and enjoy :)",
                @"Importing save",
                MessageBoxButtons.OK,
                MessageBoxIcon.Asterisk,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.ServiceNotification,
                false);
        private void ExportSaveToolStripMenuItem_Click(object sender, EventArgs e) => MessageBox.Show(
                @"Hit decode.\n
                Open the clicker heroes settings and hit import.\n
                Paste your clipboard content in there.\n",
                @"Exporting save",
                MessageBoxButtons.OK,
                MessageBoxIcon.Asterisk,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.ServiceNotification,
                false);
        private void AboutToolStripMenuItem1_Click(object sender, EventArgs e) => MessageBox.Show(
                @"Ambien",
                @"Author",
                MessageBoxButtons.OK,
                MessageBoxIcon.Asterisk,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.ServiceNotification,
                false);
    }
}
