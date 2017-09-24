# metacritic-scraper
Metacritic Scraper is a library that scrapes data about albums, tv shows, movies and persons. These data include user/critic review, review counts, credits, image url for the item thumbnail, etc.

This project is built using the .Net Framework v 4.6.1

# API
Check the API built using this library at https://github.com/marcalencc/metacritic-api/blob/master/README.md

# NuGet
* > https://www.nuget.org/packages/MetacriticScraper

# How to Use
Create an instance of WebScraper class

```c#
IScraper m_metacriticScraper = new WebScraper(ResponseChannel, limit);
```

* 'limit' is the number of request that can be processed simultaneously.
* 'ResponseChannel' is the callback that will receive all the responses. It follows the following signature:

```c#
Action<string, IMetacriticData[]>
```

-----------------
To add a request, call the 'AddItem' function.

```c#
 m_metacriticScraper.AddItem(id, url);
```

* 'id' is a user provided string that can be used to identify which responses are from which requests.
* 'url' can be any of the following:

```
/search/<dash-separated-search-string>/<mediaType>
* Multiple search strings (e.g. 'awake-dead') means all the strings should appear in the media item title.
* You can specify limit and offset on search requests. Default and maximum limit is 20. Default offset is 0. (see below)

/search/<dash-separated-search-string>/<mediaType>?limit=15&offset=21
/search/<dash-separated-search-string>/<mediaType>?offset=15
/search/<dash-separated-search-string>/<mediaType>?limit=10

/person/<dash-separated-name>/<mediaType>

* Search and person responses include an Id field which can be used in album, tvshow and movie requests (see below)
Sample Search Response: 
         {  
            "Id":"/movie/amityville-the-awakening",
            "Title":"Amityville: The Awakening",
            "ReleaseDate":"PG-13",
            "Genre":"Thriller, Horror",
            "Rating":{  
               "CriticRating":0
            }
         }
         
Sample Person Response:
          {  
            "Credit":"Primary Artist",
            "Item":{  
               "Id":"/album/bangerz",
               "Title":"Bangerz",
               "ReleaseDate":"10/08/2013",
               "Rating":{  
                  "CriticRating":61,
                  "UserRating":6.9
               }
            }
         }

* Alternatively, just plug in the dash separated title on the media requests

/<mediaType>/<dash-separated-title>
/<mediaType>/<dash-separated-title>/<year>
/<mediaType>/<dash-separated-title>/<details>
/<mediaType>/<dash-separated-title>/<year>/<details>

<mediaType> = album|tvshow|movie
<year> = specify the release year e.g (2015) or season for tv shows e.g. (6) to filter the results
<details> = to get the item credits instead of the item info and reviews

Note: If the name or title has a '-', prefix it with a '~' e.g (/person/jay~-z/album)
```

-----------------
All responses will be routed to the callback set during instantiation.

```c#
        public void ResponseChannel(string id, IMetacriticData[] responses)
        {
                ...
        }
```

* id is the id set when the request is added so be sure to keep track of them
* responses is an array of responses of type IMetacriticData. It can be an album, tvshow, movie, person or an error.

# Sample Implementation
For a complete sample implementation, check <br/> https://github.com/marcalencc/metacritic-api/blob/master/MetacriticAPI/Services/MetacriticService.cs <br/>

This is from an API that I built using this project.
