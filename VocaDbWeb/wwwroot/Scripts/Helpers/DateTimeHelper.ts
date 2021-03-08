
	export default class DateTimeHelper {
		
		private static addLeadingZero(val) {
			return (val < 10 ? "0" + val : val);
		}

		public static convertToLocal(utcDate: Date) {
			if (utcDate == null)
				return null;
			const momentDate = moment.utc(utcDate);
			return new Date(momentDate.year(), momentDate.month(), momentDate.date());
			//return new Date(utcDate.getFullYear(), utcDate.getMonth(), utcDate.getDate());
		}

		public static convertToUtc(localDate: Date) {
			if (localDate == null)
				return null;
			return moment.utc([localDate.getFullYear(), localDate.getMonth(), localDate.getDate()]).toDate();
		}

		// Formats seconds as minutes and seconds, for example 12:34
		public static formatFromSeconds = (seconds: number) => {
			
			var mins = Math.floor(seconds / 60);
			return mins + ":" + DateTimeHelper.addLeadingZero(seconds % 60);

		}

		public static parseToSeconds = (formatted: string): number => {
			var parts = formatted.split(":");
			if (parts.length == 2 && parseInt(parts[0], 10) != NaN && parseInt(parts[1], 10) != NaN) {
				return parseInt(parts[0], 10) * 60 + parseInt(parts[1], 10);
			} else if (parts.length == 1 && !isNaN(parseInt(parts[0], 10))) {
				return parseInt(parts[0], 10);
			} else {
				return 0;
			}
		}

	}