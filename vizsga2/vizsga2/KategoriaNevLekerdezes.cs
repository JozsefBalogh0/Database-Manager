using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace vizsga2
{
    public partial class KategoriaNevLekerdezes : Form
    {
        public KategoriaNevLekerdezes()
        {
            InitializeComponent();
        }

        public bool stop = false;

        private void button2_Click(object sender, EventArgs e)
        {
            stop = false;
            textBox1.Text = Regex.Replace(textBox1.Text, @"[^0-9a-zA-ZÖöÜüÓóŐőÚúÉéÁáŰűÍí]+", "");
            foreach (string item in categories)
            {
                if (textBox1.Text.Trim().ToLower() == item)
                {
                    stop = true;
                }
            }

            if (stop)
            {
                MessageBox.Show("Ez a kategória már létezik!");
                return;
            }

            else
            {
                if (char.IsDigit(textBox1.Text[0]))h
                {
                    MessageBox.Show("A kategória neve nem kezdőthet számmal!");
                    return;
                }

                if (textBox1.Text.Contains(" "))
                {
                    MessageBox.Show("A kategória neve nem tartalmazhat szóközt!");
                }
                else
                {
                    Program.newCategory = textBox1.Text;
                    Program.conn.Open();
                    string sqlCommand = "CREATE TABLE " + Program.newCategory + "(ID int NOT NULL AUTO_INCREMENT, PRIMARY KEY(ID))";
                    MySqlCommand cmd = new MySqlCommand(sqlCommand, Program.conn);
                    cmd.CommandType = CommandType.Text;
                    MySqlDataReader rdr = cmd.ExecuteReader();

                    rdr.Close();
                    Program.conn.Close();

                    Program.writeReport($"Új kategória [{Program.newCategory}] létrehozás elindítva");
                    this.Close();
                }
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Program.newCategory = "";
            stop = true;
            this.Close();
        }

        private void KategoriaNevLekerdezes_Load(object sender, EventArgs e)
        {
            this.BackColor = Program.backGroundColor;
            this.ForeColor = Program.textColor;

            foreach (var button in this.Controls.OfType<Button>())
            {
                button.BackColor = Program.buttonColor;
            }
        }

        public List<string> categories;

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                foreach (string item in categories)
                {
                    if (textBox1.Text.Trim().ToLower() == item)
                    {
                        stop = true;
                    }
                }

                if (stop)
                {
                    MessageBox.Show("Ez a kategória már létezik!");
                    return;
                }

                else
                {
                    e.SuppressKeyPress = true;

                    if (char.IsDigit(textBox1.Text[0]))
                    {
                        MessageBox.Show("A kategória neve nem kezdőthet számmal!");
                        return;
                    }

                    textBox1.Text = Regex.Replace(textBox1.Text, @"[^0-9a-zA-ZÖöÜüÓóŐőÚúÉéÁáŰűÍí]+", "");

                    Program.newCategory = textBox1.Text;
                    Program.conn.Open();
                    string sqlCommand = "CREATE TABLE " + Program.newCategory + "(ID int NOT NULL AUTO_INCREMENT, PRIMARY KEY(ID))";
                    MySqlCommand cmd = new MySqlCommand(sqlCommand, Program.conn);
                    cmd.CommandType = CommandType.Text;
                    MySqlDataReader rdr = cmd.ExecuteReader();

                    rdr.Close();
                    Program.conn.Close();

                    Program.writeReport($"Új kategória [{Program.newCategory}] létrehozás elindítva");
                    this.Close();
                }
            }
        }
    }
}
