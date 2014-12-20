
module vdb.models {

	import dc = vdb.dataContracts;

	export class ResourcesManager {

		constructor(private resourcesRepo: vdb.repositories.ResourceRepository,
			private cultureCode: string) { }

		private setsToLoad = (setNames: string[]) => {
			var missing = _.where(setNames, setName => this.resources[setName] == null);
			return missing;
		}

		public resources: KnockoutObservable<dc.ResourcesContract> = ko.observable({});

		public loadResources = (callback?: () => void, ...setNames: string[]) => {
			var setsToLoad = this.setsToLoad(setNames);
			this.resourcesRepo.getList(this.cultureCode, setsToLoad, resources => {
				_.each(setNames, setName => this.resources()[setName] = resources[setName]);
				this.resources.valueHasMutated();
				if (callback)
					callback();
			});
		}

	}

}