using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Astrometrie
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Properties.Settings.Default.message = "Press calculate to send last image to nova.astrometry.net";
            InitializeComponent();
        }




        private void Button_Click_Browse(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();

            folderDialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            folderDialog.ShowNewFolderButton = false;

            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Properties.Settings.Default.folderPath = folderDialog.SelectedPath;
                Properties.Settings.Default.Save();
            }

        }


        private void Button_Click_Calculate(object sender, RoutedEventArgs e)
        {
            calculateButton.IsEnabled = false;

            Thread t = new Thread(new ThreadStart(MainRun));

            t.Start();

        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void MainRun()
        {
            ApiHelper.InitializeClient();

            if (!ApiHelper.InitializeSession())
            {
                Properties.Settings.Default.message = "Unable to initialize session.";

                return;
            }

            AstrometryProcessor.UploadImage();

            if (!AstrometryProcessor.GetSubmissionStatus())
            {
                Properties.Settings.Default.message = "Astrometry failed";
                return;
            }

            if (!AstrometryProcessor.GeJobStatus())
            {
                Properties.Settings.Default.message = "Astrometry failed";
                return;
            }


            CoordinatesModel coordinates = AstrometryProcessor.GetAstrometryResults();

            this.Dispatcher.Invoke(() =>
            {
                Properties.Settings.Default.Ra = coordinates.HumanReadableRa();
                Properties.Settings.Default.Dec = coordinates.HumanReadableDec();
                Properties.Settings.Default.Radius = coordinates.HumanReadableRadius();
                Properties.Settings.Default.Save();

                calculateButton.IsEnabled = true;
            });

        }
    }
}
