
module vdb.helpers {
	
	export class DateTimeHelper {
		
		private static addLeadingZero(val) {
			return (val < 10 ? "0" + val : val);
		}

		public static converToLocal(utcDate: Date) {
			const momentDate = moment.utc(utcDate);
			return new Date(momentDate.year(), momentDate.month(), momentDate.date());
			//return new Date(utcDate.getFullYear(), utcDate.getMonth(), utcDate.getDate());
		}

		public static convertToUtc(localDate: Date) {
			return moment.utc([localDate.getFullYear(), localDate.getMonth(), localDate.getDate()]).toDate();
		}

		// Formats seconds as minutes and seconds, for example 12:34
		public static formatFromSeconds = (seconds: number) => {
			
			var mins = Math.floor(seconds / 60);
			return mins + ":" + DateTimeHelper.addLeadingZero(seconds % 60);

		}

	}

}