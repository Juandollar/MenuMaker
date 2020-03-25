using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EtrendKeszito
{
	public partial class KeresesForm : Form
	{
		enum ENUM
		{
            mass,
            liquidmeasure,
            nutrientcontent
		}

		enum Reláció
		{
            atleast,
            atmost
		}

		private List<Elelmiszer> elelmiszerek;

	    private bool b = true;
	    private bool t;

		internal KeresesForm(bool tipus,object e =null)
		{
			// tipus=true, ha az összes élelmiszer között keresünk
			//  tipus=false, ha csak a használtak között

		    if (e == null) b = false;
		    t = tipus;

			InitializeComponent();

			elelmiszerek = new List<Elelmiszer>();

			try
			{
                Logolas.Ment("We are on search form");
				VezerlokLetrehozasa();
				AddButtonClick();

			    if (e == null)
			    {
			        if (tipus)
			        {
			            foreach (Elelmiszer item in ABKezelo.Kiolvasas().Where(x=>x is Elelmiszer))
			            {
			                elelmiszerek.Add(item);
			            }
			        }
			        else
			        {
			            foreach (Elelmiszer item in ABKezelo.Kiolvasas().Where(x => x is Elelmiszer &&(x as Elelmiszer).Hasznalhato))
			            {
			                elelmiszerek.Add(item);
			            }
                    }

			    }
			    else
			    {
			        double ar = (e as Elelmiszer).Ar * (e as Elelmiszer).Penz.Arfolyam;
			        if (tipus)
			        {
			            foreach (Elelmiszer item in ABKezelo.Kiolvasas().Where(x =>
			                    x is Elelmiszer && (x as Elelmiszer).Ar * (x as Elelmiszer).Penz.Arfolyam >= ar)
			                .ToList())
			            {
			                elelmiszerek.Add(item);
			            }
			        }
			        else
			        {
			            foreach (Elelmiszer item in ABKezelo.Kiolvasas()
			                .Where(x => x is Elelmiszer &&
			                            (x as Elelmiszer).Ar * (x as Elelmiszer).Penz.Arfolyam <= ar &&
			                            (x as Elelmiszer).Hasznalhato).ToList())
			            {
			                elelmiszerek.Add(item);
			            }
			        }
			    }

			    listBox1.DataSource = null;
				listBox1.DataSource = elelmiszerek;
				ComboBox1_SelectedIndexChanged(new object(), new EventArgs());
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private Label label1;
		private Label label2;

		private ListBox listBox1;

		private ComboBox comboBox1;
		private ComboBox comboBox2;
		private ComboBox comboBox3;
		private ComboBox comboBox4;

		private Button button1;

		private NumericUpDown numericUpDown1;

		void AddButtonClick()
		{
			button1.Click += Button1_Click;
			comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
			comboBox3.SelectedIndexChanged += ComboBox3_SelectedIndexChanged;
			comboBox4.SelectedIndexChanged += ComboBox4_SelectedIndexChanged;
		}

		private void ComboBox4_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (comboBox4.SelectedIndex != -1)
			{
				try
				{
					Tapanyag t = (Tapanyag)ABKezelo.Kiolvasas()
						.Where(x => x is Tapanyag && (x as Tapanyag).Megnevezes == comboBox4.SelectedItem.ToString())
						.ToList().First();
					comboBox3.Items.Clear();
					comboBox3.Items.Add(t.Mertek.Megnevezes);
					comboBox3.SelectedIndex = 0;
					comboBox3.Enabled = false;
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				comboBox3.Items.Clear();

				if (comboBox1.SelectedIndex == (int)ENUM.mass)
				{
					comboBox3.Items.AddRange(ABKezelo.Kiolvasas()
						.Where(x => x is Mertekegyseg && 
									(x as Mertekegyseg).Mertek == MertekegysegFajta.weight)
						.Select(p => (p as Mertekegyseg).Megnevezes).ToArray());
					comboBox4.Visible = false;
					comboBox2.Left = comboBox1.Right + 10;
					numericUpDown1.Left = comboBox2.Right + 10;
					comboBox3.Left = numericUpDown1.Right + 10;
					comboBox3.Enabled = true;
					comboBox3.SelectedIndex = 0;
				}
				else if (comboBox1.SelectedIndex == (int)ENUM.liquidmeasure)
				{
					comboBox3.Items.AddRange(ABKezelo.Kiolvasas()
						.Where(x => x is Mertekegyseg && 
									(x as Mertekegyseg).Mertek == MertekegysegFajta.liquidmeasure)
						.Select(p => (p as Mertekegyseg).Megnevezes).ToArray());
					comboBox4.Visible = false;
					comboBox2.Left = comboBox1.Right + 10;
					numericUpDown1.Left = comboBox2.Right + 10;
					comboBox3.Left = numericUpDown1.Right + 10;
					comboBox3.Enabled = true;
					comboBox3.SelectedIndex = 0;
				}
				else
				{
					comboBox4.Items.Clear();
					comboBox4.Items.AddRange(ABKezelo.Kiolvasas()
						.Where(x => x is Tapanyag && (x as Tapanyag).Hasznalhato).Select(p => (p as Tapanyag).Megnevezes).ToArray());
					comboBox4.Visible = true;
					comboBox3.Items.Clear();
					if (comboBox4.Items.Count > 0)
					{
						comboBox4.SelectedIndex = 0;
						Tapanyag t = (Tapanyag) ABKezelo.Kiolvasas()
							.Where(x => x is Tapanyag && (x as Tapanyag).Megnevezes == comboBox4.SelectedItem.ToString())
							.ToList().First();
						comboBox3.Items.Add(t.Mertek.Megnevezes);
						comboBox3.SelectedIndex = 0;
					}
					comboBox3.Enabled = false;

					comboBox2.Left = comboBox4.Right + 10;
					numericUpDown1.Left = comboBox2.Right + 10;
					comboBox3.Left = numericUpDown1.Right + 10;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void ComboBox3_SelectedIndexChanged(object sender, EventArgs e)
		{
		}

		private void Button1_Click(object sender, EventArgs evnt)
		{
			try
			{
				List<Elelmiszer> e = new List<Elelmiszer>();

				double ertek = 0;

				int tipus = comboBox1.SelectedIndex + 3 * comboBox2.SelectedIndex;

				if (comboBox3.Items.Count == 0)
				{
					listBox1.DataSource = null;
					listBox1.DataSource = e;
					return;
				}

				Mertekegyseg mertek = (Mertekegyseg)
					ABKezelo.Kiolvasas()
						.Where(x => x is Mertekegyseg && (x as Mertekegyseg).Megnevezes == comboBox3.SelectedItem.ToString()).ToList().First();
				Tapanyag tapanyag = null;

				if (tipus % 3 == 0)
				{
					ertek = (double)numericUpDown1.Value * mertek.Valtoszam;
				}
				else if (tipus % 3 == 1)
				{
					ertek = (double)numericUpDown1.Value * mertek.Valtoszam;
				}
				else if(comboBox4.SelectedIndex!=-1)
				{
					ertek = (double)numericUpDown1.Value;
					tapanyag = (Tapanyag)ABKezelo.Kiolvasas()
						.Where(x => x is Tapanyag && (x as Tapanyag).Megnevezes == comboBox4.SelectedItem.ToString()).ToList().First();
				}
				else
				{
					listBox1.DataSource = null;
					listBox1.DataSource = e;
					return;
				}

				foreach (Elelmiszer item in elelmiszerek)
				{
					double ertek2 = 0;

					if (tipus % 3 == 0)
					{
						if (item is Etel) ertek2 = (item as Etel).EgysegTomegMennyiseg * (item as Etel).TomegMertek.Valtoszam;
						else if (item is Menu) ertek2 = (item as Menu).EgysegTomegMennyiseg * (item as Menu).TomegMertek.Valtoszam;
					}
					else if (tipus % 3 == 1)
					{
						if (item is Ital) ertek2 = (item as Ital).EgysegUrTartalomMennyiseg * (item as Ital).Urmertek.Valtoszam;
						else if (item is Menu) ertek2 = (item as Menu).EgysegUrTartalomMennyiseg * (item as Menu).Urmertek.Valtoszam;
					}
					else
					{
						ertek2 = item.TapanyagTartalom[tapanyag];
					}

					if (comboBox2.SelectedIndex == (int)Reláció.atleast)
					{
						if (ertek <= ertek2) e.Add(item);
					}
					else
					{
						if (ertek >= ertek2) e.Add(item);
					}
				}

				listBox1.DataSource = null;
				listBox1.DataSource = e;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		void VezerlokLetrehozasa()
		{
			Height = 620;
			Width = 1170;
			CenterToScreen();
			Text = "Search form";

			label1 = new Label()
			{
				Left = 30,
				Top = 30,
				Text = b?"Search results (price is at least than the double clicked item's price):":((t?"All":"Usable")+" food"),
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Brown,
				AutoSize = true,
				Parent = this
			};

			listBox1 = new ListBox()
			{
				Left = label1.Left,
				Top = label1.Bottom + 15,
				Height = 400,
				Width = 440,
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.Indigo,
				Parent = this
			};

			label2 = new Label()
			{
				Left = listBox1.Right + 15,
				Top = listBox1.Top,
				Text = "Search further:",
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Brown,
				AutoSize = true,
				Parent = this
			};

			comboBox1 = new ComboBox()
			{

				DataSource = Enum.GetValues(typeof(ENUM)),
				DropDownStyle = ComboBoxStyle.DropDownList,
				Left = label2.Left,
				Top = label2.Bottom + 10,
				Width = 135,
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Blue,
				BackColor = Color.White,
				AutoSize = true,
				Parent = this,
			};

			comboBox4 = new ComboBox()
			{

				DataSource = null,
				DropDownStyle = ComboBoxStyle.DropDownList,
				Left = comboBox1.Right + 10,
				Top = comboBox1.Top,
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Blue,
				BackColor = Color.White,
				AutoSize = true,
				Visible = false,
				Parent = this,
			};

			comboBox2 = new ComboBox()
			{

				DataSource = Enum.GetValues(typeof(Reláció)),
				DropDownStyle = ComboBoxStyle.DropDownList,
				Left = comboBox1.Right + 15,
				Top = comboBox1.Top,
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Blue,
				BackColor = Color.White,
				AutoSize = true,
				Parent = this,
			};

			numericUpDown1 = new NumericUpDown()
			{
				Left = comboBox2.Right + 15,
				Top = comboBox2.Top,
				Maximum = 1000000,
				DecimalPlaces = 6,
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
				Parent = this,
			};

			button1 = new Button()
			{
				Left = label2.Left,
				Top = comboBox1.Bottom + 15,
				Width = listBox1.Width,
				Text = "Search",
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.DarkGreen,
				AutoSize = true,
				Parent = this
			};

			ComboBox1_SelectedIndexChanged(new object(), new EventArgs());
		}
	}
}
