using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace tdb_Tool
{
    struct TDBItem
    {
        public int Row;
        public string[] Items;
    }

    public partial class Form1 : Form
    {
        List<TDBItem> FileText;
        string FileName;
        byte[] signature;
        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

            OpenFileDialog browseFile = new OpenFileDialog();
            browseFile.Filter = "Soldiers' Soul tdb (*.tdb)|*.tdb";
            browseFile.Title = "Browse for tdb File";
            if (browseFile.ShowDialog() == DialogResult.Cancel)
                return;

            FileName = browseFile.FileName;
            FileText = new List<TDBItem>();
            using (BinaryReader tdbfile = new BinaryReader(File.Open(browseFile.FileName, FileMode.Open)))
            {
                //tdbfile.BaseStream.Seek(8, SeekOrigin.Begin);
                signature = tdbfile.ReadBytes(8);
                int itemCount = tdbfile.ReadInt32();
             

                tdbfile.BaseStream.Seek(16, SeekOrigin.Begin);
                int ReadLength;
                int j = 0;
                TDBItem tdbi;
                while (tdbfile.BaseStream.Position != tdbfile.BaseStream.Length)
                {
                    tdbi = new TDBItem();
                    tdbi.Row = tdbfile.ReadInt32();
                    tdbi.Items = new string[itemCount];
                    for (int i = 0; i < itemCount; i++)
                    {
                        ReadLength = tdbfile.ReadInt32();
                        byte[] textdata = tdbfile.ReadBytes(ReadLength);
                        byte[] aTxtData = new byte[textdata.Length - 2];
                        Array.Copy(textdata, aTxtData, textdata.Length - 2);
                        tdbi.Items[i] = System.Text.Encoding.Unicode.GetString(aTxtData);
                        
                    }

                    j++;

                    FileText.Add(tdbi);
                }

                
                
                //cbLang.Items.Clear();
                //for (int i = 0; i < itemCount; i++)
                //{
                //    cbLang.Items.Add(i);
                //}
                
                cbList.Items.Clear();
                for (int i = 0; i < FileText.Count; i++)
                {
                    cbList.Items.Add(FileText[i].Items[1]);
                }
            }
        }

        private void cbList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                txtText.Text = FileText[cbList.SelectedIndex].Items[cbLang.SelectedIndex];
            }
            catch { };
        }

        private void cbLang_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                txtText.Text = FileText[cbList.SelectedIndex].Items[cbLang.SelectedIndex];
                
            }
            catch { };

            try
            {
                
                cbList.Items.Clear();
                for (int i = 0; i < FileText.Count; i++)
                {
                    cbList.Items.Add(FileText[i].Items[cbLang.SelectedIndex]);
                }
            }
            catch { };
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (BinaryWriter tdbfile = new BinaryWriter(File.Open(FileName, FileMode.Create)))
            {
                tdbfile.Write(signature);
                tdbfile.Write(FileText[0].Items.Length);
                tdbfile.Write((int)1);
                
                for (int i = 0; i < FileText.Count; i++)
                {
                    tdbfile.Write(FileText[i].Row);

                    for (int j = 0; j < FileText[i].Items.Length; j++)
                    {
                        //tdbfile.Write((int)(FileText[i].Items[j].Length * 2));
                        byte[] tBytes = Encoding.Unicode.GetBytes(FileText[i].Items[j]);
                        tdbfile.Write(tBytes.Length + 2);
                        tdbfile.Write(tBytes);
                        tdbfile.Write(new byte[] { 0x00, 0x00 });
                    }
                }
            }
        }

        private void txtText_TextChanged(object sender, EventArgs e)
        {
            FileText[cbList.SelectedIndex].Items[cbLang.SelectedIndex] = txtText.Text;
        }

        private void japanesePatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string temp = "";
            for (int i = 0; i < FileText.Count; i++)
            {
                temp = FileText[i].Items[0];
                FileText[i].Items[0] = FileText[i].Items[7];
                FileText[i].Items[7] = temp;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
