# metacritic-scraper
Metacritic Scraper is a library that scrapes data about albums, tv shows, movies and persons. These data include user/critic review, review counts, credits, image url for the item thumbnail, etc.

This project is built using the .Net Framework v 4.6.1

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
/<mediaType>/<title-separated-by-a-dash>
/<mediaType>/<title-separated-by-a-dash>/<year>
/<mediaType>/<title-separated-by-a-dash>/<details>
/<mediaType>/<title-separated-by-a-dash>/<year>/<details>
/person/<name-separated-by-a-dash>/<mediaType>

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

This is from an API that I am building using this project.
