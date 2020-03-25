using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EtrendKeszito
{
	public partial class UjTapanyagForm : Form
	{
		private Tapanyag tapanyag;
		private bool megjelenit = false;

		internal Tapanyag Tapanyag
		{
			get { return tapanyag; }
		}

		public UjTapanyagForm()
		{
			InitializeComponent();
			VezerlokLetrehozasa();
			AddButtonClick();
		}

		internal UjTapanyagForm(Tapanyag modosit, bool megjelenit = false) : this()
		{
			try
			{
				tapanyag = modosit;
				this.megjelenit = megjelenit;

				string mnev = modosit.Mertek.Megnevezes;

				textBox1.Text = tapanyag.Megnevezes;
				textBox1.Enabled = false;

				for (int i = 0; i < comboBox1.Items.Count; i++)
				{
					if (comboBox1.Items[i].ToString() == mnev)
					{
						comboBox1.SelectedIndex = i;
						break;
					}
				}
				comboBox1.Enabled = false;

				numericUpDown1.Value = (decimal) modosit.NapiMinBevitel;
				numericUpDown2.Value = (decimal) modosit.NapiMaxBevitel;
				checkBox1.Checked = modosit.NapiMax;
				checkBox2.Checked = modosit.Hasznalhato;

				if (megjelenit)
				{
					foreach (Control item in Controls)
					{
						item.Enabled = false;
					}

					button1.DialogResult = DialogResult.Cancel;
					button1.Enabled = true;
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
		}

		private void Button2_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void Button1_Click(object sender, EventArgs e)
		{
			try
			{
				string fnevhash = ABKezelo.GetCurrentUser();
				string mnev = comboBox1.SelectedItem.ToString();
				Mertekegyseg m = (Mertekegyseg)ABKezelo.Kiolvasas()
					.Where(x => (x is Mertekegyseg) && (x as Mertekegyseg).Megnevezes == mnev).First();

				if (checkBox1.Checked && numericUpDown1.Value > numericUpDown2.Value)
				{
					throw new ArgumentException("The daily minimum intake should not be less than the maximumu!");
				}

				if (tapanyag == null)
				{
					string nev = textBox1.Text.Trim();

					if (nev == "" || nev.Length > 30)
					{
						throw new ArgumentException("The name should be non-empty and at most 30 characters length!");
					}

					if (ABKezelo.Kiolvasas().Where(x => x is Tapanyag && (x as Tapanyag).Megnevezes == nev).ToList().Count > 0)
					{
						throw new ArgumentException("We have already a nutrient with this name!");
					}

					tapanyag = new Tapanyag(fnevhash, nev, m, (double)numericUpDown1.Value, (double)numericUpDown2.Value,checkBox1.Checked, checkBox2.Checked);
					ABKezelo.Beszuras(tapanyag);
				    Logolas.Ment("New nutrient added, name: " + tapanyag.Megnevezes);
                    ABKezelo.BeszurTapanyagElelmiszerekbe(tapanyag);
				}
				else
				{
					tapanyag.NapiMinBevitel = (double)numericUpDown1.Value;
					tapanyag.NapiMaxBevitel = (double)numericUpDown2.Value;
					tapanyag.NapiMax = checkBox1.Checked;
					tapanyag.Hasznalhato = checkBox2.Checked;
					ABKezelo.Modositas(tapanyag);
				    Logolas.Ment("Nutrient modification, name: " + tapanyag.Megnevezes);
                }
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				DialogResult = DialogResult.None;
			}
		}

		private Label label1;
		private Label label2;
		private Label label3;
		private Label label4;

		private TextBox textBox1;

		private ComboBox comboBox1;

		private NumericUpDown numericUpDown1;
		private NumericUpDown numericUpDown2;

		private CheckBox checkBox1;
		private CheckBox checkBox2;

		private Button button1;
		private Button button2;

		void VezerlokLetrehozasa()
		{
			Height = 350;
			Width = 400;
			CenterToScreen();
			Text = "New nutrient form";

			label1 = new Label()
			{
				Left = 20,
				Top = 20,
				Text = "Name:",
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Brown,
				AutoSize = true,
				Parent = this
			};

			textBox1 = new TextBox()
			{
				Left = label1.Right + 10,
				Top = label1.Top,
				AutoSize = true,
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.Indigo,
				Parent = this
			};

			label2 = new Label()
			{
				Left = label1.Left,
				Top = label1.Bottom + 15,
				Text = "Unit of measurement:",
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Brown,
				AutoSize = true,
				Parent = this
			};

			comboBox1 = new ComboBox()
			{
				Parent = this,
				Left = label2.Right + 10,
				Top = label2.Top,
				DataSource = null,
				DropDownStyle = ComboBoxStyle.DropDownList,
				AutoSize = true,
			};

			label3 = new Label()
			{
				Left = label2.Left,
				Top = label2.Bottom + 15,
				Text = "Daily min. intake:",
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Brown,
				AutoSize = true,
				Parent = this
			};

			numericUpDown1 = new NumericUpDown()
			{
				Left = label3.Right + 10,
				Top = label3.Top,
				DecimalPlaces = 2,
				Minimum = 0,
				Maximum = 1000000,
				AutoSize = true,
				Parent = this
			};

			label4 = new Label()
			{
				Left = label3.Left,
				Top = label3.Bottom + 15,
				Text = "Daily max. intake:",
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Brown,
				AutoSize = true,
				Parent = this
			};

			numericUpDown2 = new NumericUpDown()
			{
				Left = label4.Right + 10,
				Top = label4.Top,
				DecimalPlaces = 2,
				Minimum = 0,
				Maximum = 1000000,
				AutoSize = true,
				Parent = this
			};

			checkBox1 = new CheckBox()
			{
				Left = label4.Left,
				Top = label4.Bottom + 15,
				Text = "Consider the daily max?",
				Checked = false,
				AutoSize = true,
				Parent = this
			};

			checkBox2 = new CheckBox()
			{
				Left = label1.Left,
				Top = checkBox1.Bottom + 15,
				Text = "Usable?",
				Checked = true,
				AutoSize = true,
				Parent = this
			};

			button1 = new Button()
			{
				Left = label1.Left,
				Top = checkBox2.Bottom + 20,
				Width = 220,
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
				Width = 220,
				Text = "Cancel",
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.DarkGreen,
				DialogResult = DialogResult.Cancel,
				AutoSize = true,
				Parent = this
			};

			try
			{
				comboBox1.Items.AddRange(ABKezelo.Kiolvasas().Where(x => x is Mertekegyseg && (x as Mertekegyseg).Hasznalhato).Select(p => (p as Mertekegyseg).Megnevezes).ToArray());
				comboBox1.SelectedIndex = 0;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
