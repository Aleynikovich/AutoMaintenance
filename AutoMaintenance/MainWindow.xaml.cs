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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutoMaintenance
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ImportCreate.Click += ImportCreate_Click;

        }

        private void ImportCreate_Click(object sender, RoutedEventArgs e)
        {
            ImportCreate windowImportCreate = new ImportCreate();
            windowImportCreate.Show();
        }

        private void KUKALOGO_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.kuka.com");
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.github.com/Aleynikovik");
        }


        private void Salir_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Information_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

    }
}
