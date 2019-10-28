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
using System.Diagnostics;

namespace XAmlApp
{
    /// <summary>
    /// Логика взаимодействия для OpenFileWindow.xaml
    /// </summary>
    public partial class OpenFileWindow : Window
    {
        public OpenFileWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(Path.Text != "")
            {
                if (PathForProgramm.Text != "")
                {
                    Process.Start(PathForProgramm.Text, Path.Text);
                }
                else Process.Start(Path.Text);
            }
            else
            {
                MessageBox.Show("Напишите путь к файлу");
            }
        }
    }
}
