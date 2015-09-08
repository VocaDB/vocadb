
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
            { url: "bilibili.tv/", desc: "Bilibili", cat: c.WebLinkCategory.Official },
			{ url: "booth.pm/", desc: "Booth", cat: c.WebLinkCategory.Commercial },
            { url: "www.cdjapan.co.jp/detailview.html", desc: "CDJapan", cat: c.WebLinkCategory.Commercial },
			{ url: "www.cdjapan.co.jp/product/", desc: "CDJapan", cat: c.WebLinkCategory.Commercial },
			{ url: "d-stage.com/shop/", desc: "D-Stage", cat: c.WebLinkCategory.Commercial },
            { url: "deviantart.com/", desc: "DeviantArt", cat: c.WebLinkCategory.Official },
            { url: "www.discogs.com/", desc: "Discogs", cat: c.WebLinkCategory.Reference },
            { url: "exittunes.com/", desc: "Exit Tunes", cat: c.WebLinkCategory.Official },
            { url: "www.facebook.com/", desc: "Facebook", cat: c.WebLinkCategory.Official },
			{ url: "shop.fasic.jp/", desc: "fasic", cat: c.WebLinkCategory.Commercial }, /* UtaiteDB */
            { url: ".web.fc2.com", desc: "Website", cat: c.WebLinkCategory.Official },
            { url: ".fc2.com", desc: "Blog", cat: c.WebLinkCategory.Official },
			{ url: "play.google.com/", desc: "Google Play", cat: c.WebLinkCategory.Commercial },
            { url: "itunes.apple.com/", desc: "iTunes", cat: c.WebLinkCategory.Commercial },
            { url: "karent.jp/", desc: "KarenT", cat: c.WebLinkCategory.Commercial },
            { url: "last.fm/", desc: "Last.fm", cat: c.WebLinkCategory.Official },
            { url: "listen.jp/store/", desc: "Listen Japan", cat: c.WebLinkCategory.Commercial },
            { url: "shop.melonbooks.co.jp/", desc: "Melonbooks", cat: c.WebLinkCategory.Commercial },
            { url: "mikudb.com/", desc: "MikuDB", cat: c.WebLinkCategory.Reference },
            { url: "www5.atwiki.jp/hmiku/", desc: "MikuWiki", cat: c.WebLinkCategory.Reference },
            { url: "mora.jp/", desc: "mora", cat: c.WebLinkCategory.Commercial },
            { url: "musicbrainz.org/", desc: "MusicBrainz", cat: c.WebLinkCategory.Reference },
			{ url: "www.muzie.ne.jp/", desc: "Muzie", cat: c.WebLinkCategory.Official },
            { url: "chokuhan.nicovideo.jp/", desc: "NicoNico Chokuhan", cat: c.WebLinkCategory.Commercial },
            { url: "dic.nicovideo.jp/", desc: "NicoNicoPedia", cat: c.WebLinkCategory.Reference },
            { url: "nicovideo.jp/user/", desc: "NND Account", cat: c.WebLinkCategory.Official },
            { url: "com.nicovideo.jp/community/", desc: "NND Community", cat: c.WebLinkCategory.Official },
            { url: "nicovideo.jp/mylist/", desc: "NND MyList", cat: c.WebLinkCategory.Official },
            { url: "piapro.jp/", desc: "Piapro", cat: c.WebLinkCategory.Official },
            { url: "www.pixiv.net/", desc: "Pixiv", cat: c.WebLinkCategory.Official },
            { url: "books.rakuten.co.jp/", desc: "Rakuten", cat: c.WebLinkCategory.Commercial },
			{ url: "spotify.com/", desc: "Spotify", cat: c.WebLinkCategory.Commercial },
			{ url: "soundcloud.com/", desc: "SoundCloud", cat: c.WebLinkCategory.Official },
			{ url: "item.taobao.com/item.htm", desc: "Taobao", cat: c.WebLinkCategory.Commercial },
			{ url: "www.lagoa.jp/", desc: "THREE!", cat: c.WebLinkCategory.Commercial }, /* UtaiteDB */
            { url: "toranoana.jp/mailorder/article/", desc: "Toranoana", cat: c.WebLinkCategory.Commercial },
            { url: "tumblr.com/", desc: "Tumblr", cat: c.WebLinkCategory.Official },
            { url: "twitter.com/", desc: "Twitter", cat: c.WebLinkCategory.Official },
			{ url: "utaitedb.net/", desc: "UtaiteDB", cat: c.WebLinkCategory.Reference },
            { url: "www24.atwiki.jp/utauuuta/", desc: "UTAU wiki (JP)", cat: c.WebLinkCategory.Reference },
			{ url: "utau.wikia.com/wiki/", desc: "UTAU wiki", cat: c.WebLinkCategory.Reference },
			{ url: "utau.wikidot.com/", desc: "UTAU wiki 2.0", cat: c.WebLinkCategory.Reference },
			{ url: "utaite.wikia.com/", desc: "Utaite Wiki", cat: c.WebLinkCategory.Reference }, /* UtaiteDB */
            { url: "vgmdb.net/", desc: "VGMdb", cat: c.WebLinkCategory.Reference },
            { url: "vimeo.com/", desc: "Vimeo", cat: c.WebLinkCategory.Official },
			{ url: "vocadb.net/", desc: "VocaDB", cat: c.WebLinkCategory.Reference },
            { url: "vocaloiders.com/", desc: "Vocaloiders", cat: c.WebLinkCategory.Reference },
            { url: "vocaloid.wikia.com/wiki/", desc: "Vocaloid Wiki", cat: c.WebLinkCategory.Reference },
			{ url: "www.vocallective.net/", desc: "Vocallective", cat: c.WebLinkCategory.Official },
            { url: "www.yesasia.com/", desc: "YesAsia", cat: c.WebLinkCategory.Commercial },
            { url: "youtube.com/channel/", desc: "YouTube Channel", cat: c.WebLinkCategory.Official },
            { url: "youtube.com/user/", desc: "YouTube Channel", cat: c.WebLinkCategory.Official }
        ];

        public static matchWebLink(url: string): WebLinkMatcher {

            return _.find(WebLinkMatcher.matchers, item => (url.indexOf(item.url) != -1));

        }

        constructor(public url: string, public desc: string, public cat: vdb.models.WebLinkCategory) {
        }
    
    }

}