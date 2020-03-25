using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace EtrendKeszito
{
	public partial class BejelentkezoForm : Form
	{
		public BejelentkezoForm()
		{
			InitializeComponent();
			VezerlokLetrehozasa();
			AddButtonClick();
		}

		void AddButtonClick()
		{
			button1.Click += Button1_Click;
			button2.Click += Button2_Click;
			button3.Click += Button3_Click;
		}

		private void Button3_Click(object sender, EventArgs e)
		{
			UjFelhasznaloForm dialogus = new UjFelhasznaloForm();
			dialogus.ShowDialog();
		}

		private void Button2_Click(object sender, EventArgs e)
		{
			try
			{
				string felhasznaloNevHash = Titkositasok.SHAHash(textBox1.Text.Trim() + Konstans.salt);
				if (ABKezelo.CountUsers(felhasznaloNevHash) != 0)
				{
					string jelszoEmlekeztetoOTP = ABKezelo.getJelszoHash(felhasznaloNevHash);
					Titkositasok t = new Titkositasok();
					string jelszo = t.f0(felhasznaloNevHash, jelszoEmlekeztetoOTP, KodolasIranya.Dekódol);

					if (jelszo != "" && jelszo[0] != 0)
					{
						MessageBox.Show("The password remainder: " + jelszo, "Information", MessageBoxButtons.OK,
							MessageBoxIcon.Information);
					}
					else
					{
						throw new WarningException("There is no such username or not set password remainder!");
					}
				}
				else
				{
					throw new WarningException("There is no such username or not set password remainder!");
				}
			}
			catch (Exception ex)
			{
				if (ex is WarningException)
					MessageBox.Show(ex.Message, "Attention!", MessageBoxButtons.OK, MessageBoxIcon.Information);
				else MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void Button1_Click(object sender, EventArgs e)
		{
			try
			{
				Felhasznalo f = new Felhasznalo(Titkositasok.SHAHash(textBox1.Text.Trim() + Konstans.salt),Titkositasok.SHAHash(textBox2.Text + Konstans.salt), "");
				if (!ABKezelo.SikeresBelepes(f))
				{
					throw new WarningException("Unsuccessful login!");
				}
				ABKezelo.setCurrentUser(f);
                Logolas.Ment("Successful login, name: "+textBox1.Text.Trim());
			}
			catch (Exception ex)
			{
				if (ex is WarningException)
					MessageBox.Show(ex.Message, "Attention!", MessageBoxButtons.OK,MessageBoxIcon.Information);
				else MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				DialogResult = DialogResult.None;
			}
		}

		private Label label1;
		private Label label2;

		private TextBox textBox1;
		private TextBox textBox2;

		private Button button1;
		private Button button2;
		private Button button3;

		void VezerlokLetrehozasa()
		{
			Height = 300;
			Width = 400;
			CenterToScreen();
			Text = "Login form";

			label1 = new Label()
			{
				Left = 20,
				Top = 20,
				Text = "User name:",
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Blue,
				AutoSize = true,
				Parent = this
			};

			textBox1 = new TextBox()
			{
				Top = label1.Top,
				Left = label1.Right + 15,
				Height = 450,
				Width = 220,
				AutoSize = true,
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.Indigo,
				Parent = this
			};

			label2 = new Label()
			{
				Left = label1.Left,
				Top = label1.Bottom + 15,
				Text = "Password:",
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Blue,
				AutoSize = true,
				Parent = this
			};

			textBox2 = new TextBox()
			{
				Top = label2.Top,
				Left = textBox1.Left,
				Height = 450,
				Width = 220,
				PasswordChar = '*',
				AutoSize = true,
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.Indigo,
				Parent = this
			};

			button1 = new Button()
			{
				Top = label2.Bottom + 20,
				Width = this.Width - 45,
				Left = 15,
				Text = "Login",
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
				Width = button1.Width,
				Text = "Give the password remainder!",
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.DarkGreen,
				AutoSize = true,
				Parent = this
			};

			button3 = new Button()
			{
				Left = button1.Left,
				Top = button2.Bottom + 15,
				Width = button1.Width,
				Text = "New user registration",
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.DarkGreen,
				AutoSize = true,
				Parent = this
			};
		}
	}
}
