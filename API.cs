using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using unirest_net;
using System.Text.RegularExpressions;
using Json;

namespace kawaii_animedb
{
    class API
    {

        public Anime GetAPIData(int id, Anime anime)
        {
            string RequestUrl = MainWindow.RequestUrl + id.ToString();
            unirest_net.http.HttpResponse<string> request = unirest_net.http.Unirest.get(RequestUrl).header("X-Client-Id", MainWindow.Key).asString();
            string apibody = request.Body;
            
            Console.WriteLine(request.Code);
            if (request.Code != 200)
            {
                return anime;
            }
            
            var json = JsonParser.Deserialize(apibody);

            Console.WriteLine(json.anime.slug);

            anime.title_canonical = json.anime.titles.canonical;
            anime.title_english = json.anime.titles.english;
            anime.title_romaji = json.anime.titles.romaji;
            anime.title_japanese = json.anime.titles.japanese;
            anime.title_slug = json.anime.slug;

            anime.synopsis = json.anime.synopsis;
            anime.started_airing = json.anime.started_airing_date;
            anime.finished_airing = json.anime.finished_airing_date;
            anime.show_type = json.anime.show_type;
            anime.episode_count = Convert.ToInt32(json.anime.episode_length);
            anime.genres = json.anime.genres;
            anime.producers = json.anime.producers;

            anime.poster_image = json.anime.poster_image;
            anime.youtube_link = json.anime.youtube_video_id;

            if (json.anime.finished_airing_date != null)
            {
                anime.status = "Finished Airing";
            }
            else
            {
                if (json.anime.started_airing_date != null)
                {
                    if (anime.show_type != "TV")
                    {
                        anime.status = "Finished Airing";
                    }
                }
                else
                {
                    anime.status = "Not Yet Aired";
                }
            }
            Console.WriteLine(anime.status);
            
            return anime;
        }

    }
    
}
