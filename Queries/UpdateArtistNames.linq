<Query Kind="Statements" />

foreach (var artist in Artists) {

	artist.ArtistNames.Add(new ArtistNames { Artist = artist.Id, Language = "Japanese", Value = artist.JapaneseName });
	artist.ArtistNames.Add(new ArtistNames { Artist = artist.Id, Language = "Romaji", Value = artist.RomajiName });
	artist.ArtistNames.Add(new ArtistNames { Artist = artist.Id, Language = "English", Value = artist.EnglishName });

	foreach (var name in artist.ArtistMetadatas.Where(m => m.Type == "AlternateName")) {
	
		artist.ArtistNames.Add(new ArtistNames { Artist = artist.Id, Language = "Japanese", Value = name.Value });
	
	}
		
	artist.ArtistNames.Dump();

}

SubmitChanges();