using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VocaDb.Model.Service.BBCode {

	public interface IBBCodeElementTransformer {

		void ApplyTransform(StringBuilder bbCode);

	}

}
