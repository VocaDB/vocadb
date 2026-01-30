nicovideo-apiclient-dotnet
==========================

.NET Standard 2.0 library that provides basic functionality for querying the [NicoNicoDouga (ニコニコ動画)](https://nicovideo.jp/) XML API.

Currently includes basic methods for NND URL parsing and downloading some video and user information.

Uses [Html Agility Pack](http://htmlagilitypack.codeplex.com/) for HTML parsing.

### Installing

```
PM> Install-Package VocaDb.NicoApiClient
```

### Usage

```csharp
var response = await new NicoApiClient().ParseByUrlAsync("http://www.nicovideo.jp/watch/sm33779210");
```
