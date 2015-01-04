using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NLog;
using VocaDb.Model.Service.BrandableStrings.Collections;
using VocaDb.Model.Utils;

namespace VocaDb.Model.Service.BrandableStrings {

	public class BrandableStringsManager {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		private bool LoadBrandedStrings() {
			
			var brandedStringsAssembly = AppConfig.BrandedStringsAssembly;

			if (string.IsNullOrEmpty(brandedStringsAssembly))
				return false;
			
			Assembly assembly;

			try {
				assembly = Assembly.LoadFrom(brandedStringsAssembly);
			} catch (FileNotFoundException) {
				log.Warn("Branded strings assembly '{0}' not found.", brandedStringsAssembly);
				return false;
			}
			
			var headerType = assembly.GetTypes().FirstOrDefault(t => t.GetInterface("IBrandedStringsAssemblyHeader") != null);

			if (headerType == null) {
				log.Warn("No header type found in branded strings assembly.");				
				return false;
			}

			var header = (IBrandedStringsAssemblyHeader)Activator.CreateInstance(headerType);

			Album = header.Album;
			Artist = header.Artist;
			Home = header.Home;
			Layout = header.Layout;
			Song = header.Song;

			return true;

		}

		public BrandableStringsManager() {

			if (!LoadBrandedStrings()) {
				Album = new AlbumStrings(Resources.Views.AlbumRes.ResourceManager);				
				Artist = new ArtistStrings(Resources.Views.ArtistRes.ResourceManager);				
				Home = new HomeStrings(Resources.Views.HomeRes.ResourceManager);				
				Layout = new LayoutStrings(Resources.Views.LayoutRes.ResourceManager);				
				Song = new SongStrings(Resources.Views.SongRes.ResourceManager);				
			}

		}

		public AlbumStrings Album { get; private set; }

		public ArtistStrings Artist { get; private set; }

		public HomeStrings Home { get; private set; }
	
		public LayoutStrings Layout { get; private set; }

		public SongStrings Song { get; private set; }

	}
}
