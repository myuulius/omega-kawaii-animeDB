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

        BackgroundWorker dbFill = new BackgroundWorker();
        OutputBox outputter;
        
        //API api;
        //Database database;
        public MainWindow()
        {
            InitializeComponent();

            Loaded += PageLoaded;

            dbFill.WorkerSupportsCancellation = true;
            dbFill.DoWork += new DoWorkEventHandler(bw_DBFill);
            dbFill.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_DBFillCompleted);
            
            
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

        private void dbFill_Click(object sender, RoutedEventArgs e)
        {
            if (dbFill.IsBusy != true)
            {
                dbFill.RunWorkerAsync();
            }
        }

        private void bw_DBFill(object sender, DoWorkEventArgs e)
        {
            Database db = new Database();
            db.FillDatabase();
        }

        private void bw_DBFillCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Cancelled == true))
            {
                Console.WriteLine("Database operation cancelled.");
            }
            else if (!(e.Error == null))
            {
                Console.WriteLine("Error: " + e.Error.Message);
            }
            else
            {
                Console.WriteLine("Database operation completed.");
            }
        }

        //This doesn't work, need to pass the cancellation to the while loop in db.FillDatabase()
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            if (dbFill.WorkerSupportsCancellation == true)
            {
                dbFill.CancelAsync();
            }
        }
        
    }
    
}
