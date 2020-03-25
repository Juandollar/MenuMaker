using System;
using System.Drawing;
using System.Windows.Forms;

namespace EtrendKeszito
{
	public partial class UjFelhasznaloForm : Form
	{
		public UjFelhasznaloForm()
		{
			InitializeComponent();
			VezerlokLetrehozasa();
			AddButtonClick();
		}

		void AddButtonClick()
		{
			button1.Click += Button1_Click;
			button2.Click += Button2_Click;
		}

		private void Button1_Click(object sender, EventArgs e)
		{
			int l1 = textBox1.Text.Trim().Length;
			int l2 = textBox2.Text.Length;
			if (l1 < 4 || l1 > 24)
			{
				MessageBox.Show("User name's length should be between 4 and 24 !","Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);
				DialogResult = DialogResult.None;
			}
			else if (textBox2.Text != textBox3.Text)
			{
				MessageBox.Show("The two passwords should be the same!","Error!",MessageBoxButtons.OK,MessageBoxIcon.Error);
				DialogResult = DialogResult.None;
			}
			else if (l2 < 4 || l2 > 24)
			{
				MessageBox.Show("Password length should be between 4 and 24 !", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				DialogResult = DialogResult.None;
			}
			else if (textBox4.Text.Length > 30)
			{
				MessageBox.Show("Password remainder should be at most 30 !", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				DialogResult = DialogResult.None;
			}
			else
			{
				Titkositasok t=new Titkositasok();
				string felhasznaloNevHash = Titkositasok.SHAHash(textBox1.Text.Trim()+Konstans.salt);

				if (ABKezelo.CountUsers(felhasznaloNevHash) > 0)
				{
					MessageBox.Show("This username is already registered, choose another one!", "Error!", MessageBoxButtons.OK,
						MessageBoxIcon.Error);
					DialogResult = DialogResult.None;
				}
				else
				{
					string jelszoHash = Titkositasok.SHAHash(textBox2.Text + Konstans.salt);
					Felhasznalo f = new Felhasznalo(felhasznaloNevHash, jelszoHash,
						t.f0(felhasznaloNevHash, textBox4.Text, KodolasIranya.Kódol));
					ABKezelo.Beszuras(f);
					Logolas.Ment("Register a new user, the name: "+textBox1.Text.Trim());
				}
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
		private TextBox textBox2;
		private TextBox textBox3;
		private TextBox textBox4;

		private Button button1;
		private Button button2;

		void VezerlokLetrehozasa()
		{
			Height = 300;
			Width = 450;
			CenterToScreen();
			Text = "New user form";

			label1 = new Label()
			{
				Left = 20,
				Top = 20,
				Text = "User name(*):",
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Blue,
				AutoSize = true,
				Visible = true,
				Parent = this
			};

			textBox1 = new TextBox()
			{
				Top = label1.Top,
				Left = label1.Right + 45,
				Width = 220,
				AutoSize = true,
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.Indigo,
				Visible = true,
				Parent = this
			};

			label2 = new Label()
			{
				Left = label1.Left,
				Top = label1.Bottom+20,
				Text = "Password(*):",
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Blue,
				AutoSize = true,
				Visible = true,
				Parent = this
			};

			textBox2 = new TextBox()
			{
				Top = label2.Top,
				Left = textBox1.Left,
				Width = 220,
				PasswordChar = '*',
				AutoSize = true,
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.Indigo,
				Visible = true,
				Parent = this
			};

			label3 = new Label()
			{
				Left = label2.Left,
				Top = label2.Bottom + 20,
				Text = "Password, repeat (*):",
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Blue,
				AutoSize = true,
				Visible = true,
				Parent = this
			};

			textBox3 = new TextBox()
			{
				Top = label3.Top,
				Left = textBox1.Left,
				Width = 220,
				PasswordChar = '*',
				AutoSize = true,
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.Indigo,
				Visible = true,
				Parent = this
			};

			label4 = new Label()
			{
				Left = label3.Left,
				Top = label3.Bottom + 20,
				Text = "Password remainder:",
				Font = new Font(FontFamily.GenericSansSerif, 10),
				ForeColor = Color.Blue,
				AutoSize = true,
				Visible = true,
				Parent = this
			};

			textBox4 = new TextBox()
			{
				Top = label4.Top,
				Left = textBox1.Left,
				Height = 450,
				Width = 220,
				AutoSize = true,
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.Indigo,
				Visible = true,
				Parent = this
			};

			button1 = new Button()
			{
				Top = label4.Bottom + 20,
				Width = this.Width - 45,
				Left = 15,
				Text = "OK",
				DialogResult = DialogResult.OK,
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.DarkGreen,
				AutoSize = true,
				Parent = this
			};

			button2 = new Button()
			{
				Top = button1.Bottom + 20,
				Width = button1.Width,
				Left = 15,
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
