using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CorrectTranslation
{
    public partial class EncodingForm : Form
    {
        public Form1 parent;
        public EncodingForm() => InitializeComponent();

        public EncodingForm(Form1 par) : this()
        {
            parent = par;
        }
        

        private void button1_Click( object sender, EventArgs e )
        {
            parent.Encoding = textBox1.Text;
            Close();
        }
    }
}
