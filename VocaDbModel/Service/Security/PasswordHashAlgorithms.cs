namespace VocaDb.Model.Service.Security {

	public static class PasswordHashAlgorithms {

		public static IPasswordHashAlgorithm Default => Get(PasswordHashAlgorithmType.HMACSHA1);

		public static IPasswordHashAlgorithm Get(PasswordHashAlgorithmType algorithm) {
			switch (algorithm) {
				case PasswordHashAlgorithmType.SHA1:
					return new SHA1PasswordHashAlgorithm();
				case PasswordHashAlgorithmType.HMACSHA1:
					return new HMACSHA1PasswordHashAlgorithm();
			}
			return null;
		}

	}

	public enum PasswordHashAlgorithmType {
		SHA1,
		HMACSHA1
	}

}
