using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace vizsga2
{
    public partial class KategoriaSzerkesztes : Form
    {
        public KategoriaSzerkesztes()
        {
            InitializeComponent();
        }

        public string categoryToChange;

        private List<string> Columns = new List<string>();
        DataTable dt = new DataTable();

        private void loadCategoryCols()
        {
            dt.Columns.Clear();
            Columns.Clear();

            Program.conn.Open();
            string sqlCommand = "SHOW COLUMNS FROM " + categoryToChange;
            MySqlCommand cmd = new MySqlCommand(sqlCommand, Program.conn);
            cmd.CommandType = CommandType.Text;
            MySqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                Columns.Add(rdr["Field"].ToString());
            }
            rdr.Close();
            Program.conn.Close();

            foreach (string col in Columns)
            {
                dt.Columns.Add(col);
            }

            dataGridView1.DataSource = dt;
            dataGridView1.Columns[0].Visible = false;

            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            textBox1.Clear();
            textBox2.Clear();
            textBox1.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
        }

        private void Form4_Load(object sender, EventArgs e)
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

            dataGridView1.ForeColor = Color.Black;

            loadCategoryCols();
            this.Text = $"{categoryToChange} Kategória Szerkesztése";
        }

        private static int colIndex;
        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            colIndex = dataGridView1.Columns[e.ColumnIndex].Index;
            textBox1.Text = dataGridView1.Columns[e.ColumnIndex].HeaderText.ToString();

            textBox1.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = true;
        }

        //Átnevezés
        private void rename()
        {
            string oldname = dataGridView1.Columns[colIndex].HeaderText.ToString();
            string newname = textBox1.Text;
            newname = Regex.Replace(newname, @"[^0-9a-zA-ZÖöÜüÓóŐőÚúÉéÁáŰűÍí]+", "");

            if (oldname == newname)
            {
                MessageBox.Show("Új név nem lehet ugyan az mint a régi név.");
            }

            else
            {
                Program.conn.Open();
                string sqlCommand = "ALTER TABLE " + categoryToChange + " CHANGE COLUMN " + oldname + " " + newname + " TEXT";
                MySqlCommand cmd = new MySqlCommand(sqlCommand, Program.conn);
                cmd.CommandType = CommandType.Text;
                MySqlDataReader rdr = cmd.ExecuteReader();



                rdr.Close();
                Program.conn.Close();

                Program.writeReport($"Kategória [{Program.category}] oszlopa [{oldname}] átnevezve: {newname}");
                loadCategoryCols();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            rename();
        }
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                rename();
            }
        }

        //Törlés
        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Biztosan törölni szeretné az adatot?", "", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                Program.conn.Open();
                string sqlCommand = "ALTER TABLE " + categoryToChange + " DROP COLUMN " + dataGridView1.Columns[colIndex].HeaderText.ToString();
                MySqlCommand cmd = new MySqlCommand(sqlCommand, Program.conn);
                cmd.CommandType = CommandType.Text;
                MySqlDataReader rdr = cmd.ExecuteReader();



                rdr.Close();
                Program.conn.Close();

                Program.writeReport($"Kategória [{Program.category}] oszlopa [{dataGridView1.Columns[colIndex].HeaderText.ToString()}] törölve");
                loadCategoryCols();
            }
        }

        //Hozzáadás
        private void add()
        {
            string colName = textBox2.Text;
            colName = Regex.Replace(colName, @"[^0-9a-zA-ZÖöÜüÓóŐőÚúÉéÁáŰűÍí]+", "");

            Program.conn.Open();
            string sqlCommand = "SHOW COLUMNS FROM " + categoryToChange;
            MySqlCommand cmd = new MySqlCommand(sqlCommand, Program.conn);
            cmd.CommandType = CommandType.Text;
            MySqlDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                if (rdr["Field"].ToString().Contains(colName))
                {
                    MessageBox.Show("Ilyen oszlop már létezik!");
                    rdr.Close();
                    Program.conn.Close();
                    return;
                }
            }
            rdr.Close();
            Program.conn.Close();


            Program.conn.Open();
            string sqlCommand2 = "ALTER TABLE " + categoryToChange + " ADD COLUMN " + colName + " TEXT AFTER " + Columns.Last();
            MySqlCommand cmd2 = new MySqlCommand(sqlCommand2, Program.conn);
            cmd.CommandType = CommandType.Text;
            MySqlDataReader rdr2 = cmd2.ExecuteReader();

            rdr2.Close();
            Program.conn.Close();

            Program.writeReport($"Kategória [{Program.newCategory}] oszlop hozzáadva: {colName}");
            loadCategoryCols();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            add();
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                add();
            }
        }
    }
}
