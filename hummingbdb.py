import Unirest
import re
import MySQLdb

db = MySQLdb.connect(host="192.168.20.120", # your host, usually localhost
                     user="user", # your username
                      passwd="passwd", # your password
                      db="hbdb") # name of the data base
cur = db.cursor()

def hummingget(urlid):
    
    status = 3

    response = Unirest.get("https://vikhyat-hummingbird-v2.p.mashape.com/anime/" + urlid,
    headers={"X-Mashape-Key": "XrBhpdN9HFmshrHK2eOHy3bwyQ9Op1UuTmzjsnbXhNt6OiTYLL"}
    )
    d = response.raw_body

    id = re.search('id":(\d+),', d)
    if id:
        id = id.group(1)
    else:
        id = None

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

    slug = re.search('"slug":"([^"]*)', d)
    if slug:
        slug = slug.group(1)
    else:
        slug = ""

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
        episode_count = 0

    poster_image = re.search('poster_image":"([^"]*?)"', d)
    if poster_image:
        poster_image = poster_image.group(1)
    else:
        poster_image = ""

    if isinstance(id, (int, str)):

        db.query("""select hbid from malData where hbid = """ + id)
        r = db.store_result()
        r = str(r.fetch_row())
        r = re.search('\(\((\d+)\D*', r)
        if r:
            r = r.group(1)


        if r != id:       
            cur.execute("""
                INSERT INTO malData (hbID, title, english_title, romaji_title, slug, episodes, status, startdate, enddate, image, synopsis, type) VALUES (%s,%s,%s,%s,%s,%s,%s,%s,%s,%s,%s,%s) """,(str(id), str(title), str(english_title), str(romaji_title), str(slug), str(episode_count), str(status), (started_airing if started_airing != "" else None), (finished_airing if finished_airing != "" else None), str(poster_image), str(synopsis), str(animetype)))


            #db.commit()

            print id + " " + title + " " + slug
    else:
        print "no data"

#get all anime, takes some time, around 8400 titles as of 2014-07-31
def humminginitialise():
    for n in range(1, 9000):
        hummingget(n)

#humminginitialise()



def hummingcheck():
    
    cur.execute("select max(hbid) from maldata")
    max = cur.fetchone()
    max = max[0]

    cur.execute("select hbid from maldata")
    nums = cur.fetchall()
    nums = list(nums)
    nums = [x[0] for x in nums]
    
    missing_anime = set(range(1, max - 1)).difference(set(nums))

    for n in missing_anime:
        hummingget(str(n))

# Checks Hummingbird API for missing anime in the gaps
#hummingcheck()



def hummingupdate():
    cur.execute("select max(hbid) from maldata")
    max = cur.fetchone()
    max = max[0] + 1
    for n in range(max, max + 5):
        hummingget(str(n))
        
# Checks Hummingbird API for 5 new shows after the max    
#hummingupdate()
