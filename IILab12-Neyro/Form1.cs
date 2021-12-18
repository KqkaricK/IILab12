using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            openFileDialog1.Filter = "Text files (*.txt)|*.txt|All files(*.*)|*.*";
        }

        public int[,] input = new int[3, 5];
        Neyro NW1;

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            openFileDialog1.Filter = "Image files (*.bmp)|*.bmp|All files(*.*)|*.*";
            openFileDialog1.Title = "Укажите тестируемый файл";
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            label1.Text = Path.GetFileName(openFileDialog1.FileName);
            pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);
            Bitmap im = pictureBox1.Image as Bitmap;
            for (var i = 0; i <= 5; i++) listBox1.Items.Add(" ");

            for (var x = 0; x <= 2; x++)
            {
                for (var y = 0; y <= 4; y++)
                {
                    int n = (im.GetPixel(x, y).R);
                    if (n >= 250) n = 0;
                    else n = 1;
                    listBox1.Items[y] = listBox1.Items[y] + "  " + Convert.ToString(n);
                    input[x, y] = n;
                }

            }
            recognize();
        }

        public void recognize()
        {
            NW1.mul_w();
            NW1.Sum();
            if (NW1.Rez()) listBox1.Items.Add(" - True, Sum = "+Convert.ToString(NW1.sum));
            else listBox1.Items.Add( " - False, Sum = "+Convert.ToString(NW1.sum));
        }

        class Neyro
        {
            public int[,] mul;
            public int[,] weight;
            public int[,] input;
            public int limit = 9;
            public int sum ;

            public Neyro(int sizex, int sizey, int[,] inP)
            {
                weight = new int[sizex, sizey];
                mul = new int[sizex, sizey];
                input = new int[sizex, sizey];
                input = inP;
            }

            public void mul_w()
            {
                for (int x = 0; x <= 2; x++)
                {
                    for (int y = 0; y <= 4; y++)
                    {
                        mul[x, y] = input[x,y]*weight[x,y];
                    }
                }
            }
            public void Sum()
            {
                sum = 0;
                for (int x = 0; x <= 2; x++)
                {
                    for (int y = 0; y <= 4; y++)
                    {
                        sum += mul[x, y];
                    }
                }
            }
            public bool Rez()
            {
                if (sum >= limit)
                    return true;
                else return false;
            }
            public void incW(int[,] inP)
            {
                for (int x = 0; x <= 2; x++)
                {
                    for (int y = 0; y <= 4; y++)
                    {
                        weight[x, y] += inP[x, y];
                    }
                }
            }
            public void decW(int[,] inP)
            {
                for (int x = 0; x <= 2; x++)
                {
                    for (int y = 0; y <= 4; y++)
                    {
                        weight[x, y] -= inP[x, y];
                    }
                }
            }

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.RowCount = 5;
            dataGridView1.ColumnCount = 3;
            for (int i = 0; i <= 4; i++)
            {
                for (int j = 0; j <= 2; j++)
                {
                    dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.White;
                }
            }
            dataGridView1.ClearSelection();
            NW1 = new Neyro(3, 5, input);
            openFileDialog1.Title = "Укажите файл весов";
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
            {
                System.Environment.Exit(1);
            }
            string s = openFileDialog1.FileName;
            StreamReader sr = File.OpenText(s);
            string line;
            string[] s1;
            int k = 0;
            while ((line = sr.ReadLine()) != null)
            {
                s1 = line.Split(' ');
                for (int i = 0; i < s1.Length; i++)
                {
                    listBox1.Items.Add("");
                    if (k < 5)
                    {
                        NW1.weight[i, k] = Convert.ToInt32(s1[i]);
                        listBox1.Items[k] += Convert.ToString(NW1.weight[i, k]);
                    }
                }
                k++;
            }
            sr.Close();
            button2.Enabled = false;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            if (NW1.Rez() == false)
                NW1.incW(input);
            else NW1.decW(input);

            string s = "";
            string[] s1 = new string[5];
            System.IO.File.Delete("w.txt");
            FileStream FS = new FileStream("w.txt", FileMode.OpenOrCreate);
            StreamWriter SW = new StreamWriter(FS);
            for (int y = 0; y <= 4; y++)
            {
                s = Convert.ToString(NW1.weight[0, y]) + " " + Convert.ToString(NW1.weight[1, y]) + " " + Convert.ToString(NW1.weight[2, y]);
                s1[y] = s;
                SW.WriteLine(s);
            }
            SW.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (button2.Enabled == true)
            {
                button2.Enabled = false;
            }
            else
            {
                button2.Enabled = true;
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            for (var i = 0; i <= 5; i++) listBox1.Items.Add(" ");
            for (int i = 0; i <= 2; i++)
            {
                for (int j = 0; j <= 4; j++)
                {
                    int n = 0;
                    if (dataGridView1.Rows[j].Cells[i].Style.BackColor == Color.White)
                    {
                        n = 0;
                    }
                    else if (dataGridView1.Rows[j].Cells[i].Style.BackColor == Color.Black)
                    {
                        n = 1;
                    }
                    listBox1.Items[j] = listBox1.Items[j] + "  " + Convert.ToString(n);
                    input[i, j] = n;
                }
            }
            recognize();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor == Color.White)
            {
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Black;
            }
            else if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor == Color.Black)
            {
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.White;
            }
            dataGridView1.ClearSelection();
        }
    }

}
