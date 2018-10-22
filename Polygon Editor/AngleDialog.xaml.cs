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

namespace Polygon_Editor
{
    /// <summary>
    /// Interaction logic for AngleDialog.xaml
    /// </summary>
    public partial class AngleDialog : Window
    {
        public double Angle { get; set; }

        public AngleDialog()
        {
            InitializeComponent();
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtAngle.Text))
            {
                Angle = double.Parse(txtAngle.Text);
                this.Close();
            }
            else
                MessageBox.Show("Must provide an angle in the textbox.");
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
