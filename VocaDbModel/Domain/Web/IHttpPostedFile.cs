namespace VocaDb.Model.Domain.Web {

	public interface IHttpPostedFile {
        string ContentType { get; }
        string FileName { get; }
        void SaveAs(string path);
	}
}
