
module vdb.helpers {
	
	export class KnockoutHelper {
		
		public static stringEnum<T>(observable: KnockoutObservable<T>, enumType: any) {

			return ko.computed({
				read: () => {
					var val: any = observable();
					return enumType[val];
				},
				write: (val: string) => observable(enumType[val])
			});

		}

	}

}