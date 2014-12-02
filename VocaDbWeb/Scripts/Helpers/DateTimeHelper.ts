
module vdb.helpers {
	
	export class DateTimeHelper {
		
		private static addLeadingZero(val) {
			return (val < 10 ? "0" + val : val);
		}

		// Formats seconds as minutes and seconds, for example 12:34
		public static formatFromSeconds = (seconds: number) => {
			
			var mins = Math.floor(seconds / 60);
			return mins + ":" + DateTimeHelper.addLeadingZero(seconds % 60);

		}

	}

}