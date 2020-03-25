using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace EtrendKeszito
{
    enum EtkezesTipus
    {
        breakfast,
        lunch,
        dinner
    }

    enum EtkezesTipus2
    {
        breakfast,
        lunch,
        dinner,
        daily
    }

    enum ElelmiszerTipus2
    {
        food,
        drink
    }

    enum Szamlalo
    {
        count,
        quantity
    }

    enum ElelmiszerTipus
    {
        meal,
        drink,
        menu
    }

    enum MertekegysegFajta
    {
        weight,
        liquidmeasure,
        energy
    }

    enum TapanyagMertekegyseg
    {
        gram,
        milligram,
        calorie,
        joule
    }

    enum default_mertekegysegek
    {
        gram,
        liter,
        calorie
    }

    public partial class Form1 : Form
    {
        private ListBox listBox1;

        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label label9;

        private Button button1;
        private Button button2;
        private Button button3;
        private Button button4;
        private Button button5;
        private Button button6;
        private Button button7;
        private Button button8;
        private Button button9;
        private Button button10;

        private DateTimePicker dateTimePicker1;
        private DateTimePicker dateTimePicker2;

        private CheckBox checkBox1;
        private CheckBox checkBox2;
        private CheckBox checkBox3;
        private CheckBox checkBox4;
        private CheckBox checkBox5;

        private NumericUpDown numericUpDown1;
        private NumericUpDown numericUpDown2;
        private NumericUpDown numericUpDown3;

        private TextBox textBox1;

        private ComboBox comboBox1;
        private ComboBox comboBox2;
        private ComboBox comboBox3;

        private RadioButton radioButton1;
        private RadioButton radioButton2;

        private OpenFileDialog openFileDialog1;
        private SaveFileDialog saveFileDialog1;

        public Form1()
        {
            try
            {
                Logolas.Ment("The program has been opened.");

                Csatlakozas();
                BejelentkezoForm dialogus = new BejelentkezoForm();

                if (dialogus.ShowDialog() != DialogResult.OK)
                {
                    Logolas.Ment("The program has been closed." + Environment.NewLine + "----------------------------");
                    Close();
                }
                else
                {
                    InitializeComponent();
                    Inits.Init();
                    VezerlokLetrehozasa();
                    AddButtonClick();

                    FormRefresh();
                    EtrendFeltetelMentes();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        void FormRefresh()
        {
            try
            {
                EtrendFeltetelKiolvas();
                ComboBox3_SelectedIndexChanged(new object(), new EventArgs());
                ListBoxRefresh();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        void AddButtonClick()
        {
            button1.Click += Button1_Click;
            button2.Click += Button2_Click;
            button3.Click += Button3_Click;
            button4.Click += Button4_Click;
            button5.Click += Button5_Click;
            button6.Click += Button6_Click;
            button7.Click += Button7_Click;
            button8.Click += Button8_Click;
            button9.Click += Button9_Click;
            button10.Click += Button10_Click;
            radioButton1.CheckedChanged += RadioButton1_CheckedChanged;
            radioButton2.CheckedChanged += RadioButton2_CheckedChanged;
            checkBox1.CheckedChanged += CheckBox1_CheckedChanged;
            checkBox2.CheckedChanged += CheckBox2_CheckedChanged;
            checkBox3.CheckedChanged += CheckBox3_CheckedChanged;
            checkBox4.CheckedChanged += CheckBox4_CheckedChanged;
            checkBox5.CheckedChanged += CheckBox5_CheckedChanged;

            dateTimePicker1.ValueChanged += DateTimePicker1_ValueChanged;
            dateTimePicker2.ValueChanged += DateTimePicker2_ValueChanged;

            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
            comboBox2.SelectedIndexChanged += ComboBox2_SelectedIndexChanged;
            comboBox3.SelectedIndexChanged += ComboBox3_SelectedIndexChanged;

            numericUpDown1.ValueChanged += NumericUpDown1_ValueChanged;
            numericUpDown2.ValueChanged += NumericUpDown2_ValueChanged;
            numericUpDown3.ValueChanged += NumericUpDown3_ValueChanged;

            listBox1.DoubleClick += ListBox1_DoubleClick;
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count > 0)
            {
                KeresesForm dialogus = new KeresesForm(radioButton1.Checked,null);
                dialogus.ShowDialog();
            }
        }

        private void ListBox1_DoubleClick(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                KeresesForm dialogus = new KeresesForm(radioButton1.Checked,(Elelmiszer)listBox1.SelectedItem);
                dialogus.ShowDialog();
            }
        }

        private void Button10_Click(object sender, EventArgs e)
        {
            Button10_Click_general(sender, e);
        }


        private void Button10_Click_general(object sender, EventArgs e, string str = null, bool msg = true)
        {//xml exportálás
            if (str != null || saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (str == null) str = saveFileDialog1.FileName;
                    EtrendXML.XMLSave(str, msg);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Button9_Click(object sender, EventArgs e)
        {
            Button9_Click_general(sender, e);
        }

        private void Button9_Click_general(object sender, EventArgs e, string filename = null, bool save = true, bool check = true, bool msg = true)
        {//xml importálás
            if (filename != null || openFileDialog1.ShowDialog() == DialogResult.OK)
            {

                string fnevhash = ABKezelo.GetCurrentUser();
                try
                {
                    if (save)
                    {
                        Button10_Click_general(sender, e, "tempetrend.xml", false);
                    }
                    ABKezelo.TorolMinden();

                    if (filename == null)
                    {
                        filename = openFileDialog1.FileName;
                    }

                    EtrendXML.XMLRead(filename, check, fnevhash, msg);
                    FormRefresh();
                }
                catch (Exception ex)
                {
                    Button9_Click_general(sender, e, "tempetrend.xml", false, false, false);
                    MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void CheckBox5_CheckedChanged(object sender, EventArgs e)
        {
            EtrendFeltetelMentes();
        }

        private void CheckBox4_CheckedChanged(object sender, EventArgs e)
        {
            EtrendFeltetelMentes();
        }

        private void ComboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Penznem p = (Penznem)ABKezelo.Kiolvasas()
                    .Where(x => x is Penznem && (x as Penznem).Megnevezes == comboBox3.SelectedItem.ToString()).First();
                numericUpDown1.DecimalPlaces = p.TizedesekSzama;
                EtrendFeltetelMentes();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            EtrendFeltetelMentes();
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            EtrendFeltetelMentes();
        }

        private void NumericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            EtrendFeltetelMentes();
        }

        private void NumericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            EtrendFeltetelMentes();
        }

        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            EtrendFeltetelMentes();
        }

        private void CheckBox3_CheckedChanged(object sender, EventArgs e)
        {
            EtrendFeltetelMentes();
        }

        void EtrendFeltetelKiolvas()
        {
            try
            {
                var e = ABKezelo.Kiolvasas().Where(x => x is EtrendFeltetel).ToList();
                if (e.Count != 0)
                {
                    EtrendFeltetel et = e.First() as EtrendFeltetel;
                    dateTimePicker1.Value = et.Datum1;
                    comboBox1.SelectedIndex = (int)et.Etkezes1;
                    dateTimePicker2.Value = et.Datum2;
                    comboBox2.SelectedIndex = (int)et.Etkezes2;
                    comboBox3.SelectedItem = et.Penz.Megnevezes;
                    numericUpDown1.DecimalPlaces = et.Penz.TizedesekSzama;
                    numericUpDown1.Value = (decimal)et.Maxpenz;

                    checkBox1.Checked = et.Koltsegmin;
                    checkBox2.Checked = et.Orommax;
                    checkBox3.Checked = et.Solverelrejt;
                    checkBox4.Checked = et.Folytonosmodell;
                    checkBox5.Checked = et.NaptarbaMent;
                    numericUpDown2.Value = et.Numvaltozatossag;
                    numericUpDown3.Value = et.Maxfutasiido;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void EtrendFeltetelMentes()
        {
            try
            {
                string fnevhash = ABKezelo.GetCurrentUser();

                Penznem p = (Penznem)ABKezelo.Kiolvasas().Where(x => (x is Penznem) && (x as Penznem).Megnevezes == comboBox3.SelectedItem.ToString()).First();

                EtrendFeltetel e = new EtrendFeltetel(fnevhash, dateTimePicker1.Value, (EtkezesTipus)comboBox1.SelectedIndex, dateTimePicker2.Value, (EtkezesTipus)comboBox2.SelectedIndex, checkBox1.Checked, checkBox2.Checked, (double)numericUpDown1.Value, p, checkBox3.Checked, checkBox4.Checked, (int)numericUpDown2.Value, (int)numericUpDown3.Value, checkBox5.Checked);

                if (ABKezelo.Kiolvasas().Where(x => x is EtrendFeltetel).ToList().Count == 0)
                {
                    ABKezelo.Beszuras(e);
                }
                else
                {
                    ABKezelo.Modositas(e);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            EtrendFeltetelMentes();
        }

        private void DateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            EtrendFeltetelMentes();
        }

        private void Button8_Click(object sender, EventArgs e)
        {
            RendezesForm dialogus = new RendezesForm(radioButton1.Checked);
            dialogus.ShowDialog();
        }

        private void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked && checkBox2.Checked) checkBox1.Checked = false;
            EtrendFeltetelMentes();
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked && checkBox2.Checked) checkBox2.Checked = false;

            bool b = !checkBox1.Checked;
            label6.Visible = b;
            numericUpDown1.Visible = b;
            comboBox3.Visible = b;

            EtrendFeltetelMentes();
        }

        private void RadioButton2_CheckedChanged(object sender, EventArgs e)
        {
            ListBoxRefresh();
        }

        private void RadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            ListBoxRefresh();
        }

        private void ListBoxRefresh()
        {
            try
            {
                int index = listBox1.SelectedIndex;
                listBox1.DataSource = null;

                if (radioButton1.Checked) listBox1.DataSource = ABKezelo.Kiolvasas().Where(x => x is Elelmiszer).ToList();
                else
                    listBox1.DataSource = ABKezelo.Kiolvasas().Where(x => x is Elelmiszer && (x as Elelmiszer).Hasznalhato).ToList();

                if (index >= 0 && index < listBox1.Items.Count) listBox1.SelectedIndex = index;
                else if (listBox1.Items.Count > 0) listBox1.SelectedIndex = 0;
                else listBox1.SelectedIndex = -1;

                string nev = comboBox3.SelectedItem.ToString();
                comboBox3.Items.Clear();
                comboBox3.Items.AddRange(ABKezelo.Kiolvasas().Where(x => x is Penznem).Select(p => (p as Penznem).Megnevezes)
                    .ToArray());
                comboBox3.SelectedIndex = 0;
                int cnt = comboBox3.Items.Count;
                for (int i = 0; i < cnt; i++)
                {
                    if (comboBox3.Items[i].ToString() == nev)
                    {
                        comboBox3.SelectedIndex = i;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Button6_Click(object sender, EventArgs eventargs)
        {
            try
            {
                if (numericUpDown3.Value <= 0 || numericUpDown3.Value > Konstans.maxValaszthatoFutasiIdo)
                {
                    throw new ArgumentException("Running time is positive and at most " + Konstans.maxValaszthatoFutasiIdo + " !");
                }

                Logolas.Ment("Diet making/running with the Glpk program.");
                EtrendMegold e0 = new EtrendMegold();

                List<object> ans = e0.Megoldas();

                if ((bool)ans[0])
                {
                    EtrendFeltetel e = (EtrendFeltetel)ABKezelo.Kiolvasas().Where(x => x is EtrendFeltetel).First();

                    var l = ABKezelo.Kiolvasas().Where(x => x is EtrendElelmiszer).ToList();

                    DateTime e1 = e.Datum1;
                    DateTime e2 = e.Datum2;
                    e1 = new DateTime(e1.Year, e1.Month, e1.Day, 0, 0, 0);
                    e2 = new DateTime(e2.Year, e2.Month, e2.Day, 0, 0, 0);
                    DateTime d = e1;

                    string str = "";

                    if (checkBox1.Checked) str += String.Format("Minimum cost: {0} {1}", ans[1], (ans[2] as Penznem).Megnevezes) + Environment.NewLine;
                    else if (checkBox2.Checked) str += String.Format("Maximum joy: {0}", ans[1]) + Environment.NewLine;

                    while (d <= e2)
                    {
                        str += String.Format("{0:yyyy.MM.dd.}", d) + Environment.NewLine;
                        for (int i = 0; i < 3; i++)
                        {
                            if (d == e1 && i < (int)e.Etkezes1) continue;
                            if (d == e2 && i > (int)e.Etkezes2) continue;

                            var l2 = l.Where(x => (x as EtrendElelmiszer).Datum == d && (int)(x as EtrendElelmiszer).Etkezes == i).ToList();

                            str += "   " + (EtkezesTipus)i + ":" + Environment.NewLine;
                            foreach (EtrendElelmiszer item in l2)
                            {
                                Elelmiszer elelmiszer = item.Elelmiszer;
                                if (elelmiszer is Etel)
                                    str += String.Format("      {0:0.00} {1} {2}", item.Tomeg, item.Tomegmertekegyseg.Megnevezes, elelmiszer.Megnevezes);
                                else if (elelmiszer is Ital)
                                    str += String.Format("      {0:0.00} {1} {2}", item.Urmertek, item.Urmertekegyseg.Megnevezes, elelmiszer.Megnevezes);
                                else
                                    str += String.Format("      {0:0.00} {1} [{2:0.00} {3}, {4:0.00} {5}]", item.Val, elelmiszer.Megnevezes, item.Tomeg,
                                        item.Tomegmertekegyseg.Megnevezes, item.Urmertek, item.Urmertekegyseg.Megnevezes);
                                str += Environment.NewLine;
                            }
                        }
                        d = d.AddDays(1);
                    }

                    textBox1.Text = str;

                    File.AppendAllText("diets.txt", "Solution:" + Environment.NewLine + str + Environment.NewLine);

                    if (checkBox5.Checked)
                    {
                        Naptar.NaptarbaRak(); // google calendar-ba mentés, ha checkbox be van jelölve
                    }
                }
                else
                {
                    MessageBox.Show("The linear program has no solution!", "Attention!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    textBox1.Text = "No solution!";
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            Logolas.Ment("Glpk's run has finished.");
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            EtrendBeallitForm dialogus = new EtrendBeallitForm();
            dialogus.ShowDialog();
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            ElelmiszerForm dialogus = new ElelmiszerForm();
            dialogus.ShowDialog();
            ListBoxRefresh();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            TapanyagForm dialogus = new TapanyagForm();
            dialogus.ShowDialog();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            PenznemForm dialogus = new PenznemForm();
            dialogus.ShowDialog();
            ListBoxRefresh();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            MertekegysegForm dialogus = new MertekegysegForm();
            dialogus.ShowDialog();
        }

        void VezerlokLetrehozasa()
        {
            Height = 768;
            Width = 1366;
            CenterToScreen();// képernyő közepére teszi a formot
            Text = "Diet preparing";

            openFileDialog1 = new OpenFileDialog()
            {
                Filter = "XML fájlok | *.xml"
            };

            saveFileDialog1 = new SaveFileDialog()
            {
                Filter = "XML fájlok | *.xml"
            };

            radioButton1 = new RadioButton()
            {
                Left = 15,
                Top = 25,
                Width = 250,
                Text = "All foods",
                Font = new Font(FontFamily.GenericSansSerif, 12),
                ForeColor = Color.Indigo,
                Checked = true,
                Parent = this
            };

            radioButton2 = new RadioButton()
            {
                Left = radioButton1.Left,
                Top = radioButton1.Bottom + 15,
                Width = 250,
                Text = "Foods used for diet",
                Font = new Font(FontFamily.GenericSansSerif, 12),
                ForeColor = Color.Indigo,
                Parent = this
            };

            listBox1 = new ListBox()
            {
                Left = radioButton1.Left,
                Top = radioButton2.Bottom + 15,
                Height = 400,
                Width = 440,
                Font = new Font(FontFamily.GenericSansSerif, 12),
                ForeColor = Color.Indigo,
                Parent = this
            };
            button1 = new Button()
            {
                Left = listBox1.Right + 30,
                Top = radioButton1.Top,
                Width = 320,
                Text = "Units edit",
                Font = new Font(FontFamily.GenericSansSerif, 12),
                ForeColor = Color.DarkGreen,
                AutoSize = true,
                Parent = this
            };

            button2 = new Button()
            {
                Left = button1.Left,
                Top = button1.Bottom + 10,
                Width = button1.Width,
                Text = "Currency edit",
                Font = new Font(FontFamily.GenericSansSerif, 12),
                ForeColor = Color.DarkGreen,
                AutoSize = true,
                Parent = this
            };

            button3 = new Button()
            {
                Left = button1.Left,
                Top = button2.Bottom + 10,
                Width = button1.Width,
                Text = "Nutrients edit",
                Font = new Font(FontFamily.GenericSansSerif, 12),
                ForeColor = Color.DarkGreen,
                AutoSize = true,
                Parent = this
            };

            button4 = new Button()
            {
                Left = button1.Left,
                Top = button3.Bottom + 10,
                Width = button1.Width,
                Text = "Foods edit",
                Font = new Font(FontFamily.GenericSansSerif, 12),
                ForeColor = Color.DarkGreen,
                AutoSize = true,
                Parent = this
            };

            label2 = new Label()
            {
                Left = button1.Left,
                Top = button4.Bottom + 20,
                Text = "Starting date:",
                Font = new Font(FontFamily.GenericSansSerif, 10),
                ForeColor = Color.Brown,
                AutoSize = true,
                Parent = this
            };

            dateTimePicker1 = new DateTimePicker()
            {
                Left = label2.Right + 25,
                Top = label2.Top,
                CustomFormat = "yyyy.MM.dd",
                MinDate = DateTime.Parse("2001.01.01"),
                AutoSize = true,
                Parent = this
            };

            label3 = new Label()
            {
                Left = label2.Left,
                Top = label2.Bottom + 10,
                Text = "Starting meal:",
                Font = new Font(FontFamily.GenericSansSerif, 10),
                ForeColor = Color.Brown,
                AutoSize = true,
                Parent = this
            };

            comboBox1 = new ComboBox()
            {
                Parent = this,
                DataSource = Enum.GetValues(typeof(EtkezesTipus)),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Left = dateTimePicker1.Left,
                Top = label3.Top,
                Font = new Font(FontFamily.GenericSansSerif, 10),
                ForeColor = Color.Blue,
                BackColor = Color.White,
                AutoSize = true
            };

            label4 = new Label()
            {
                Left = label3.Left,
                Top = label3.Bottom + 20,
                Text = "End date:",
                Font = new Font(FontFamily.GenericSansSerif, 10),
                ForeColor = Color.Brown,
                AutoSize = true,
                Parent = this
            };

            dateTimePicker2 = new DateTimePicker()
            {
                Left = dateTimePicker1.Left,
                Top = label4.Top,
                CustomFormat = "yyyy.MM.dd",
                MinDate = DateTime.Parse("2001.01.01"),
                AutoSize = true,
                Parent = this
            };

            label5 = new Label()
            {
                Left = label4.Left,
                Top = label4.Bottom + 10,
                Text = "End meal:",
                Font = new Font(FontFamily.GenericSansSerif, 10),
                ForeColor = Color.Brown,
                AutoSize = true,
                Parent = this
            };

            comboBox2 = new ComboBox()
            {
                Parent = this,
                DataSource = Enum.GetValues(typeof(EtkezesTipus)),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Left = comboBox1.Left,
                Top = label5.Top,
                Font = new Font(FontFamily.GenericSansSerif, 10),
                ForeColor = Color.Blue,
                BackColor = Color.White,
                AutoSize = true,
            };

            checkBox1 = new CheckBox()
            {
                Left = label5.Left,
                Top = label5.Bottom + 15,
                Checked = false,
                Text = "Minimize cost?",
                Font = new Font(FontFamily.GenericSansSerif, 10),
                ForeColor = Color.DarkMagenta,
                AutoSize = true,
                Parent = this
            };

            checkBox2 = new CheckBox()
            {
                Left = checkBox1.Left,
                Top = checkBox1.Bottom + 5,
                Checked = false,
                Text = "Maximize joy?",
                Font = new Font(FontFamily.GenericSansSerif, 10),
                ForeColor = Color.DarkMagenta,
                AutoSize = true,
                Parent = this
            };

            label6 = new Label()
            {
                Left = button1.Left,
                Top = checkBox2.Bottom + 5,
                Text = "Maximal money:",
                Font = new Font(FontFamily.GenericSansSerif, 10),
                ForeColor = Color.DarkMagenta,
                AutoSize = true,
                Parent = this
            };

            numericUpDown1 = new NumericUpDown()
            {
                Left = button1.Left + 20,
                Top = label6.Bottom + 5,
                Maximum = Konstans.maxElkolthetoPenz,
                DecimalPlaces = 2,
                AutoSize = true,
                Parent = this
            };

            comboBox3 = new ComboBox()
            {
                DataSource = null,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Left = numericUpDown1.Right + 15,
                Top = numericUpDown1.Top,
                Font = new Font(FontFamily.GenericSansSerif, 10),
                ForeColor = Color.Blue,
                BackColor = Color.White,
                AutoSize = true,
                Parent = this
            };

            checkBox3 = new CheckBox()
            {
                Left = button1.Left,
                Top = numericUpDown1.Bottom + 5,
                Checked = true,
                Text = "Hide solver?",
                Font = new Font(FontFamily.GenericSansSerif, 10),
                ForeColor = Color.DarkMagenta,
                AutoSize = true,
                Parent = this
            };

            checkBox4 = new CheckBox()
            {
                Left = checkBox3.Left,
                Top = checkBox3.Bottom + 5,
                Checked = false,
                Text = "Continous model?",
                Font = new Font(FontFamily.GenericSansSerif, 10),
                ForeColor = Color.DarkMagenta,
                AutoSize = true,
                Parent = this
            };

            numericUpDown2 = new NumericUpDown()
            {
                Left = checkBox4.Left,
                Top = checkBox4.Bottom + 5,
                Minimum = 1,
                Maximum = 100,
                Value = 1,
                AutoSize = true,
                Parent = this
            };

            label7 = new Label()
            {
                Left = numericUpDown2.Right + 5,
                Top = numericUpDown2.Top,
                Text = "Varied food for how many diets",
                Font = new Font(FontFamily.GenericSansSerif, 10),
                ForeColor = Color.DarkMagenta,
                AutoSize = true,
                Parent = this
            };

            numericUpDown3 = new NumericUpDown()
            {
                Left = numericUpDown2.Left,
                Top = numericUpDown2.Bottom + 5,
                Value = 30,
                Minimum = 1,
                Maximum = Konstans.maxValaszthatoFutasiIdo,
                AutoSize = true,
                Parent = this
            };

            label8 = new Label()
            {
                Left = numericUpDown3.Right + 5,
                Top = numericUpDown3.Top,
                Text = "Max. running time (second)",
                Font = new Font(FontFamily.GenericSansSerif, 10),
                ForeColor = Color.DarkMagenta,
                AutoSize = true,
                Parent = this
            };

            checkBox5 = new CheckBox()
            {
                Left = numericUpDown3.Left,
                Top = numericUpDown3.Bottom + 5,
                Checked = false,
                Text = "Save to calendar?",
                Font = new Font(FontFamily.GenericSansSerif, 10),
                ForeColor = Color.DarkMagenta,
                AutoSize = true,
                Parent = this
            };
            button5 = new Button()
            {
                Left = checkBox5.Left + 50,
                Top = checkBox5.Bottom + 15,
                Width = 220,
                Text = "Diet conditions",
                Font = new Font(FontFamily.GenericSansSerif, 12),
                ForeColor = Color.DarkGreen,
                AutoSize = true,
                Parent = this
            };

            button6 = new Button()
            {
                Left = button5.Left,
                Top = button5.Bottom + 15,
                Width = 220,
                Text = "Prepare diet",
                Font = new Font(FontFamily.GenericSansSerif, 12),
                ForeColor = Color.DarkGreen,
                AutoSize = true,
                Parent = this
            };

            button9 = new Button()
            {
                Left = button6.Left,
                Top = button6.Bottom + 15,
                Width = 220,
                Text = "XML import",
                Font = new Font(FontFamily.GenericSansSerif, 12),
                ForeColor = Color.DarkGreen,
                AutoSize = true,
                Parent = this
            };

            button10 = new Button()
            {
                Left = button9.Left,
                Top = button9.Bottom + 15,
                Width = 220,
                Text = "XML export",
                Font = new Font(FontFamily.GenericSansSerif, 12),
                ForeColor = Color.DarkGreen,
                AutoSize = true,
                Parent = this
            };

            label9 = new Label()
            {
                Left = Right - 550,
                Top = radioButton1.Top,
                Text = "Solution:",
                Font = new Font(FontFamily.GenericSansSerif, 12),
                ForeColor = Color.Indigo,
                AutoSize = true,
                Parent = this
            };

            textBox1 = new TextBox()
            {
                Parent = this,
                Top = label9.Bottom + 10,
                Left = label9.Left,
                Height = 500,
                Width = 440,
                AutoSize = true,
                Font = new Font(FontFamily.GenericSansSerif, 12),
                BackColor = Color.Chartreuse,
                ForeColor = Color.Indigo,
                Multiline = true,
                ReadOnly = true,
            };

            button8 = new Button()
            {
                Left = listBox1.Left,
                Top = listBox1.Bottom + 15,
                Width = listBox1.Width,
                Text = "Sort",
                Font = new Font(FontFamily.GenericSansSerif, 12),
                ForeColor = Color.DarkGreen,
                AutoSize = true,
                Parent = this
            };

            button7 = new Button()
            {
                Left = listBox1.Left,
                Top = button8.Bottom + 15,
                Width = listBox1.Width,
                Text = "Search",
                Font = new Font(FontFamily.GenericSansSerif, 12),
                ForeColor = Color.DarkGreen,
                AutoSize = true,
                Parent = this
            };

            try
            {
                comboBox3.DataSource = null;
                comboBox3.Items.AddRange(ABKezelo.Kiolvasas().Where(x => x is Penznem && (x as Penznem).Hasznalhato).Select(p => (p as Penznem).Megnevezes)
                    .ToArray());
                comboBox3.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Csatlakozas()
        {
            try
            {
                ABKezelo.Csatlakozas(ConfigurationManager.ConnectionStrings["EtrendString"].ConnectionString);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_FormClosing2(object sender, FormClosingEventArgs e)
        {
        }

        private void Form1_FormClosing1(object sender, FormClosingEventArgs e)
        {
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Really want to exit?", "Question", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                ABKezelo.KapcsolatBontas();
                Logolas.Ment("The program has been closed." + Environment.NewLine + "----------------------------");
            }
        }

        private void kilépésToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Menu_beolvas(object sender, EventArgs e, string s)
        {
            Button9_Click_general(sender, e, s, true, false, true);
        }

        private void dantzigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Menu_beolvas(sender, e, "test_input1.xml");
        }

        private void mymenuToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void mértékegységSzerkesztésToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Button1_Click(new object(), new EventArgs());
        }

        private void mértékegységToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Button1_Click(new object(), new EventArgs());
        }

        private void pénznemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Button2_Click(new object(), new EventArgs());
        }

        private void tápanyagToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Button3_Click(new object(), new EventArgs());
        }

        private void élelmiszerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Button4_Click(new object(), new EventArgs());
        }

        private void étrendToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Button5_Click(new object(), new EventArgs());
        }

        private void étrendKészítéseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Button6_Click(new object(), new EventArgs());
        }

        private void rendezToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Button8_Click(new object(), new EventArgs());
        }

        private void keresésekToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count > 0)
            {
                KeresesForm dialogus = new KeresesForm(radioButton1.Checked, null);
                dialogus.ShowDialog();
            }
        }

        private void xMLInputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Button9_Click(new object(),new EventArgs());
        }

        private void xMLOutputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Button10_Click(new object(),new EventArgs());
        }

        private void kilépésToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Close();
        }

        private void ételToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Menu_beolvas(sender, e, "test_input3.xml");
        }

        private void élelmiszerToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Menu_beolvas(sender, e, "test_input1.xml");
        }

        private void munkaszervezésToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Menu_beolvas(sender, e, "test_input2.xml");
        }

        private void stiglerFeladatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Menu_beolvas(sender, e, "test_input4.xml");
        }

        private void stiglerFeladataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Menu_beolvas(sender, e, "test_input5.xml");
        }

        private void szerkesztésekToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
