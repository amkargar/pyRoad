using System.Data.SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace pyRoad
{
    /// <summary>
    /// Interaction logic for newQuestionDialog.xaml
    /// </summary>
    public partial class newQuestionDialog : Window
    {
        List<string> inputs = new List<string>();
        List<string> outputs = new List<string>();

        public string author;
        public int id;
        public bool isOk = false;

        #region Sqlite
        public string connection = @"Data Source=testData.db";

        SQLiteConnection SQLcon = new SQLiteConnection();
        SQLiteCommand SQLcmd = new SQLiteCommand();
        #endregion

        public newQuestionDialog()
        {
            InitializeComponent();

            SQLite_Initialize();

            listTestcases.Items.Add(new { Icon = "+", Text = "تست کیس جدید", Description = "برای اضافه کردن کلیک کنید", Tag = "new" });
        }

        private void btnTestcaseItem_Click(object sender, RoutedEventArgs e)
        {

            if (((Button)sender).Tag.ToString() == "new")
            {
                newTestcaseDialog tc = new newTestcaseDialog();
                tc.Owner = this;
                tc.ShowDialog();

                if (tc.Inputs != null && tc.Outputs != null)
                {
                    inputs.Add(tc.Inputs);
                    outputs.Add(tc.Outputs);

                    listTestcases.Items.Add(new { Icon = "?", Text = string.Format("تست کیس {0}م", inputs.Count), Description = "", Tag = "" });
                }
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            isOk = false;
            this.Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            string allInputs = "[";
            foreach (string inp in inputs)
            {
                allInputs += inp + "][";
            }
            allInputs = allInputs.Substring(0, allInputs.Length - 2) + "]";

            string allOutputs = "[";
            foreach (string ou in outputs)
            {
                allOutputs += ou + "][";
            }
            allOutputs = allOutputs.Substring(0, allOutputs.Length - 2) + "]";

            SQLcmd.CommandText = String.Format("INSERT INTO Questions VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');", txtTime.Text, txtName.Text, txtText.Text.Replace("'", "\\'"), allInputs, allOutputs, author, 0, "");
            SQLcmd.ExecuteNonQuery();

            isOk = true;
            this.Close();
        }

        private void SQLite_Initialize()
        {
            SQLcon = new SQLiteConnection(connection);
            SQLcon.ConnectionString = connection;
            SQLcon.Open();
            SQLcmd.Connection = SQLcon;
        }
    }
}
