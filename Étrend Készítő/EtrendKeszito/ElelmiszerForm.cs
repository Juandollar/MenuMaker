using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EtrendKeszito
{
	public partial class ElelmiszerForm : Form
	{
		public ElelmiszerForm()
		{
			try
			{
				InitializeComponent();

				VezerlokLetrehozasa();
				AddButtonClick();
				listBoxRefresh();
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
			button5.Click += Button5_Click;
			button6.Click += Button6_Click;
			listBox1.SelectedIndexChanged += ListBox1_SelectedIndexChanged;
			listBox2.SelectedIndexChanged += ListBox2_SelectedIndexChanged;
		}

		private void ListBox2_SelectedIndexChanged(object sender, EventArgs e)
		{
			int index = listBox2.SelectedIndex;
			if (index != -1)
			{
				listBox1.SelectedIndex = -1;
				listBox2.SelectedIndex = index;
			}
		}

		private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			int index = listBox1.SelectedIndex;
			if (index != -1)
			{
				listBox2.SelectedIndex = -1;
				listBox1.SelectedIndex = index;
			}
		}

		private void listBoxRefresh()
		{
			try
			{
				int index1 = listBox1.SelectedIndex;
				int index2 = listBox2.SelectedIndex;

				List<Elelmiszer> e1 = new List<Elelmiszer>();
				List<Elelmiszer> e2 = new List<Elelmiszer>();

				foreach (Elelmiszer item in ABKezelo.Kiolvasas().Where(x => x is Elelmiszer))
				{
					if (item is Menu)
					{
						(item as Menu).update();
					}

					if (item.Hasznalhato) e2.Add(item);
					else e1.Add(item);
				}

				listBox1.DataSource = null;
				listBox1.DataSource = e1;

				listBox2.DataSource = null;
				listBox2.DataSource = e2;

				listBox1.SelectedIndex = -1;
				listBox2.SelectedIndex = -1;

				if (index1 >= 0 && index1 < listBox1.Items.Count) listBox1.SelectedIndex = index1;
				else if (index2 >= 0 && index2 < listBox2.Items.Count) listBox2.SelectedIndex = index2;
				else if (listBox1.Items.Count > 0) listBox1.SelectedIndex = 0;
				else if (listBox2.Items.Count > 0) listBox2.SelectedIndex = 0;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void Button6_Click(object sender, EventArgs e)
		{
			try
			{
				if (listBox1.SelectedIndex != -1 || listBox2.SelectedIndex != -1)
				{
					Elelmiszer el;
					if (listBox1.SelectedIndex != -1) el = (Elelmiszer)listBox1.SelectedItem;
					else el = (Elelmiszer)listBox2.SelectedItem;

					bool torolheto = true;
					foreach (Menu item in ABKezelo.Kiolvasas().Where(x => x is Menu))
					{
						if (item.Osszetevo.ContainsKey(el))
						{
							torolheto = false;
							break;
						}
					}

					if (!torolheto)
					{
						throw new WarningException("The food has been used by a menu, so it is not deletable! [delete all menu that is using it or only the food from the menu]");
					}

					if (MessageBox.Show("Really want to delete the food?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
					{
					    string nev = el.Megnevezes;
						ABKezelo.Torol(el);
					    Logolas.Ment("Deleted food, name: " + nev);

                        listBoxRefresh();
					}
				}
				else
				{
					throw new WarningException("For deletion choose a food!");
				}
			}
			catch (Exception ex)
			{
				if (ex is WarningException)
					MessageBox.Show(ex.Message, "Attention!", MessageBoxButtons.OK, MessageBoxIcon.Information);
				else MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void Button5_Click(object sender, EventArgs e)
		{
			if (listBox1.SelectedIndex != -1 || listBox2.SelectedIndex != -1)
			{
				try
				{
					if (listBox1.SelectedIndex != -1)
					{
						UjElelmiszerForm dialogus = new UjElelmiszerForm((Elelmiszer)listBox1.SelectedItem);
						if (dialogus.ShowDialog() == DialogResult.OK)
						{
							listBoxRefresh();
						}
					}
					else
					{
						UjElelmiszerForm dialogus = new UjElelmiszerForm((Elelmiszer)listBox2.SelectedItem);
						if (dialogus.ShowDialog() == DialogResult.OK)
						{
							listBoxRefresh();
						}
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			else
			{
				MessageBox.Show("For modification choose a food!", "Attention!", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		private void Button4_Click(object sender, EventArgs e)
		{
			if (listBox1.SelectedIndex != -1 || listBox2.SelectedIndex != -1)
			{
				try
				{
					if (listBox1.SelectedIndex != -1)
					{
						UjElelmiszerForm dialogus = new UjElelmiszerForm((Elelmiszer)listBox1.SelectedItem, true);
						dialogus.ShowDialog();
					}
					else
					{
						UjElelmiszerForm dialogus = new UjElelmiszerForm((Elelmiszer)listBox2.SelectedItem, true);
						dialogus.ShowDialog();
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			else
			{
				MessageBox.Show("For display choose a food!", "Attention!", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		private void Button3_Click(object sender, EventArgs e)
		{
			try
			{
				UjElelmiszerForm dialogus = new UjElelmiszerForm();

				if (dialogus.ShowDialog() == DialogResult.OK)
				{
					listBoxRefresh();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void Button2_Click(object sender, EventArgs e)
		{
			if (listBox2.SelectedIndex != -1)
			{
				try
				{
					Elelmiszer el = (listBox2.SelectedItem as Elelmiszer);
					el.Hasznalhato = false;
					ABKezelo.Modositas(el);
					listBoxRefresh();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private void Button1_Click(object sender, EventArgs e)
		{
			if (listBox1.SelectedIndex != -1)
			{
				try
				{
					Elelmiszer el = (listBox1.SelectedItem as Elelmiszer);
					el.Hasznalhato = true;
					ABKezelo.Modositas(el);
					listBoxRefresh();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private ListBox listBox1;
		private ListBox listBox2;

		private Label label1;
		private Label label2;

		private Button button1;
		private Button button2;
		private Button button3;
		private Button button4;
		private Button button5;
		private Button button6;

		void VezerlokLetrehozasa()
		{
			Height = 600;
			Width = 1320;
			CenterToScreen();
			Text = "Food form";

			listBox1 = new ListBox()
			{
				Left = 25,
				Top = 50,
				Height = 500,
				Width = 500,
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.Indigo,
				Parent = this
			};

			listBox2 = new ListBox()
			{
				Left = listBox1.Right + 40,
				Top = 50,
				Height = 500,
				Width = 500,
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.Indigo,
				Parent = this
			};

			label1 = new Label()
			{
				Left = listBox1.Left,
				Top = listBox1.Top - 20,
				Text = "Foods (for diets not used only in menus):",
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.Blue,
				AutoSize = true,
				Parent = this
			};

			label2 = new Label()
			{
				Left = listBox2.Left,
				Top = listBox2.Top - 20,
				Text = "Usable for diet:",
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.Blue,
				AutoSize = true,
				Parent = this
			};

			button1 = new Button
			{
				Left = listBox1.Right + 5,
				Top = (listBox1.Top + listBox1.Bottom) / 2 - 40,
				Width = 10,
				Text = ">",
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.DarkGreen,
				AutoSize = true,
				Parent = this
			};

			button2 = new Button
			{
				Left = button1.Left,
				Top = button1.Top + 30,
				Width = 10,
				Text = "<",
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.DarkGreen,
				AutoSize = true,
				Parent = this
			};

			button3 = new Button
			{
				Left = listBox2.Right + 10,
				Top = listBox1.Top,
				Width = 10,
				Text = "New food addition",
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.DarkGreen,
				AutoSize = true,
				Parent = this
			};

			button4 = new Button
			{
				Left = button3.Left,
				Top = button3.Top + 40,
				Width = 10,
				Text = "Food display",
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.DarkGreen,
				Size = button3.Size,
				Parent = this
			};

			button5 = new Button
			{
				Left = button4.Left,
				Top = button4.Top + 40,
				Width = 10,
				Text = "Food modification",
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.DarkGreen,
				Size = button3.Size,
				Parent = this
			};

			button6 = new Button
			{
				Left = button5.Left,
				Top = button5.Top + 40,
				Width = 10,
				Text = "Food deletion",
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.DarkGreen,
				Size = button3.Size,
				Parent = this
			};
		}
	}
}
