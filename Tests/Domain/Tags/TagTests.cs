#nullable disable

using System;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Tags;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.Domain.Tags
{
	/// <summary>
	/// Tests for <see cref="Tag"/>.
	/// </summary>
	[TestClass]
	public class TagTests
	{
		private readonly Tag _tag;
		private readonly Tag _tag2;

		public TagTests()
		{
			_tag = CreateEntry.Tag("rock");
			_tag2 = CreateEntry.Tag("metal");
		}

		[TestMethod]
		public void AddRelatedTag()
		{
			_tag.AddRelatedTag(_tag2);

			_tag.RelatedTags.Any(t => t.LinkedTag.Equals(_tag2)).Should().BeTrue("Related tag was added from the owner side");
			_tag2.RelatedTags.Any(t => t.LinkedTag.Equals(_tag)).Should().BeTrue("Owner tag was added from the linked side");
		}

		[TestMethod]
		public void Delete()
		{
			var relatedTag = _tag2;
			_tag.AddRelatedTag(relatedTag);

			_tag.Delete();

			relatedTag.RelatedTags.Any(t => t.LinkedTag.Equals(_tag)).Should().BeFalse("Tag was removed from the linked side");
		}

		[TestMethod]
		public void SetParent()
		{
			_tag.SetParent(_tag2);

			_tag.Parent.Should().Be(_tag2, "Parent");
			_tag2.Children.Contains(_tag).Should().BeTrue("Child tag was added for parent tag");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void SetParent_Self()
		{
			_tag.SetParent(_tag);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void SetParent_Ancestor()
		{
			_tag.SetParent(_tag2);
			_tag2.SetParent(_tag); // Circular parenthood not allowed
		}

		[TestMethod]
		public void SyncRelatedTags()
		{
			var tag3 = CreateEntry.Tag("power metal");
			var repository = new FakeTagRepository(_tag, _tag2, tag3);

			_tag.AddRelatedTag(_tag2);

			var result = _tag.SyncRelatedTags(new[] { _tag2, tag3 }, repository.Load);

			result.Added.Length.Should().Be(1, "Number of added items");
			result.Unchanged.Length.Should().Be(1, "Number of unchanged items");
			result.Removed.Length.Should().Be(0, "Number of removed items");

			_tag.RelatedTags.Count.Should().Be(2, "Number of related tags for tag1");
			_tag2.RelatedTags.Count.Should().Be(1, "Number of related tags for tag2");
		}
	}
}
