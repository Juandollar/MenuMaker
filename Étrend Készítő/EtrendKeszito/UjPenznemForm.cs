using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EtrendKeszito
{
	public partial class UjPenznemForm : Form
	{
		private Penznem penznem;
		private bool megjelenit = false;

		internal Penznem Penznem
		{
			get { return penznem; }
		}

		public UjPenznemForm()
		{
			InitializeComponent();
			VezerlokLetrehozasa();
			AddButtonClick();
		}

		internal UjPenznemForm(Penznem modosit, bool megjelenit = false) : this()
		{
			try
			{
				penznem = modosit;
				this.megjelenit = megjelenit;
				textBox1.Text = penznem.Megnevezes;
				textBox1.Enabled = false;
				textBox2.Text = penznem.PenzKod;
				textBox2.Enabled = false;
				numericUpDown1.Value = Convert.ToDecimal(penznem.Arfolyam);
				numericUpDown2.Value = penznem.TizedesekSzama;
				checkBox1.Checked = penznem.Hasznalhato;
				checkBox2.Checked = penznem.Torolheto;
				checkBox2.Enabled = false;

				if (megjelenit)
				{
					foreach (Control item in Controls)
					{
						item.Enabled = false;
					}

					button1.Enabled = true;
					button1.DialogResult = DialogResult.Cancel;
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
				if (numericUpDown1.Value <= 0)
				{
					throw new ArgumentException("The exchange rate should be positive!");
				}

				if (penznem == null)
				{
					string fnevhash = ABKezelo.GetCurrentUser();
					string nev = textBox1.Text.Trim();
					string kod = textBox2.Text.Trim().ToUpper();

					if (nev == "" || nev.Length > 30)
					{
						throw new ArgumentException("The name should be non-empty and at most 30 characters length!");
					}

					if (kod.Length != 3)
					{
						throw new ArgumentException("Currency code should have exactly 3 characters!");
					}

					if (ABKezelo.Kiolvasas().Where(x => x is Penznem && (x as Penznem).Megnevezes == nev).ToList().Count != 0)
					{
						throw new ArgumentException("We have already a currency with this name!");
					}

					penznem = new Penznem(fnevhash, nev, kod,
							(double)numericUpDown1.Value, (byte)numericUpDown2.Value, checkBox1.Checked, checkBox2.Checked);
					ABKezelo.Beszuras(penznem);
				    Logolas.Ment("New currency added, name: " + penznem.Megnevezes);
                }
				else
				{
					if (!checkBox1.Checked && ABKezelo.Kiolvasas().Where(x => (x is Elelmiszer)&& (x as Elelmiszer).Penz.Megnevezes==penznem.Megnevezes).ToList().Count>0)
					{
						throw new ArgumentException("The currency should be usable, because there is a food that is using it!");
					}

					penznem.Arfolyam = (double)numericUpDown1.Value;
					penznem.TizedesekSzama = (byte)numericUpDown2.Value;
					penznem.Hasznalhato = checkBox1.Checked;
					ABKezelo.Modositas(penznem);
				    Logolas.Ment("Currency modification, name: " + penznem.Megnevezes);
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
		private Label label5;

		private TextBox textBox1;
		private TextBox textBox2;

		private NumericUpDown numericUpDown1;
		private NumericUpDown numericUpDown2;

		private Button button1;
		private Button button2;

		private CheckBox checkBox1;
		private CheckBox checkBox2;

		void VezerlokLetrehozasa()
		{
			Height = 340;
			Width = 350;
			CenterToScreen();
			Text = "New currency form";

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
				Text = "3 letters code:",
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Brown,
				AutoSize = true,
				Parent = this
			};

			textBox2 = new TextBox()
			{
				Left = label2.Right + 10,
				Top = label2.Top,
				AutoSize = true,
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.Indigo,
				Parent = this
			};

			label3 = new Label()
			{
				Left = label2.Left,
				Top = label2.Bottom + 15,
				Text = "Exchange rate:",
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Brown,
				AutoSize = true,
				Parent = this
			};

			numericUpDown1 = new NumericUpDown()
			{
				Left = label3.Right + 10,
				Top = label3.Top,
				Minimum = 0,
				Maximum = 1000000,
				Value = 0,
				DecimalPlaces = 2,
				AutoSize = true,
				Parent = this
			};

			label5 = new Label()
			{
				Left = numericUpDown1.Right + 10,
				Top = label3.Top,
				Text = "(forint)",
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Brown,
				AutoSize = true,
				Parent = this
			};

			label4 = new Label()
			{
				Left = label3.Left,
				Top = label3.Bottom + 15,
				Text = "Decimals number:",
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Brown,
				AutoSize = true,
				Parent = this
			};

			numericUpDown2 = new NumericUpDown()
			{
				Left = label4.Right + 10,
				Top = label4.Top,
				Minimum = 0,
				Maximum = 6,
				AutoSize = true,
				Parent = this
			};

			checkBox1 = new CheckBox()
			{
				Left = label1.Left,
				Top = label4.Bottom + 15,
				Checked = true,
				Text = "Usable?",
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.DarkMagenta,
				AutoSize = true,
				Parent = this
			};

			checkBox2 = new CheckBox()
			{
				Left = label1.Left,
				Top = checkBox1.Bottom + 15,
				Checked = true,
				Text = "Deletable?",
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.DarkMagenta,
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
		}
	}
}
