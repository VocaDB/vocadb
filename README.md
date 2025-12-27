[![Build status](https://ci.appveyor.com/api/projects/status/dnkxh6qoqcnxfei5/branch/master?svg=true)](https://ci.appveyor.com/project/riipah/vocadb/branch/master)

[VocaDB](https://vocadb.net) is a Vocaloid Database with translated artists, albums, music videos and more. Our goal is to be the most accurate and complete source of Vocaloid discography and artists.

The software is based on [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/) ([.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)), [jQuery](https://jquery.com/), [jQuery UI](https://jqueryui.com/), [Bootstrap](https://getbootstrap.com/2.3.2/), [React](https://reactjs.org/), [MobX](https://mobx.js.org/), and it uses a SQL database through the [NHibernate ORM](https://nhibernate.info/). 
Server side code is written in C#, most of the client side is TypeScript.

The same software is used for [UtaiteDB](https://utaitedb.net/) and [TouhouDB](https://touhoudb.com/).

## How to contribute

Take a look at the code and instructions for setting up the development environment ([Windows](https://wiki.vocadb.net/docs/development/development-environment-windows) or [Linux](https://wiki.vocadb.net/docs/development/development-environment-linux)).

Any bugs and change requests are to be reported here, on the issues tab. 
If you think you could help, take a look at the list of reported issues, 
create a fork and you can fix it there and create a pull request back to the main repository.

## Web API

We have a [comprehensive web API](https://wiki.vocadb.net/docs/development/public-api) for programmatic access. The [full list of available endpoints](https://vocadb.net/swagger/index.html) is documented using OpenAPI. Please take a look if you're interested.
<!--stackedit_data:
eyJoaXN0b3J5IjpbMTU1OTQyODkxOV19
-->
