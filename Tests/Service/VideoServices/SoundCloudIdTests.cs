using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Tests.Service.VideoServices
{
	/// <summary>
	/// Tests for <see cref="SoundCloudId"/>.
	/// </summary>
	[TestClass]
	public class SoundCloudIdTests
	{
		[TestMethod]
		public void Ctor_QueryParams()
		{
			var result = new SoundCloudId("3939", "voicetextactor/original-demo-song-ver2017-07-6?in=voicetextactor/sets/all-1");

			Assert.AreEqual("voicetextactor/original-demo-song-ver2017-07-6", result.SoundCloudUrl);
		}
	}
}
