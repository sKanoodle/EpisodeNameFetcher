using EpisodeNameFetcher.TheTVDBApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
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

namespace EpisodeNameFetcher
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string Directory
        {
            get
            {
                Uri baseUri = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
                return WebUtility.UrlDecode(Path.GetDirectoryName(baseUri.AbsolutePath));
            }
        }

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

        public bool SeriesShortHandleEnabled
        {
            get
            {
                if ((ParsingMode.MovieTitle | ParsingMode.Debug).HasFlag(Mode))
                    return false;
                return true;
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

        private ParsingMode _Mode = ParsingMode.Wikipedia;
        public ParsingMode Mode
        {
            get => _Mode;
            set
            {
                if (_Mode == value) return;
                _Mode = value;
                OnPropertyChanged(nameof(Mode));
                OnPropertyChanged(nameof(SeriesShortHandleEnabled));
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
                if (String.IsNullOrWhiteSpace(Input))
                    return false;
                if ((ParsingMode.Debug | ParsingMode.MovieTitle).HasFlag(Mode))
                    return true;
                if (String.IsNullOrWhiteSpace(SeriesShortHandle))
                    return false;
                return true;
            }
        }

        private void Parse()
        {
            Func<string, string> parse;
            switch (Mode)
            {
                case ParsingMode.Wikipedia: parse = ParseWikipedia; break;
                case ParsingMode.TheTVDB: parse = ParseTheTVDB; break;
                case ParsingMode.MovieTitle: parse = ParseMovie; break;
                case ParsingMode.Debug: parse = ParseDebug; break;
                default: throw new NotImplementedException();
            }
            Output = parse(Input);
        }

        private string ParseWikipedia(string input)
        {
            return input
                .CleanWikipediaInput()
                .MakeWikipediaDictionary(SeriesShortHandle)
                .PadSeasonOrEpisodeNumber()
                .DeleteInvalidFilenameCharacters()
                .InsertSeasonNumbers();
        }

        private string ParseTheTVDB(string input)
        {
            return input
                .MakeTheTVDBDictionary(SeriesShortHandle)
                .PadSeasonOrEpisodeNumber()
                .DeleteInvalidFilenameCharacters();
        }

        private string ParseMovie(string input)
        {
            string pattern = @"^{""Title"":""([^""]+)"",""Year"":""([^""]+).*$";
            string replacement = "$2 $1";

            WebRequest request = WebRequest.Create($"http://www.omdbapi.com/?t={input}");
            using (Stream stream = request.GetResponse().GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
                return Regex.Replace(reader.ReadToEnd(), pattern, replacement).DeleteInvalidFilenameCharacters();
        }

        private string ParseDebug(string input)
        {
            StringBuilder result = new StringBuilder();
            TheTVDBApiClient client = new TheTVDBApiClient("https://api.thetvdb.com", File.ReadAllText(Path.Combine(Directory, "TheTVDB.apikey")));
            var shows = client.SearchShow(input);
            if (shows.Length == 1)
                foreach (var e in client.Episodes(shows[0].ID).OrderBy(e => e.AbsoluteNumber).ThenBy(e => e.AiredSeason).ThenBy(e => e.AiredEpisodeNumber))
                    //result.AppendFormat("[\"S{0:00}E{1:00}\"] = \"{3} S{0:00}E{1:00} - {2}\",\n", e.AiredSeason, e.AiredEpisodeNumber, e.EpisodeName, input);
                    result.AppendFormat("{3} S{0:00}E{1:00}A{4:000} - {2}\n", e.AiredSeason, e.AiredEpisodeNumber, e.EpisodeName, shows[0].SeriesName, e.AbsoluteNumber);
            else
                foreach (var s in shows)
                    result.AppendFormat("{0,-50}{1,-20}{2,-20}{3}\n", s.SeriesName, s.Status, s.Network, String.Join(", ", s.Aliases));
            return result.ToString();
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
