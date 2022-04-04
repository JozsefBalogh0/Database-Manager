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
    public partial class AdatSzerkesztes : Form
    {
        public Sor kivalasztottSor;

        public List<string> Columns = new List<string>();

        DataTable dt = new DataTable();

        public AdatSzerkesztes()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            this.BackColor = Program.backGroundColor;
            this.ForeColor = Program.textColor;

            foreach (var groupBox in this.Controls.OfType<GroupBox>())
            {
                groupBox.BackColor = Program.foreGroundColor;
                groupBox.ForeColor = Program.textColor;
            }

            foreach (var button in this.Controls.OfType<Button>())
            {
                button.BackColor = Program.buttonColor;
            }

            listView1.View = View.Details;

            foreach (string col in Columns)
            {
                listView1.Columns.Add(col);
                dt.Columns.Add(col);
            }
            listView1.Items.Add(new ListViewItem(kivalasztottSor.data.ToArray<string>()));
            dt.Rows.Add(kivalasztottSor.data.ToArray<string>());

            dataGridView1.DataSource = dt;
            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.ForeColor = Color.Black;

            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            this.Text = $"{Program.category} Adatszerkesztés";

        }

        //Mentés
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

            List<string> stringList = new List<string>();

            for (int i = 0; i < Columns.Count; i++)
            {
                stringList.Add(Columns[i] + "='" + Regex.Replace(dt.Rows[0].ItemArray[i].ToString(), @"[^0-9a-zA-ZÖöÜüÓóŐőÚúÉéÁáŰűÍí ]+", "").Trim() + "'");
            }

            string allEditedItems = string.Join(",", stringList.Select(x => x.ToString()).ToArray());

            Program.conn.Open();
            string sqlCommand = "UPDATE " + Program.category + " SET " + allEditedItems + " WHERE ID =" + dt.Rows[0].ItemArray[0];
            MySqlCommand cmd = new MySqlCommand(sqlCommand, Program.conn);
            cmd.CommandType = CommandType.Text;
            MySqlDataReader rdr = cmd.ExecuteReader();



            rdr.Close();
            Program.conn.Close();

            Program.writeReport($"Adat [{string.Join(",", kivalasztottSor.data)}] megváltoztatva [{Program.category}] kategóriában: {string.Join(",", stringList)}");
            MessageBox.Show("Az adat elmentve.");
            this.Close();
        }

        //Törlés
        private void button3_Click(object sender, EventArgs e)
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
                string sqlCommand = "DELETE FROM " + Program.category + " WHERE ID =" + dt.Rows[0].ItemArray[0];
                MySqlCommand cmd = new MySqlCommand(sqlCommand, Program.conn);
                cmd.CommandType = CommandType.Text;
                MySqlDataReader rdr = cmd.ExecuteReader();



                rdr.Close();
                Program.conn.Close();

                Program.writeReport($"Adat [{string.Join(",", kivalasztottSor.data)}] törölve [{Program.category}] kategóriában.");
                MessageBox.Show("Az adat törölve.");
                this.Close();
            }
        }

        //Mégse
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
