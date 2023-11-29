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
    public partial class Form1 : Form
    {
        List<Reply> Replies;
        string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Canned Replies";
            

        public Form1()
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
                Replies.Add(newReply);
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
                XmlSerializer reader = new XmlSerializer(typeof(List<Reply>));
      
                using (StreamReader file = new StreamReader(appDataFolder + @"\Replies.xml"))
                {
                    Replies = (List<Reply>)reader.Deserialize(file);
                }

            }
            PopulateForm();
        }

        private void PopulateForm()
        {
            listBox.Items.Clear();
            textBox.Text = "";
            foreach (var reply in Replies)
            {
                listBox.Items.Add(reply);
            }
        }

        private void MakeNewFile()
        {
                Replies = new List<Reply>() {
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

            XmlSerializer writer = new XmlSerializer(Replies.GetType());

            using (XmlWriter file = XmlWriter.Create(appDataFolder + @"\Replies.xml", writerSettings))
            {
                writer.Serialize(file, Replies);
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
                Replies.Remove((Reply)listBox.SelectedItem);
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
                this.WindowState = FormWindowState.Minimized;
            }
        }
    }
}
