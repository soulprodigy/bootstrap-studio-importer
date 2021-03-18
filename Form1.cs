using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bootstrap_Importer
{
    public partial class Form1 : Form
    {
        Dictionary<string, string> directories = new Dictionary<string, string>();
        List<string> files = new List<string>();
        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            button.DragOver += new DragEventHandler(_DragOver);
            button.MouseDown += new MouseEventHandler(_MouseDown);
            this.SizeChanged += (s, e) => autoresize();
            listView1.ControlAdded += (s, e) => autoresize(false);
            listView2.ControlAdded += (s, e) => autoresize(false);
        }

        /// <summary>
        /// autoresize its a item listener that resizes listview controls.
        /// </summary>
        public void autoresize(bool both = true)
        {
            foreach (ColumnHeader cole in listView1.Columns)
            {
                cole.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
            foreach (ColumnHeader cole in listView2.Columns)
            {
                cole.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
        }

        void _DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        void _MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                List<string> _files = new List<string>();
                foreach (ListViewItem lvi in listView1.CheckedItems) //files
                {
                    _files.Add(lvi.SubItems[1].Text);
                }
                foreach (ListViewItem lvi in listView2.CheckedItems) //folders
                {
                    foreach (string item in scandirfiles(lvi.SubItems[1].Text)) {
                        if (!_files.Contains(item))
                        {
                            _files.Add(item);
                        }
                    }
                    
                }
                button.DoDragDrop(new DataObject(DataFormats.FileDrop, _files.ToArray()), DragDropEffects.Copy);
            }
            catch
            {

            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            files.Clear();
            directories.Clear();
            listView1.Items.Clear(); //files
            listView2.Items.Clear(); //directories
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    string path = fbd.SelectedPath;
                    textBox1.Text = path;
                    string[] results = scandirfiles(path);
                    files.AddRange(results);
                    int i = 0;
                    //import to listview
                    foreach (string file in results)
                    {
                        string smartdir = file.Replace(path, ""); //remove root directory from project.
                        while (smartdir.StartsWith('/'))
                        {
                            int offset = 1; //character offset
                            smartdir = smartdir.Substring(offset, smartdir.Length - offset);
                        }
                        smartdir = smartdir.Replace(Path.GetFileName(file), "");

                        ListViewItem lvi = new ListViewItem();
                        lvi.Name = "_"+(i++).ToString();
                        lvi.Text = Path.GetFileName(file);
                        lvi.Checked = false;
                        lvi.SubItems.Add(file);
                        lvi.SubItems.Add(file);
                        listView1.Items.Add(lvi);

                        //fix import bugging
                        if (directories.ContainsKey(smartdir))
                        {
                            string value = directories.GetValueOrDefault(smartdir);


                        } else
                        {
                            directories.Add(smartdir, Path.GetDirectoryName(file));
                        }
                    }
                }
                autoresize();
            }

            foreach (KeyValuePair<string, string> directory in directories.Distinct().ToArray())
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Name = "Bootstrap Component";
                lvi.Text = directory.Key;
                lvi.Checked = true;
                lvi.SubItems.Add(directory.Value);
                listView2.Items.Add(lvi);
            }
        }

        private string[] scandirfiles(string path)
        {
            List<string> results = new List<string>();
            foreach (string file in Directory.GetFiles(path))
            {
                results.Add(file);
            }

            foreach (string dir in Directory.GetDirectories(path))
            {
                results.AddRange(scandirfiles(dir));
            }

            return results.ToArray();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0) //folders
            {
                foreach (ListViewItem item in listView2.Items)
                {
                    item.Checked = true;
                }
            }

            if (tabControl1.SelectedIndex == 1) //files
            {
                foreach (ListViewItem item in listView1.Items)
                {
                    item.Checked = true;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0) //folders
            {
                foreach (ListViewItem item in listView2.Items)
                {
                    item.Checked = false;
                }
            }

            if (tabControl1.SelectedIndex == 1) //files
            {
                foreach (ListViewItem item in listView1.Items)
                {
                    item.Checked = false;
                }
            }
        }
    }


}
