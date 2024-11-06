using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace lab6
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        List<int> m2;
        int n, e, d;
        public static bool IsPrimeNumber(uint n)
        {
            var result = true;

            if (n > 1)
            {
                for (var i = 2u; i < n; i++)
                {
                    if (n % i == 0)
                    {
                        result = false;
                        break;
                    }
                }
            }
            else
            {
                result = false;
            }

            return result;
        }
        public static int ModPow(int a, int b, int m)
        {
            int res = 1;
            a %= m;
            while (b > 0)
            {
                if ((b & 1) != 0)
                    res = (res * a) % m;
                a = (a * a) % m;
                b >>= 1;
            }
            if (res < 0)
                res += m;
            return res;
        }
        void Generate()
        {
            int n = 500;
            List<int> x = new List<int>();
            for (var i = 0u; i < n; i++)
            {
                if (IsPrimeNumber(i))
                {
                    x.Add(Convert.ToInt32(i));
                }
            }
            Random r = new Random();
            int j,k = 1;
            do
            {
                j = r.Next(x.Count);
                //k = r.Next(x.Count);
            } while (j < 3);
            if (x[j] > 100)
            {
                do
                {
                    k = r.Next(x.Count);
                } while (x[k] > 100);
            }
            else
            {
                do
                {
                    k = r.Next(x.Count);
                } while (x[k] < 100);
            }
            tbp.Text = x[j].ToString();
            tbq.Text = x[k].ToString();
        }
        public static bool AreRelativelyPrime(int a, int b)
        {
            int gcd = 1;
            for (int i = 1; i <= a && i <= b; ++i)
            {
                if (a % i == 0 && b % i == 0)
                    gcd = i;
            }
            return gcd == 1;
        }
        public static int FindD(int p, int q, int e)
        {
            int phi = (p - 1) * (q - 1);
            int d = 0;
            int k = 1;

            while (true)
            {
                d = (k * phi + 1) / e;

                if (d * e == k * phi + 1)
                    return d;

                k++;
            }
        }
        void Calc()
        {
            listBox1.Items.Clear();
            listBox3.Items.Clear();
            Random r = new Random();
            string text = tbtext.Text;
            byte[] hashBytes = new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(text));
            string s = Convert.ToBase64String(hashBytes);
            listBox1.Items.Add("h1 = " + s);
            int p = Convert.ToInt32(tbp.Text);
            int q = Convert.ToInt32(tbq.Text);
            n = p * q;
            int f = (p - 1) * (q - 1);
            List<int> e1 = new List<int>();
            List<int> x = new List<int>();
            bool t;
            for (var i = 0u; i < f; i++)
            {
                if (IsPrimeNumber(i))
                {
                    x.Add(Convert.ToInt32(i));
                    t = AreRelativelyPrime(x[x.Count - 1], f);
                    if (t == true) e1.Add(x[x.Count - 1]);
                    if (e1.Count == 10) break;
                }
            }
            int e2 = r.Next(0, e1.Count - 1);
            e = e1[e2];
            d = FindD(p, q, e);
            m2 = Encrypt_RSA(hashBytes, d, n);
            listBox3.Items.Add("n = " + n.ToString());
            listBox3.Items.Add("fi = " + f.ToString());
            listBox3.Items.Add("e = " + e.ToString());
            listBox3.Items.Add("d = " + d.ToString());
        }
        List<int> Encrypt_RSA(byte[] t1, int d1, int n1)
        {
            byte[] text = t1;
            List<int> c = new List<int>();
            string c1 = "";
            for (int i = 0; i < text.Length; i++)
            {
                c.Add(ModPow(text[i], d1, n1));
                if (i != text.Length - 1) c1 += c[i].ToString() + " ";
                else c1 += c[i].ToString();
            }
            
            int l = (int)(c1.Length / 4.0);
            string firstHalf = c1.Substring(0, l);
            string secondHalf = c1.Substring(l, l);
            string thirdHalf = c1.Substring(2 * l, l);
            string forthHalf = c1.Substring(3 * l);
            listBox1.Items.Add("h_d = " + firstHalf); 
            listBox1.Items.Add(secondHalf);
            listBox1.Items.Add(thirdHalf);
            listBox1.Items.Add(forthHalf);

            return c;
        }

        string Decrypt_RSA(List<int> t1, int e1, int n1)
        {
            byte[] m2 = new byte[t1.Count];
            for (int i = 0; i < t1.Count; i++)
            {
                m2[i] = Convert.ToByte(ModPow(t1[i], e1, n1));
            }
            string s = Convert.ToBase64String(m2);
            listBox2.Items.Add("h_e = " + s);
            return s;
        }

        void Calc2()
        {
            listBox2.Items.Clear();
            string text = tbtext.Text;
            byte[] hashBytes = new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(text));
            string s = Convert.ToBase64String(hashBytes);
            listBox2.Items.Add("h2 = " + s);
            string s1 = Decrypt_RSA(m2, e, n);
            if(s1 == s)
            {
                listBox2.Items.Add("Підпис є вірним");
            }
            else
            {
                listBox2.Items.Add("Підпис не вірним");
            }
        }
        private void btnRand_Click(object sender, EventArgs e)
        {
            Generate();
        }

        private void btnCalc_Click(object sender, EventArgs e)
        {
            Calc();
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            Calc2();
        }
    }
}
