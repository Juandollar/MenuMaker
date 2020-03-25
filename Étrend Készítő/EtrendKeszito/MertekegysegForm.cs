using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EtrendKeszito
{
	public partial class MertekegysegForm : Form
	{
		public MertekegysegForm()
		{
			InitializeComponent();
			VezerlokLetrehozasa();
			AddButtonClick();
			ListBoxRefresh();
		}

		void AddButtonClick()
		{
			button1.Click += Button1_Click;
			button2.Click += Button2_Click;
			button3.Click += Button3_Click;
			button4.Click += Button4_Click;
		}

		private void Button4_Click(object sender, EventArgs e)
		{
			try
			{
				if (listBox1.SelectedIndex != -1)
				{
					string str = (listBox1.SelectedItem as Mertekegyseg).Megnevezes;
					bool torolheto = true;
					foreach (Elelmiszer item in ABKezelo.Kiolvasas().Where(x => x is Elelmiszer))
					{
						if (item is Ital && (item as Ital).Urmertek.Megnevezes == str)
						{
							torolheto = false;
							break;
						}

						if (item is Etel && (item as Etel).TomegMertek.Megnevezes == str)
						{
							torolheto = false;
							break;
						}
					}

					if (!torolheto)
					{
						throw new WarningException(
							"This unit is not deletable, beaucse there is a food using it!");
					}

					if (ABKezelo.Kiolvasas().Where(x => x is Tapanyag && (x as Tapanyag).Mertek.Megnevezes == str).ToList().Count > 0)
					{
						throw new WarningException(
							"This unit is not deletable, because there is a nutrient using it!");
					}

					if (ABKezelo.Kiolvasas().Where(x => x is Etel && (x as Etel).TomegMertek.Megnevezes == str).ToList().Count > 0)
					{
						throw new WarningException(
							"This unit is not deletable, because there is a meal using it!");
					}

					if (ABKezelo.Kiolvasas().Where(x => x is Ital && (x as Ital).Urmertek.Megnevezes == str).ToList().Count > 0)
					{
						throw new WarningException(
							"This unit is not deletable, because there is a drink using it!");
					}

					if (((Mertekegyseg)listBox1.SelectedItem).Torolheto)
					{
						if (MessageBox.Show("Really want to delete the unit?", "Question", MessageBoxButtons.YesNo,
								MessageBoxIcon.Question) == DialogResult.Yes)
						{
						    string nev = ((Mertekegyseg) listBox1.SelectedItem).Megnevezes;
							ABKezelo.Torol((Mertekegyseg)listBox1.SelectedItem);
                            Logolas.Ment("Unit deleted, name: "+nev);
							ListBoxRefresh();
						}
					}
					else
					{
						throw new WarningException("This unit is not deletable!");
					}
				}
				else
				{
					throw new WarningException("To delete a unit choose one!");
				}
			}
			catch (Exception ex)
			{
				if (ex is WarningException)
					MessageBox.Show(ex.Message, "Attention!", MessageBoxButtons.OK, MessageBoxIcon.Information);
				else MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void Button3_Click(object sender, EventArgs e)
		{
			if (listBox1.SelectedIndex != -1)
			{
				try
				{
					string nev = ((Mertekegyseg)listBox1.SelectedItem).Megnevezes;
					if (nev == "gram" || nev == "milligram" || nev == "liter" || nev == "joule" || nev == "calorie")
					{
						throw new WarningException("It is default unit, not deletable, not modifiable!");
					}

					if (ABKezelo.Kiolvasas().Where(x => x is Tapanyag && (x as Tapanyag).Mertek.Megnevezes == nev).ToList().Count > 0)
					{
						throw new WarningException(
							"The unit is not modifiable, because there is a nutrient using it!");
					}

					if (ABKezelo.Kiolvasas().Where(x => x is Etel && (x as Etel).TomegMertek.Megnevezes == nev).ToList().Count > 0)
					{
						throw new WarningException(
							"The unit is not modifiable, because there is a meal using it!");
					}

					if (ABKezelo.Kiolvasas().Where(x => x is Ital && (x as Ital).Urmertek.Megnevezes == nev).ToList().Count > 0)
					{
						throw new WarningException(
							"The unit is not modifiable, because there is a drink using it!");
					}

					UjMertekegysegForm dialogus = new UjMertekegysegForm((Mertekegyseg)listBox1.SelectedItem);
					if(dialogus.ShowDialog()==DialogResult.OK) ListBoxRefresh();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			else
			{
				MessageBox.Show("To modify a unit choose one!", "Attention!", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		private void Button2_Click(object sender, EventArgs e)
		{
			if (listBox1.SelectedIndex != -1)
			{
				try
				{
					UjMertekegysegForm dialogus = new UjMertekegysegForm((Mertekegyseg)listBox1.SelectedItem, true);
					dialogus.ShowDialog();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			else
			{
				MessageBox.Show("To display a unit choose one!", "Attention!", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		private void Button1_Click(object sender, EventArgs e)
		{
			try
			{
				UjMertekegysegForm dialogus = new UjMertekegysegForm();
				if(dialogus.ShowDialog()==DialogResult.OK) ListBoxRefresh();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private ListBox listBox1;

		private Label label1;

		private Button button1;
		private Button button2;
		private Button button3;
		private Button button4;

		private void ListBoxRefresh()
		{
			try
			{
				int index = listBox1.SelectedIndex;
				listBox1.DataSource = null;
				listBox1.DataSource = ABKezelo.Kiolvasas().Where(x => x is Mertekegyseg).ToList();
				if (index >= 0 && index < listBox1.Items.Count) listBox1.SelectedIndex = index;
				else if (listBox1.Items.Count > 0) listBox1.SelectedIndex = 0;
				else listBox1.SelectedIndex = -1;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		void VezerlokLetrehozasa()
		{
			Height = 350;
			Width = 450;
			CenterToScreen();
			Text = "Unit form";

			listBox1 = new ListBox()
			{
				Left = 25,
				Top = 50,
				Height = 250,
				Width = 120,
				Font = new Font(FontFamily.GenericSansSerif, 12),
				Parent = this
			};

			label1 = new Label()
			{
				Left = listBox1.Left,
				Top = listBox1.Top - 20,
				Text = "Units:",
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.Blue,
				AutoSize = true,
				Parent = this
			};

			button1 = new Button()
			{
				Left = listBox1.Right + 30,
				Top = listBox1.Top,
				Width = 220,
				Text = "New unit",
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.DarkGreen,
				AutoSize = true,
				Parent = this
			};

			button2 = new Button()
			{
				Left = button1.Left,
				Top = button1.Bottom + 10,
				Width = 220,
				Text = "Display the unit",
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.DarkGreen,
				AutoSize = true,
				Parent = this
			};

			button3 = new Button()
			{
				Left = button2.Left,
				Top = button2.Bottom + 10,
				Width = 220,
				Text = "Modify the unit",
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.DarkGreen,
				AutoSize = true,
				Parent = this
			};

			button4 = new Button()
			{
				Left = button3.Left,
				Top = button3.Bottom + 10,
				Width = 220,
				Text = "Delete the unit",
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.DarkGreen,
				AutoSize = true,
				Parent = this
			};
		}
	}
}
