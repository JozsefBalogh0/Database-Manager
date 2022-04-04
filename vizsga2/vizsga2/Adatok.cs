using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace vizsga2
{
    public partial class Adatok : Form
    {
        public Adatok()
        {
            InitializeComponent();
        }

        private List<string> Columns = new List<string>();
        DataTable dt = new DataTable();
        DataTable dtDisplay = new DataTable();

        private void loadData()
        {
            if (Program.category != null)
            {
                this.Text = $"{Program.category} Adatai";
                dt.Rows.Clear();
                dt.Columns.Clear();
                sorok.Clear();
                dtDisplay.Rows.Clear();
                dtDisplay.Columns.Clear();
                Columns.Clear();

                dtDisplay.DefaultView.Sort = string.Empty;

                Program.conn.Open();
                string sqlCommand = "SHOW COLUMNS FROM " + Program.category;
                MySqlCommand cmd = new MySqlCommand(sqlCommand, Program.conn);
                cmd.CommandType = CommandType.Text;

                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Columns.Add(rdr["Field"].ToString());
                }
                rdr.Close();

                foreach (string col in Columns)
                {
                    dtDisplay.Columns.Add(col);
                    dt.Columns.Add(col);
                }

                MySqlCommand selectAllRow = new MySqlCommand("SELECT * FROM " + Program.category, Program.conn);
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
                    dtDisplay.Rows.Add(sorok[i].data.ToArray<string>());
                }
                dataGridView2.DataSource = dtDisplay;
                
                dataGridView1.DataSource = dt;
                dataGridView1.Columns[0].Visible = false;

                foreach (DataGridViewColumn col in dataGridView2.Columns)
                {
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }

                foreach (DataGridViewColumn col in dataGridView1.Columns)
                {
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
            }
        }

        static List<Sor> sorok = new List<Sor>() { };

        private void Form1_Load(object sender, EventArgs e)
        {
            this.BackColor = Program.backGroundColor;
            this.ForeColor = Program.textColor;

            foreach (var groupBox in this.Controls.OfType<GroupBox>())
            {
                groupBox.BackColor = Program.foreGroundColor;
                groupBox.ForeColor = Program.textColor;

                foreach (var groupBox2 in groupBox.Controls.OfType<GroupBox>())
                {
                    foreach (var button in groupBox2.Controls.OfType<Button>())
                    {
                        button.BackColor = Program.buttonColor;
                    }
                }

                foreach (var button in groupBox.Controls.OfType<Button>())
                {
                    button.BackColor = Program.buttonColor;
                }
            }

            dataGridView2.ForeColor = Color.Black;
            dataGridView1.ForeColor = Color.Black;

            JelszoKero askPAss = new JelszoKero();
            askPAss.ShowDialog();

            if (Program.passwordCorrect == false)
            {
                this.Close();
                return;
            }

            button3.Enabled = false;

            KategoriaValasztas form = new KategoriaValasztas();
            form.ShowDialog();
            this.Refresh();

            loadData();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            KategoriaValasztas form = new KategoriaValasztas();
            form.ShowDialog();
            this.Refresh();
            loadData();
        }

        //Módosítás
        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView2.Rows[e.RowIndex];
                Sor kivalasztott = sorok[Convert.ToInt32(row.Cells[0].Value) - 1];



                AdatSzerkesztes modositasForm = new AdatSzerkesztes();
                modositasForm.Columns = Columns;
                modositasForm.kivalasztottSor = kivalasztott;
                modositasForm.ShowDialog();
                loadData();
            }
        }

        private void search()
        {
            string keresndo = textBox1.Text.ToLower();
            List<Sor> query = new List<Sor>();

            for (int i = 0; i < sorok.Count; i++)
            {
                if (sorok[i].data.ConvertAll(x => x.ToLower()).Contains(keresndo) || sorok[i].data.ConvertAll(x => x.ToLower()).Any(x => x.Contains(keresndo)))
                {
                    query.Add(sorok[i]);
                }
            }

            dtDisplay.Rows.Clear();

            if (keresndo.Trim() == "")
            {
                for (int i = 0; i < sorok.Count; i++)
                {
                    dtDisplay.Rows.Add(sorok[i].data.ToArray<string>());
                }
            }
            else
            {
                for (int i = 0; i < query.Count; i++)
                {
                    dtDisplay.Rows.Add(query[i].data.ToArray<string>());
                }
            }
            query.Clear();
        }

        //Keresés
        private void button2_Click(object sender, EventArgs e)
        {
            search();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                search();
            }
        }

        //Hozzáadás
        private void button4_Click(object sender, EventArgs e)
        {
            if (!(dt.Rows.Count > 0))
            {
                MessageBox.Show("Kérem adjon meg adatot.");
                return;
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {   
                Sor newRow = new Sor();

                for (int j = 0; j < dt.Rows[i].ItemArray.Count(); j++)
                {
                    newRow.AddData("'" + Regex.Replace(dt.Rows[i].ItemArray[j].ToString(), @"[^0-9a-zA-ZÖöÜüÓóŐőÚúÉéÁáŰűÍí ]+", "").Trim() + "'");
                }
                
                newRow.data[0] = null;

                string allCols = string.Join(",", Columns.Select(x => x.ToString()).ToArray());
                string rowData = string.Join(",", newRow.data.ToArray());

                Program.conn.Open();
                string sqlCommand = "INSERT INTO " + Program.category + " (" + allCols + ") VALUES (NULL " + rowData + ")";
                MySqlCommand cmd = new MySqlCommand(sqlCommand, Program.conn);
                cmd.CommandType = CommandType.Text;
                MySqlDataReader rdr = cmd.ExecuteReader();

                rdr.Close();
                Program.conn.Close();

                newRow.data[0] = "NULL";
                string rowDataRep = string.Join(",", newRow.data.ToArray());
                Program.writeReport($"Adat hozzáadva [{Program.category}] kategóriához: {rowDataRep}");
            }

            loadData();
            MessageBox.Show("Adat hozzáadva.");
        }

        //Szerkesztés
        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView2.CurrentRow.Index >= 0)
            {
                DataGridViewRow row = dataGridView2.CurrentRow;
                Sor kivalasztott = sorok[Convert.ToInt32(row.Cells[0].Value) - 1];

                AdatSzerkesztes modositasForm = new AdatSzerkesztes();
                modositasForm.Columns = Columns;
                modositasForm.kivalasztottSor = kivalasztott;
                modositasForm.ShowDialog();
                loadData();
            }
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                button3.Enabled = true;
            }
            else
            {
                button3.Enabled = false;
            }
        }

        //Adatexportálás
        private void button5_Click(object sender, EventArgs e)
        {
            string csv = string.Empty;

            foreach (DataGridViewColumn column in dataGridView2.Columns)
            {
                csv += column.HeaderText + ",";
            }

            csv += "\r\n";
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    csv += cell.Value.ToString().Replace(",", ";") + ",";
                }
                csv += "\r\n";
            }

            File.WriteAllText($"{Program.DataExportPath}/{Program.category} {DateTime.Now.ToString("yyyy.MM.dd")}.csv", csv, Encoding.UTF8);
            DialogResult dialogResult = MessageBox.Show("Adatok exportálva.\nMeg szeretné nyitni a tartalmazó mappát?", "", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                System.Diagnostics.Process.Start("explorer.exe", Program.DataExportPath);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Beallitasok settings = new Beallitasok();
            settings.ShowDialog();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Biztos ki szeretne lépni?", "", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}
