using MySql.Data.MySqlClient;
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


namespace kawaii_animedb
{

    public partial class MainWindow : Window
    {


        public static string ImageFolder = "Posters";
        public static string DownloadsDir = "F:\\Downloads\\completed\\anime";
        public static string AnimeDir = "Z:\\";

        MySqlConnection conn;
        //MySqlDataAdapter adapter;
        public MainWindow()
        {
            InitializeComponent();

            Loaded += PageLoaded;
            
            
            if (!Directory.Exists(ImageFolder))
                Directory.CreateDirectory(ImageFolder);
        }

            
        public void PageLoaded(object sender, RoutedEventArgs e)
        {
            //Initial SQL connection
            String connString = "server=localhost;uid=user;pwd=passwd;database=hbdb;";
            try
            {
                conn = new MySqlConnection(connString);
                conn.Open();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            

            //Populate Folders List
            string[] dirs = Directory.GetDirectories(AnimeDir);
            foreach (string dir in dirs)
                folderList.Items.Add(dir.Substring(3));
                

            string[] dirsw = Directory.GetDirectories(DownloadsDir);
            foreach (string dirw in dirsw)
                watchingList.Items.Add(dirw.Substring(29));
            
            
            
            //Populate Archive List
            string query = "select maldata.hbid, title, image from maldata join archived ON maldata.hbID=archived.hbid";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();


            while (rdr.Read())
            {
                archiveList.Items.Add(rdr.GetString(1));
            }

            rdr.Close();
            conn.Close();

            //Populate Watching List
            string query2 = "select maldata.hbid, title, image from maldata join watching ON maldata.hbID=watching.hbid";
            MySqlCommand cmd2 = new MySqlCommand(query2, conn);
            MySqlDataReader rdr2 = cmd2.ExecuteReader();

            while (rdr2.Read())
            {
                watchingList.Items.Add(rdr2.GetString(1));
            }
 
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
            
            String connString = "server=localhost;uid=user;pwd=passwd;database=hbdb;";
            try
            {
                conn = new MySqlConnection(connString);
                conn.Open();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }

            BitmapSource poster = null;
            string folder = "";
            string animeDetailsContent = "";
            string query = "select image, title, english_title, romaji_title, episodes, status, synopsis, type, folder from maldata join archived on maldata.hbid=archived.hbid where maldata.title = \"" + sValue + "\"";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            while(rdr.Read())
            {
                poster = GetImageStream(GetImageFromUrl(rdr.GetString(0)));
                
                string status = rdr.GetString(5);
                if (status == "0")
                {
                    status = "Not Yet Aired";
                }
                else
                {
                    if (status == "1")
                    {
                        status = "Currently Airing";
                    }
                    else
                    {
                        status = "Finished Airing";
                    }
                }
                animeDetailsContent = 
                    "Title: " + rdr.GetString(1) + 
                    "\nEnglish Title: " + rdr.GetString(2) +
                    "\nRomaji Title: " + rdr.GetString(3) +
                    "\nStatus: " + status +
                    "\nType: " + rdr.GetString(7) +
                    "\nNumber of Episodes: " + rdr.GetString(4) +
                    "\nSynopsis: " + rdr.GetString(6);
                folder = rdr.GetString(8);
            }
            
            episodeList.Items.Clear();
            ListView listview = (ListView)sender;


            string[] files = Directory.GetFiles(AnimeDir + folder);
            foreach (string file in files)
                episodeList.Items.Add(file.Substring(1 + file.LastIndexOf("\\")));


            animeDetails.Text = animeDetailsContent;
            animePoster.Source = poster;
            
                
            

        }

        public static Drawing.Image GetImageFromUrl(string url)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            using (HttpWebResponse httpWebReponse = (HttpWebResponse)httpWebRequest.GetResponse())
            {
                using (Stream stream = httpWebReponse.GetResponseStream())
                {
                    return Drawing.Image.FromStream(stream);
                }
            }

        }

        public static BitmapSource GetImageStream(Drawing.Image myImage)
        {
            var bitmap = new Drawing.Bitmap(myImage);
            IntPtr bmpPt = bitmap.GetHbitmap();
            BitmapSource bitmapSource =
             System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                   bmpPt,
                   IntPtr.Zero,
                   Int32Rect.Empty,
                   BitmapSizeOptions.FromEmptyOptions());
            bitmapSource.Freeze();

            return bitmapSource;
        }

    }

}
