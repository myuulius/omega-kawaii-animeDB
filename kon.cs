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
using Shapes = System.Windows.Shapes;
using System.IO;
using System.Diagnostics;
using System.Data;
using System.ComponentModel;
using System.Net;
using System.Reflection;
using Drawing = System.Drawing;
using System.Data.Entity;
using System.Data.SQLite;
using System.Configuration;
using unirest_net;


namespace kawaii_animedb
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public static string ImageFolder = "Posters";
        public static string DownloadsDir = "F:\\Downloads\\completed\\anime";
        public static string AnimeDir = "Z:\\";
        public static string RequestUrl = "https://hummingbird.me/api/v2/anime/";
        public static string Key = "redacted";

        
        OutputBox outputter;
        
        public MainWindow()
        {
            InitializeComponent();

            Loaded += PageLoaded;
            
            
            if (!Directory.Exists(ImageFolder))
                Directory.CreateDirectory(ImageFolder);

            outputter = new OutputBox(Output);
            Console.SetOut(outputter);
            Console.WriteLine("Working!");
           
            Database database = new Database();
            database.CreateAnimeDatabase();
        }

            
        public void PageLoaded(object sender, RoutedEventArgs e)
        {
            
            //Populate Folders List
            ListviewPopulate populate = new ListviewPopulate();
            populate.PopulateLists(this);

            Database db = new Database();
            db.FillDatabase();
        }

        public void SelectAnimeFromListFolders(Object sender, SelectionChangedEventArgs e)
        {
            episodeList.Items.Clear();
            ListView listview = (ListView)sender;
            string[] files = Directory.GetFiles(AnimeDir + listview.SelectedItem);

            foreach (string file in files)
                episodeList.Items.Add(file.Substring(1 + file.LastIndexOf("\\")));
        }


        public void SelectAnimeFromListWatching(Object sender, SelectionChangedEventArgs e)
        {
            episodeList.Items.Clear();
            ListView listview = (ListView)sender;
            string[] files = Directory.GetFiles("F:\\Downloads\\completed\\anime\\" + listview.SelectedItem);
            foreach (string file in files)
                episodeList.Items.Add(file.Substring(1 + file.LastIndexOf("\\")));
        }


        private void SelectAnimeFromListArchive(Object sender, SelectionChangedEventArgs e)
        {
            string sValue = "";
            if (archiveList.SelectedItem != null)
            {
                sValue = (String)archiveList.SelectedItem.ToString();
            }
            ListviewPopulate populate = new ListviewPopulate();
            populate.AnimeDetails(this, sender, sValue);

        }
        
        
    }
    
}
