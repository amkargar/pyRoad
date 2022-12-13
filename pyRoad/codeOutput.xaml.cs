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
    /// Interaction logic for codeOutput.xaml
    /// </summary>
    public partial class codeOutput : Window
    {
        private string[] results;

        public string[] Results
        {
            get { return results; }
            set
            {
                results = value;

                for (int i = 0; i < results.Length; i++)
                {
                    string clr;
                    string res = "";
                    if (results[i].Contains("True")) clr = "#388E3C"; else clr = "#F44336";
                    if (results[i].Contains("True"))
                    {
                        res = "CORRECT ANSWER";

                    }
                    else if (results[i].Contains("False"))
                    {
                        res = "INCORRECT ANSWER";

                    }
                    else if (results[i].Contains("RuntimeError"))
                    {
                        res = "RUNTIME ERROR";
                    }
                    listResults.Items.Add(new { Text = string.Format("TEST {0} → {1}", i + 1, res), Foreground = clr });

                    int progress = results.Where(c => c.Contains("True")).ToArray().Length / results.Length * 100;
                    lblPercent.Content = string.Format("{0} %", progress);

                    if (progress == 100)
                    {
                        lblPercent.Foreground = new SolidColorBrush(Color.FromRgb(56, 160, 60));
                    }
                    else if (progress > 80)
                    {
                        lblPercent.Foreground = new SolidColorBrush(Color.FromRgb(244, 211, 33));

                    }
                    else if (progress > 40)
                    {
                        lblPercent.Foreground = new SolidColorBrush(Color.FromRgb(244, 143, 13));

                    }
                    else
                    {
                        lblPercent.Foreground = new SolidColorBrush(Color.FromRgb(244, 67, 54));
                    }

                    Random rnd = new Random();

                    if (progress == 100)
                    {
                        var resourcePath = new System.Uri(string.Format("pack://application:,,,/Resources/TestResults/h{0}.JPG", rnd.Next(1, 7)));
                        var bitmap = new BitmapImage(resourcePath);
                        imgResult.Source = bitmap;
                    }
                    else
                    {
                        var resourcePath = new System.Uri(string.Format("pack://application:,,,/Resources/TestResults/s{0}.JPG", rnd.Next(1, 8)));
                        var bitmap = new BitmapImage(resourcePath);
                        imgResult.Source = bitmap;
                    }
                }
            }
        }

        public codeOutput()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
