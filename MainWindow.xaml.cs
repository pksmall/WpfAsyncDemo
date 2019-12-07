using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

namespace WpfAsyncDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void executeSync_Click(object sender, RoutedEventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            RunDownloadSync();

            watch.Stop();
            var elepsedMs = watch.ElapsedMilliseconds;

            resultsWindow.Text += $"Total execute time: {elepsedMs}";
        }
        private async void executeAsync_Click(object sender, RoutedEventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            await RunDownloadAsync();

            watch.Stop();
            var elepsedMs = watch.ElapsedMilliseconds;

            resultsWindow.Text += $"Total execute time: {elepsedMs} {Environment.NewLine}";
        }

        private async void executeParallelAsync_Click(object sender, RoutedEventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            await RunDownloadParallelAsync();

            watch.Stop();
            var elepsedMs = watch.ElapsedMilliseconds;

            resultsWindow.Text += $"Total execute time: {elepsedMs} {Environment.NewLine}";
        }

        private List<string> PrepData() 
        {
            List<string> output = new List<string>();

            resultsWindow.Text = "";

            output.Add("https://www.yahoo.com");
            output.Add("https://www.google.com");
            output.Add("https://www.microsoft.com");
            output.Add("http://www.cnn.com");
            output.Add("http://www.codeproject.com");
            output.Add("http://www.stackoverflow.com");

            return output;
        }

        private async Task RunDownloadAsync()
        {
            List<string> websites = PrepData();

            foreach (string site in websites)
            {
                WebsiteDataModel result = await Task.Run(() => DownloadWebsite(site));
                ReportWebsiteInfo(result);
            }
        }

        private async Task RunDownloadParallelAsync()
        {
            List<string> websites = PrepData();
            List<Task<WebsiteDataModel>> tasks = new List<Task<WebsiteDataModel>>();

            foreach (string site in websites)
            {
                tasks.Add(DownloadWebsiteAsync(site));
            }

            var results = await Task.WhenAll(tasks);

            foreach(var item in results)
            {
                ReportWebsiteInfo(item);
            }
        }

        private void RunDownloadSync() 
        {
            List<string> websites = PrepData();

            foreach(string site in websites)
            {
                WebsiteDataModel result = DownloadWebsite(site);
                ReportWebsiteInfo(result);
            }
        }

        private WebsiteDataModel DownloadWebsite(string websiteURL)
        {
            WebsiteDataModel output = new WebsiteDataModel();
            WebClient client = new WebClient();

            output.WebsiteURL = websiteURL;
            output.WebsiteData = client.DownloadString(websiteURL);

            return output;            
        }

        private async Task<WebsiteDataModel> DownloadWebsiteAsync(string websiteURL)
        {
            WebsiteDataModel output = new WebsiteDataModel();
            WebClient client = new WebClient();

            output.WebsiteURL = websiteURL;
            output.WebsiteData = await client.DownloadStringTaskAsync(websiteURL);

            return output;
        }

        private void cancelOperation_Click(object sender, RoutedEventArgs e) 
        {
            
        }

        private void ReportWebsiteInfo(WebsiteDataModel data)
        {
            resultsWindow.Text += $"{ data.WebsiteURL } downloaded { data.WebsiteData.Length } characters long.{ Environment.NewLine }";
        }
        
        private void PrintResult(List<WebsiteDataModel> results)
        {
            resultsWindow.Text = "";

            foreach(var item in results)
            {
                resultsWindow.Text += $"{ item.WebsiteURL } downloaded { item.WebsiteData.Length } characters long.{ Environment.NewLine }";
            }
        }
    }
}
