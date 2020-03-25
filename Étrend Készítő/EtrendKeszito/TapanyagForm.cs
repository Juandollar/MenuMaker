using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EtrendKeszito
{
	public partial class TapanyagForm : Form
	{
		internal TapanyagForm()
		{
			InitializeComponent();
			VezerlokLetrehozasa();
			AddButtonClick();
			ListBoxRefresh();
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
				listBox1.DataSource = ABKezelo.Kiolvasas().Where(x => x is Tapanyag).ToList();
				if (index >= 0 && index < listBox1.Items.Count) listBox1.SelectedIndex = index;
				else if (listBox1.Items.Count > 0) listBox1.SelectedIndex = 0;
				else listBox1.SelectedIndex = -1;
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
			button3.Click += Button3_Click;
			button4.Click += Button4_Click;
		}

		private void Button4_Click(object sender, EventArgs e)
		{
			if (listBox1.SelectedIndex != -1)
			{
				if (MessageBox.Show("Really want to delete the nutrient?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					try
					{
						Tapanyag t = (Tapanyag)listBox1.SelectedItem;

						//élelmiszerek dictinoray-ből is törölni kell a tápanyagot
						foreach (Elelmiszer item in ABKezelo.Kiolvasas().Where(x => x is Elelmiszer))
						{
							item.TapanyagTartalom.Remove(t);
							ABKezelo.Modositas(item);
						}

					    string nev = t.Megnevezes;
						ABKezelo.Torol(t);
					    Logolas.Ment("Nutrient deleted, name: " + nev);

                        ListBoxRefresh();
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			}
			else
			{
				MessageBox.Show("To delete a nutrient choose one!", "Attention!", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		private void Button3_Click(object sender, EventArgs e)
		{
			if (listBox1.SelectedIndex != -1)
			{
				try
				{
					UjTapanyagForm dialogus = new UjTapanyagForm((Tapanyag)listBox1.SelectedItem);
					if(dialogus.ShowDialog()==DialogResult.OK) ListBoxRefresh();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			else
			{
				MessageBox.Show("To modify a nutrient choose one!", "Attention!", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		private void Button2_Click(object sender, EventArgs e)
		{
			if (listBox1.SelectedIndex != -1)
			{
				try
				{
					UjTapanyagForm dialogus = new UjTapanyagForm((Tapanyag)listBox1.SelectedItem, true);
					dialogus.ShowDialog();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			else
			{
				MessageBox.Show("To display a nutrient choose one!", "Attention!", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		private void Button1_Click(object sender, EventArgs e)
		{
			try
			{
				UjTapanyagForm dialogus = new UjTapanyagForm();
				if(dialogus.ShowDialog()==DialogResult.OK) ListBoxRefresh();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		void VezerlokLetrehozasa()
		{
			Height = 600;
			Width = 650;
			CenterToScreen();
			Text = "Nutrient form";

			listBox1 = new ListBox()
			{
				Left = 25,
				Top = 50,
				Height = 500,
				Width = 320,
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.Indigo,
				Parent = this
			};

			label1 = new Label()
			{
				Left = listBox1.Left,
				Top = listBox1.Top - 20,
				Text = "Nutrients:",
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
				Text = "New nutrient",
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
				Text = "Display the nutrient",
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
				Text = "Modify the nutrient",
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
				Text = "Delete the nutrient",
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.DarkGreen,
				AutoSize = true,
				Parent = this
			};
		}
	}
}
