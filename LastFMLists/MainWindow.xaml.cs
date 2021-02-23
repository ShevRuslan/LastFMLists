using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace LastFMLists
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int currentPage = 1;
        uint i = 1;
        private readonly LastFMAPI lastFM = new LastFMAPI();
        private string typoTags = "Кол-во уникальный тегов - ";
        private ObservableCollection<Tag> uniqueTags = new ObservableCollection<Tag>();
        public MainWindow()
        {
            InitializeComponent();
            tagsList.ItemsSource = uniqueTags;
        }
        private void RenderTracks()
        {
            List<Track> newTracks = GetTracks();
            foreach (Track track in newTracks)
            {
                Dispatcher.Invoke(() =>
                {
                    TrackParse.Text = "Текущий трек: " + track.name + " исполнителя - " + track.artist;
                    Progress.Maximum = newTracks.Count;
                    tracksList.Items.Add(track);
                    Progress.Value++;
                    Typography.Text = i.ToString();
                });
                i++;
                RenderTags(track.tags);
            }
        }
        private void RenderTags(List<string> tags)
        {
            foreach(string tag in tags)
            {
                try
                {
                    Tag exsistTag = uniqueTags.Single(i => i.name.ToLower() == tag.ToLower());
                    exsistTag.count++;
                }
                catch(Exception err)
                {
                    Tag newTag = new Tag
                    {
                        name = tag,
                        count = 1,
                    };
                    Dispatcher.Invoke(() =>
                    {
                        uniqueTags.Add(newTag);
                        TagsTypography.Text = typoTags + uniqueTags.Count();
                    });
                }
            }
        }
        private List<Track> GetTracks()
        {
            return lastFM.GetTracks(currentPage);
        }

        private async void addNewTracks(object sender, RoutedEventArgs e)
        {
            TrackParse.Text = "Начался парсинг с LAST FM API!";
            await Task.Run(() => RenderTracks());
            this.currentPage++;
            Progress.Value = 0;
            TrackParse.Text = "";
        }
    }
}
