using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Data;

namespace WpfQSearchExcel
{
    /// <summary>
    /// QSearchExcel.xaml 的交互逻辑
    /// </summary>
    public partial class QSearchExcel : Window
    {
        protected DataTable m_dt;
        protected string m_current_col = "";
        DataTable destDT;
        protected string m_file = "";

        public QSearchExcel()
        {
            InitializeComponent();
        }

        //打开文件
        private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "*.xls|*.xls|*.*|*.*";

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {

                    //读取Sheet集
                    m_file = openFileDialog.FileName;
                    this.Title = m_file;

                    tableNameComboBox.Items.Clear();
                    string[] tables = ExcelTools.GetExcelSheetNames(m_file);
                    if (tables == null)
                        return;
                    foreach (string t in tables)
                    {
                        tableNameComboBox.Items.Add(t);

                    }
                    //转到 SelectionChanged 事件
                    tableNameComboBox.SelectedIndex = 0;

                    //m_dt = ExcelTools.ExcelToDS(openFileDialog.FileName, tableNameComboBox.Text).Tables[0];
                    //destDT = m_dt.Clone();
                    //m_current_col = "";

                    ////动态添加 列菜单
                    //columnMenuItem.Items.Clear();
                    //foreach (DataColumn col in m_dt.Columns)
                    //{
                    //    System.Windows.Controls.MenuItem menu = new System.Windows.Controls.MenuItem();
                    //    menu.Header = col.ColumnName;
                    //    menu.IsCheckable = true;
                    //    menu.Click += new RoutedEventHandler(menu_Click);
                    //    columnMenuItem.Items.Add(menu);

                    //}

                    //dataGridView1.DataSource = m_dt;
                }
            }
            catch (Exception ex)
            {

                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        void menu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //先取消其他选中列
                System.Windows.Controls.MenuItem menu = e.Source as System.Windows.Controls.MenuItem;
                foreach (System.Windows.Controls.MenuItem item in columnMenuItem.Items)
                {
                    item.IsChecked = false;
                }

                //选中单击列 菜单
                menu.IsChecked = true;
                m_current_col = menu.Header.ToString();
                //更改顶菜单显示
                columnMenuItem.Header = menu.Header;
            }
            catch (Exception ex)
            {

                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //设置Grid奇数行的颜色
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.BackColor = System.Drawing.Color.FromArgb(255, 192, 255, 192);
            dataGridView1.AlternatingRowsDefaultCellStyle = style;

            keyTextbox.Focus();

        }

        private void keyTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {

                if (m_dt == null)
                {
                    System.Windows.MessageBox.Show("请先打开表");
                    return;
                }
                if (m_current_col == "")
                {
                    System.Windows.MessageBox.Show("请先选择搜索列!");
                    return;
                }
                destDT.Clear();
                foreach (DataRow row in m_dt.Rows)
                {
                    //取所选列的内容
                    string col_content = row[m_current_col].ToString();

                    //按简拼查询
                    if (isPyChkbox.IsChecked == true)
                    {
                        string py = PySearch.getSpells(col_content).ToUpper();
                        if (py.StartsWith(keyTextbox.Text.Trim().ToUpper()) == true)
                            destDT.ImportRow(row);
                    }
                    else
                    {
                        if (col_content.StartsWith(keyTextbox.Text.Trim(), StringComparison.OrdinalIgnoreCase) == true)
                            destDT.ImportRow(row);
                    }

                }
                dataGridView1.DataSource = destDT;
            }
            catch (Exception ex)
            {

                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        //注册快捷键

        KeyGesture exitGesture = new KeyGesture(Key.X, ModifierKeys.Control);

        private void tableNameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //if (m_file == null) return;
                if (tableNameComboBox.SelectedValue == null)
                    return;

                m_dt = ExcelTools.ExcelToDS(m_file, tableNameComboBox.SelectedValue.ToString()).Tables[0];
                destDT = m_dt.Clone();
                m_current_col = "";
                keyTextbox.Text = "";
                columnMenuItem.Header = "选择列(_C)";
                //动态添加 列菜单
                columnMenuItem.Items.Clear();
                foreach (DataColumn col in m_dt.Columns)
                {
                    System.Windows.Controls.MenuItem menu = new System.Windows.Controls.MenuItem();
                    menu.Header = col.ColumnName;
                    menu.IsCheckable = true;
                    menu.Click += new RoutedEventHandler(menu_Click);
                    columnMenuItem.Items.Add(menu);

                }

                dataGridView1.DataSource = m_dt;
                keyTextbox.Focus();
            }
            catch (Exception ex)
            {

                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        protected override void OnPreviewKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            e.Handled = true;

            if (exitGesture.Matches(null, e))
            {
                exitMenuItem_Click(this, e);

            }
            else
            {
                e.Handled = false;
            }

        }
        //protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        //{
        //    base.OnKeyDown(e);
        //    if ( exitGesture.Matches(null,e))
        //        exitMenuItem.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent,
        //}


        private void slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (dataGridView1 != null)
            {
                dataGridView1.Font = new System.Drawing.Font("黑体", (float)e.NewValue);

            }
        }

       

        private void aboutMeMenuItem_Click(object sender, RoutedEventArgs e)
        {
           System.Windows.Forms.MessageBox.Show("Author:im@xingquan.org\r\n", "About Me", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void exitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }



    }
}
