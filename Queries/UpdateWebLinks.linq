<Query Kind="Statements" />

foreach (var artist in Artists) {

	foreach (var data in artist.ArtistMetadatas.Where(m => m.Type == "WebLink")) {
	
		artist.ArtistWebLinks.Add(new ArtistWebLinks { Artist = artist.Id, Description = data.Value, Url = data.Value });
	
	}
		
	artist.ArtistWebLinks.Dump();

}

SubmitChanges();