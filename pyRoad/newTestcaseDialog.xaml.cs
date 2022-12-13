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
    /// Interaction logic for newTestcaseDialog.xaml
    /// </summary>
    public partial class newTestcaseDialog : Window
    {
        private string inputs;
        public string Inputs
        {
            get { return inputs; }
            set { inputs = value; }
        }

        private string outputs;
        public string Outputs
        {
            get { return outputs; }
            set { outputs = value; }
        }

        public newTestcaseDialog()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (txtInputs.Text == "" || txtOutputs.Text == "")
            {
                MessageBox.Show("همه ی مقادیر را وارد کنید", "خطا");
                return;
            }

            bool inputIsOk = false;
            bool outputIsOk = false;

            if (!(txtInputs.Text.Contains("[") && txtInputs.Text.Contains("]")))
            {
                Inputs = txtInputs.Text.Replace("\n", "{\\s\\}");
                inputIsOk = true;
            }
            else
            {
                MessageBox.Show("ورودی شامل کاراکتر های [ یا ] می باشد", "خطا در مقادیر ورودی");
            }

            if (!(txtOutputs.Text.Contains("[") && txtOutputs.Text.Contains("]")))
            {
                Outputs = txtOutputs.Text.Replace("\n", "{\\s\\}");
                outputIsOk = true;
            }
            else
            {
                MessageBox.Show("خروجی شامل کاراکتر های [ یا ] می باشد", "خطا در مقادیر خروجی");
            }

            if (inputIsOk && outputIsOk)
            {
                this.Close();
            }

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
