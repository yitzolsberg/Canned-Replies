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
using System.Xml;
using System.Xml.Serialization;

namespace Canned_Replies
{
    public partial class MainForm : Form
    {
        private List<Reply> _replies;
        string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Canned Replies";
        private bool _loading = false;
        public MainForm()
        {
            InitializeComponent();
            LoadReplies();
            if (listBox.Items.Count > 0) listBox.SelectedIndex = 0;
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            var newReply = new Reply();
            var formAdd = new FormAdd(newReply);
            if (formAdd.ShowDialog() == DialogResult.OK)
            {
                _replies.Add(newReply);
                PopulateForm();
                listBox.SelectedIndex = listBox.Items.Count - 1;
                SaveReplies();
            }
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            if (listBox.SelectedIndex < 0) return;
            Form formEdit = new FormAdd((Reply)listBox.SelectedItem);
            formEdit.Controls["addButton"].Text = "Apply";
            if (formEdit.ShowDialog() == DialogResult.OK)
            {
                int sel = listBox.SelectedIndex;
                PopulateForm();
                listBox.SelectedIndex = sel;
                SaveReplies();
            }
        }

        private void copyButton_Click(object sender, EventArgs e)
        {
            CopyToClipboard();
        }

        private void LoadReplies()
        {
            if (!Directory.Exists(appDataFolder) || !File.Exists(appDataFolder + @"\Replies.xml"))
            {
                MakeNewFile();
            }
            else
            {
                _loading = true;

                try
                {
                    XmlSerializer reader = new XmlSerializer(typeof(SaveData));
      
                    using (StreamReader file = new StreamReader(appDataFolder + @"\Replies.xml"))
                    {
                        var saveData = (SaveData)reader.Deserialize(file);
                        _replies = saveData.Replies;
                        cbMinimize.Checked = saveData.Minimize;
                    }
                }
                catch
                {
                    MakeNewFile();
                }
            }
            _loading = false;
            PopulateForm();
        }

        private void PopulateForm()
        {
            listBox.Items.Clear();
            textBox.Text = "";
            foreach (var reply in _replies)
            {
                listBox.Items.Add(reply);
            }
        }

        private void MakeNewFile()
        {
                _replies = new List<Reply>() {
                new Reply() { name = "Out of stock", replyText = "Hi" + Environment.NewLine + Environment.NewLine +
                    "Unfortunately we have oversold on this item." + Environment.NewLine + Environment.NewLine +
                    "name"}
            };
            if (!Directory.Exists(appDataFolder)) Directory.CreateDirectory(appDataFolder);
            SaveReplies();
        }

        private void SaveReplies()
        {

            XmlWriterSettings writerSettings = new XmlWriterSettings();
            writerSettings.NewLineHandling = NewLineHandling.Entitize;

            XmlSerializer writer = new XmlSerializer(typeof(SaveData));

            using (XmlWriter file = XmlWriter.Create(appDataFolder + @"\Replies.xml", writerSettings))
            {
                var saveData = new SaveData
                {
                    Replies = _replies,
                    Minimize = cbMinimize.Checked
                };

                writer.Serialize(file, saveData);
            }
        }

        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox.SelectedIndex >= 0)
                textBox.Text = ((Reply)listBox.SelectedItem).replyText;
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (listBox.SelectedIndex < 0) return;
            if(MessageBox.Show("Are you sure you want to delete " + listBox.SelectedItem.ToString() + "?",
                "Are you sure?",
                MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                _replies.Remove((Reply)listBox.SelectedItem);
                PopulateForm();
                SaveReplies();
            }
        }

        private void listBox_DoubleClick(object sender, EventArgs e)
        {
            CopyToClipboard();
        }

        private void CopyToClipboard()
        {
            if (!String.IsNullOrWhiteSpace(textBox.Text))
            {
                Clipboard.SetText(textBox.Text);
                if(cbMinimize.Checked)
                    this.WindowState = FormWindowState.Minimized;
            }
        }

        private void cbMinimize_CheckedChanged(object sender, EventArgs e)
        {
            if(_loading)
                return;
            SaveReplies();
        }
    }

    public class SaveData
    {
        public List<Reply> Replies { get; set; }
        public bool Minimize { get; set; }
    }
}
