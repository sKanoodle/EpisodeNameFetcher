using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace EpisodeNameFetcher
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string _SeriesShortHandle;
        public string SeriesShortHandle
        {
            get => _SeriesShortHandle;
            set
            {
                if (_SeriesShortHandle == value) return;
                _SeriesShortHandle = value;
                OnPropertyChanged(nameof(SeriesShortHandle));
            }
        }

        private string _Input;
        public string Input
        {
            get => _Input;
            set
            {
                if (_Input == value) return;
                _Input = value;
                OnPropertyChanged(nameof(Input));
            }
        }

        private string _Output;
        public string Output
        {
            get => _Output;
            set
            {
                if (_Output == value) return;
                _Output = value;
                OnPropertyChanged(nameof(Output));
            }
        }

        private ICommand _Convert;

        public ICommand Convert
        {
            get
            {
                if (_Convert == null)
                    _Convert = new RelayCommand(_ => Parse(),_ => IsValid);
                return _Convert;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private bool IsValid
        {
            get
            {
                if (String.IsNullOrWhiteSpace(SeriesShortHandle))
                    return false;
                if (String.IsNullOrWhiteSpace(Input))
                    return false;
                return true;
            }
        }

        private void Parse()
        {
            string result = CleanInput(Input);
            result = MakeDictionary(result);
            result = ToTwoDigitEpisodeNumber(result);
            result = DeleteInvalidFilenameCharacters(result);
            result = InsertSeasonNumbers(result);
            Output = result;
        }

        private string CleanInput(string input)
        {
            string pattern = @"\r\n(?!\d|Season)";
            string replacementPattern = String.Empty;
            input = Regex.Replace(input, pattern, replacementPattern);

            pattern = @"^Season.*$";
            return Regex.Replace(input, pattern, replacementPattern, RegexOptions.Multiline);
        }

        private string MakeDictionary(string input)
        {
            string pattern = @"^\d+\t(\d+)\t""([^""]+).*$";
            string replacementPattern = $@"[""S0E$1""] = ""{SeriesShortHandle} S0E$1 - $2"",";
            return Regex.Replace(input, pattern, replacementPattern, RegexOptions.Multiline);
        }

        private string ToTwoDigitEpisodeNumber(string input)
        {
            string pattern = @"S0E(\d)(?!\d)";
            string replacementPattern = @"S0E0$1";
            return Regex.Replace(input, pattern, replacementPattern);
        }

        private string DeleteInvalidFilenameCharacters(string input)
        {
            string pattern = @"[:?<>*|…]";
            string replacementPattern = String.Empty;
            return Regex.Replace(input, pattern, replacementPattern);
        }

        private string InsertSeasonNumbers(string input)
        {
            StringBuilder result = new StringBuilder();
            int season = 1;
            string line;
            using (TextReader reader = new StringReader(input))
                while ((line = reader.ReadLine()) != null)
                {
                    if (line == String.Empty)
                        season += 1;
                    else
                        result.AppendLine(insert(line, season));
                }
            return result.ToString();

            string insert(string l, int s)
            {
                if (s > 99) throw new ArgumentException();
                string pattern = @"S0(E\d{2})";
                string replacementPattern = $@"S{season:00}$1";
                return Regex.Replace(l, pattern, replacementPattern);
            }
        }

        public class RelayCommand : ICommand
        {
            private readonly Action<object> _Execute;
            private readonly Func<object, bool> _CanExecute;

            public RelayCommand(Action<object> execute) : this(execute, null) { }
            public RelayCommand(Action<object> execute, Func<object, bool> canExecute)
            {
                _Execute = execute ?? throw new ArgumentNullException(nameof(execute));
                _CanExecute = canExecute;
            }

            public event EventHandler CanExecuteChanged
            {
                add => CommandManager.RequerySuggested += value;
                remove => CommandManager.RequerySuggested -= value;
            }

            public bool CanExecute(object parameter) => _CanExecute?.Invoke(parameter) ?? true;

            public void Execute(object parameter) => _Execute?.Invoke(parameter);
        }
    }
}
