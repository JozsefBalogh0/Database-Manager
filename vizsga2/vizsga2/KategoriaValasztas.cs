using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace vizsga2
{
    public partial class KategoriaValasztas : Form
    {
        public KategoriaValasztas()
        {
            InitializeComponent();
        }

        public void loadCategory()
        {
            listBox1.Items.Clear();
            try
            {
                Program.conn.Open();
                string sqlCommand = "select table_name from information_schema.tables where table_schema = 'leltarrendszer'";
                MySqlCommand cmd = new MySqlCommand(sqlCommand, Program.conn);
                cmd.CommandType = CommandType.Text;
                MySqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    listBox1.Items.Add(rdr["table_name"]);
                }
                rdr.Close();
            }
            catch
            {
                Program.writeReport($"SQL Adatbázis csatlakozási hiba");
                MessageBox.Show("Nem lehet csatlakozni az adatbázishoz!\nAz alkalmazás kilép.");
                Application.Exit();
            }
            Program.conn.Close();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.BackColor = Program.backGroundColor;
            this.ForeColor = Program.textColor;

            foreach (var groupBox in this.Controls.OfType<GroupBox>())
            {
                groupBox.BackColor = Program.foreGroundColor;
                groupBox.ForeColor = Program.textColor;

                foreach (var button in groupBox.Controls.OfType<Button>())
                {
                    button.BackColor = Program.buttonColor;
                }
            }

            foreach (var button in this.Controls.OfType<Button>())
            {
                button.BackColor = Program.buttonColor;
            }

            button1.Enabled = false;
            button4.Enabled = false;
            button3.Enabled = false;
            loadCategory();
        }

        //Megnyitás
        private void button3_Click(object sender, EventArgs e)
        {
            Program.category = listBox1.Items[listBox1.SelectedIndex].ToString();
            Program.writeReport($"Megnyitott kategória: {Program.category}");
            this.Close();
        }

        //Törlés
        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Biztosan törölni szeretné az adatot?", "", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                if (Program.askPassword)
                {
                    JelszoKero askPAss = new JelszoKero();
                    askPAss.ShowDialog();

                    if (Program.passwordCorrect == false)
                    {
                        return;
                    }
                }

                Program.conn.Open();
                string sqlCommand = "DROP TABLE " + listBox1.Items[listBox1.SelectedIndex].ToString();
                MySqlCommand cmd = new MySqlCommand(sqlCommand, Program.conn);
                cmd.CommandType = CommandType.Text;
                MySqlDataReader rdr = cmd.ExecuteReader();

                rdr.Close();
                Program.conn.Close();

                Program.writeReport($"Kategória törölve: {listBox1.Items[listBox1.SelectedIndex].ToString()}");
                MessageBox.Show("Kategória törölve.");

                button4.Enabled = false;
                button1.Enabled = false;
                button1.Enabled = false;
                loadCategory();
            }
        }

        //Módosítás
        private void button1_Click(object sender, EventArgs e)
        {
            if (Program.askPassword)
            {
                JelszoKero askPAss = new JelszoKero();
                askPAss.ShowDialog();

                if (Program.passwordCorrect == false)
                {
                    return;
                }
            }

            KategoriaSzerkesztes modositasForm = new KategoriaSzerkesztes();
            modositasForm.categoryToChange = listBox1.Items[listBox1.SelectedIndex].ToString();
            modositasForm.ShowDialog();
        }

        //Létrehozás
        private void button2_Click(object sender, EventArgs e)
        {
            if (Program.askPassword)
            {
                JelszoKero askPAss = new JelszoKero();
                askPAss.ShowDialog();

                if (Program.passwordCorrect == false)
                {
                    return;
                }
            }

            List<string> items = new List<string>();

            KategoriaNevLekerdezes letrehozas = new KategoriaNevLekerdezes();
            foreach (string item in listBox1.Items)
            {
                items.Add(item);
            }
            letrehozas.categories = items;
            letrehozas.ShowDialog();

            if (!letrehozas.stop)
            {
                KategoriaLetrehozas oszlopokForm = new KategoriaLetrehozas();
                oszlopokForm.ShowDialog();

                if (!string.IsNullOrWhiteSpace(Program.newCategory))
                {
                    DialogResult dialogResult = MessageBox.Show("Meg szeretné nyitni a létrehozott kategóriát?", "", MessageBoxButtons.YesNo);

                    if (dialogResult == DialogResult.Yes)
                    {
                        Program.category = Program.newCategory;
                        Program.writeReport($"Megnyitott kategória: {Program.category}");

                        Program.newCategory = "";
                        this.Close();
                    }
                }
                loadCategory();
            }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex > -1)
            {
                Program.category = listBox1.Items[listBox1.SelectedIndex].ToString();
                Program.writeReport($"Megnyitott kategória: {Program.category}");
                this.Close();
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex > -1)
            {
                button1.Enabled = true;
                button4.Enabled = true;
                button3.Enabled = true;
                if (Program.category == listBox1.Items[listBox1.SelectedIndex].ToString())
                {
                    button4.Enabled = false;
                }
            }
        }

        //Beállítások
        private void button5_Click(object sender, EventArgs e)
        {
            Beallitasok settings = new Beallitasok();
            settings.ShowDialog();
        }
    }
}
