using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace EtrendKeszito
{
	internal partial class PenznemForm : Form
	{
		public PenznemForm()
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
		private Button button5;

		private void ListBoxRefresh()
		{
			try
			{
				int index = listBox1.SelectedIndex;

				listBox1.DataSource = null;
				listBox1.DataSource = ABKezelo.Kiolvasas().Where(x => x is Penznem).ToList();
				if (index >= 0 && index < listBox1.Items.Count) listBox1.SelectedIndex = index;
				else if (listBox1.Items.Count > 0) listBox1.SelectedIndex = 0;
				else listBox1.SelectedIndex = -1;

				foreach (Menu item in ABKezelo.Kiolvasas().Where(x => x is Menu))
				{
					item.update();
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
			button3.Click += Button3_Click;
			button4.Click += Button4_Click;
			button5.Click += Button5_Click;
		}

		private void Button5_Click(object sender, EventArgs e)
		{
			try
			{// elképzelhető, hogy nincs hálózat

				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(Konstans.ekb_bank_xml_link);

				XmlNodeList nodes = xmlDocument.SelectNodes("//*[@currency]");

				Dictionary<string, double> dict = new Dictionary<string, double>();

				dict.Add("EUR", 1);//triviális árfolyam

				if (nodes != null)
				{
					foreach (XmlNode node in nodes)
					{
						string str = node.Attributes["currency"].Value.ToUpper();
						double arfolyam = Convert.ToDouble(Decimal.Parse(node.Attributes["rate"].Value, NumberStyles.Any, new CultureInfo("en-Us")));
						dict.Add(str, arfolyam);
					}
				}

				if (!dict.ContainsKey("HUF"))
				{
					throw new ArgumentException("Synchronization is unsuccessful!");
				}

				foreach (Penznem item in ABKezelo.Kiolvasas().Where(x => x is Penznem).ToList())
				{
					if (dict.ContainsKey(item.PenzKod))
					{
						item.Arfolyam = dict["HUF"] / dict[item.PenzKod];
						ABKezelo.Modositas(item);
					}
				}

				ListBoxRefresh();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void Button4_Click(object sender, EventArgs e)
		{
			try
			{
				if (listBox1.SelectedIndex != -1)
				{
					Penznem p = (Penznem)listBox1.SelectedItem;
					bool torolheto = true;

					foreach (Elelmiszer item in ABKezelo.Kiolvasas().Where(x => x is Elelmiszer))
					{
						if (item.Penz.Megnevezes == p.Megnevezes)
						{
							torolheto = false;
							break;
						}
					}

					if (!torolheto)
					{
						throw new WarningException("This currency is not deletable, because there is a food using it!");
					}

					EtrendFeltetel et = (EtrendFeltetel)ABKezelo.Kiolvasas().Where(x => x is EtrendFeltetel).ToList().First();

					if (et.Penz.Megnevezes == p.Megnevezes)
					{
						throw new WarningException("This currency is not deletable, bceause Form1 is using it!");
					}

					if (((Penznem)listBox1.SelectedItem).Torolheto)
					{
						if (MessageBox.Show("Really want to delete the currency?", "Question", MessageBoxButtons.YesNo,
								MessageBoxIcon.Question) == DialogResult.Yes)
						{
						    string nev = ((Penznem) listBox1.SelectedItem).Megnevezes;
							ABKezelo.Torol((Penznem)listBox1.SelectedItem);
						    Logolas.Ment("Currency deleted, name: " + nev);
                            ListBoxRefresh();
						}
					}
					else
					{
						throw new WarningException("This currency is not deletable!");
					}
				}
				else
				{
					throw new WarningException("To delete a currency choose one!");
				}
			}
			catch (Exception ex)
			{
				if (ex is WarningException)
				{
					MessageBox.Show(ex.Message, "Attention!", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				else
				{
					MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		private void Button3_Click(object sender, EventArgs e)
		{
			if (listBox1.SelectedIndex != -1)
			{
				try
				{
					UjPenznemForm dialogus = new UjPenznemForm((Penznem)listBox1.SelectedItem);
					if (dialogus.ShowDialog() == DialogResult.OK) ListBoxRefresh();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			else
			{
				MessageBox.Show("To modify a currency choose one!", "Attention!", MessageBoxButtons.OK,
					MessageBoxIcon.Information);
			}
		}

		private void Button2_Click(object sender, EventArgs e)
		{
			if (listBox1.SelectedIndex != -1)
			{
				try
				{
					UjPenznemForm dialogus = new UjPenznemForm((Penznem)listBox1.SelectedItem, true);
					dialogus.ShowDialog();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
			else
			{
				MessageBox.Show("To display a currency choose one!", "Attention!", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}

		}

		private void Button1_Click(object sender, EventArgs e)
		{
			try
			{
				UjPenznemForm dialogus = new UjPenznemForm();
				if(dialogus.ShowDialog()==DialogResult.OK) ListBoxRefresh();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		void VezerlokLetrehozasa()
		{
			Height = 350;
			Width = 620;
			CenterToScreen();
			Text = "Currency form";

			listBox1 = new ListBox()
			{
				Left = 25,
				Top = 50,
				Height = 250,
				Width = 240,
				Font = new Font(FontFamily.GenericSansSerif, 12),
				Parent = this
			};

			label1 = new Label()
			{
				Left = listBox1.Left,
				Top = listBox1.Top - 20,
				Text = "Currencies:",
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.Blue,
				AutoSize = true,
				Parent = this
			};

			button1 = new Button()
			{
				Left = listBox1.Right + 30,
				Top = listBox1.Top,
				Width = 260,
				Text = "New currency",
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.DarkGreen,
				AutoSize = true,
				Parent = this
			};

			button2 = new Button()
			{
				Left = button1.Left,
				Top = button1.Bottom + 10,
				Width = 260,
				Text = "Display the currency",
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.DarkGreen,
				AutoSize = true,
				Parent = this
			};

			button3 = new Button()
			{
				Left = button2.Left,
				Top = button2.Bottom + 10,
				Width = 260,
				Text = "Modify the currency",
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.DarkGreen,
				AutoSize = true,
				Parent = this
			};

			button4 = new Button()
			{
				Left = button3.Left,
				Top = button3.Bottom + 10,
				Width = 260,
				Text = "Delete the currency",
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.DarkGreen,
				AutoSize = true,
				Parent = this
			};

			button5 = new Button()
			{
				Left = button4.Left,
				Top = button4.Bottom + 10,
				Width = 260,
				Text = "Synchronize the exchange rates (EKB)",
				Font = new Font(FontFamily.GenericSansSerif, 12),
				ForeColor = Color.DarkGreen,
				AutoSize = true,
				Parent = this
			};
		}
	}
}
