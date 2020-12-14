#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VocaDb.Model.Service.VideoServices.Youtube
{
	public interface IYoutubeItem { }

	public class YoutubeItem<TSnippet> : IYoutubeItem where TSnippet : Snippet
	{
		public TSnippet Snippet { get; set; }
	}
}
