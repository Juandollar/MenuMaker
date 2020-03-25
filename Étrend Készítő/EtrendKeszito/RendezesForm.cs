using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EtrendKeszito
{
	public partial class RendezesForm : Form
	{
		private List<Elelmiszer> elelmiszerek;
		public RendezesForm(bool tipus)
		{
		    Logolas.Ment("We are on the sort form");

            InitializeComponent();
			elelmiszerek=new List<Elelmiszer>();

			try
			{
				if (tipus)
				{
					foreach (Elelmiszer item in ABKezelo.Kiolvasas().Where(x => x is Elelmiszer).ToList())
					{
						elelmiszerek.Add(item);
					}
				}
				else
				{
					foreach (Elelmiszer item in ABKezelo.Kiolvasas().Where(x => x is Elelmiszer && (x as Elelmiszer).Hasznalhato)
						.ToList())
					{
						elelmiszerek.Add(item);
					}
				}

				VezerlokLetrehozasa();
				AddButtonClick();
				Rendezes();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private Label label1;
		private Label label2;
		private Label label3;

		private ListBox listBox1;

		private RadioButton radioButton1;
		private RadioButton radioButton2;
		private RadioButton radioButton3;
		private RadioButton radioButton4;
		private RadioButton radioButton5;

		private ComboBox comboBox1;

		private CheckBox checkBox1;
		private CheckBox checkBox2;

		void AddButtonClick()
		{
			radioButton1.CheckedChanged += RadioButton1_CheckedChanged;
			radioButton2.CheckedChanged += RadioButton2_CheckedChanged;
			radioButton3.CheckedChanged += RadioButton3_CheckedChanged;
			radioButton4.CheckedChanged += RadioButton4_CheckedChanged;
			radioButton5.CheckedChanged += RadioButton5_CheckedChanged;
			comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
			checkBox1.CheckedChanged += CheckBox1_CheckedChanged;
		}

		void Rendezes()
		{
			try
			{
				if (radioButton1.Checked)
				{
					if (checkBox1.Checked) elelmiszerek = elelmiszerek.OrderBy(x => x.Megnevezes).ToList();
					else elelmiszerek = elelmiszerek.OrderByDescending(x => x.Megnevezes).ToList();
				}
				else if (radioButton2.Checked)
				{
					if (checkBox1.Checked) elelmiszerek = elelmiszerek.OrderBy(x => x.Ar * x.Penz.Arfolyam).ToList();
					else elelmiszerek = elelmiszerek.OrderByDescending(x => x.Ar * x.Penz.Arfolyam).ToList();
				}
				else if (radioButton3.Checked)
				{
					if (checkBox1.Checked)
						elelmiszerek = elelmiszerek.OrderBy(
							x => (x is Ital)
								? 0
								: (x is Etel
									? (x as Etel).EgysegTomegMennyiseg * (x as Etel).TomegMertek.Valtoszam
									: (x as Menu).EgysegTomegMennyiseg * (x as Menu).TomegMertek.Valtoszam)).ToList();
					else
						elelmiszerek = elelmiszerek.OrderByDescending(x =>
							(x is Ital)
								? 0
								: (x is Etel
									? (x as Etel).EgysegTomegMennyiseg * (x as Etel).TomegMertek.Valtoszam
									: (x as Menu).EgysegTomegMennyiseg * (x as Menu).TomegMertek.Valtoszam)).ToList();
				}
				else if (radioButton4.Checked)
				{
					if (checkBox1.Checked)
						elelmiszerek = elelmiszerek.OrderBy(
							x => (x is Etel)
								? 0
								: (x is Ital
									? (x as Ital).EgysegUrTartalomMennyiseg * (x as Ital).Urmertek.Valtoszam
									: (x as Menu).EgysegUrTartalomMennyiseg * (x as Menu).Urmertek.Valtoszam)).ToList();
					else
						elelmiszerek = elelmiszerek.OrderByDescending(x =>
							(x is Etel)
								? 0
								: (x is Ital
									? (x as Ital).EgysegUrTartalomMennyiseg * (x as Ital).Urmertek.Valtoszam
									: (x as Menu).EgysegUrTartalomMennyiseg * (x as Menu).Urmertek.Valtoszam)).ToList();
				}
				else if(comboBox1.SelectedIndex!=-1)
				{
					Tapanyag t = (Tapanyag)ABKezelo.Kiolvasas()
						.Where(x => x is Tapanyag && (x as Tapanyag).Megnevezes == comboBox1.SelectedItem.ToString()).ToList().First();
					if (checkBox1.Checked) elelmiszerek = elelmiszerek.OrderBy(x => x.TapanyagTartalom[t]).ToList();
					else elelmiszerek = elelmiszerek.OrderByDescending(x => x.TapanyagTartalom[t]).ToList();
				}

				listBox1.DataSource = null;
				listBox1.DataSource = elelmiszerek;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void CheckBox1_CheckedChanged(object sender, EventArgs e)
		{
			Rendezes();
		}

		private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			Rendezes();
		}

		private void RadioButton5_CheckedChanged(object sender, EventArgs e)
		{
			Rendezes();
		}

		private void RadioButton4_CheckedChanged(object sender, EventArgs e)
		{
			Rendezes();
		}

		private void RadioButton3_CheckedChanged(object sender, EventArgs e)
		{
			Rendezes();
		}

		private void RadioButton2_CheckedChanged(object sender, EventArgs e)
		{
			Rendezes();
		}

		private void RadioButton1_CheckedChanged(object sender, EventArgs e)
		{
			Rendezes();
		}

		void VezerlokLetrehozasa()
		{
			Height = 520;
			Width = 760;
			CenterToScreen();
			Text = "Sort form";

			label1 = new Label()
			{
				Left = 30,
				Top = 30,
				Text = "Sorted list",
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
				Text = "Order by:",
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Brown,
				AutoSize = true,
				Parent = this
			};

			radioButton1 = new RadioButton()
			{
				Left = label2.Left,
				Top = label2.Bottom+10,
				Text = "Name",
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.Indigo,
				Checked = true,
				Parent = this
			};

			radioButton2 = new RadioButton()
			{
				Left = radioButton1.Left,
				Top = radioButton1.Bottom + 10,
				Text = "Price",
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.Indigo,
				Parent = this
			};

			radioButton3 = new RadioButton()
			{
				Left = radioButton2.Left,
				Top = radioButton2.Bottom + 10,
				Text = "Mass",
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.Indigo,
				Parent = this
			};

			radioButton4 = new RadioButton()
			{
				Left = radioButton3.Left,
				Top = radioButton3.Bottom + 10,
				Text = "Liquid measure",
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.Indigo,
				Parent = this
			};

			radioButton5 = new RadioButton()
			{
				Left = radioButton4.Left,
				Top = radioButton4.Bottom + 10,
				Text = "Nutrient",
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.Indigo,
				Parent = this
			};

			comboBox1 = new ComboBox()
			{
				DataSource = null,
				DropDownStyle = ComboBoxStyle.DropDownList,
				Left = radioButton5.Right + 10,
				Top = radioButton5.Top,
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Blue,
				BackColor = Color.White,
				AutoSize = true,
				Parent = this
			};

			label3 = new Label()
			{
				Left = radioButton5.Left,
				Top = radioButton5.Bottom + 20,
				Text = "Order's direction:",
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Brown,
				AutoSize = true,
				Parent = this
			};

			checkBox1 = new CheckBox()
			{
				Left = label3.Left,
				Top = label3.Bottom + 10,
				Checked = true,
				Text = "Increasing order (in ABC)?",
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.DarkMagenta,
				AutoSize = true,
				Parent = this
			};

			radioButton1.Checked = true;

			try
			{
				comboBox1.Items.Clear();
				comboBox1.Items.AddRange(ABKezelo.Kiolvasas().Where(x => x is Tapanyag && (x as Tapanyag).Hasznalhato).Select(x => (x as Tapanyag).Megnevezes).ToArray());
				if(comboBox1.Items.Count>0) comboBox1.SelectedIndex = 0;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
