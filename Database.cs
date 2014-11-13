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
            sqlite_conn.Close();
            return sqlite_datareader;
        }

        public void CreateAnimeDatabase()
        {
            if (!File.Exists("animedata.sqlite"))
            {
                SQLiteConnection.CreateFile("animedata.sqlite");

                string sql = "create if table not exists anime (id INTEGER PRIMARY KEY, title VARCHAR(255), english_title VARCHAR(255), romaji_title VARCHAR(255), episodes INTEGER, status INTEGER, startdate DATE, enddate DATE, image VARCHAR(140), synopsis VARCHAR(5000), type VARCHAR(10), prefgroup INTEGER); "
                    + "create if table not exists watching (id INTEGER, folder VARCHAR(80), preftitle VARCHAR(80), progress INTEGER); " + "create if table not exists archived (id INTEGER, folder VARCHAR(80), preftitle VARCHAR(80)); "
                    + "create if table not exists subgroups (groupid INTEGER PRIMARY KEY, groupname VARCHAR(80), isjoint BOOLEAN, priority SMALLINT, website VARCHAR(120)); "
                    + "create if table not exists episodes (id INTEGER, episodeNumber INTEGER, version INTEGER, dateaired DATE, datedownloaded DATE, groupid INTEGER, isBD BOOLEAN, crc CHAR(8), epname VARCHAR(255));";
                
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
    }
}