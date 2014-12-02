<Query Kind="Statements" />

foreach (var album in Albums) {

	album.AlbumNames.Add(new AlbumNames { Album = album.Id, Language = "Japanese", Value = album.JapaneseName });
	album.AlbumNames.Add(new AlbumNames { Album = album.Id, Language = "Romaji", Value = album.RomajiName });
	album.AlbumNames.Add(new AlbumNames { Album = album.Id, Language = "English", Value = album.EnglishName });

	album.AlbumNames.Dump();

}

SubmitChanges();