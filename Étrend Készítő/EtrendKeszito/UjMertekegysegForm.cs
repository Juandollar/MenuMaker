using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EtrendKeszito
{
	public partial class UjMertekegysegForm : Form
	{
		private Mertekegyseg mertekegyseg;
		bool megjelenit = false;

		internal Mertekegyseg Mertekegyseg
		{
			get { return mertekegyseg; }
		}

		public UjMertekegysegForm()
		{
			InitializeComponent();
			VezerlokLetrehozasa();
			AddButtonClick();
		}

		internal UjMertekegysegForm(Mertekegyseg modosit, bool megjelenit = false) : this()
		{
			try
			{
				mertekegyseg = modosit;
				this.megjelenit = megjelenit;
				textBox1.Text = mertekegyseg.Megnevezes;
				textBox1.Enabled = false;
				comboBox1.SelectedIndex = (int) mertekegyseg.Mertek;
				comboBox1.Enabled = false;
				numericUpDown1.Value = (decimal) mertekegyseg.Valtoszam;
				numericUpDown1.Enabled = false;
				checkBox1.Checked = mertekegyseg.Hasznalhato;
				checkBox2.Checked = mertekegyseg.Torolheto;
				checkBox2.Enabled = false;

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
			comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
		}

		private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			label4.Text = ((default_mertekegysegek)comboBox1.SelectedIndex).ToString();
		}

		private void Button1_Click(object sender, EventArgs e)
		{
			try
			{
				if (numericUpDown1.Value <= 0)
				{
					throw new WarningException("The rate should be positive!");
				}

				if (mertekegyseg == null)
				{
					string fnevhash = ABKezelo.GetCurrentUser();
					string nev = textBox1.Text.Trim();

					if (nev == "" || nev.Length > 30)
					{
						throw new WarningException("The name should be non-empty and at most 30 characters!");
					}

					if (ABKezelo.Kiolvasas().Where(x =>x is Mertekegyseg &&
						(x as Mertekegyseg).Megnevezes == nev).ToList().Count != 0)
					{
						throw new WarningException("We have already a unit with that name!");
					}

					mertekegyseg = new Mertekegyseg(fnevhash, nev, (MertekegysegFajta)comboBox1.SelectedIndex,(double)numericUpDown1.Value, checkBox1.Checked,
						checkBox2.Checked);
					ABKezelo.Beszuras(mertekegyseg);
				    Logolas.Ment("New unit added, name: " + mertekegyseg.Megnevezes);
				}
                else
				{
					mertekegyseg.Hasznalhato = checkBox1.Checked;
					ABKezelo.Modositas(mertekegyseg);
                    Logolas.Ment("Unit modification, name: "+mertekegyseg.Megnevezes);
				}
			}
			catch (Exception ex)
			{
				if (ex is WarningException)
					MessageBox.Show(ex.Message, "Attention!", MessageBoxButtons.OK, MessageBoxIcon.Information);
				else MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				DialogResult = DialogResult.None;
			}
		}

		private void Button2_Click(object sender, EventArgs e)
		{
			Close();
		}
		
		private Label label1;
		private Label label2;
		private Label label3;
		private Label label4;

		private TextBox textBox1;

		private NumericUpDown numericUpDown1;

		private Button button1;
		private Button button2;

		private CheckBox checkBox1;
		private CheckBox checkBox2;

		private ComboBox comboBox1;

		void VezerlokLetrehozasa()
		{
			Height = 320;
			Width = 320;
			CenterToScreen();
			Text = "New unit form";

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
				DataSource = Enum.GetValues(typeof(MertekegysegFajta)),
				DropDownStyle = ComboBoxStyle.DropDownList,
				AutoSize = true,
			};

			label3 = new Label()
			{
				Left = label1.Left,
				Top = label2.Bottom+15,
				Text = "Rate:",
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Brown,
				AutoSize = true,
				Parent = this
			};

			numericUpDown1 = new NumericUpDown()
			{
				Left = label3.Right+10,
				Top = label3.Top,
				DecimalPlaces = 6,
				Minimum = 0,
				Maximum = 1000000,
				AutoSize = true,
				Parent = this
			};

			label4 = new Label()
			{
				Left = numericUpDown1.Right + 10,
				Top = numericUpDown1.Top,
				Text = ((default_mertekegysegek)0).ToString(),
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Brown,
				AutoSize = true,
				Parent = this
			};

			checkBox1 = new CheckBox()
			{
				Left = label1.Left,
				Top = label3.Bottom + 15,
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
