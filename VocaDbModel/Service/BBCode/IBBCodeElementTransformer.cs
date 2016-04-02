using System.Text;

namespace VocaDb.Model.Service.BBCode {

	public interface IBBCodeElementTransformer {

		void ApplyTransform(StringBuilder bbCode);

	}

}
