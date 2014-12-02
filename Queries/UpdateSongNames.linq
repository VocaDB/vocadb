<Query Kind="Statements" />

foreach (var song in Songs) {

	song.SongNames.Add(new SongNames { Song = song.Id, Language = "Japanese", Value = song.JapaneseName });
	song.SongNames.Add(new SongNames { Song = song.Id, Language = "Romaji", Value = song.RomajiName });
	song.SongNames.Add(new SongNames { Song = song.Id, Language = "English", Value = song.EnglishName });
		
	song.SongNames.Dump();

}

SubmitChanges();