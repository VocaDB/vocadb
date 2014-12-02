namespace VocaDb.Model.Domain.Globalization {

	public interface INameFactory<out T> {

		T CreateName(string val, ContentLanguageSelection language);

	}
}
