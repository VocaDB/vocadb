#nullable disable

using System;
using System.Linq;
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

			Assert.IsTrue(_tag.RelatedTags.Any(t => t.LinkedTag.Equals(_tag2)), "Related tag was added from the owner side");
			Assert.IsTrue(_tag2.RelatedTags.Any(t => t.LinkedTag.Equals(_tag)), "Owner tag was added from the linked side");
		}

		[TestMethod]
		public void Delete()
		{
			var relatedTag = _tag2;
			_tag.AddRelatedTag(relatedTag);

			_tag.Delete();

			Assert.IsFalse(relatedTag.RelatedTags.Any(t => t.LinkedTag.Equals(_tag)), "Tag was removed from the linked side");
		}

		[TestMethod]
		public void SetParent()
		{
			_tag.SetParent(_tag2);

			Assert.AreEqual(_tag2, _tag.Parent, "Parent");
			Assert.IsTrue(_tag2.Children.Contains(_tag), "Child tag was added for parent tag");
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

			Assert.AreEqual(1, result.Added.Length, "Number of added items");
			Assert.AreEqual(1, result.Unchanged.Length, "Number of unchanged items");
			Assert.AreEqual(0, result.Removed.Length, "Number of removed items");

			Assert.AreEqual(2, _tag.RelatedTags.Count, "Number of related tags for tag1");
			Assert.AreEqual(1, _tag2.RelatedTags.Count, "Number of related tags for tag2");
		}
	}
}
