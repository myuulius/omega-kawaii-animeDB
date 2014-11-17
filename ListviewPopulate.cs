using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SQLite;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Net;

namespace kawaii_animedb
{
    
    class ListviewPopulate
    {

        public static string DownloadsDir = "F:\\Downloads\\completed\\anime";
        public static string AnimeDir = "Z:\\";

        public void PopulateLists(MainWindow mainwindow)
        {
            
            string[] dirs = Directory.GetDirectories(AnimeDir);
            foreach (string dir in dirs)
                mainwindow.folderList.Items.Add(dir.Substring(3));


            string[] dirsw = Directory.GetDirectories(DownloadsDir);
            foreach (string dirw in dirsw)
                mainwindow.watchingList.Items.Add(dirw.Substring(29));

            
            //Populate Archive List
            string query = "select folder from archived";
            Database database = new Database();
            SQLiteDataReader rdr3 = database.GetData(query);

            while (rdr3.Read())
            {
                mainwindow.archiveList.Items.Add(rdr3.GetString(0));
            }
            rdr3.Close();

            //Populate Watching List
            /*string query2 = "select anime.id, title, image from anime join watching ON anime.id=watching.id";
            SQLiteDataReader rdr2 = database.GetData(query2);
            while (rdr2.Read())
            {
                mainwindow.watchingList.Items.Add(rdr2.GetString(1));
            }*/
        }

        public void AnimeDetails(MainWindow mainwindow, Object sender, string sValue)
        {
            // Populate Details Pane
            string status = "";
            string posterurl = "";
            string folder = "";
            string id = "";
            string animeDetailsContent = "";
            string query = "select image, title, english_title, romaji_title, episodes, status, synopsis, type, folder, anime.id from anime inner join archived on anime.id=archived.id where anime.title = \"" + sValue + "\"";
            Database database = new Database();
            SQLiteDataReader rdr = database.GetData(query);


            while (rdr.Read())
            {
                status = rdr.GetInt32(5).ToString();
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
                    "\nNumber of Episodes: " + rdr.GetInt32(4).ToString() +
                    "\nSynopsis: " + rdr.GetString(6);
                folder = rdr.GetString(8);
                id = rdr.GetInt32(9).ToString();
                posterurl = rdr.GetString(0);
            }

            mainwindow.episodeList.Items.Clear();
            ListView listview = (ListView)sender;

            string[] files = Directory.GetFiles(AnimeDir + folder);
            foreach (string file in files)
                mainwindow.episodeList.Items.Add(file.Substring(1 + file.LastIndexOf("\\")));

            mainwindow.animeDetails.Text = animeDetailsContent;


            // Set poster image (not working correctly)
            string posterpath = "Posters\\" + id + ".jpg";
            if (!File.Exists(posterpath))
            {
                Console.WriteLine("Poster doesn't exist, downloading...");
                WebClient webClient = new WebClient();
                webClient.DownloadFile(posterurl, posterpath);
                Console.WriteLine("Poster downloaded.");
            }
            else
            {
                Console.WriteLine("Poster already exists.");
            }
            
            Uri uri = new Uri(posterpath, UriKind.RelativeOrAbsolute);
            BitmapImage bitmap = new BitmapImage(uri);
            mainwindow.animePoster.Source = bitmap;
            


            rdr.Close();
        }



    }
}
