
module vdb.helpers {
	
	export class KnockoutHelper {

		public static stringEnum<T>(observable: KnockoutObservable<T>, enumType: any): KnockoutComputed<string> {

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