using System;
using System.Linq;
using System.Web.Mvc;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Search.Artists;
using VocaDb.Web.Controllers;
using VocaDb.Web.Controllers.DataAccess;

namespace VocaDb.Web.API.v1.Controllers {

	public class ArtistApiController : Web.Controllers.ControllerBase {

		private const int defaultMax = 10;
		private readonly ArtistQueries queries;
		private readonly ArtistService service;

		public ArtistApiController(ArtistService service, ArtistQueries queries) {
			this.service = service;
			this.queries = queries;
		}

		private ArtistService Service {
			get { return service; }
		}

		public ActionResult ByName(string query, ContentLanguagePreference? lang, int? start, int? maxResults, NameMatchMode? nameMatchMode,
			string callback, DataFormat format = DataFormat.Auto) {

			if (string.IsNullOrEmpty(query))
				return Object(new PartialFindResult<SongForApiContract>(), format, callback);

			var param = new ArtistQueryParams(ArtistSearchTextQuery.Create(query, nameMatchMode ?? NameMatchMode.Exact), new ArtistType[] { }, 0, defaultMax, false, true, ArtistSortRule.Name, false);

			if (start.HasValue)
				param.Paging.Start = start.Value;

			if (maxResults.HasValue)
				param.Paging.MaxEntries = Math.Min(maxResults.Value, defaultMax);

			var songs = Service.FindArtists(s => new ArtistForApiContractOld(s, null, lang ?? ContentLanguagePreference.Default), param);

			return Object(songs, format, callback);

		}

		public ActionResult Versions(DataFormat format = DataFormat.Auto) {

			var versions = queries
				.HandleQuery(ctx => ctx.Query()
					.Where(a => !a.Deleted)
					.Select(a => new { a.Id, a.Version })
					.ToArray()
					.Select(v => new EntryIdAndVersionContract(v.Id, v.Version))
					.ToArray());

			return ObjectLowercase(versions, format);

		}

	}
}