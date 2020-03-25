using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EtrendKeszito
{
	public partial class UjElelmiszerForm : Form
	{
		private Elelmiszer elelmiszer;
		private bool megjelenit = false;

		internal Elelmiszer Elelmiszer
		{
			get { return elelmiszer; }
		}

		private List<Tapanyag> tapanyagok;
		private List<Penznem> penznemek;
		private List<Mertekegyseg> mertekegysegek;
		private List<Elelmiszer> elelmiszerek;

		public UjElelmiszerForm()
		{
			try
			{
				List<EtrendAdat> e = ABKezelo.Kiolvasas().ToList();
				tapanyagok = new List<Tapanyag>();
				penznemek = new List<Penznem>();
				mertekegysegek = new List<Mertekegyseg>();
				elelmiszerek = new List<Elelmiszer>();
				foreach (EtrendAdat item in e)
				{
					if ((item is Tapanyag) && (item as Tapanyag).Hasznalhato)
					{
						tapanyagok.Add((Tapanyag)item);
					}
					else if ((item is Penznem) && (item as Penznem).Hasznalhato) penznemek.Add((Penznem)item);
					else if ((item is Mertekegyseg) && (item as Mertekegyseg).Hasznalhato) mertekegysegek.Add((Mertekegyseg)item);
					else if (item is Elelmiszer) elelmiszerek.Add((Elelmiszer)item); //menüben minden élelmiszer használható, attól függetlenül, hogy az adott étel/ital használható-e
				}

				InitializeComponent();
				VezerlokLetrehozasa();
				AddButtonClick();
				ComboBox1_SelectedIndexChanged(new object(), new EventArgs());
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		internal UjElelmiszerForm(Elelmiszer modosit, bool megjelenit = false) : this()
		{
			try
			{
				this.megjelenit = megjelenit;

				if (modosit is Menu)
				{
					(modosit as Menu).update();
				}

				elelmiszer = modosit;

				textBox1.Text = modosit.Megnevezes;
				textBox1.Enabled = false;

				numericUpDown1.Value = elelmiszer.Orom;
				numericUpDown2.Value = (decimal)elelmiszer.Ar;

				comboBox2.SelectedItem = elelmiszer.Penz.Megnevezes;

				checkBox1.Checked = elelmiszer.EgysegTobbszorose;
				checkBox3.Checked = elelmiszer.Fogyaszthato[0];
				checkBox4.Checked = elelmiszer.Fogyaszthato[1];
				checkBox5.Checked = elelmiszer.Fogyaszthato[2];
				checkBox6.Checked = elelmiszer.Valtozatossag;
				checkBox7.Checked = elelmiszer.Hasznalhato;

				int i;
				for (i = 0; i < labelek.Count; i++)
				{
					string tnev = labelek[i].Text.Remove(labelek[i].Text.Length - 1);
					Tapanyag t = tapanyagok.Where(x => x.Megnevezes == tnev).ToList().First();
					szamlalok[i].Value = (decimal)elelmiszer.TapanyagTartalom[t];
					labelek2[i].Text = t.Mertek.Megnevezes;
				}

				if (elelmiszer is Etel)
				{
					comboBox1.SelectedIndex = (int)ElelmiszerTipus.meal;
					comboBox1.Enabled = false;

					comboBox3.SelectedItem = (elelmiszer as Etel).TomegMertek.Megnevezes;
					label8.Text = comboBox3.SelectedText;
					label9.Text = comboBox3.SelectedText;
					numericUpDown3.Value = (decimal)(elelmiszer as Etel).EgysegTomegMennyiseg;
					numericUpDown5.Value = (decimal)(elelmiszer as Etel).MinTomeg;
					numericUpDown6.Value = (decimal)(elelmiszer as Etel).MaxTomeg;
					checkBox2.Checked = (elelmiszer as Etel).MaxTomegE;
				}
				else if (elelmiszer is Ital)
				{
					comboBox1.SelectedIndex = (int)ElelmiszerTipus.drink;
					comboBox1.Enabled = false;

					comboBox4.SelectedItem = (elelmiszer as Ital).Urmertek.Megnevezes;
					label8.Text = comboBox4.SelectedText;
					label9.Text = comboBox4.SelectedText;

					numericUpDown4.Value = (decimal)(elelmiszer as Ital).EgysegUrTartalomMennyiseg;
					numericUpDown5.Value = (decimal)(elelmiszer as Ital).MinUrTartalom;
					numericUpDown6.Value = (decimal)(elelmiszer as Ital).MaxUrTartalom;
					checkBox2.Checked = (elelmiszer as Ital).MaxUrTartalomE;
				}
				else if (elelmiszer is Menu)
				{
					comboBox1.SelectedIndex = (int)ElelmiszerTipus.menu;
					comboBox1.Enabled = false;

					checkBox8.Checked = (elelmiszer as Menu).Arszamitas;

					label8.Visible = false;
					label9.Visible = false;
					comboBox3.SelectedItem = (elelmiszer as Menu).TomegMertek.Megnevezes;
					numericUpDown3.Value = (decimal)(elelmiszer as Menu).EgysegTomegMennyiseg;

					comboBox4.SelectedItem = (elelmiszer as Menu).Urmertek.Megnevezes;
					numericUpDown4.Value = (decimal)(elelmiszer as Menu).EgysegUrTartalomMennyiseg;

				    numericUpDown7.Value = (decimal) (elelmiszer as Menu).MaxDarab;

					for (i = 0; i < labelek3.Count; i++)
					{
						string str = labelek3[i].Text.Remove(labelek3[i].Text.Length - 1);
						Elelmiszer e = elelmiszerek.Where(x => x.Megnevezes == str).First();
						if ((elelmiszer as Menu).Osszetevo.ContainsKey(e))
						{
							szamlalok3[i].Value = (decimal)(elelmiszer as Menu).Osszetevo[e];
						}
					}
				}

				if (megjelenit)
				{
					foreach (Control item in Controls)
					{
						item.Enabled = false;
					}

					button1.Enabled = true;
					button1.DialogResult = DialogResult.Cancel;
				}

				ComboBox1_SelectedIndexChanged(new object(), new EventArgs());
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
			comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
			comboBox2.SelectedIndexChanged += ComboBox2_SelectedIndexChanged;
			comboBox3.SelectedIndexChanged += ComboBox3_SelectedIndexChanged;
			comboBox4.SelectedIndexChanged += ComboBox4_SelectedIndexChanged;

			checkBox8.CheckedChanged += CheckBox8_CheckedChanged;
		}

		private void CheckBox8_CheckedChanged(object sender, EventArgs e)
		{
			label4.Visible = !checkBox8.Checked || megjelenit;
			comboBox2.Visible = !checkBox8.Checked || megjelenit;
			numericUpDown2.Visible = !checkBox8.Checked || megjelenit;
		}

		private void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				Penznem p = penznemek.Where(x => x.Megnevezes == comboBox2.SelectedItem.ToString()).First();
				numericUpDown2.DecimalPlaces = p.TizedesekSzama;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (comboBox1.SelectedIndex == (int)ElelmiszerTipus.menu)
			{
				label10.Visible = true;
				label12.Visible = true;
				numericUpDown7.Visible = true;

				checkBox8.Visible = true;
				CheckBox8_CheckedChanged(new object(), new EventArgs());

				foreach (Label item in labelek3)
				{
					item.Visible = true;
				}
				foreach (Label item in labelek4)
				{
					item.Visible = true;
				}
				foreach (NumericUpDown item in szamlalok3)
				{
					item.Visible = true;
				}

				bool b = megjelenit;

				label5.Visible = b;
				comboBox3.Visible = b;
				numericUpDown3.Visible = b;

				label6.Visible = b;
				comboBox4.Visible = b;
				numericUpDown4.Visible = b;

				label7.Visible = false;
				numericUpDown5.Visible = false;
				label8.Visible = false;
				checkBox2.Visible = false;
				numericUpDown6.Visible = false;
				label9.Visible = false;

				label11.Visible = b;
				foreach (Label item in labelek)
				{
					item.Visible = b;
				}

				foreach (Label item in labelek2)
				{
					item.Visible = b;
				}

				foreach (NumericUpDown item in szamlalok)
				{
					item.Visible = b;
				}
			}
			else
			{
				label10.Visible = false;
				label12.Visible = false;
				numericUpDown7.Visible = false;
				checkBox8.Visible = false;
				foreach (Label item in labelek3)
				{
					item.Visible = false;
				}
				foreach (Label item in labelek4)
				{
					item.Visible = false;
				}
				foreach (NumericUpDown item in szamlalok3)
				{
					item.Visible = false;
				}

				label7.Visible = true;
				numericUpDown5.Visible = true;
				label8.Visible = true;
				checkBox2.Visible = true;
				numericUpDown6.Visible = true;
				label9.Visible = true;

				label11.Visible = true;
				foreach (Label item in labelek)
				{
					item.Visible = true;
				}

				foreach (Label item in labelek2)
				{
					item.Visible = true;
				}

				foreach (NumericUpDown item in szamlalok)
				{
					item.Visible = true;
				}

				if (comboBox1.SelectedIndex == (int)ElelmiszerTipus.meal)
				{
					label5.Visible = true;
					comboBox3.Visible = true;
					numericUpDown3.Visible = true;

					label6.Visible = false;
					comboBox4.Visible = false;
					numericUpDown4.Visible = false;

					label8.Text = comboBox3.SelectedItem.ToString();
					label9.Text = label8.Text;
				}
				else
				{
					label5.Visible = false;
					comboBox3.Visible = false;
					numericUpDown3.Visible = false;

					label6.Visible = true;
					comboBox4.Visible = true;
					numericUpDown4.Visible = true;

					label8.Text = comboBox4.SelectedItem.ToString();
					label9.Text = label8.Text;
				}
			}
		}

		private void ComboBox4_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (comboBox4.SelectedIndex != -1)
			{
				label8.Text = comboBox4.SelectedItem.ToString();
				label9.Text = label8.Text;
			}
		}

		private void ComboBox3_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (comboBox3.SelectedIndex != -1)
			{
				label8.Text = comboBox3.SelectedItem.ToString();
				label9.Text = label8.Text;
			}
		}

		private void Button2_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}

		private void Button1_Click(object sender, EventArgs e)
		{
			try
			{
				string fnevhash = ABKezelo.GetCurrentUser();

				if (comboBox1.SelectedIndex != (int)ElelmiszerTipus.menu && checkBox2.Checked &&
					numericUpDown5.Value > numericUpDown6.Value)
				{
					throw new WarningException("The maximal consumption should not be less than the minimal!");
				}

				if (elelmiszer == null)
				{
					string nev = textBox1.Text.Trim();

					if (nev == "" || nev.Length > 30)
					{
						throw new WarningException("The name should be non-empty and at most 30 characters!");
					}
					if (elelmiszerek.Where(x => x.Megnevezes == nev).ToList().Count > 0)
					{
						throw new WarningException("You already have a food with this name!");
					}
					int i;
					List<bool> fogyaszthato = new List<bool> { checkBox3.Checked, checkBox4.Checked, checkBox5.Checked };

					double ar = (double)numericUpDown2.Value;

					Penznem penz = penznemek.Where(p => p.Megnevezes == comboBox2.SelectedItem.ToString()).First();

					Mertekegyseg mertek1 = mertekegysegek.Where(m => m.Megnevezes == comboBox3.SelectedItem.ToString()).First();
					double mennyiseg1 = (double)numericUpDown3.Value;

					Mertekegyseg mertek2 = mertekegysegek.Where(m => m.Megnevezes == comboBox4.SelectedItem.ToString()).First();
					double mennyiseg2 = (double)numericUpDown4.Value;

					//étel
					Elelmiszer el = null;
					switch ((ElelmiszerTipus)comboBox1.SelectedIndex)
					{
						case ElelmiszerTipus.meal:
							{
								el = new Etel(fnevhash, textBox1.Text.Trim(),
									(byte)numericUpDown1.Value,
									penz,
									ar,
									checkBox1.Checked,
									fogyaszthato,
									checkBox6.Checked,
									checkBox7.Checked,
									new Dictionary<Tapanyag, double>(),
									mennyiseg1,
									(double)numericUpDown5.Value,
									(double)numericUpDown6.Value,
									checkBox2.Checked,
									mertek1);
								break;
							}
						//ital
						case ElelmiszerTipus.drink:
							{
								el = new Ital(fnevhash, textBox1.Text.Trim(),
									(byte)numericUpDown1.Value,
									penz,
									ar,
									checkBox1.Checked,
									fogyaszthato,
									checkBox6.Checked,
									checkBox7.Checked,
									new Dictionary<Tapanyag, double>(),
									mennyiseg2,
									(double)numericUpDown5.Value,
									(double)numericUpDown6.Value,
									checkBox2.Checked,
									mertek2);
								break;
							}
						//menü
						case ElelmiszerTipus.menu:
							{
								el = new Menu(fnevhash, textBox1.Text.Trim(),
									(byte)numericUpDown1.Value,
									penz,
									ar,
									checkBox1.Checked,
									fogyaszthato,
									checkBox6.Checked,
									checkBox7.Checked,
									new Dictionary<Tapanyag, double>(),
									(byte)numericUpDown7.Value,
									1,
									mertek1,
									1,
									mertek2,
									new Dictionary<Elelmiszer, double>(),
									checkBox8.Checked);
								break;
							}
					}

					foreach (Tapanyag item in ABKezelo.Kiolvasas().Where(x=>x is Tapanyag).ToList())
					{
						el.TapanyagTartalom.Add(item,0);
					}

					for (i = 0; i < labelek.Count(); i++)
					{
						string str = labelek[i].Text.Remove(labelek[i].Text.Length - 1);
						double m = (double)szamlalok[i].Value;
						Tapanyag t = tapanyagok.Where(x => x.Megnevezes == str).First();
						el.TapanyagTartalom[t] = m;
					}

					if (el is Menu)
					{
						bool van = false;
						for (i = 0; i < labelek3.Count; i++)
						{
							string str = labelek3[i].Text.Remove(labelek3[i].Text.Length - 1);
							double m = (double)szamlalok3[i].Value; 
							if (m > 0) van = true;
							Elelmiszer el2 = elelmiszerek.Where(x => x.Megnevezes == str).First();
							(el as Menu).Osszetevo.Add(el2, m);
						}

						if (!van)
						{
							throw new WarningException("There is no meal/drink with positive mass/volume!");
						}
					}
					ABKezelo.Beszuras(el);
				    Logolas.Ment("New food added, name: " + el.Megnevezes);
                }
				else
				{
					elelmiszer.Orom = (byte)numericUpDown1.Value;
					elelmiszer.Ar = (double)numericUpDown2.Value;
					string pnev = comboBox2.SelectedItem.ToString();
					elelmiszer.Penz = penznemek.Where(x => x.Megnevezes == pnev).First();
					elelmiszer.EgysegTobbszorose = checkBox1.Checked;
					elelmiszer.Fogyaszthato = new List<bool>()
						{
							checkBox3.Checked,
							checkBox4.Checked,
							checkBox5.Checked
						};
					elelmiszer.Valtozatossag = checkBox6.Checked;
					elelmiszer.Hasznalhato = checkBox7.Checked;

					int i;
					for (i = 0; i < labelek.Count; i++)
					{
						string tnev = labelek[i].Text.Remove(labelek[i].Text.Length - 1);
						Tapanyag t = tapanyagok.Where(x => x.Megnevezes == tnev).First();
						elelmiszer.TapanyagTartalom[t] = (double)szamlalok[i].Value;
					}

					if (elelmiszer is Etel) 
					{
						(elelmiszer as Etel).TomegMertek.Megnevezes = comboBox3.SelectedItem.ToString();
						(elelmiszer as Etel).EgysegTomegMennyiseg = (double)numericUpDown3.Value;
						(elelmiszer as Etel).MinTomeg = (double)numericUpDown5.Value;
						(elelmiszer as Etel).MaxTomeg = (double)numericUpDown6.Value;
						(elelmiszer as Etel).MaxTomegE = checkBox2.Checked;
					}
					else if (elelmiszer is Ital)
					{
						(elelmiszer as Ital).Urmertek.Megnevezes = comboBox4.SelectedItem.ToString();
						(elelmiszer as Ital).EgysegUrTartalomMennyiseg = (double)numericUpDown4.Value;
						(elelmiszer as Ital).MinUrTartalom = (double)numericUpDown5.Value;
						(elelmiszer as Ital).MaxUrTartalom = (double)numericUpDown6.Value;
						(elelmiszer as Ital).MaxUrTartalomE = checkBox2.Checked;
					}
					else if (elelmiszer is Menu)
					{
						(elelmiszer as Menu).TomegMertek.Megnevezes = comboBox3.SelectedItem.ToString();
						(elelmiszer as Menu).EgysegTomegMennyiseg = (double)numericUpDown3.Value;
						(elelmiszer as Menu).Urmertek.Megnevezes = comboBox4.SelectedItem.ToString();
						(elelmiszer as Menu).EgysegUrTartalomMennyiseg = (double)numericUpDown4.Value;
						(elelmiszer as Menu).MaxDarab = (byte)numericUpDown7.Value; 

						(elelmiszer as Menu).Osszetevo = new Dictionary<Elelmiszer, double>();

						double total = 0;
						for (i = 0; i < labelek3.Count; i++)
						{
							string str = labelek3[i].Text.Remove(labelek3[i].Text.Length - 1);
							Elelmiszer el2 = elelmiszerek.Where(x => x.Megnevezes == str).First();
							double m = (double)szamlalok3[i].Value; 
							if (m > 0)
							{
								(elelmiszer as Menu).Osszetevo.Add(el2, m);
								total += m;
							}
						}

						if (total == 0)
						{
							throw new WarningException("The unit should be positive");
						}
					}
					ABKezelo.Modositas(elelmiszer);
				    Logolas.Ment("Food modification, name: " + elelmiszer.Megnevezes);
                }
			}
			catch (Exception ex)
			{
				if (ex is WarningException)
				{
					MessageBox.Show(ex.Message, "Attention!", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				else
				{
					MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}

				DialogResult = DialogResult.None;
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

		private ComboBox comboBox1;
		private ComboBox comboBox2;
		private ComboBox comboBox3;
		private ComboBox comboBox4;


		private TextBox textBox1;

		private NumericUpDown numericUpDown1;
		private NumericUpDown numericUpDown2;
		private NumericUpDown numericUpDown3;
		private NumericUpDown numericUpDown4;
		private NumericUpDown numericUpDown5;
		private NumericUpDown numericUpDown6;
		private NumericUpDown numericUpDown7;

		private CheckBox checkBox1;
		private CheckBox checkBox2;
		private CheckBox checkBox3;
		private CheckBox checkBox4;
		private CheckBox checkBox5;
		private CheckBox checkBox6;
		private CheckBox checkBox7;
		private CheckBox checkBox8;

		private Button button1;
		private Button button2;

		private List<Label> labelek;
		private List<Label> labelek2;
		private List<NumericUpDown> szamlalok;

		private List<NumericUpDown> szamlalok3;
		private List<Label> labelek3;
		private List<Label> labelek4;

		void VezerlokLetrehozasa()
		{
			try
			{
				Width = 800;
				CenterToScreen();
				Text = "New food form";

				label1 = new Label()
				{
					Left = 20,
					Top = 20,
					Text = "Type of food:",
					Font = new Font(FontFamily.GenericSansSerif, 10),
					ForeColor = Color.Brown,
					AutoSize = true,
					Parent = this
				};

				comboBox1 = new ComboBox()
				{
					DataSource = null,
					DropDownStyle = ComboBoxStyle.DropDownList,
					Left = label1.Right + 20,
					Top = label1.Top,
					AutoSize = true,
					Parent = this
				};

				label2 = new Label()
				{
					Left = label1.Left,
					Top = label1.Bottom + 15,
					Text = "Name:",
					Font = new Font(FontFamily.GenericSansSerif, 10),
					ForeColor = Color.Brown,
					AutoSize = true,
					Parent = this
				};

				textBox1 = new TextBox()
				{
					Top = label2.Top,
					Left = comboBox1.Left,
					Height = 20,
					Width = 200,
					AutoSize = true,
					Font = new Font(FontFamily.GenericSansSerif, 12),
					ForeColor = Color.Indigo,
					Parent = this
				};

				label3 = new Label()
				{
					Left = label1.Left,
					Top = label2.Bottom + 15,
					Text = "Joy:",
					Font = new Font(FontFamily.GenericSansSerif, 10),
					ForeColor = Color.Brown,
					AutoSize = true,
					Parent = this
				};

				numericUpDown1 = new NumericUpDown()
				{
					Left = label3.Right + 15,
					Top = label3.Top,
					Maximum = Konstans.maxOrom,
					Minimum = 0,
					AutoSize = true,
					Parent = this
				};

				label4 = new Label()
				{
					Left = label1.Left,
					Top = label3.Bottom + 15,
					Text = "Unit price:",
					Font = new Font(FontFamily.GenericSansSerif, 10),
					ForeColor = Color.Brown,
					AutoSize = true,
					Parent = this
				};

				numericUpDown2 = new NumericUpDown()
				{
					Left = label4.Right + 15,
					Top = label4.Top,
					Maximum = Konstans.maxElkolthetoPenz,
					Minimum = 0,
					AutoSize = true,
					Parent = this
				};

				comboBox2 = new ComboBox()
				{
					DataSource = null, 
					DropDownStyle = ComboBoxStyle.DropDownList,
					Left = numericUpDown2.Right + 15,
					Top = label4.Top,
					AutoSize = true,
					Parent = this
				};

				label5 = new Label()
				{
					Left = label1.Left,
					Top = label4.Bottom + 15,
					Text = "Unit mass:",
					Font = new Font(FontFamily.GenericSansSerif, 10),
					ForeColor = Color.Brown,
					AutoSize = true,
					Parent = this
				};

				numericUpDown3 = new NumericUpDown()
				{
					Left = label5.Right + 15,
					Top = label5.Top,
					Maximum = (decimal) Konstans.maxMennyiseg,
					Minimum = 0,
					DecimalPlaces = 2,
					AutoSize = true,
					Parent = this
				};

				comboBox3 = new ComboBox()
				{
					DataSource = null, 
					DropDownStyle = ComboBoxStyle.DropDownList,
					Left = numericUpDown3.Right + 15,
					Top = label5.Top,
					AutoSize = true,
					Parent = this
				};

				label6 = new Label()
				{
					Left = label1.Left,
					Top = label5.Bottom + 15,
					Text = "Unit liquid measure:",
					Font = new Font(FontFamily.GenericSansSerif, 10),
					ForeColor = Color.Brown,
					AutoSize = true,
					Visible = false,
					Parent = this
				};

				numericUpDown4 = new NumericUpDown()
				{
					Left = label6.Right + 15,
					Top = label6.Top,
					Maximum = (decimal) Konstans.maxMennyiseg,
					Minimum = 0,
					DecimalPlaces = 1,
					AutoSize = true,
					Visible = false,
					Parent = this
				};

				comboBox4 = new ComboBox()
				{
					DataSource = null, 
					DropDownStyle = ComboBoxStyle.DropDownList,
					Left = numericUpDown4.Right + 15,
					Top = label6.Top,
					AutoSize = true,
					Visible = false,
					Parent = this
				};

				checkBox1 = new CheckBox()
				{
					Left = label1.Left,
					Top = label6.Bottom + 15,
					Checked = false,
					Text = "Consumption of only unit multiple?",
					Font = new Font(FontFamily.GenericSansSerif, 10),
					ForeColor = Color.DarkMagenta,
					AutoSize = true,
					Parent = this
				};

				label7 = new Label()
				{
					Left = label1.Left,
					Top = checkBox1.Bottom + 15,
					Text =
						"Minimum consumption:", // vagy nem eszünk az élelmiszerből, vagy legalább ennyit fogyasztunk belőle
					Font = new Font(FontFamily.GenericSansSerif, 10),
					ForeColor = Color.Brown,
					AutoSize = true,
					Parent = this
				};

				numericUpDown5 = new NumericUpDown()
				{
					Left = label7.Right + 15,
					Top = label7.Top,
					Maximum = (decimal) Konstans.maxMennyiseg,
					Minimum = 0,
					DecimalPlaces = 1,
					AutoSize = true,
					Parent = this
				};

				label8 = new Label()
				{
					Left = numericUpDown5.Right + 15,
					Top = label7.Top,
					Text = "",
					Font = new Font(FontFamily.GenericSansSerif, 10),
					ForeColor = Color.Brown,
					AutoSize = true,
					Parent = this
				};

				checkBox2 = new CheckBox()
				{
					Left = label1.Left,
					Top = label7.Bottom + 15,
					Checked = false,
					Text = "See maximum consumption? This:",
					Font = new Font(FontFamily.GenericSansSerif, 10),
					ForeColor = Color.DarkMagenta,
					AutoSize = true,
					Parent = this
				};

				numericUpDown6 = new NumericUpDown()
				{
					Left = checkBox2.Right + 15,
					Top = checkBox2.Top,
					Maximum = (decimal) Konstans.maxMennyiseg,
					Minimum = 0,
					DecimalPlaces = 1,
					AutoSize = true,
					Parent = this
				};

				label9 = new Label()
				{
					Left = numericUpDown6.Right + 15,
					Top = checkBox2.Top,
					Text = "",
					Font = new Font(FontFamily.GenericSansSerif, 10),
					ForeColor = Color.Brown,
					AutoSize = true,
					Parent = this
				};


				label10 = new Label()
				{
					Left = label1.Left,
					Top = checkBox2.Bottom + 15,
					Text = "Orders (menus) max. number",
					Font = new Font(FontFamily.GenericSansSerif, 10),
					ForeColor = Color.Brown,
					AutoSize = true,
					Visible = false,
					Parent = this
				};

				numericUpDown7 = new NumericUpDown()
				{
					Left = label10.Right + 15,
					Top = label10.Top,
					Maximum = (decimal) Konstans.maxMenu,
					Minimum = 0,
					Value = 1,
					AutoSize = true,
					Visible = false,
					Parent = this
				};

				checkBox3 = new CheckBox()
				{
					Left = label1.Left,
					Top = label10.Bottom + 15,
					Checked = false,
					Text = "Consumption at breakfast?",
					Font = new Font(FontFamily.GenericSansSerif, 10),
					ForeColor = Color.DarkMagenta,
					AutoSize = true,
					Parent = this
				};

				checkBox4 = new CheckBox()
				{
					Left = label1.Left,
					Top = checkBox3.Bottom + 15,
					Checked = false,
					Text = "Consumption at lunch?",
					Font = new Font(FontFamily.GenericSansSerif, 10),
					ForeColor = Color.DarkMagenta,
					AutoSize = true,
					Parent = this
				};

				checkBox5 = new CheckBox()
				{
					Left = label1.Left,
					Top = checkBox4.Bottom + 15,
					Checked = false,
					Text = "Consumption at dinner?",
					Font = new Font(FontFamily.GenericSansSerif, 10),
					ForeColor = Color.DarkMagenta,
					AutoSize = true,
					Parent = this
				};

				checkBox6 = new CheckBox()
				{
					Left = label1.Left,
					Top = checkBox5.Bottom + 15,
					Checked = false,
					Text = "See variety?",
					Font = new Font(FontFamily.GenericSansSerif, 10),
					ForeColor = Color.DarkMagenta,
					AutoSize = true,
					Parent = this
				};

				checkBox7 = new CheckBox()
				{
					Left = label1.Left,
					Top = checkBox6.Bottom + 15,
					Checked = true,
					Text = "Usable?",
					Font = new Font(FontFamily.GenericSansSerif, 10),
					ForeColor = Color.DarkMagenta,
					AutoSize = true,
					Parent = this
				};

				label11 = new Label()
				{
					Left = label1.Left,
					Top = checkBox7.Bottom + 15,
					Text = "Nutrients (for one unit of food):",
					Font = new Font(FontFamily.GenericSansSerif, 10),
					ForeColor = Color.Brown,
					AutoSize = true,
					Parent = this
				};

				int i;
				int bal = label11.Left;
				int fel = label11.Bottom + 15;

				labelek = new List<Label>();
				labelek2 = new List<Label>();
				szamlalok = new List<NumericUpDown>();

				foreach (Tapanyag item in tapanyagok)
				{
					Label label = new Label()
					{
						Left = bal,
						Top = fel,
						Text = item.Megnevezes + ":",
						Font = new Font(FontFamily.GenericSansSerif, 10),
						ForeColor = Color.Brown,
						AutoSize = true,
						Parent = this
					};
					labelek.Add(label);

					NumericUpDown num = new NumericUpDown()
					{
						Left = label.Right + 5,
						Top = fel,
						Maximum = (decimal) Konstans.maxMennyiseg,
						Minimum = 0,
						DecimalPlaces = 2,
						AutoSize = true,
						Parent = this
					};
					szamlalok.Add(num);

					label = new Label()
					{
						Left = num.Right + 15,
						Top = fel,
						Text = item.Mertek.Megnevezes,
						Font = new Font(FontFamily.GenericSansSerif, 10),
						ForeColor = Color.Brown,
						AutoSize = true,
						Parent = this
					};
					labelek2.Add(label);
					fel = label.Bottom + 15;
				}

				checkBox8 = new CheckBox()
				{
					Left = comboBox4.Right + 60,
					Top = label1.Top,
					Checked = false,
					Text = "Request price calculation",
					Font = new Font(FontFamily.GenericSansSerif, 10),
					ForeColor = Color.DarkMagenta,
					AutoSize = true,
					Visible = false,
					Parent = this
				};

				bal = checkBox8.Left;
				int fel2 = checkBox8.Bottom + 15;

				label12 = new Label()
				{
					Left = bal,
					Top = fel2,
					Text = "Used foods (for one unit):",
					Font = new Font(FontFamily.GenericSansSerif, 10),
					ForeColor = Color.Brown,
					AutoSize = true,
					Visible = false,
					Parent = this
				};
				fel2 = label12.Bottom + 15;

				List<Elelmiszer> elelmiszerek2 = new List<Elelmiszer>();
				foreach (Elelmiszer item in elelmiszerek)
				{
					if (item is Ital) elelmiszerek2.Add((Ital) item);
				}

				foreach (Elelmiszer item in elelmiszerek)
				{
					if (item is Etel) elelmiszerek2.Add((Etel) item);
				}

				int size = elelmiszerek2.Count;
				labelek3 = new List<Label>();
				szamlalok3 = new List<NumericUpDown>();
				labelek4 = new List<Label>();

				for (i = 0; i < size; i++)
				{
					Label label = new Label()
					{
						Left = bal,
						Top = fel2,
						Text = elelmiszerek2[i].Megnevezes + ":",
						Font = new Font(FontFamily.GenericSansSerif, 10),
						ForeColor = Color.Brown,
						AutoSize = true,
						Visible = false,
						Parent = this
					};
					labelek3.Add(label);

					NumericUpDown num = new NumericUpDown()
					{
						Left = bal + 100,
						Top = fel2,
						Maximum = (decimal) Konstans.maxMennyiseg,
						Minimum = 0,
						DecimalPlaces = 2,
						AutoSize = true,
						Visible = false,
						Parent = this
					};
					szamlalok3.Add(num);

					string merteknev = null;
					if (elelmiszerek2[i] is Etel) merteknev = (elelmiszerek2[i] as Etel).TomegMertek.Megnevezes;
					else merteknev = (elelmiszerek2[i] as Ital).Urmertek.Megnevezes;

					label = new Label()
					{
						Left = num.Right + 5,
						Top = fel2,
						Text = merteknev,
						Font = new Font(FontFamily.GenericSansSerif, 10),
						ForeColor = Color.Brown,
						AutoSize = true,
						Visible = false,
						Parent = this
					};
					labelek4.Add(label);
					fel2 = label.Bottom + 5;
				}


				button1 = new Button()
				{
					Left = label1.Left,
					Top = fel + 15,
					Width = Width - 60,
					Text = "OK",
					Font = new Font(FontFamily.GenericSansSerif, 12),
					ForeColor = Color.DarkGreen,
					DialogResult = DialogResult.OK,
					AutoSize = true,
					Parent = this
				};

				button2 = new Button()
				{
					Left = button1.Left,
					Top = button1.Bottom + 10,
					Width = button1.Width,
					Text = "Cancel",
					Font = new Font(FontFamily.GenericSansSerif, 12),
					ForeColor = Color.DarkGreen,
					DialogResult = DialogResult.Cancel,
					AutoSize = true,
					Parent = this
				};

				numericUpDown2.DecimalPlaces = penznemek[0].TizedesekSzama;

				comboBox1.DataSource = null;
				comboBox1.DataSource = Enum.GetValues(typeof(ElelmiszerTipus));
				comboBox1.SelectedIndex = 0;

				comboBox2.Items.Clear();
				comboBox2.Items.AddRange(penznemek.Select(p => p.Megnevezes).ToArray());
				comboBox2.SelectedIndex = 0;

				comboBox3.Items.Clear();
				comboBox3.Items.AddRange(mertekegysegek.Where(x => x.Mertek == MertekegysegFajta.weight).Select(m => m.Megnevezes)
					.ToArray());
				comboBox3.SelectedIndex = 0;

				comboBox4.Items.Clear();
				comboBox4.Items.AddRange(mertekegysegek.Where(x => x.Mertek == MertekegysegFajta.liquidmeasure)
					.Select(m => m.Megnevezes).ToArray());
				comboBox4.SelectedIndex = 0;

				label8.Text = comboBox3.SelectedItem.ToString();
				label9.Text = comboBox3.SelectedItem.ToString();

				checkBox3.Checked = true;
				checkBox4.Checked = true;
				checkBox5.Checked = true;
				checkBox7.Checked = true;

				Button btn = new Button();
				Height = button2.Bottom + 45;
				btn.Text = "Scrolled Button";
				btn.Size = new Size(10, 10);
				btn.Location = new Point(this.Size.Width - 50, this.Size.Height - 50);
				Controls.Add(btn);
				AutoScroll = true;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
	}
}
