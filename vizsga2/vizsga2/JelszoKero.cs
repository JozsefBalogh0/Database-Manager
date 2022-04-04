using System;
using System.Linq;
using System.Windows.Forms;

namespace vizsga2
{
    public partial class JelszoKero : Form
    {
        public JelszoKero()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Program.passwordCorrect = false;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("Kérem adja meg a jelszót!");
                return;
            }

            if (Program.processPassword(textBox1.Text) == Program.password)
            {
                Program.passwordCorrect = true;
            }
            else
            {
                MessageBox.Show("Hibás jelszó!");
                return;
            }
            this.Close();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;

                if (textBox1.Text == "")
                {
                    MessageBox.Show("Kérem adja meg a jelszót!");
                    return;
                }

                if (Program.processPassword(textBox1.Text) == Program.password)
                {
                    Program.passwordCorrect = true;
                }
                else
                {
                    MessageBox.Show("Hibás jelszó!");
                    return;
                }
                this.Close();
            }
        }

        public string text = "Kérem adja meg a jelszót:";

        private void JelszoKero_Load(object sender, EventArgs e)
        {
            this.BackColor = Program.backGroundColor;
            this.ForeColor = Program.textColor;

            foreach (var button in this.Controls.OfType<Button>())
            {
                button.BackColor = Program.buttonColor;
            }

            label1.Text = text;
            Program.passwordCorrect = false;
        }
    }
}
