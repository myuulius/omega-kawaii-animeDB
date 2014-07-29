import Unirest
import re
import MySQLdb
import string

db = MySQLdb.connect(host="localhost", # your host, usually localhost
                     user="user", # your username
                      passwd="passwd", # your password
                      db="hbdb") # name of the data base
cur = db.cursor()

def hummingget(start, finish):
        
    for i in range(start,finish):
    
        response = Unirest.get("https://vikhyat-hummingbird-v2.p.mashape.com/anime/" + str(i),
        headers={"X-Mashape-Key": "XrBhpdN9HFmshrHK2eOHy3bwyQ9Op1UuTmzjsnbXhNt6OiTYLL"}
        )
        d = response.raw_body
        status = 3 
        
        id = re.search('id":([^,]*)', d)
        if id:
            id = id.group(1)
                    
        title = re.search(r'canonical_title":"([^"]*)', d)
        if title:
            title = title.group(1)
        else:
            title = ""
        
        english_title = re.search('"english_title":"([^"]*)', d)
        if english_title:
            english_title = english_title.group(1)
        else:
            english_title = ""
            
        romaji_title = re.search('"romaji_title":"([^"]*)', d)
        if romaji_title:
            romaji_title = romaji_title.group(1)
        else:
            romaji_title = ""
            
        synopsis = re.search('"synopsis":"(.*?)","', d)
        if synopsis:
            synopsis = synopsis.group(1).replace("\\\"", "\"")
        else:
            synopsis = ""
            
        #genres = re.search('"genres":(\[[^\]]*?\])', d)
        #if genres:
        #    genres = genres.group(1)
        #else:
        #    genres = ""
            
        animetype = re.search('type":"([^"]*?)"', d)
        if animetype:
            animetype = animetype.group(1)
            
        finished_airing = re.search('finished_airing":"([^"]*?)"', d)
        if finished_airing:
            finished_airing = finished_airing.group(1)
            status = 2
        
        else:
            if animetype != "TV":
               status = 2
            
        started_airing = re.search('started_airing":"([^"]*?)"', d)
        if started_airing:
            started_airing = started_airing.group(1)
            if status != 2:
                status = 1
        else:       
            status = 0
                    
        episode_count = re.search('episode_count":(\d+)\D', d)
        if episode_count:
            episode_count = episode_count.group(1)
        else:
            episode_count = ""
            
        poster_image = re.search('poster_image":"([^"]*?)"', d)
        if poster_image:
            poster_image = poster_image.group(1)
        else:
            poster_image = ""
        
        if id:
            db.query("""select hbid from malData where hbid = """ + id)
            r = db.store_result()
            r = str(r.fetch_row())
            r = re.search('\(\((\d+)\D*', r).group(1)
            if type(id) == "Str": 
                if r != id:       
                    cur.execute("""
                        INSERT INTO malData (hbID, title, english_title, romaji_title, episodes, status, startdate, enddate, image, synopsis, type) VALUES (%s,%s,%s,%s,%s,%s,%s,%s,%s,%s,%s) """,(str(id), str(title), str(english_title), str(romaji_title), str(episode_count), str(status), (started_airing if started_airing != "" else None), (finished_airing if finished_airing != "" else None), str(poster_image), str(synopsis), str(animetype)))
        
        
                    db.commit()
            
                    print id + ", " + i
hummingget(1,50000)
