using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace kawaii_animedb
{
    class Database
    {
        SQLiteConnection sqlite_conn;
        SQLiteCommand sqlite_cmd;
        SQLiteDataReader sqlite_datareader;

        private void SetConnection()
        {
            sqlite_conn = new SQLiteConnection("Data Source=animedata.sqlite;Version=3;");
        }

        private void ExecuteQuery(string query)
        {
            SetConnection();
            sqlite_conn.Open();
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = query;
            sqlite_cmd.ExecuteNonQuery();
            sqlite_conn.Close();
        }

        public SQLiteDataReader GetData(string query)
        {
            SetConnection();
            sqlite_conn.Open();
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = query;
            sqlite_datareader = sqlite_cmd.ExecuteReader();
            return sqlite_datareader;
            //sqlite_conn.Close();
        }

        public void CreateAnimeDatabase()
        {
            if (!File.Exists("animedata.sqlite"))
            {
                SQLiteConnection.CreateFile("animedata.sqlite");

                string sql = "create table anime (id INTEGER PRIMARY KEY, title VARCHAR(255), english_title VARCHAR(255), romaji_title VARCHAR(255), episodes INTEGER, status INTEGER, startdate VARCHAR(30), enddate VARCHAR(30), image VARCHAR(140), synopsis VARCHAR(5000), type VARCHAR(10), prefgroup INTEGER); "
                    + "create table watching (id INTEGER, folder VARCHAR(80), preftitle VARCHAR(80), progress INTEGER); " + "create table archived (id INTEGER, folder VARCHAR(80), preftitle VARCHAR(80)); "
                    + "create table subgroups (groupid INTEGER PRIMARY KEY, groupname VARCHAR(80), isjoint BOOLEAN, priority SMALLINT, website VARCHAR(120)); "
                    + "create table episodes (id INTEGER, episodeNumber INTEGER, version INTEGER, dateaired DATE, datedownloaded DATE, groupid INTEGER, isBD BOOLEAN, crc CHAR(8), epname VARCHAR(255));";
                
                ExecuteQuery(sql);
                
                
                Console.WriteLine("DB Created.");

            }
            else
            {
                Console.WriteLine("DB exists.");
            }


        }

        private void UpdateDatabase()
        {
            SQLiteDataReader rdr = GetData("select max(id) from anime");
            while (rdr.Read())
            {
                int max = rdr.GetInt32(0) + 1;
                for (int i = max; i <= max + 20; i++)
                {
                    //newAnime();
                }
            }
        }

        public async void FillDatabase()
        {
            int maxid = 1;
            SQLiteDataReader rdr = GetData("select max(id) from anime");
            while (rdr.Read())
            {
                maxid = rdr.GetInt32(0);
            }
            int count = 0;
            int id = maxid + 1;
            while (count < 10)
            {
                API api = new API();
                Anime anime = new Anime();
                anime = api.GetAPIData(id, anime);
                if (anime != null)
                {
                    string sql = "insert into anime values (" + id + ", \"" + anime.title_canonical + "\", \"" + anime.title_english + "\", \"" + anime.title_romaji + "\", " + anime.episode_count + ", \"" + anime.status + "\", \"" + anime.started_airing + "\", \"" + anime.finished_airing + "\", \"" + anime.poster_image + "\", \"" + anime.synopsis + "\", \"" + anime.show_type + "\", \"\"" + ")";
                    Console.WriteLine(sql);
                    ExecuteQuery(sql);
                    count = 0;
                    //count++;
                }
                else
                {
                    count++;
                }
                id++;
            }
            Console.WriteLine("Reached 10 consecutive empty entries.");
            await Task.Delay(1000);
        }

    }
}
