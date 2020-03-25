using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EtrendKeszito
{
	public partial class EtrendBeallitForm : Form
	{
		public EtrendBeallitForm()
		{
		    try
		    {
		        InitializeComponent();
		        VezerlokLetrehozasa();
		        AddButtonClick();
		        var e = ABKezelo.Kiolvasas().Where(x => x is EtkezesFeltetel).ToList();

		        int i, j, k;
		        for (i = 0; i < 2; i++)
		        {
		            for (j = 0; j < 4; j++)
		            {
		                for (k = 0; k < 2; k++)
		                {
		                    if (e.Where(x => i == (int) (x as EtkezesFeltetel).Eltipus2 &&
		                                     j == (int) (x as EtkezesFeltetel).Ettipus2 &&
		                                     k == (int) (x as EtkezesFeltetel).Szamlalo).Count() > 0)
		                    {
		                        var e0 = e.Where(x =>
		                            i == (int) (x as EtkezesFeltetel).Eltipus2 &&
		                            j == (int) (x as EtkezesFeltetel).Ettipus2 &&
		                            k == (int) (x as EtkezesFeltetel).Szamlalo).First();

		                        double v1 = (e0 as EtkezesFeltetel).Minval;
		                        bool c = (e0 as EtkezesFeltetel).MaxvalE;
		                        double v2 = (e0 as EtkezesFeltetel).Maxval;

		                        int pos = 8 * i + 2 * j + k;
		                        checkBoxs[pos].Checked = c;
		                        if (k == 1)
		                        {
		                            numericUpDowns[2 * pos].Value = (decimal) v1;
		                            numericUpDowns[2 * pos + 1].Value = (decimal) v2;
		                        }
		                        else
		                        {
		                            numericUpDowns[2 * pos].Value = (int) v1;
		                            numericUpDowns[2 * pos + 1].Value = (int) v2;
		                        }
		                    }
		                }
		            }
		        }

		        e = ABKezelo.Kiolvasas().Where(x => x is EtrendIdopont).ToList();

		        if (e.Count > 0)
		        {
		            EtrendIdopont et = (EtrendIdopont) e.First();
		            for (i = 0; i < 3; i++)
		            {
		                numericUpDowns2[2 * i].Value = et.Datum[i].Hour;
		                numericUpDowns2[2 * i + 1].Value = et.Datum[i].Minute;
		            }
		        }
		        else
		        {
		            for (i = 0; i < 3; i++)
		            {
		                // 7.30; 13.30; 19.30
		                numericUpDowns2[2 * i].Value = 7 + 6 * i;
		                numericUpDowns2[2 * i + 1].Value = 30;
		            }
		        }
		    }
		    catch (Exception ex)
		    {
		        MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
		    }
		}

		void AddButtonClick()
		{
			button1.Click += Button1_Click;
			button2.Click += Button2_Click;
			checkBox1.CheckedChanged += CheckBox1_CheckedChanged; ;
		}

		private void CheckBox1_CheckedChanged(object sender, EventArgs e)
		{
			foreach (Control item in Controls)
			{
				if (item is CheckBox) (item as CheckBox).Checked = false;
				else if (item is NumericUpDown) (item as NumericUpDown).Value = 0;
			}

			for (int i = 0; i < 3; i++)
			{
				numericUpDowns2[2 * i].Value = 7 + 6 * i;
				numericUpDowns2[2 * i + 1].Value = 30;
			}
		}

		private void Button2_Click(object sender, EventArgs e)
		{

		}

		private void Button1_Click(object sender, EventArgs e)
		{
		    try
		    {
		        string fnev = ABKezelo.GetCurrentUser();
		        int i, j, k;
		        int p = 0, q = 0;
		        for (i = 0; i < 2; i++)
		        {
		            for (j = 0; j < 4; j++)
		            {
		                for (k = 0; k < 2; k++)
		                {
		                    EtkezesFeltetel etk = new EtkezesFeltetel(fnev, (ElelmiszerTipus2) i, (EtkezesTipus2) j,
		                        (Szamlalo) k, (double) numericUpDowns[p].Value, checkBoxs[q].Checked,
		                        (double) numericUpDowns[p + 1].Value);
		                    p += 2;
		                    q++;
		                    ABKezelo.Torol(etk);
		                    ABKezelo.Beszuras(etk);
		                }
		            }
		        }

		        EtrendIdopont et = new EtrendIdopont(fnev, new List<DateTime>(3));
		        for (i = 0; i < 3; i++)
		        {
		            et.Datum.Add(new DateTime(2018, 1, 1, (int) numericUpDowns2[2 * i].Value,
		                (int) numericUpDowns2[2 * i + 1].Value, 0));
		        }

		        if (ABKezelo.Kiolvasas().Where(x => x is EtrendIdopont).ToList().Count > 0)
		        {
		            ABKezelo.Modositas(et);
		        }
		        else
		        {
		            ABKezelo.Beszuras(et);
		        }
                Logolas.Ment("Saved diet conditions.");
		    }
		    catch (Exception ex)
		    {
		        MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
		    }
		}

		private Label label1;
		private Label label2;
		private Label label3;
		private Label label4;
		private Label label5;
		private Label label6;
		private Label label7;
		private Label label8;
		private Label label9;
		private Label label10;
		private Label label11;
		private Label label12;
		private Label label13;
		private Label label14;
		private Label label15;
		private Label label16;
		private Label label17;
		private Label label18;
		private Label label19;
		private Label label20;

		private Button button1;
		private Button button2;

		private List<NumericUpDown> numericUpDowns;
		private List<CheckBox> checkBoxs;

		private List<NumericUpDown> numericUpDowns2;
		private List<Label> labels2;

		private CheckBox checkBox1;

		void VezerlokLetrehozasa()
		{
			Height = 650;
			Width = 1200;
			CenterToScreen();
			Text = "Diet conditions";

			NumericUpDown szamlalo;
			Label label;

			numericUpDowns = new List<NumericUpDown>();
			checkBoxs = new List<CheckBox>();

			int[] pos = new int[4];

			numericUpDowns2 = new List<NumericUpDown>();
			labels2 = new List<Label>();

			int bal, fel, i, j;
			string[] st = { "Breakfast time", "Lunch time", "Dinner time" };


			checkBox1 = new CheckBox()
			{
				Left = 15,
				Top = 20,
				Checked = false,
				Text = "Reset everything?",
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.DarkMagenta,
				AutoSize = true,
				Parent = this
			};

			fel = checkBox1.Bottom+10;
			for (i = 0; i < 3; i++)
			{
				bal = 15;
				label = new Label()
				{
					Left = bal,
					Top = fel,
					Text = st[i]+": ",
					Font = new Font(FontFamily.GenericSansSerif, 10),
					ForeColor = Color.Blue,
					AutoSize = true,
					Parent = this
				};
				labels2.Add(label);

				bal = 160;
				for (j = 0; j < 2; j++)
				{
					szamlalo = new NumericUpDown()
					{
						Left = bal,
						Top = fel,
						Maximum = (j == 0) ? 23 : 59,
						AutoSize = true,
						Parent = this
					};
					numericUpDowns2.Add(szamlalo);
					bal = szamlalo.Right + 5;

					label = new Label()
					{
						Left = bal,
						Top = fel,
						Text = (j==0)?"hour":"minute",
						Font = new Font(FontFamily.GenericSansSerif, 10),
						ForeColor = Color.Blue,
						AutoSize = true,
						Parent = this
					};
					labels2.Add(label);
					bal = label.Right + 10;
				}
				fel = label.Bottom + 15;
			}

			label1 = new Label()
			{
				Left = 70,
				Top = fel,
				Text = "Meals min. number",
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Blue,
				AutoSize = true,
				Parent = this
			};

			label2 = new Label()
			{
				Left = label1.Right + 15,
				Top = label1.Top,
				Text = "See meals max. number?",
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Blue,
				AutoSize = true,
				Parent = this
			};

			label3 = new Label()
			{
				Left = label2.Right + 15,
				Top = label1.Top,
				Text = "meals max. number",
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Blue,
				AutoSize = true,
				Parent = this
			};

			label4 = new Label()
			{
				Left = label3.Right + 15,
				Top = label1.Top,
				Text = "meals min. weight (gram)",
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Blue,
				AutoSize = true,
				Parent = this
			};

			label5 = new Label()
			{
				Left = label4.Right + 15,
				Top = label1.Top,
				Text = "See meals max. weight?",
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Blue,
				AutoSize = true,
				Parent = this
			};

			label6 = new Label()
			{
				Left = label5.Right + 15,
				Top = label1.Top,
				Text = "meals max. weight (gramm)",
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Blue,
				AutoSize = true,
				Parent = this
			};

			fel = label1.Bottom + 15;
			int fel2;
			CheckBox cbox;
			for (i = 0; i < 4; i++)
			{
				szamlalo = new NumericUpDown()
				{
					Left = label1.Left,
					Top = fel,
					Maximum = 20,
					AutoSize = true,
					Parent = this
				};
				numericUpDowns.Add(szamlalo);
				fel2 = szamlalo.Bottom + 15;
				pos[i] = szamlalo.Top;

				cbox = new CheckBox()
				{
					Left = label2.Left,
					Top = fel,
					Checked = false,
					Text = "",
					Font = new Font(FontFamily.GenericSansSerif, 10),
					ForeColor = Color.DarkMagenta,
					AutoSize = true,
					Parent = this
				};
				checkBoxs.Add(cbox);

				szamlalo = new NumericUpDown()
				{
					Left = label3.Left,
					Top = fel,
					Maximum = 20,
					AutoSize = true,
					Parent = this
				};
				numericUpDowns.Add(szamlalo);

				szamlalo = new NumericUpDown()
				{
					Left = label4.Left,
					Top = fel,
					Maximum = (int)Konstans.maxMennyiseg,
					AutoSize = true,
					Parent = this
				};
				numericUpDowns.Add(szamlalo);

				cbox = new CheckBox()
				{
					Left = label5.Left,
					Top = fel,
					Checked = false,
					Text = "",
					Font = new Font(FontFamily.GenericSansSerif, 10),
					ForeColor = Color.DarkMagenta,
					AutoSize = true,
					Parent = this
				};
				checkBoxs.Add(cbox);

				szamlalo = new NumericUpDown()
				{
					Left = label6.Left,
					Top = fel,
					Maximum = (int)Konstans.maxMennyiseg,
					AutoSize = true,
					Parent = this
				};
				numericUpDowns.Add(szamlalo);
				fel = fel2;
			}

			label7 = new Label()
			{
				Left = 5,
				Top = pos[0],
				Text = "Breakfast:",
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Blue,
				AutoSize = true,
				Parent = this
			};

			label8 = new Label()
			{
				Left = 5,
				Top = pos[1],
				Text = "Lunch:",
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Blue,
				AutoSize = true,
				Parent = this
			};

			label9 = new Label()
			{
				Left = 5,
				Top = pos[2],
				Text = "Dinner:",
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Blue,
				AutoSize = true,
				Parent = this
			};

			label10 = new Label()
			{
				Left = 5,
				Top = pos[3],
				Text = "Daily:",
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Blue,
				AutoSize = true,
				Parent = this
			};




			label11 = new Label()
			{
				Left = 70,
				Top = label10.Bottom + 15,
				Text = "Drinks min. number",
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Blue,
				AutoSize = true,
				Parent = this
			};

			label12 = new Label()
			{
				Left = label11.Right + 15,
				Top = label11.Top,
				Text = "See drinks max. number?",
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Blue,
				AutoSize = true,
				Parent = this
			};

			label13 = new Label()
			{
				Left = label12.Right + 15,
				Top = label11.Top,
				Text = "drinks max. number",
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Blue,
				AutoSize = true,
				Parent = this
			};

			label14 = new Label()
			{
				Left = label13.Right + 15,
				Top = label11.Top,
				Text = "drinks min. volume (deciliter)",
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Blue,
				AutoSize = true,
				Parent = this
			};

			label15 = new Label()
			{
				Left = label14.Right + 15,
				Top = label11.Top,
				Text = "See drinks max. volume?",
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Blue,
				AutoSize = true,
				Parent = this
			};

			label16 = new Label()
			{
				Left = label15.Right + 15,
				Top = label11.Top,
				Text = "drinks max. volume (deciliter)",
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Blue,
				AutoSize = true,
				Parent = this
			};

			fel = label11.Bottom + 15;
			for (i = 0; i < 4; i++)
			{
				szamlalo = new NumericUpDown()
				{
					Left = label11.Left,
					Top = fel,
					Maximum = 20,
					AutoSize = true,
					Parent = this
				};
				numericUpDowns.Add(szamlalo);
				fel2 = szamlalo.Bottom + 15;
				pos[i] = szamlalo.Top;

				cbox = new CheckBox()
				{
					Left = label12.Left,
					Top = fel,
					Checked = false,
					Text = "",
					Font = new Font(FontFamily.GenericSansSerif, 10),
					ForeColor = Color.DarkMagenta,
					AutoSize = true,
					Parent = this
				};
				checkBoxs.Add(cbox);

				szamlalo = new NumericUpDown()
				{
					Left = label13.Left,
					Top = fel,
					Maximum = 20,
					AutoSize = true,
					Parent = this
				};
				numericUpDowns.Add(szamlalo);

				szamlalo = new NumericUpDown()
				{
					Left = label14.Left,
					Top = fel,
					Maximum = (int)Konstans.maxMennyiseg,
					DecimalPlaces = 1,
					AutoSize = true,
					Parent = this
				};
				numericUpDowns.Add(szamlalo);

				cbox = new CheckBox()
				{
					Left = label15.Left,
					Top = fel,
					Checked = false,
					Text = "",
					Font = new Font(FontFamily.GenericSansSerif, 10),
					ForeColor = Color.DarkMagenta,
					AutoSize = true,
					Parent = this
				};
				checkBoxs.Add(cbox);

				szamlalo = new NumericUpDown()
				{
					Left = label16.Left,
					Top = fel,
					Maximum = (int)Konstans.maxMennyiseg,
					DecimalPlaces = 1,
					AutoSize = true,
					Parent = this
				};
				numericUpDowns.Add(szamlalo);
				fel = fel2;
			}

			label17 = new Label()
			{
				Left = 5,
				Top = pos[0],
				Text = "Breakfast:",
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Blue,
				AutoSize = true,
				Parent = this
			};

			label18 = new Label()
			{
				Left = 5,
				Top = pos[1],
				Text = "Lunch:",
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Blue,
				AutoSize = true,
				Parent = this
			};

			label19 = new Label()
			{
				Left = 5,
				Top = pos[2],
				Text = "Dinner:",
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Blue,
				AutoSize = true,
				Parent = this
			};

			label20 = new Label()
			{
				Left = 5,
				Top = pos[3],
				Text = "Daily:",
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Blue,
				AutoSize = true,
				Parent = this
			};

			button1 = new Button()
			{
				Left = this.Width / 2 - 110,
				Top = label20.Bottom + 15,
				Width = 220,
				Text = "OK",
				DialogResult = DialogResult.OK,
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.DarkGreen,
				AutoSize = true,
				Parent = this
			};

			button2 = new Button()
			{
				Left = button1.Left,
				Top = button1.Bottom + 15,
				Width = 220,
				Text = "Cancel",
				DialogResult = DialogResult.Cancel,
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.DarkGreen,
				AutoSize = true,
				Parent = this
			};
		}
	}
}