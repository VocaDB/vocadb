
module vdb.utils {

    import c = vdb.models;

    export class WebLinkMatcher {
        
		static matchers: WebLinkMatcher[] = [
			{ url: "www.5sing.com/", desc: "5SING", cat: c.WebLinkCategory.Official },
			{ url: "about.me/", desc: "about.me", cat: c.WebLinkCategory.Official },
			{ url: "www.akibaoo.com/", desc: "Akibaoo", cat: c.WebLinkCategory.Commercial },
            { url: "alice-books.com/", desc: "Alice Books", cat: c.WebLinkCategory.Commercial },
            { url: "www.amazon.co.jp/", desc: "Amazon", cat: c.WebLinkCategory.Commercial },
            { url: "www.amazon.com/", desc: "Amazon", cat: c.WebLinkCategory.Commercial },
            { url: "ameblo.jp/", desc: "Blog", cat: c.WebLinkCategory.Official },
            { url: "www.amiami.com/", desc: "AmiAmi", cat: c.WebLinkCategory.Commercial },
            { url: "anidb.net/", desc: "AniDB", cat: c.WebLinkCategory.Reference },
            { url: "www.animate-onlineshop.jp/", desc: "Animate Online Shop", cat: c.WebLinkCategory.Commercial },
            { url: "bandcamp.com", desc: "Bandcamp", cat: c.WebLinkCategory.Commercial },
			{ url: "beatport.com", desc: "Beatport", cat: c.WebLinkCategory.Commercial },
			{ url: "bilibili.com/", desc: "Bilibili", cat: c.WebLinkCategory.Official },
            { url: "bilibili.tv/", desc: "Bilibili", cat: c.WebLinkCategory.Official },
			{ url: "booth.pm/", desc: "Booth", cat: c.WebLinkCategory.Commercial },
            { url: "www.cdjapan.co.jp/detailview.html", desc: "CDJapan", cat: c.WebLinkCategory.Commercial },
			{ url: "www.cdjapan.co.jp/product/", desc: "CDJapan", cat: c.WebLinkCategory.Commercial },
			{ url: "creofuga.net/", desc: "Creofuga", cat: c.WebLinkCategory.Official },
			{ url: "d-stage.com/shop/", desc: "D-Stage", cat: c.WebLinkCategory.Commercial },
            { url: "deviantart.com/", desc: "DeviantArt", cat: c.WebLinkCategory.Official },
            { url: "www.discogs.com/", desc: "Discogs", cat: c.WebLinkCategory.Reference },
			{ url: "reference.discogslabs.com/", desc: "Discogs", cat: c.WebLinkCategory.Reference },
            { url: "exittunes.com/", desc: "Exit Tunes", cat: c.WebLinkCategory.Official },
            { url: "www.facebook.com/", desc: "Facebook", cat: c.WebLinkCategory.Official },
			{ url: "plus.google.com", desc: "Google Plus", cat: c.WebLinkCategory.Official },
			{ url: "shop.fasic.jp/", desc: "fasic", cat: c.WebLinkCategory.Commercial }, /* UtaiteDB */
            { url: ".web.fc2.com", desc: "Website", cat: c.WebLinkCategory.Official },
            { url: ".fc2.com", desc: "Blog", cat: c.WebLinkCategory.Official },
			{ url: "play.google.com/", desc: "Google Play", cat: c.WebLinkCategory.Commercial },
            { url: "instagram.com/", desc: "Instagram", cat: c.WebLinkCategory.Official },
            { url: "itunes.apple.com/us/", desc: "iTunes (US)", cat: c.WebLinkCategory.Commercial },
            { url: "itunes.apple.com/jp/", desc: "iTunes (JP)", cat: c.WebLinkCategory.Commercial },
            { url: "itunes.apple.com/", desc: "iTunes", cat: c.WebLinkCategory.Commercial },
            { url: "karent.jp/", desc: "KarenT", cat: c.WebLinkCategory.Commercial },
			{ url: "last.fm/user/", desc: "Last.fm profile", cat: c.WebLinkCategory.Official },
            { url: "last.fm/", desc: "Last.fm", cat: c.WebLinkCategory.Reference },
			{ url: "listography.com", desc: "Listography", cat: c.WebLinkCategory.Official },
            { url: "listen.jp/store/", desc: "Listen Japan", cat: c.WebLinkCategory.Commercial },
			{ url: "www.instagram.com/", desc: "Instagram", cat: c.WebLinkCategory.Official },
            { url: "shop.melonbooks.co.jp/", desc: "Melonbooks", cat: c.WebLinkCategory.Commercial },
			{ url: "www.melonbooks.co.jp/", desc: "Melonbooks", cat: c.WebLinkCategory.Commercial },
			{ url: "www.metal-archives.com/", desc: "Metal Archives", cat: c.WebLinkCategory.Reference },
            { url: "mikumikudance.wikia.com/wiki/", desc: "MikuMikuDance Wiki", cat: c.WebLinkCategory.Reference },
			{ url: "mikumikudance.fandom.com/wiki/", desc: "MikuMikuDance Wiki", cat: c.WebLinkCategory.Reference },
            { url: "www5.atwiki.jp/hmiku/", desc: "MikuWiki", cat: c.WebLinkCategory.Reference },
			{ url: "www.mixcloud.com/", desc: "Mixcloud", cat: c.WebLinkCategory.Official },
            { url: "mora.jp/", desc: "mora", cat: c.WebLinkCategory.Commercial },
			{ url: "mqube.net/user/", desc: "MQube", cat: c.WebLinkCategory.Official },
            { url: "musicbrainz.org/", desc: "MusicBrainz", cat: c.WebLinkCategory.Reference },
			{ url: "www.muzie.ne.jp/", desc: "Muzie", cat: c.WebLinkCategory.Official },
			{ url: "myfigurecollection.net/", desc: "MyFigureCollection", cat: c.WebLinkCategory.Reference },
            { url: "chokuhan.nicovideo.jp/", desc: "NicoNico Chokuhan", cat: c.WebLinkCategory.Commercial },
            { url: "dic.nicovideo.jp/", desc: "NicoNicoPedia", cat: c.WebLinkCategory.Reference },
            { url: "nicovideo.jp/user/", desc: "NND Account", cat: c.WebLinkCategory.Official },
            { url: "com.nicovideo.jp/community/", desc: "NND Community", cat: c.WebLinkCategory.Official },
            { url: "nicovideo.jp/mylist/", desc: "NND MyList", cat: c.WebLinkCategory.Official },
            { url: "nicovideo.jp/tag/", desc: "NND Tag", cat: c.WebLinkCategory.Reference },
			{ url: "otoyapage.jp/user/", desc: "Otoya Page", cat: c.WebLinkCategory.Official },
			{ url: "www.patreon.com/", desc: "Patreon", cat: c.WebLinkCategory.Official },
            { url: "piapro.jp/", desc: "Piapro", cat: c.WebLinkCategory.Official },
			{ url: "www.poppro.cn/", desc: "Poppro", cat: c.WebLinkCategory.Official },
            { url: "www.pixiv.net/", desc: "Pixiv", cat: c.WebLinkCategory.Official },
            { url: "books.rakuten.co.jp/", desc: "Rakuten", cat: c.WebLinkCategory.Commercial },
			{ url: "spotify.com/", desc: "Spotify", cat: c.WebLinkCategory.Commercial },
			{ url: "soundcloud.com/", desc: "SoundCloud", cat: c.WebLinkCategory.Official },
			{ url: "www.suruga-ya.jp/", desc: "Suruga-ya", cat: c.WebLinkCategory.Commercial },
			{ url: "item.taobao.com/item.htm", desc: "Taobao", cat: c.WebLinkCategory.Commercial },
			{ url: "www.lagoa.jp/", desc: "THREE!", cat: c.WebLinkCategory.Commercial }, /* UtaiteDB */
			{ url: "item.taobao.com", desc: "Taobao", cat: c.WebLinkCategory.Commercial },
			{ url: "thwiki.cc/", desc: "THBWiki", cat: c.WebLinkCategory.Reference }, /* TouhouDB */
            { url: "toranoana.jp/mailorder/article/", desc: "Toranoana", cat: c.WebLinkCategory.Commercial },
			{ url: "touhoudb.com/", desc: "TouhouDB", cat: c.WebLinkCategory.Reference },
			{ url: "en.touhouwiki.net/wiki/", desc: "Touhou Wiki", cat: c.WebLinkCategory.Reference },
            { url: "tumblr.com/", desc: "Tumblr", cat: c.WebLinkCategory.Official },
            { url: "twitter.com/", desc: "Twitter", cat: c.WebLinkCategory.Official },
            { url: "twipla.jp/", desc: "TwiPla", cat: c.WebLinkCategory.Official },
			{ url: "utaitedb.net/", desc: "UtaiteDB", cat: c.WebLinkCategory.Reference },
			{ url: "utaulyrics.wikia.com/wiki/", desc: "UTAU Lyrics Wiki", cat: c.WebLinkCategory.Reference },
            { url: "www24.atwiki.jp/utauuuta/", desc: "UTAU Wiki (JP)", cat: c.WebLinkCategory.Reference },
			{ url: "utau.wikia.com/wiki/", desc: "UTAU Wiki", cat: c.WebLinkCategory.Reference },
			{ url: "utau.fandom.com/wiki/", desc: "UTAU Wiki", cat: c.WebLinkCategory.Reference },
			{ url: "utau.wikidot.com/", desc: "UTAU wiki 2.0", cat: c.WebLinkCategory.Reference },
			{ url: "utau.wiki/", desc: "UTAU wiki 2.0", cat: c.WebLinkCategory.Reference },
			{ url: "utaite.wikia.com/", desc: "Utaite Wiki", cat: c.WebLinkCategory.Reference }, /* UtaiteDB */
            { url: "vgmdb.net/", desc: "VGMdb", cat: c.WebLinkCategory.Reference },
            { url: "vimeo.com/", desc: "Vimeo", cat: c.WebLinkCategory.Official },
			{ url: "://vk.com/", desc: "VK", cat: c.WebLinkCategory.Official },
			{ url: "vocadb.net/", desc: "VocaDB", cat: c.WebLinkCategory.Reference },
            { url: "vocaloiders.com/", desc: "Vocaloiders", cat: c.WebLinkCategory.Reference },
			{ url: "vocaloidlyrics.wikia.com/wiki/", desc: "Vocaloid Lyrics Wiki", cat: c.WebLinkCategory.Reference },
			{ url: "vocaloidlyrics.fandom.com/wiki/", desc: "Vocaloid Lyrics Wiki", cat: c.WebLinkCategory.Reference },
            { url: "vocaloid.wikia.com/wiki/", desc: "Vocaloid Wiki", cat: c.WebLinkCategory.Reference },
			{ url: "vocaloid.fandom.com/wiki/", desc: "Vocaloid Wiki", cat: c.WebLinkCategory.Reference },
			{ url: "voiceroid.wikia.com/wiki/", desc: "Voiceroid Wiki", cat: c.WebLinkCategory.Reference },
			{ url: "voiceroid.fandom.com/wiki/", desc: "Voiceroid Wiki", cat: c.WebLinkCategory.Reference },
			{ url: "cevio.wikia.com/wiki/", desc: "CeVIO Wiki", cat: c.WebLinkCategory.Reference },
			{ url: "cevio.fandom.com/wiki/", desc: "CeVIO Wiki", cat: c.WebLinkCategory.Reference },
			{ url: "alterego.wikia.com/wiki/", desc: "Alter/Ego Wiki", cat: c.WebLinkCategory.Reference },
			{ url: "alterego.fandom.com/wiki/", desc: "Alter/Ego Wiki", cat: c.WebLinkCategory.Reference },
			{ url: "www.vocaloid.com/products/", desc: "VOCALOID SHOP (JPN)", cat: c.WebLinkCategory.Commercial },
			{ url: "www.vocaloid.com/en/products/", desc: "VOCALOID SHOP (ENG)", cat: c.WebLinkCategory.Commercial },
			{ url: "vocalotracks.ssw.co.jp/", desc: "Vocalotracks", cat: c.WebLinkCategory.Commercial },
			{ url: "www.vocallective.net/", desc: "Vocallective", cat: c.WebLinkCategory.Official },
			{ url: "weibo.com/", desc: "Weibo", cat: c.WebLinkCategory.Official },
			{ url: "en.wikipedia.org/wiki/", desc: "Wikipedia (EN)", cat: c.WebLinkCategory.Reference },
			{ url: "ja.wikipedia.org/wiki/", desc: "Wikipedia (JP)", cat: c.WebLinkCategory.Reference },
			{ url: "wikipedia.org/wiki/", desc: "Wikipedia", cat: c.WebLinkCategory.Reference },
			{ url: "wixsite.com/", desc: "Website", cat: c.WebLinkCategory.Official },
            { url: "www.yesasia.com/", desc: "YesAsia", cat: c.WebLinkCategory.Commercial },
            { url: "youtube.com/channel/", desc: "YouTube Channel", cat: c.WebLinkCategory.Official },
            { url: "youtube.com/user/", desc: "YouTube Channel", cat: c.WebLinkCategory.Official }
		];

		private static isMatch(url: string, item: WebLinkMatcher) {
			return (url.indexOf(item.url) !== -1);
		}

        public static matchWebLink(url: string): WebLinkMatcher {
            return _.find(WebLinkMatcher.matchers, item => this.isMatch(url, item));
        }

        constructor(public url: string, public desc: string, public cat: vdb.models.WebLinkCategory) {}
    
    }

}
