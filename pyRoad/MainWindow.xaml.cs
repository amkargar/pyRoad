using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SQLite;

namespace pyRoad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region FontIcons
        public const string LightbulbQuestionOutline = "\U000f19e4";
        public const string LightbulbOutline = "\U000f0336";
        #endregion

        #region App Settings
        string applicationDirectory = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);

        public string name, selectedQs;

        int selectedTab;
        string selectedFileName;
        string selectedFilePath;

        Dictionary<string, object> selectedQuestion;
        #endregion

        #region Sqlite
        String connection = "";

        SQLiteConnection SQLcon = new SQLiteConnection();
        SQLiteCommand SQLcmd = new SQLiteCommand();

        String connection2 = "";
        SQLiteConnection SQLcon2 = new SQLiteConnection();
        SQLiteCommand SQLcmd2 = new SQLiteCommand();
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            selectTab(0);

            SQLite_Initialize();
            SQLite_Initialize2();

            load_python();
            load_account();
            load_Questions();
            load_selectedQuestions();
        }

        #region Tabs

        public void selectTab(int tabIndex)
        {
            selectedTab = tabIndex;

            Border[] allTabs = { brQuestionsTab, brSelectedQuestionsTab, brNewQuestionsTab, brSettingsTab, brAboutUsTab };
            Button[] allTabButtons = { btnQuestionsTab, btnSelectedQuestionsTab, btnNewQuestionsTab, btnSettingsTab, btnAboutUsTab };
            Border[] allTabSelectors = { brQuestionsTabSelector, brSelectedQuestionsTabSelector, brNewQuestionsTabSelector, brSettingsTabSelector, brAboutUsTabSelector };

            foreach (Border b in allTabs)
            {
                if ((allTabs.ToList()).IndexOf(b) != tabIndex)
                {
                    b.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00000000"));
                    allTabSelectors[allTabs.ToList().IndexOf(b)].Visibility = Visibility.Hidden;
                    allTabButtons[allTabs.ToList().IndexOf(b)].Tag = false;
                }
                else
                {
                    b.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#0A000000"));
                    allTabSelectors[allTabs.ToList().IndexOf(b)].Visibility = Visibility.Visible;
                    allTabButtons[allTabs.ToList().IndexOf(b)].Tag = true;
                }
            }
        }

        private void btnQustionsTab_Click(object sender, RoutedEventArgs e)
        {
            goBack(sender, e);

            setTitle("پای روود > همه ی سوالات");
            selectTab(0);
        }

        private void btnSelectedQuestionsTab_Click(object sender, RoutedEventArgs e)
        {
            goBack(sender, e);

            setTitle("پای روود > سوالات شرکت شده");
            selectTab(1);
        }

        private void btnNewQuestionsTab_Click(object sender, RoutedEventArgs e)
        {
            goBack(sender, e);

            setTitle("پای روود > سوالات جدید");
            selectTab(2);
        }

        private void btnSettingsTab_Click(object sender, RoutedEventArgs e)
        {
            goBack(sender, e);

            setTitle("پای روود > تنظیمات");
            selectTab(3);
        }

        private void btnAboutUsTab_Click(object sender, RoutedEventArgs e)
        {
            goBack(sender, e);

            setTitle("پای روود > راهنما");
            selectTab(4);
        }

        private void btnQuestionItemBack_Click(object sender, RoutedEventArgs e)
        {
            btnQuestionItemBack.Tag = false;
            setTitle("پای روود > همه ی سوالات");
        }

        #endregion


        #region Questions

        private void btnAddNewQuestions_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "Python files (.pyroad)|*.pyroad";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                connection2 = String.Format(@"Data Source={0}", dialog.FileName);
                SQLcon2 = new SQLiteConnection(connection2);
                SQLcon2.ConnectionString = connection2;
                SQLcon2.Open();
                SQLcmd2.Connection = SQLcon2;

                SQLcmd2.CommandText = String.Format("SELECT time, title, body, inputs, outputs, author, progress, code FROM 'Questions'");
                SQLiteDataReader SQLdr = SQLcmd2.ExecuteReader();
                while (SQLdr.Read())
                {
                    SQLcmd.CommandText = String.Format("INSERT into Questions(time, title, body, inputs, outputs, author, progress, code) values('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');", SQLdr.GetInt32(0), SQLdr.GetString(1), SQLdr.GetString(2), SQLdr.GetString(3), SQLdr.GetString(4), SQLdr.GetString(5), SQLdr.GetInt32(6), SQLdr.GetString(7));
                    SQLcmd.ExecuteNonQuery();
                }

                SQLdr.Close();

                load_Questions();
                SQLite_Initialize2();
            }
        }

        #endregion

        #region Question Item

        private void btnQuestionsItem_Click(object sender, RoutedEventArgs e)
        {
            btnQuestionItemBack.Tag = true;
            selectedQuestion = (Dictionary<string, object>)((Button)sender).Tag;

            progQuestionItem.Value = int.Parse(selectedQuestion["progress"].ToString());

            lblQuestionTitle.Content = selectedQuestion["title"];
            lblQuestionAuthor.Content = string.Format("طراح: {0}", selectedQuestion["author"]);
            lblQuestionTime.Content = string.Format("زمان مورد انتظار: {0} ثانیه", toPersian((int.Parse(selectedQuestion["time"].ToString()) / 1000).ToString()));

            txtQuestionBody.Text = selectedQuestion["body"].ToString();

            setTitle(string.Format("پای روود > همه ی سوالات > {0}", selectedQuestion["title"]));
        }

        private void btnQuestionItemFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "Python files (.py)|*.py";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                Mouse.OverrideCursor = Cursors.Wait;

                selectedFilePath = dialog.FileName;
                selectedFileName = selectedFilePath.Substring(selectedFilePath.LastIndexOf("\\") + 1);

                string args = string.Format(@"{0}\testPythonCode.py {1} {2} {3} {4}", applicationDirectory, selectedFilePath.Replace("\\", "/"), selectedQuestion["time"], selectedQuestion["inputs"], selectedQuestion["outputs"]);
                string[] results = openApp("Python", args).Split(' ');

                Mouse.OverrideCursor = null;
                codeOutput co = new codeOutput();
                co.Owner = this;
                co.Results = results;
                co.ShowDialog();

                int progress = results.Where(c => c.Contains("True")).ToArray().Length / results.Length * 100;
                progQuestionItem.Value = progress;

                SQLcmd.CommandText = String.Format("UPDATE Questions SET progress = {0}, code = '{1}' WHERE id = {2};", progress, File.ReadAllText(selectedFilePath), selectedQuestion["id"]);
                SQLcmd.ExecuteNonQuery();

                SQLcmd.CommandText = String.Format("UPDATE Account SET selectedQs = '{0}' WHERE name = '{1}';", selectedQs + ", " + selectedQuestion["id"], lblName.Content);
                SQLcmd.ExecuteNonQuery();

                reload_SelectedQuestion();

                load_account();
                load_Questions();
                load_selectedQuestions();
            }
        }

        private void btnQuestionItemCode_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.Filter = "Python files (.py)|*.py";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                File.WriteAllText(dialog.FileName, selectedQuestion["code"].ToString());
                openApp("Notepad.exe", dialog.FileName);
            }

        }

        #endregion

        #region NewQuestions Tab

        private void btnNewQuestionExport_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.Filter = "Pyroad files (.pyroad)|*.pyroad";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
                if (File.Exists(dialog.FileName))
                {
                    File.Delete(dialog.FileName);
                }

                File.Copy(@"testData.db", dialog.FileName);
            }
        }

        private void btnNewQuestion_Click(object sender, RoutedEventArgs e)
        {
            newQuestionDialog nq = new newQuestionDialog();
            nq.Owner = this;
            nq.author = name;
            nq.ShowDialog();

            if (nq.isOk)
            {
                listNewQuestions.Items.Add(new { Icon = "?", Text = string.Format("سوال {0}م", listNewQuestions.Items.Count + 1), Description = "سوال اضافه شده است" });
            }

        }

        #endregion

        #region Settings Tab

        private void txtName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SQLcmd.CommandText = String.Format("UPDATE Account SET name = '{0}';", txtName.Text);
                SQLcmd.ExecuteNonQuery();

                lblName.Content = txtName.Text;
            }
        }

        #endregion


        #region Load Datas

        private void load_python()
        {
            lblPythonVersion.Content = openApp("Python", "--version");
            lblPythonPath.Content = openApp("Python", "-c \"import sys; print(sys.executable)\"");
        }

        private void load_account()
        {
            SQLcmd.CommandText = String.Format("SELECT * FROM 'Account'");
            SQLiteDataReader SQLdr = SQLcmd.ExecuteReader();
            while (SQLdr.Read())
            {
                name = SQLdr.GetString(0);
                selectedQs = SQLdr.GetString(1);

                txtName.Text = name;
                lblName.Content = name;
            }
            SQLdr.Close();
        }

        private void load_Questions()
        {
            listQuestions.Items.Clear();

            SQLcmd.CommandText = String.Format("SELECT * FROM 'Questions'");
            SQLiteDataReader SQLdr = SQLcmd.ExecuteReader();
            while (SQLdr.Read())
            {
                Dictionary<string, object> question = new Dictionary<string, object>();

                for (int col = 0; col < SQLdr.FieldCount; col++)
                {
                    question.Add(SQLdr.GetName(col), SQLdr.GetValue(col));
                }

                string ic = LightbulbQuestionOutline;
                if (int.Parse(question["progress"].ToString()) == 100)
                {
                    ic = LightbulbOutline;
                }

                listQuestions.Items.Add(new { Icon = ic, Text = SQLdr.GetString(2), Description = SQLdr.GetString(6), Tag = question, Progress = SQLdr.GetInt32(7) });
            }
            SQLdr.Close();
        }

        private void load_selectedQuestions()
        {
            listSelectedQuestions.Items.Clear();

            Dictionary<string, object> question = new Dictionary<string, object>();

            SQLcmd.CommandText = String.Format("SELECT * FROM 'Questions' WHERE id IN ({0})", selectedQs);
            SQLiteDataReader SQLdr = SQLcmd.ExecuteReader();
            while (SQLdr.Read())
            {
                for (int col = 0; col < SQLdr.FieldCount; col++)
                {
                    question.Add(SQLdr.GetName(col), SQLdr.GetValue(col));
                }

                string ic = LightbulbQuestionOutline;
                if (int.Parse(question["progress"].ToString()) == 100)
                {
                    ic = LightbulbOutline;
                }

                listSelectedQuestions.Items.Add(new { Icon = ic, Text = SQLdr.GetString(2), Description = SQLdr.GetString(6), Tag = question, Progress = SQLdr.GetInt32(7) });
            }
            SQLdr.Close();

        }

        private void reload_SelectedQuestion()
        {
            SQLcmd.CommandText = String.Format("SELECT * FROM 'Questions' WHERE id = {0}", selectedQuestion["id"]);
            selectedQuestion.Clear();
            SQLiteDataReader SQLdr = SQLcmd.ExecuteReader();
            while (SQLdr.Read())
            {
                for (int col = 0; col < SQLdr.FieldCount; col++)
                {
                    selectedQuestion.Add(SQLdr.GetName(col), SQLdr.GetValue(col));
                }
            }
            SQLdr.Close();
        }

        #endregion

        #region Initialization

        private void SQLite_Initialize()
        {
            connection = String.Format(@"Data Source={0}\Datas.db", applicationDirectory);
            SQLcon = new SQLiteConnection(connection);
            SQLcon.ConnectionString = connection;
            SQLcon.Open();
            SQLcmd.Connection = SQLcon;
        }

        private void SQLite_Initialize2()
        {
            connection2 = String.Format(@"Data Source={0}\testData.db", applicationDirectory);
            SQLcon2 = new SQLiteConnection(connection2);
            SQLcon2.ConnectionString = connection2;
            SQLcon2.Open();
            SQLcmd2.Connection = SQLcon2;

            SQLcmd2.CommandText = String.Format("DELETE FROM Questions");
            SQLcmd2.ExecuteNonQuery();
        }

        #endregion

        #region Others

        private void goBack(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((bool)btnQuestionItemBack.Tag == true)
                {
                    btnQuestionItemBack_Click(sender, e);
                }
            }
            catch { }

        }

        private string toPersian(string en)
        {
            en = en.Replace("0", "۰").Replace("1", "۱").Replace("2", "۲").Replace("3", "۳").Replace("4", "۴");
            en = en.Replace("5", "۵").Replace("6", "۶").Replace("7", "۷").Replace("8", "۸").Replace("9", "۹");

            return en;
        }

        private string openApp(string app, string arguments)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = app;
            cmd.StartInfo.Arguments = arguments;
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            string codeOutput = cmd.StandardOutput.ReadToEnd();

            return codeOutput;
        }

        private void setTitle(string title)
        {
            lblTitle.Content = title;
        }

        #endregion
    }
}