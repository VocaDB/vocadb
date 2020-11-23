using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.QueryableExtenders;

namespace VocaDb.Tests.Service.QueryableExtenders
{
	/// <summary>
	/// Tests for <see cref="GenericQueryableExtender"/>
	/// </summary>
	public class GenericQueryableExtenderTests
	{
		[TestClass]
		public class SelectObject
		{
			class Song
			{
				public string Name { get; set; }
				public int Length { get; set; }
				public string Description { get; set; }
				public string Artists { get; set; }
			}

			class SongDto
			{
				public string Name { get; set; }
				public int Length { get; set; }
			}

			class SongDto2
			{
				public string Name { get; set; }
				public double Length { get; set; }
			}

			private readonly IQueryable<Song> songs = new[] {
				new Song { Name = "Nebula", Length = 3939, Description = "Cool Mikuelectro", Artists = "Tripshots feat. Miku" },
				new Song { Name = "Rise to Eternity", Length = 39, Description = "Gumimetal", Artists = "A-DASH feat. Gumi" },
			}.AsQueryable();

			[TestMethod]
			public void MapToSubset()
			{
				var result = songs.SelectObject<Song, SongDto>().ToArray();

				Assert.AreEqual(2, result.Length, "Number of results");
				Assert.AreEqual("Nebula", result[0].Name, "First song name");
				Assert.AreEqual(3939, result[0].Length, "First song length");
			}

			[TestMethod]
			public void IgnoreIncompatibleProperties()
			{
				var result = songs.SelectObject<Song, SongDto2>().ToArray();

				Assert.AreEqual(2, result.Length, "Number of results");
				Assert.AreEqual("Nebula", result[0].Name, "First song name");
				Assert.AreEqual(0, result[0].Length, "First song length");
			}
		}
	}
}
