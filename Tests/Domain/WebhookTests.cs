using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain;

namespace VocaDb.Tests.Domain
{
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
