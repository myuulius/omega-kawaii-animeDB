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
        Database database;
        MainWindow mainwindow;

        public static string DownloadsDir = "F:\\Downloads\\completed\\anime";
        public static string AnimeDir = "Z:\\";

        public void PopulateLists()
        {
            string[] dirs = Directory.GetDirectories(AnimeDir);
            foreach (string dir in dirs)
                mainwindow.folderList.Items.Add(dir.Substring(3));


            string[] dirsw = Directory.GetDirectories(DownloadsDir);
            foreach (string dirw in dirsw)
                mainwindow.watchingList.Items.Add(dirw.Substring(29));



            //Populate Archive List
            string query = "select anime.hbid, title, image from anime join archived ON anime.hbID=archived.hbid";
            SQLiteDataReader rdr = database.GetData(query);

            while (rdr.Read())
            {
                mainwindow.archiveList.Items.Add(rdr.GetString(1));
            }

            rdr.Close();

            //Populate Watching List
            string query2 = "select anime.hbid, title, image from anime join watching ON anime.hbID=watching.hbid";
            SQLiteDataReader rdr2 = database.GetData(query2);
            while (rdr2.Read())
            {
                mainwindow.watchingList.Items.Add(rdr2.GetString(1));
            }
        }

        public void AnimeDetails(Object sender, string sValue)
        {
            BitmapSource poster = null;
            string folder = "";
            string id = "";
            string animeDetailsContent = "";
            string query = "select image, title, english_title, romaji_title, episodes, status, synopsis, type, folder, id from anime join archived on anime.hbid=archived.hbid where anime.title = \"" + sValue + "\"";

            SQLiteDataReader rdr = database.GetData(query);


            while (rdr.Read())
            {
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
                id = rdr.GetString(9);
            }

            mainwindow.episodeList.Items.Clear();
            ListView listview = (ListView)sender;



            string[] files = Directory.GetFiles(AnimeDir + folder);
            foreach (string file in files)
                mainwindow.episodeList.Items.Add(file.Substring(1 + file.LastIndexOf("\\")));

            mainwindow.animeDetails.Text = animeDetailsContent;

            string posterpath = "/Posters/" + id;
            //string posterpath = "Posters\\" + rdr.GetString(9);
            if (File.Exists(posterpath))
            {
                mainwindow.animePoster.Source = new BitmapImage(new Uri(posterpath));
            }
            else
            {
                //poster = GetImageFromUrl(rdr.GetString(0));
                WebClient webClient = new WebClient();
                webClient.DownloadFile(id, posterpath);
                mainwindow.animePoster.Source = poster;
            }


            rdr.Close();
        }



    }
}
