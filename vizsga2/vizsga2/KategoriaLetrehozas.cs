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
    public partial class KategoriaLetrehozas : Form
    {
        public KategoriaLetrehozas()
        {
            InitializeComponent();
        }

        private List<string> Columns = new List<string>();
        DataTable dt = new DataTable();

        private void loadCategoryCols()
        {
            dt.Columns.Clear();
            Columns.Clear();

            Program.conn.Open();
            string sqlCommand = "SHOW COLUMNS FROM " + Program.newCategory;
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

            textBox1.Clear();
            textBox2.Clear();
            textBox1.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
        }

        private void Form5_Load(object sender, EventArgs e)
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

            dataGridView1.ForeColor = Color.Black;
            dataGridView2.ForeColor = Color.Black;

            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            loadCategoryCols();
            listView1.View = View.Details;

            dataGridView2.Enabled = false;
            listView1.Enabled = false;
            button4.Enabled = false;

            this.Text = $"{Program.newCategory} Létrehozása";
        }

        private static int colIndex;
        private void dataGridView1_ColumnHeaderMouseClick_1(object sender, DataGridViewCellMouseEventArgs e)
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

            if (oldname == newname)
            {
                MessageBox.Show("Új név nem lehet ugyan az mint a régi név.");
            }

            else
            {
                if (newname.Contains(" "))
                {
                    MessageBox.Show("Az oszlop neve nem tartalmazhat szóközt!");
                }
                else
                {
                    Program.conn.Open();
                    string sqlCommand = "ALTER TABLE " + Program.newCategory + " CHANGE COLUMN " + oldname + " " + newname + " TEXT";
                    MySqlCommand cmd = new MySqlCommand(sqlCommand, Program.conn);
                    cmd.CommandType = CommandType.Text;
                    MySqlDataReader rdr = cmd.ExecuteReader();



                    rdr.Close();
                    Program.conn.Close();

                    Program.writeReport($"Új kategória [{Program.newCategory}] oszlopa [{oldname}] átnevezve: {newname}");
                    loadCategoryCols();
                    loadData();
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            rename();
        }
        private void textBox1_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                rename();
            }
        }

        //Törlés
        private void button2_Click_1(object sender, EventArgs e)
        {
            Program.conn.Open();
            string sqlCommand = "ALTER TABLE " + Program.newCategory + " DROP COLUMN " + dataGridView1.Columns[colIndex].HeaderText.ToString();
            MySqlCommand cmd = new MySqlCommand(sqlCommand, Program.conn);
            cmd.CommandType = CommandType.Text;
            MySqlDataReader rdr = cmd.ExecuteReader();



            rdr.Close();
            Program.conn.Close();

            Program.writeReport($"Új kategória [{Program.newCategory}] oszlopa törölve: {dataGridView1.Columns[colIndex].HeaderText.ToString()}");
            loadCategoryCols();
            loadData();
        }

        //Hozzáadás
        private void add()
        {
            string colName = textBox2.Text;
            if (char.IsDigit(colName[0]))
            {
                MessageBox.Show("Az oszlop neve nem kezdőthet számmal!");
                return;
            }

            if (colName.Contains(" "))
            {
                MessageBox.Show("Az oszlop neve nem tartalmazhat szóközt!");
            }
            else
            {
                colName = Regex.Replace(colName, @"[^0-9a-zA-ZÖöÜüÓóŐőÚúÉéÁáŰűÍí]+", "");

                Program.conn.Open();
                string sqlCommand = "SHOW COLUMNS FROM " + Program.newCategory;
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
                string sqlCommand2 = "ALTER TABLE " + Program.newCategory + " ADD COLUMN " + colName + " TEXT AFTER " + Columns.Last();
                MySqlCommand cmd2 = new MySqlCommand(sqlCommand2, Program.conn);
                cmd.CommandType = CommandType.Text;
                MySqlDataReader rdr2 = cmd2.ExecuteReader();
                
                rdr2.Close();
                Program.conn.Close();

                Program.writeReport($"Új kategória [{Program.newCategory}] oszlop hozzáadva: {colName}");
                loadCategoryCols();
                loadData();
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            add();
        }

        private void textBox2_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                add();
            }
        }

        //Mégse
        private void button5_Click(object sender, EventArgs e)
        {
            Program.conn.Open();
            string sqlCommand = "DROP TABLE " + Program.newCategory;
            MySqlCommand cmd = new MySqlCommand(sqlCommand, Program.conn);
            cmd.CommandType = CommandType.Text;
            MySqlDataReader rdr = cmd.ExecuteReader();

            rdr.Close();
            Program.conn.Close();

            Program.writeReport($"Új kategória [{Program.newCategory}] létrehozás törölve");
            Program.newCategory = null;
            this.Close();
        }

        //Létrehozás
        private void button6_Click(object sender, EventArgs e)
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

            Program.writeReport($"Új kategória [{Program.newCategory}] létrehozva");
            this.Close();
        }

        //Adat hozzáadás

        private List<string> ColumnsData = new List<string>();
        DataTable dtData = new DataTable();
        static List<Sor> sorok = new List<Sor>() { };

        private void loadData()
        {
            if (Program.newCategory != null)
            {
                dtData.Rows.Clear();
                dtData.Columns.Clear();
                sorok.Clear();
                listView1.Clear();
                ColumnsData.Clear();

                Program.conn.Open();
                string sqlCommand = "SHOW COLUMNS FROM " + Program.newCategory;
                MySqlCommand cmd = new MySqlCommand(sqlCommand, Program.conn);
                cmd.CommandType = CommandType.Text;

                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    ColumnsData.Add(rdr["Field"].ToString());
                }
                rdr.Close();

                foreach (string col in Columns)
                {
                    listView1.Columns.Add(col);
                    dtData.Columns.Add(col);
                }

                MySqlCommand selectAllRow = new MySqlCommand("SELECT * FROM " + Program.newCategory, Program.conn);
                selectAllRow.CommandType = CommandType.Text;
                MySqlDataReader rdr2 = selectAllRow.ExecuteReader();

                while (rdr2.Read())
                {
                    Sor s = new Sor();

                    for (int i = 0; i < rdr2.FieldCount; i++)
                    {
                        s.AddData(rdr2.GetValue(i).ToString());
                    }
                    sorok.Add(s);
                }

                rdr2.Close();

                Program.conn.Close();

                for (int i = 0; i < sorok.Count; i++)
                {
                    listView1.Items.Add(new ListViewItem(sorok[i].data.ToArray<string>()));
                }

                dataGridView2.DataSource = dtData;
                dataGridView2.Columns[0].Visible = false;

                foreach (DataGridViewColumn col in dataGridView2.Columns)
                {
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dtData.Rows.Count; i++)
            {
                Sor newRow = new Sor();

                for (int j = 0; j < dtData.Rows[i].ItemArray.Count(); j++)
                {
                    newRow.AddData("'" + Regex.Replace(dtData.Rows[i].ItemArray[j].ToString(), @"[^0-9a-zA-ZÖöÜüÓóŐőÚúÉéÁáŰűÍí ]+", "").Trim() + "'");
                }

                newRow.data[0] = null;

                string allCols = string.Join(",", ColumnsData.Select(x => x.ToString()).ToArray());
                string rowData = string.Join(",", newRow.data.ToArray());

                Program.conn.Open();
                string sqlCommand = "INSERT INTO " + Program.newCategory + " (" + allCols + ") VALUES (NULL " + rowData + ")";
                MySqlCommand cmd = new MySqlCommand(sqlCommand, Program.conn);
                cmd.CommandType = CommandType.Text;
                MySqlDataReader rdr = cmd.ExecuteReader();

                rdr.Close();
                Program.conn.Close();

                newRow.data[0] = "NULL";
                string rowDataRep = string.Join(",", newRow.data.ToArray());
                Program.writeReport($"Új kategória [{Program.newCategory}] adat hozzáadva: {rowDataRep}");
            }
            loadData();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                dataGridView2.Enabled = true;
                listView1.Enabled = true;
                button4.Enabled = true;
            }
            else
            {
                dataGridView2.Enabled = false;
                listView1.Enabled = false;
                button4.Enabled = false;
            }
        }
    }
}
