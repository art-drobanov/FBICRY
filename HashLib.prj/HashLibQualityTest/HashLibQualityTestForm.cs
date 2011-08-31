using System;
using System.Linq;
using System.Collections.Generic;
using HashLibQualityTest.Configurations;
using System.Windows.Forms;
using HashLibQualityTest.DataSourceRows;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace HashLibQualityTest
{
    public partial class HashLibQualityTestForm : Form
    {
        public List<AvalancheTestDataSourceRow> m_avalancheDataSource = 
            new List<AvalancheTestDataSourceRow>();

        public List<AvalancheTestDataSourceRow> m_avalancheCryptoDataSource =
            new List<AvalancheTestDataSourceRow>();

        public List<SpeedTestDataSourceRow> m_speedTestDataSource =
            new List<SpeedTestDataSourceRow>();

        public List<SpeedTestDataSourceRow> m_speedTestCryptoDataSource =
            new List<SpeedTestDataSourceRow>();

        public List<CalculatorDataSourceRow> m_calculatorDataSource =
            new List<CalculatorDataSourceRow>();

        public List<CalculatorDataSourceRow> m_calculatorCryptoDataSource =
            new List<CalculatorDataSourceRow>();

        public List<CalculatorDataSourceRow> m_calculatorHMACDataSource =
            new List<CalculatorDataSourceRow>();

        public HashLibQualityTestForm()
        {
            #if !DEBUG
            Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)1;
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            #endif

            InitializeComponent();

            FormState.Instance.Init(this);
        }

        private void HashLibQualityTestForm_Load(object sender, EventArgs e)
        {
            string mode = (IntPtr.Size == 8) ? " (64bit)" : " (32bit)";
            Text = Text + mode;

            speedTestDataGridView.AutoGenerateColumns = false;
            speedTestCryptoDataGridView.AutoGenerateColumns = false;
            avalancheDataGridView.AutoGenerateColumns = false;
            avalancheCryptoDataGridView.AutoGenerateColumns = false;
            calculatorDataGridView.AutoGenerateColumns = false;
            calculatorHMACDataGridView.AutoGenerateColumns = false;
            calculatorCryptoDataGridView.AutoGenerateColumns = false;

            m_avalancheDataSource = (from f in HashesList.AvalancheList
                                     select new AvalancheTestDataSourceRow(f.Name)
                                     {
                                         HashFunction = f
                                     }).ToList();

            avalancheDataGridView.DataSource = m_avalancheDataSource;

            m_avalancheCryptoDataSource = (from f in HashesList.AvalancheListCrypto
                                           select new AvalancheTestDataSourceRow(f.Name)
                                           {
                                               HashFunction = f
                                           }).ToList();

            avalancheCryptoDataGridView.DataSource = m_avalancheCryptoDataSource;

            m_speedTestDataSource = (from f in HashesList.SpeedList
                                     select new SpeedTestDataSourceRow()
                                     {
                                         Algorithm = f.Name,
                                         HashFunction = f
                                     }).ToList();

            foreach (SpeedTestDataSourceRow row in m_speedTestDataSource)
            {
                if (SpeedTests.Instance.List[row.Algorithm] != null)
                    SpeedTests.Instance.List[row.Algorithm].CopyTo(row);
            }

            speedTestDataGridView.DataSource = m_speedTestDataSource;

            m_speedTestCryptoDataSource = (from f in HashesList.SpeedListCrypto
                                           select new SpeedTestDataSourceRow()
                                           {
                                               Algorithm = f.Name,
                                               HashFunction = f
                                           }).ToList();

            foreach (SpeedTestDataSourceRow row in m_speedTestCryptoDataSource)
            {
                if (SpeedTests.Instance.List[row.Algorithm] != null)
                    SpeedTests.Instance.List[row.Algorithm].CopyTo(row);
            }

            speedTestCryptoDataGridView.DataSource = m_speedTestCryptoDataSource;

            m_calculatorDataSource = (from f in HashesList.CalculatorList
                                      select new CalculatorDataSourceRow()
                                      {
                                          Algorithm = f.Name,
                                          HashFunction = f
                                      }).ToList();

            calculatorDataGridView.DataSource = m_calculatorDataSource;

            m_calculatorCryptoDataSource = (from f in HashesList.CalculatorListCrypto
                                            select new CalculatorDataSourceRow()
                                            {
                                                Algorithm = f.Name,
                                                HashFunction = f
                                            }).ToList();

            calculatorCryptoDataGridView.DataSource = m_calculatorCryptoDataSource;

            m_calculatorHMACDataSource = (from f in HashesList.CalculatorListHMAC
                                          select new CalculatorDataSourceRow()
                                          {
                                              Algorithm = f.Name,
                                              HashFunction = f
                                          }).ToList();

            calculatorHMACDataGridView.DataSource = m_calculatorHMACDataSource;

            hashFileTextBox.Text = LastOpenFiles.Instance.HashFile;
            hashCryptoFileTextBox.Text = LastOpenFiles.Instance.HashFileCrypto;
            hashHMACFileTextBox.Text = LastOpenFiles.Instance.HashFileCrypto;

            hashTextBox.Text = LastOpenFiles.Instance.HashText;
            hashCryptoTextBox.Text = LastOpenFiles.Instance.HashTextCrypto;
            hashHMACTextBox.Text = LastOpenFiles.Instance.HashTextHMAC;

            keyTextBox.Text = LastOpenFiles.Instance.HashKey;

            tabControl.SelectedIndex = FormState.Instance.LastTabIndex;
        }

        private void speedTestbutton_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                foreach (SpeedTestDataSourceRow row in m_speedTestDataSource)
                {
                    if (row.Calculate)
                        new SpeedTest().Test(row);

                    SpeedTests.Instance.List.Add(row);
                }
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                System.Media.SystemSounds.Beep.Play();
                speedTestDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                speedTestDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                speedTestDataGridView.Invalidate();
            }
        }

        private void avalancheTestButton_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                foreach (AvalancheTestDataSourceRow row in m_avalancheDataSource)
                    if (row.Calculate)
                    {
                        new AvalancheTest().Test(row);
                        row.Save();
                    }
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                System.Media.SystemSounds.Beep.Play();
                avalancheDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                avalancheDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                avalancheDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                avalancheDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                avalancheDataGridView.Invalidate();
            }
        }

        private void speedTestDataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridViewColumnHeaderMouseClick(m_speedTestDataSource, e, speedTestDataGridView);
        }

        private void avalancheDataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridViewColumnHeaderMouseClick(m_avalancheDataSource, e, avalancheDataGridView);
        }

        private void calculatorDataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridViewColumnHeaderMouseClick(m_calculatorDataSource, e, calculatorDataGridView);
        }

        private void DataGridViewColumnHeaderMouseClick<T>(
            List<T> a_dataSource, DataGridViewCellMouseEventArgs e, DataGridView a_dataGridView) where T : TestDataSourceRow
        {
            a_dataGridView.EndEdit();

            if (e.ColumnIndex == 0)
            {
                if (a_dataSource.All(r => r.Calculate))
                    a_dataSource.ForEach(r => r.Calculate = false);
                else 
                    a_dataSource.ForEach(r => r.Calculate = true);
            }
            else
            {
                if (a_dataGridView.Columns[e.ColumnIndex].SortMode != DataGridViewColumnSortMode.NotSortable)
                {
                    a_dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection =
                        a_dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection ==
                            SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;

                    List<object> tosort = new List<object>();
                    for (int row = 0; row < a_dataGridView.Rows.Count; row++)
                        tosort.Add(a_dataGridView.Rows[row].Cells[e.ColumnIndex].Value);

                    Sort(a_dataSource, tosort, a_dataGridView.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection);
                }
            }

            a_dataGridView.Invalidate();
        }

        private void Sort<T>(List<T> a_list, List<object> a_sort, SortOrder a_sortOrder)
        {
            if (a_sortOrder == SortOrder.Ascending)
            {
                List<T> l =  a_list.Select((obj, index) => new { obj = a_list[index], sort_obj = a_sort[index] }).
                    OrderBy(gr => gr.sort_obj).Select(gr => gr.obj).ToList();
                a_list.Clear();
                a_list.AddRange(l);
            }
            else
            {
                List<T> l = a_list.Select((obj, index) => new { obj = a_list[index], sort_obj = a_sort[index] }).
                    OrderByDescending(gr => gr.sort_obj).Select(gr => gr.obj).ToList();
                a_list.Clear();
                a_list.AddRange(l);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                foreach (CalculatorDataSourceRow row in m_calculatorDataSource)
                    if (row.Calculate)
                        row.CalculateHashFromText(hashTextBox.Text);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                calculatorDataGridView.Invalidate();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (!new FileInfo(hashFileTextBox.Text).Exists)
                {
                    System.Media.SystemSounds.Exclamation.Play();
                    return;
                }
                if (new DirectoryInfo(hashFileTextBox.Text).Exists)
                {
                    System.Media.SystemSounds.Exclamation.Play();
                    return;
                }
            }
            catch
            {
                System.Media.SystemSounds.Exclamation.Play();
                return;
            }

            Cursor.Current = Cursors.WaitCursor;
            try
            {
                foreach (CalculatorDataSourceRow row in m_calculatorDataSource)
                    if (row.Calculate)
                        row.CalculateHashFromFile(hashFileTextBox.Text);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                System.Media.SystemSounds.Beep.Play();
                calculatorDataGridView.Invalidate();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LastOpenFiles.Instance.Check();

            openFileDialog.InitialDirectory = LastOpenFiles.Instance.HashFileDir;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;

                if (fileName.Equals(""))
                    return;


                LastOpenFiles.Instance.HashFile = fileName;
                openFileDialog.FileName = new FileInfo(fileName).Name;

                hashFileTextBox.Text = fileName;
            }
        }

        private void hashTextBox_TextChanged(object sender, EventArgs e)
        {
            LastOpenFiles.Instance.HashText = hashTextBox.Text;
        }

        private void hashFileTextBox_TextChanged(object sender, EventArgs e)
        {
            LastOpenFiles.Instance.HashFile = hashFileTextBox.Text;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            FormState.Instance.LastTabIndex = tabControl.SelectedIndex;
        }

        private void speedTestCryptoDataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridViewColumnHeaderMouseClick(m_speedTestCryptoDataSource, e, speedTestCryptoDataGridView);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                foreach (SpeedTestDataSourceRow row in m_speedTestCryptoDataSource)
                {
                    if (row.Calculate)
                        new SpeedTest().Test(row);

                    SpeedTests.Instance.List.Add(row);
                }
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                System.Media.SystemSounds.Beep.Play();
                speedTestCryptoDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                speedTestCryptoDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                speedTestCryptoDataGridView.Invalidate();
            }
        }

        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridViewColumnHeaderMouseClick(m_avalancheCryptoDataSource, e, avalancheCryptoDataGridView);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                foreach (AvalancheTestDataSourceRow row in m_avalancheCryptoDataSource)
                    if (row.Calculate)
                    {
                        new AvalancheTest().Test(row);
                        row.Save();
                    }
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                System.Media.SystemSounds.Beep.Play();
                avalancheCryptoDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                avalancheCryptoDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                avalancheCryptoDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                avalancheCryptoDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                avalancheCryptoDataGridView.Invalidate();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            LastOpenFiles.Instance.HashFileCrypto = hashCryptoFileTextBox.Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            LastOpenFiles.Instance.HashTextCrypto = hashCryptoTextBox.Text;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            LastOpenFiles.Instance.HashTextHMAC = hashHMACTextBox.Text;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            LastOpenFiles.Instance.Check();

            openFileDialog.InitialDirectory = LastOpenFiles.Instance.HashFileDirCrypto;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;

                if (fileName.Equals(""))
                    return;

                LastOpenFiles.Instance.HashFileCrypto = fileName;
                openFileDialog.FileName = new FileInfo(fileName).Name;

                hashCryptoFileTextBox.Text = fileName;
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            LastOpenFiles.Instance.Check();

            openFileDialog.InitialDirectory = LastOpenFiles.Instance.HashFileDirHMAC;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog.FileName;

                if (fileName.Equals(""))
                    return;

                LastOpenFiles.Instance.HashFileHMAC = fileName;
                openFileDialog.FileName = new FileInfo(fileName).Name;

                hashHMACFileTextBox.Text = fileName;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                if (!new FileInfo(hashCryptoFileTextBox.Text).Exists)
                {
                    System.Media.SystemSounds.Exclamation.Play();
                    return;
                }
                if (new DirectoryInfo(hashCryptoFileTextBox.Text).Exists)
                {
                    System.Media.SystemSounds.Exclamation.Play();
                    return;
                }
            }
            catch
            {
                System.Media.SystemSounds.Exclamation.Play();
                return;
            }

            Cursor.Current = Cursors.WaitCursor;
            try
            {
                foreach (CalculatorDataSourceRow row in m_calculatorCryptoDataSource)
                    if (row.Calculate)
                        row.CalculateHashFromFile(hashCryptoFileTextBox.Text);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                System.Media.SystemSounds.Beep.Play();
                calculatorCryptoDataGridView.Invalidate();
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            try
            {
                if (!new FileInfo(hashHMACFileTextBox.Text).Exists)
                {
                    System.Media.SystemSounds.Exclamation.Play();
                    return;
                }
                if (new DirectoryInfo(hashHMACFileTextBox.Text).Exists)
                {
                    System.Media.SystemSounds.Exclamation.Play();
                    return;
                }
            }
            catch
            {
                System.Media.SystemSounds.Exclamation.Play();
                return;
            }

            Cursor.Current = Cursors.WaitCursor;
            try
            {
                foreach (CalculatorDataSourceRow row in m_calculatorHMACDataSource)
                    if (row.Calculate)
                        row.CalculateHMACFromFile(hashHMACFileTextBox.Text, keyTextBox.Text);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                System.Media.SystemSounds.Beep.Play();
                calculatorHMACDataGridView.Invalidate();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                foreach (CalculatorDataSourceRow row in m_calculatorCryptoDataSource)
                    if (row.Calculate)
                        row.CalculateHashFromText(hashCryptoTextBox.Text);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                calculatorCryptoDataGridView.Invalidate();
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                foreach (CalculatorDataSourceRow row in m_calculatorHMACDataSource)
                    if (row.Calculate)
                        row.CalculateHMACFromText(hashHMACTextBox.Text, keyTextBox.Text);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                calculatorHMACDataGridView.Invalidate();
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            LastOpenFiles.Instance.HashFileHMAC = hashHMACFileTextBox.Text;
        }

        private void keyTextBox_TextChanged(object sender, EventArgs e)
        {
            LastOpenFiles.Instance.HashKey = keyTextBox.Text;
        }

        private void calculatorCryptoDataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridViewColumnHeaderMouseClick(m_calculatorCryptoDataSource, e, calculatorCryptoDataGridView);
        }

        private void calculatorHMACDataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridViewColumnHeaderMouseClick(m_calculatorHMACDataSource, e, calculatorHMACDataGridView);
        }

        private void hashTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                button4_Click(sender, e);
        }

        private void hashCryptoTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                button6_Click(sender, e);
        }

        private void hashHMACTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                button9_Click(sender, e);
        }

        private void keyTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                button9_Click(sender, e);
        }
    }
}