#nullable disable

using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Service.Search;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.Service.Search
{
	/// <summary>
	/// Tests for <see cref="ReleaseEventSearch"/>.
	/// </summary>
	[TestClass]
	public class ReleaseEventSearchTests
	{
		private ReleaseEvent _eventInSeries;
		private ReleaseEvent _unsortedEvent;
		private QuerySourceList _querySource;
		private ReleaseEventSeries _series;
		private int _eventId;
		private ReleaseEventSearch _target;

		private void AreEqual(ReleaseEvent expected, ReleaseEventFindResultContract actual)
		{
			actual.Should().NotBeNull("Result");
			actual.EventName.Should().Be(expected.DefaultName, "EventName");
			actual.EventId.Should().Be(expected.Id, "EventId");
		}

		private ReleaseEvent CreateEvent(ReleaseEventSeries series, int number, string suffix = "")
		{
			var e = CreateEntry.SeriesEvent(series, number, suffix, id: _eventId++);
			_querySource.Add(e);
			series.AllEvents.Add(e);

			return e;
		}

		private ReleaseEvent CreateEvent(string name)
		{
			var e = CreateEntry.ReleaseEvent(name, id: _eventId++);
			_querySource.Add(e);

			return e;
		}

		private ReleaseEventSeries CreateSeries(params string[] aliases)
		{
			var s = new ReleaseEventSeries(ContentLanguageSelection.English, aliases.Select(a => new LocalizedString(a, ContentLanguageSelection.English)).ToArray(), string.Empty);
			_querySource.Add(s);

			return s;
		}

		[TestInitialize]
		public void SetUp()
		{
			_querySource = new QuerySourceList();

			_target = new ReleaseEventSearch(_querySource);

			_series = CreateSeries("Comiket", "C", "c", "Comic Market");

			_eventInSeries = CreateEvent(_series, 84);
			_unsortedEvent = CreateEvent("Vocaloid Festa");
		}

		private ReleaseEventFindResultContract Find(string query)
		{
			return _target.Find(query, ContentLanguagePreference.English);
		}

		/// <summary>
		/// Test preconditions
		/// </summary>
		[TestMethod]
		public void Ctor()
		{
			_eventInSeries.DefaultName.Should().Be("Comiket 84", "Name");
			_unsortedEvent.DefaultName.Should().Be("Vocaloid Festa", "Name");
		}

		/// <summary>
		/// Find by combined series and event.
		/// </summary>
		[TestMethod]
		public void FindSeriesAndEvent()
		{
			var result = Find("Comiket 84");

			AreEqual(_eventInSeries, result);
		}

		/// <summary>
		/// Find by alias.
		/// </summary>
		[TestMethod]
		public void FindAlias()
		{
			var result = Find("C84");

			AreEqual(_eventInSeries, result);
		}

		/// <summary>
		/// Find by series, unknown event.
		/// </summary>
		[TestMethod]
		public void FindNewEventEventInSeriesExact()
		{
			var result = Find("Comiket 85");

			result.Should().NotBeNull("Result");
			result.Series.Should().NotBeNull("Series");
			result.Series.Name.Should().Be("Comiket", "Series");
			result.SeriesNumber.Should().Be(85, "SeriesNumber");
		}

		/// <summary>
		/// Find by combined series and event, partial match.
		/// </summary>
		[TestMethod]
		public void FindSeriesAndEventPartial()
		{
			var voMas = CreateSeries("The Voc@loid M@ster");
			var voMas23 = CreateEvent(voMas, 23);
			var result = Find("Voc@loid M@ster 23");

			AreEqual(voMas23, result);
		}

		/// <summary>
		/// Find event, part of a series, but the series isn't added yet.
		/// </summary>
		[TestMethod]
		public void FindUnknownSeries()
		{
			// Note: earlier the "c" in this name matched with Comiket's "c", causing Comiket to be returned as the series.
			var result = Find("Gackpoid's birthday 2011");

			// Note: could also return assumed series and allow creating the series as well. Right now, only an ungrouped event can be created.
			result.Should().NotBeNull("Result");
			result.Series.Should().Be(null, "Series"); // Series not found
			result.EventName.Should().Be("Gackpoid's birthday 2011", "EventName");
		}

		/// <summary>
		/// Find by series whose name contains a number.
		/// TODO: this doesn't work yet - series name with numbers is supported only for known events
		/// See https://code.google.com/p/vocadb/issues/detail?id=164
		/// </summary>
		[TestMethod]
		public void FindSeriesWithNumber()
		{
			CreateSeries("M3");

			var result = Find("M3 2013");

			result.Should().NotBeNull("Result");
			result.Series.Should().NotBeNull("Series");
			result.Series.Name.Should().Be("M3", "Series");
			result.SeriesNumber.Should().Be(2013, "SeriesNumber");
		}

		/// <summary>
		/// Find by series whose name contains a number.
		/// TODO: this doesn't work yet - series name with numbers is supported only for known events
		/// See https://code.google.com/p/vocadb/issues/detail?id=164
		/// </summary>
		[TestMethod]
		public void FindSeriesWithNumberAndSuffix()
		{
			CreateSeries("M3");

			var result = Find("M3 2013 Spring");

			result.Should().NotBeNull("Result");
			result.Series.Should().NotBeNull("Series");
			result.Series.Name.Should().Be("M3", "Series");
			result.SeriesNumber.Should().Be(2013, "SeriesNumber");
			result.SeriesSuffix.Should().Be("Spring", "SeriesSuffix");
		}

		/// <summary>
		/// Find known event by exact name when the event name contains a suffix.
		/// </summary>
		[TestMethod]
		public void FindSeriesWithNumberAndSuffix_Exact()
		{
			_series = CreateSeries("M3");
			CreateEvent(_series, 2013, "Fall");

			var result = Find("M3 2013 Fall");

			result.Should().NotBeNull("Result");
			result.IsKnownEvent.Should().BeTrue("IsKnownEvent");
			result.EventName.Should().Be("M3 2013 Fall", "EventName");
		}

		/// <summary>
		/// Find by series, unknown event.
		/// </summary>
		[TestMethod]
		public void FindUnsortedEvent()
		{
			var result = Find("The Vocaloid Festa");

			AreEqual(_unsortedEvent, result);
		}

		/// <summary>
		/// Doesn't match any series.
		/// </summary>
		[TestMethod]
		public void FindNothing()
		{
			var result = Find("Does not exist");

			result.Should().NotBeNull("Result");
			result.EventName.Should().Be("Does not exist", "EventName");
		}
	}
}
