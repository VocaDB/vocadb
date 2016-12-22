
module vdb.models {
	
	// Class for getting URLs of PV service icons.
	export class PVServiceIcons {

		private icons: any;

		constructor(urlMapper: vdb.UrlMapper) {
			
			this.icons = {
				File: urlMapper.mapRelative("/Content/Icons/music.png"),
				LocalFile: urlMapper.mapRelative("/Content/Icons/music.png"),
				NicoNicoDouga: urlMapper.mapRelative("/Content/nico.png"),
				Youtube: urlMapper.mapRelative("/Content/youtube.png"),
				SoundCloud: urlMapper.mapRelative("/Content/Icons/soundcloud.png"),
				Vimeo: urlMapper.mapRelative("/Content/ExtIcons/vimeo.png"),
				Piapro: urlMapper.mapRelative("/Content/ExtIcons/piapro.png"),
				Bilibili: urlMapper.mapRelative("/Content/ExtIcons/bilibili.png"),
				Creofuga: urlMapper.mapRelative("/Content/ExtIcons/creofuga.png")
			};

		}

		getIconUrl = (service: string) => {
			return this.icons[service];
		}

		// Gets icon URLs from a comma separated list of services
		getIconUrls = (servicesStr: string) => {

			if (!servicesStr || servicesStr == "Nothing")
				return [];

			var services = servicesStr.split(',');
			return _.map(services, service => {

				var trimmed = service.trim();

				return {
					service: trimmed,
					url: this.icons[trimmed]					
				}
			});

		}

	}

}