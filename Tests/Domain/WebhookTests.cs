using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain;

namespace VocaDb.Tests.Domain
{
	[TestClass]
	public sealed class WebhookContentTypeTests
	{
		[DataRow("Form", nameof(WebhookContentType.Form))]
		[DataRow("Json", nameof(WebhookContentType.Json))]
		[TestMethod]
		public void Name(string expected, string actual)
		{
			actual.Should().Be(expected);
		}
	}

	[TestClass]
	public sealed class WebhookEventsTests
	{
		[DataRow(0, WebhookEvents.Default)]
		[DataRow(1, WebhookEvents.User)]
		[DataRow(2, WebhookEvents.EntryReport)]
		[TestMethod]
		public void Value(int expected, WebhookEvents actual)
		{
			((int)actual).Should().Be(expected);
		}
	}
}
