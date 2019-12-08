using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

namespace WpfAsyncDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CancellationTokenSource cts = new CancellationTokenSource();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void executeSync_Click(object sender, RoutedEventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var results = DemoMethods.RunDownloadSync();
            PrintResults(results);

            watch.Stop();
            var elepsedMs = watch.ElapsedMilliseconds;

            resultsWindow.Text += $"Total execute time: {elepsedMs}";
        }
        private void executeParallelSync_Click(object sender, RoutedEventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            var results = DemoMethods.RunDownloadParallelSync();
            PrintResults(results);

            watch.Stop();
            var elepsedMs = watch.ElapsedMilliseconds;

            resultsWindow.Text += $"Total execute time: {elepsedMs}";
        }

        private async void executeAsync_Click(object sender, RoutedEventArgs e)
        {
            Progress<ProgressReportModel> progress = new Progress<ProgressReportModel>();
            progress.ProgressChanged += ReportProgress;
            var watch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                var results = await DemoMethods.RunDownloadAsync(progress, cts.Token);
                PrintResults(results);
            }
            catch (OperationCanceledException)
            {
                resultsWindow.Text += $"The async download was cancel. { Environment.NewLine}";
            }

            watch.Stop();
            var elepsedMs = watch.ElapsedMilliseconds;

            resultsWindow.Text += $"Total execute time: {elepsedMs} {Environment.NewLine}";
        }

        private async void executeParallelAsync_Click(object sender, RoutedEventArgs e)
        {
            Progress<ProgressReportModel> progress = new Progress<ProgressReportModel>();
            progress.ProgressChanged += ReportProgress;
            var watch = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                //var results = await DemoMethods.RunDownloadParallelAsync();
                var results = await DemoMethods.RunDownloadParallelAsyncV2(progress, cts.Token);
                PrintResults(results);

            }
            catch (OperationCanceledException)
            {
                resultsWindow.Text += $"The async download was cancel. { Environment.NewLine}";
            }

            watch.Stop();
            var elepsedMs = watch.ElapsedMilliseconds;

            resultsWindow.Text += $"Total execute time: {elepsedMs} {Environment.NewLine}";
        }

        private void ReportProgress(object sender, ProgressReportModel e)
        {
            dashboardProgress.Value = e.PercantageComplete;
            PrintResults(e.SitesDownloaded);
        }

        private void cancelOperation_Click(object sender, RoutedEventArgs e) 
        {
            cts.Cancel();            
        }

        private void PrintResults(List<WebsiteDataModel> results)
        {
            resultsWindow.Text = "";

            foreach(var item in results)
            {
                resultsWindow.Text += $"{ item.WebsiteURL } downloaded { item.WebsiteData.Length } characters long.{ Environment.NewLine }";
            }
        }
    }
}
