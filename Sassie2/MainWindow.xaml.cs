using System.Windows;

namespace Sassie2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            HondaCPOInspectionReport report = new HondaCPOInspectionReport();
            report.GetData();
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
