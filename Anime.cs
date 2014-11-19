using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kawaii_animedb
{
    public class Anime
    {
        public string title_canonical;
        public string title_english;
        public string title_romaji;
        public string title_japanese;
        public string title_slug;

        public string synopsis;
        public string started_airing;
        public string finished_airing;
        public string status;
        public string show_type;
        public int episode_count;
        public List<System.Object> genres;
        public List<System.Object> producers;

        public string poster_image;
        public string youtube_link;

        public bool GetStatus()
        {
            return false;
        }
        
    }
}
