
module vdb.helpers {
	
	export class EntryMergeValidationHelper {
		
		public static validate(baseStatus: string, targetStatus: string, baseCreated: string, targetCreated: string) {
			return {
				validationError_targetIsLessComplete: moment(targetCreated) <= moment(baseCreated) && models.EntryStatus[targetStatus] === models.EntryStatus.Draft && models.EntryStatus[baseStatus] > models.EntryStatus.Draft,
				validationError_targetIsNewer: !(models.EntryStatus[targetStatus] > models.EntryStatus.Draft && models.EntryStatus[baseStatus] === models.EntryStatus.Draft) && moment(targetCreated) > moment(baseCreated)
			};
		}

		public static validationError_targetIsLessComplete = (baseStatus: string, targetStatus: string) =>
			models.EntryStatus[targetStatus] < models.EntryStatus[baseStatus];

		public static validationError_targetIsNewer = (baseCreated: string, targetCreated: string) =>
			moment(targetCreated) > moment(baseCreated);

	}

	export interface IEntryMergeValidationResult {
		validationError_targetIsLessComplete: boolean;
		validationError_targetIsNewer: boolean;
	}

}