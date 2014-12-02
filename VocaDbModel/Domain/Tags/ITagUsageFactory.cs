using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VocaDb.Model.Domain.Tags {

	public interface ITagUsageFactory<T> where T : TagUsage {

		T CreateTagUsage(Tag tag);

	}
}
