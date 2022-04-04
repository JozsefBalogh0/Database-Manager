using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace vizsga2
{
    public partial class Beallitasok : Form
    {
        public Beallitasok()
        {
            InitializeComponent();
        }

        private void Beallitasok_Load(object sender, EventArgs e)
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

            button4.BackColor = Program.backGroundColor;
            button5.BackColor = Program.foreGroundColor;
            button6.BackColor = Program.textColor;
            button6.ForeColor = Program.backGroundColor;

            checkBox1.Checked = Program.askPassword;
            textBox1.Text = Program.DataExportPath;
        }

        //Mégse
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public bool requireRestart = false;

        //Mentés
        private void button2_Click(object sender, EventArgs e)
        {
            if (Program.password == Program.processPassword(textBox2.Text))
            {
                MessageBox.Show("Új jelszó nem lehet ugyan az mint a jelenlegi jelszó!");
                return;
            }

            JelszoKero askPAss = new JelszoKero();
            if (!string.IsNullOrEmpty(textBox2.Text))
            {
                askPAss.text = "Kérem adja meg a jelenlegi jelszót:";
            }

            askPAss.ShowDialog();

            if (Program.passwordCorrect == false)
            {
                return;
            }
            else
            {
                var lines = File.ReadAllLines("config.txt");

                if (!string.IsNullOrEmpty(textBox2.Text))
                {
                    lines[0] = $"Password={Program.processPassword(textBox2.Text)}";
                    Program.writeReport($"Jelszó módosítva");
                }

                lines[1] = $"AskPassword={checkBox1.Checked}";
                lines[2] = $"DataExportPath={textBox1.Text}";

                if (backGroundColor.A != 0)
                {
                    lines[3] = $"BackGroundColor={backGroundColor.ToArgb()}";
                    requireRestart = true;
                }
                if (foreGroundColor.A != 0)
                {
                    lines[4] = $"ForeGroundColor={foreGroundColor.ToArgb()}";
                    requireRestart = true;
                }
                if (textColor.A != 0)
                {
                    lines[5] = $"TextColor={textColor.ToArgb()}";
                    requireRestart = true;
                }
                if (buttonColor.A != 0)
                {
                    lines[6] = $"ButtonColor={buttonColor.ToArgb()}";
                    requireRestart = true;
                }

                File.WriteAllLines("config.txt", lines);
                textBox2.Clear();


                Program.loadConfig();
                MessageBox.Show("Változtatások elmentve.");

                if (requireRestart)
                {
                    DialogResult dialogResult = MessageBox.Show("Néhány beállítás frissítéséhez újra kell indítani a programot.\nÚjra szeretné indítani most?", "", MessageBoxButtons.YesNo);

                    if (dialogResult == DialogResult.Yes)
                    {
                        Application.Restart();
                        Environment.Exit(0);
                    }
                }
            }
        }

        //Adat Expo Path
        private void button3_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.SelectedPath = Program.DataExportPath;
                DialogResult newPath = fbd.ShowDialog();

                if (newPath == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    textBox1.Text = fbd.SelectedPath.ToString();
                    Program.writeReport($"Adatexportálás helye megváltoztatva: {fbd.SelectedPath.ToString()}");
                }
            }
        }

        static private Color backGroundColor;
        static private Color foreGroundColor;
        static private Color textColor;
        static private Color buttonColor;

        //Színek
        private void button4_Click(object sender, EventArgs e)
        {
            ColorDialog backColor = new ColorDialog();
            backColor.FullOpen = true;
            backColor.Color = Program.backGroundColor;

            if (backColor.ShowDialog() == DialogResult.OK)
            {
                backGroundColor = backColor.Color;
                Program.writeReport($"Háttérszín módosítva: {backGroundColor.ToArgb()}");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ColorDialog foreColor = new ColorDialog();
            foreColor.FullOpen = true;
            foreColor.Color = Program.foreGroundColor;

            if (foreColor.ShowDialog() == DialogResult.OK)
            {
                foreGroundColor = foreColor.Color;
                Program.writeReport($"Előtéri szín módosítva: {foreGroundColor.ToArgb()}");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ColorDialog textColorDial = new ColorDialog();
            textColorDial.FullOpen = true;
            textColorDial.Color = Program.textColor;

            if (textColorDial.ShowDialog() == DialogResult.OK)
            {
                textColor = textColorDial.Color;
                Program.writeReport($"Szövegszín módosítva: {textColor.ToArgb()}");
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            ColorDialog buttonColorDial = new ColorDialog();
            buttonColorDial.FullOpen = true;
            buttonColorDial.Color = Program.buttonColor;

            if (buttonColorDial.ShowDialog() == DialogResult.OK)
            {
                buttonColor = buttonColorDial.Color;
                Program.writeReport($"Gombszín módosítva: {buttonColor.ToArgb()}");
            }
        }
    }
}
