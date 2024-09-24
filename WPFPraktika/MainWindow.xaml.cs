using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using WPFPraktika.Entities;
using WPFPraktika.Services.Implementations;
using WPFPraktika.Services.Interfaces;

namespace WPFPraktika
{
  
    public partial class MainWindow : Window
    {
        private Timer _monitorTimer;
        private List<IFileLoader> _fileLoaders;
        private static Dictionary<string,List<TradeData>> _tradeData = new Dictionary<string,List<TradeData>>();
        private string _directoryPath;
        private int _monitoringFrequency;

        public MainWindow()
        {
            InitializeComponent();            
            InitializeFileLoaders();
            LoadSettingsFromConfig();
           
        }
        private void InitializeFileLoaders()
        {
            _fileLoaders = new List<IFileLoader>
            {
                new XmlFileLoader(),
                new CsvFileLoader(),
                new TxtFileLoader()
            };
        }
        private bool LoadSettingsFromConfig()
        {

            _directoryPath = ConfigurationManager.AppSettings["InputDirectory"];
            var result = CheckConfigurations(true);

            DirectoryPath.Text = _directoryPath;
            Frequency.Text = _monitoringFrequency.ToString();
            return result;
        }
        private bool CheckConfigurations(bool startApp)
        {
            var result = true;
            if (startApp)
            {
                if (string.IsNullOrEmpty(_directoryPath) || !Directory.Exists(_directoryPath))
                {
                    MessageBox.Show("The path to the directory in the configuration file is incorrect.");
                    result = false;
                }

                if (!int.TryParse(ConfigurationManager.AppSettings["MonitoringFrequency"], out _monitoringFrequency) || _monitoringFrequency <= 0)
                {
                    MessageBox.Show("Incorrect monitoring interval in the configuration file.");
                    result = false;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(DirectoryPath.Text) || !Directory.Exists(DirectoryPath.Text))
                {
                    MessageBox.Show("The path to the directory in the configuration file is incorrect.");
                    result = false;
                }

                if (!int.TryParse(Frequency.Text, out _monitoringFrequency) || _monitoringFrequency <= 0)
                {
                    MessageBox.Show("Incorrect monitoring interval in the configuration file.");
                    result = false;
                }
            }
            return result;
        }
        private void StartMonitoring_Click(object sender, RoutedEventArgs e)
        {
            var result = CheckConfigurations(false);
            if (result)
            {
                _directoryPath = DirectoryPath.Text;
                int frequency = int.Parse(Frequency.Text);
                StartDirectoryMonitoring(frequency);
            }      
        }

        private  void StartDirectoryMonitoring(int interval)
        {
            if (_monitorTimer != null)
            {
                _monitorTimer.Stop();
            }

            _monitorTimer = new Timer(interval);
            _monitorTimer.Elapsed +=  MonitorDirectory;
            _monitorTimer.Start();
        }

        private  async void MonitorDirectory(object sender, ElapsedEventArgs e)
        {
            bool isDataUpdated = false;
            var allFiles = Directory.GetFiles(_directoryPath);
            foreach (var file in allFiles)
            {
                if (_tradeData.ContainsKey(file))
                    continue;

                var loader = GetFileLoader(file);
                if (loader != null)
                {
                    List<TradeData> newTradeData = await loader.LoadFileAsync(file);
                    if (!_tradeData.ContainsKey(file))
                    {
                        _tradeData.Add(file, newTradeData);
                        isDataUpdated = true;
                    }
                              

                }
            }
            if (isDataUpdated)
            {
                var values = _tradeData.Values.SelectMany(v => v);
                Dispatcher.Invoke(() => DataGrid.ItemsSource = values);
            }
        }

        private IFileLoader GetFileLoader(string filePath)
        {
            foreach (var loader in _fileLoaders)
            {
                if (loader.CanLoad(filePath))
                {
                    return loader;
                }
            }
            return null;
        }  
    }
}
