using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Canned_Replies
{
    public partial class FormAdd : Form
    {
        public Reply newReply;
        private bool cancelClose = false;

        public FormAdd(Reply rep)

        {
            InitializeComponent();
            if (rep != null)
            {
                textTitle.Text = rep.name;
                textText.Text = rep.replyText;
                newReply = rep;
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(textTitle.Text) && !String.IsNullOrWhiteSpace(textText.Text))
            {
                newReply.name = textTitle.Text;
                newReply.replyText = textText.Text;
            }
            else
            {
                MessageBox.Show("Please enter a name and a reply");
                cancelClose = true;
            }
        }

        private void FormAdd_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (cancelClose)
            {
                e.Cancel = true;
                cancelClose = false;
            }
        }
    }
}
