using System.IO;

namespace VocaDb.NicoApi.Tests.TestData {

    public static class TestDataHelper {

        public static Stream GetFileStream(string fileName) {

            var asm = typeof(TestDataHelper).Assembly;
            return asm.GetManifestResourceStream($"{typeof(TestDataHelper).Namespace}.{fileName}");

        }

        public static Stream NicoResponseError => GetFileStream("NicoResponse_Error.xml");

        public static Stream NicoResponseOk => GetFileStream("NicoResponse_Ok.xml");

    }

}
