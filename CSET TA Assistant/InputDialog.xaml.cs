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

namespace CSET_TA_Assistant
{
    /// <summary>
    /// Interaction logic for InputDialog.xaml
    /// </summary>
    public partial class InputDialog : Window
    {
        public string value
        {
            get { return txtAnswer.Text; }
        }
        public string Text
        {
            get
            {
                return lblText.Content.ToString();
            }
            set
            {
                lblText.Content = value;
            }
        }
        public InputDialog()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            txtAnswer.Text = "";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtAnswer.Focus();
        }
    }
}
