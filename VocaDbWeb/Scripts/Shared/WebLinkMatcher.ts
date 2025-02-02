import { WebLinkCategory } from './WebLinkCategory';

/*Links are alphabetized by description with websites and blogs at the end.*/
export class WebLinkMatcher {
	public static matchers: WebLinkMatcher[] = [
		{
			url: '5sing.kugou.com/',
			desc: '5SING',
			cat: WebLinkCategory.Official,
		},
		{
			url: '.5sing.com/',
			desc: '5SING',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'about.me/',
			desc: 'about.me',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'ovs.akbh.jp/',
			desc: 'Akiba Hobby (Overseas)',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: '.akbh.jp/',
			desc: 'Akiba Hobby',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'www.akibaoo.com/',
			desc: 'Akibaoo',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'alice-books.com/',
			desc: 'Alice Books',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'allcpp.cn',
			desc: 'AllCPP',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'alterego.wikia.com/wiki/',
			desc: 'Alter/Ego Wiki',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'alterego.fandom.com/wiki/',
			desc: 'Alter/Ego Wiki',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'music.amazon.co.jp/',
			desc: 'Amazon Music Unlimited (JP)',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'amazon.co.jp/music/',
			desc: 'Amazon Music Unlimited (JP)',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'music.amazon.com/',
			desc: 'Amazon Music Unlimited (US)',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'www.amazon.co.jp/gp/product/',
			desc: 'Amazon MP3 (JP)',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'www.amazon.co.jp/',
			desc: 'Amazon (JP)',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'www.amazon.com/gp/product/',
			desc: 'Amazon MP3 (US)',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: '.amazon.com/',
			desc: 'Amazon (US)',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: '//amazon.com/',
			desc: 'Amazon (US)',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'amazon.de/',
			desc: 'Amazon (DE)',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'amazon.fr/',
			desc: 'Amazon (FR)',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'amazon.co.uk/',
			desc: 'Amazon (UK)',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'amzn.to/',
			desc: 'Amazon',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'www.amiami.com/',
			desc: 'AmiAmi',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'anidb.net/',
			desc: 'AniDB',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'www.animate-onlineshop.jp/',
			desc: 'Animate Online Shop',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'pc.animelo.jp/',
			desc: 'animelo mix',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'animenewsnetwork.com/',
			desc: 'AnimeNewsNetwork',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'anison.info/',
			desc: 'Anison',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'artstation.com',
			desc: 'ArtStation',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'ask.fm/',
			desc: 'ASKfm',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'audiostock.jp/artists/',
			desc: 'Audiostock artist page',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'audiostock.jp/audio/',
			desc: 'Audiostock audio',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'audiostock.jp/',
			desc: 'Audiostock',
			cat: WebLinkCategory.Official,
		},
		{
			url: 's.awa.fm/',
			desc: 'AWA',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'baike.baidu.com/',
			desc: 'Baidu Baike',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'tieba.baidu.com/',
			desc: 'Baidu Tieba',
			cat: WebLinkCategory.Reference,
		},
		{
			url: '.baidu.com/',
			desc: 'Baidu',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'bandcamp.com',
			desc: 'Bandcamp',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'bangumi.tv',
			desc: 'Bangumi',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'bmssearch.net',
			desc: 'BMS Search',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'chii.in',
			desc: 'Bangumi',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'bgm.tv',
			desc: 'Bangumi',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'beatport.com',
			desc: 'Beatport',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'big-up.style/',
			desc: 'BIG UP!',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'bilibili.com/',
			desc: 'Bilibili',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'bilibili.tv/',
			desc: 'Bilibili',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'bilibili.com/opus/',
			desc: 'Bilibili column', /* 专栏 */
			cat: WebLinkCategory.Official,
		},
		{
			url: 'bilibili.com/read/',
			desc: 'Bilibili column', /* 专栏 */
			cat: WebLinkCategory.Official,
		},
		{
			url: 'bilibili.com/audio/',
			desc: 'Bilibili Music',
			cat: WebLinkCategory.Official,
		},
		{
			url: 't.bilibili.com/',
			desc: 'Bilibili post', /* 动态 */
			cat: WebLinkCategory.Official,
		},
		{
			url: 'bsky.app',
			desc: 'Bluesky',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'lofter.com/',
			desc: 'Blog on Lofter',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'bookmate-net.com/',
			desc: 'Bookmate',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'boomplay.com/',
			desc: 'Boomplay',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'booth.pm/',
			desc: 'Booth',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: '.booth.pm',
			desc: 'Booth',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'bowlroll.net',
			desc: 'Bowlroll',
			cat: WebLinkCategory.Official,
		},
		{
			url: '.carrd.co',
			desc: 'Carrd',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'cdjapan.co.jp/',
			desc: 'CDJapan',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'cevio.wikia.com/wiki/',
			desc: 'CeVIO Wiki',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'cevio.fandom.com/wiki/',
			desc: 'CeVIO Wiki',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'atwiki.jp/cevio_synthv/',
			desc: 'CeVIO/SynthV Wiki',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'uta.573.jp/item/',
			desc: 'Chakushin★Uta♪ album',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'uta.573.jp/artist/',
			desc: 'Chakushin★Uta♪ artist',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'uta.573.jp/song/',
			desc: 'Chakushin★Uta♪ song',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'coconala.com',
			desc: 'Coconala',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'creofuga.net/',
			desc: 'Creofuga',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'selection.music.dmkt-sp.jp',
			desc: 'd hits',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'music.dmkt-sp.jp/',
			desc: 'd music',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'dmusic.docomo.ne.jp/album/',
			desc: 'd music album', /* written as "d music" at https://www.docomo.ne.jp/english/service/ */
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'dmusic.docomo.ne.jp/artist/',
			desc: 'd music artist',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'dmusic.docomo.ne.jp/song/',
			desc: 'd music song',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'd-stage.com/shop/',
			desc: 'D-Stage',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: '.deezer.com/',
			desc: 'Deezer',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'deviantart.com/',
			desc: 'DeviantArt',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'www.discogs.com/',
			desc: 'Discogs',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'reference.discogslabs.com/',
			desc: 'Discogs',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'distrokid.com/',
			desc: 'DistroKid',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'diverse.direct/',
			desc: 'Diverse Direct',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'www.dizzylab.net',
			desc: 'Dizzylab',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'www.dlsite.com',
			desc: 'DLsite',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'dojin-music.info',
			desc: 'Dojin Music Info',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'music.douban.com',
			desc: 'douban',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'pc.dwango.jp/',
			desc: 'dwango.jp',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'www.e-onkyo.com/',
			desc: 'e-onkyo music',
			cat: WebLinkCategory.Commercial,
		} /* UtaiteDB */,
		{
			url: '.ebay.com/',
			desc: 'eBay',
			cat: WebLinkCategory.Other,
		},
		{
			url: 'exittunes.com/',
			desc: 'Exit Tunes',
			cat: WebLinkCategory.Official,
		},
		{
			url: '.facebook.com/',
			desc: 'Facebook',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'fantia.jp',
			desc: 'Fantia',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'shop.fasic.jp/',
			desc: 'fasic',
			cat: WebLinkCategory.Commercial,
		} /* UtaiteDB */,
		{
			url: '.flavors.me',
			desc: 'Flavors.me',
			cat: WebLinkCategory.Official,
		},
		{
			url: '//flavors.me',
			desc: 'Flavors.me',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'www.music-flo.com/detail/album/',
			desc: 'FLO album',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'www.music-flo.com/detail/artist/',
			desc: 'FLO artist',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'www.music-flo.com/detail/track/',
			desc: 'FLO song',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'foriio.com',
			desc: 'Foriio',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'gamers.co.jp/',
			desc: 'GAMERS',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'gamers-onlineshop.jp/',
			desc: 'GAMERS',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'genius.com/',
			desc: 'Genius',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'gensokyoradio.net/',
			desc: 'Gensokyo Radio',
			cat: WebLinkCategory.Reference,
		} /* TouhouDB */,
		{
			url: 'goodsrepublic.com',
			desc: 'Goods Republic',
			cat: WebLinkCategory.Other,
		},
		{
			url: 'play.google.com/',
			desc: 'Google Play',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'plus.google.com',
			desc: 'Google Plus',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'grep-shop.com/',
			desc: 'Grep Shop',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'vocaloid.blog120.fc2.com',
			desc: 'Hatsune Miku Miku',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'music.hikaritv.net/',
			desc: 'Hikari TV Music',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'hmv.co.jp',
			desc: 'HMV',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'instagram.com/',
			desc: 'Instagram',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'www.instagram.com/',
			desc: 'Instagram',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'www.ssw.co.jp/products/',
			desc: 'INTERNET Co.,Ltd Product Page',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'www.ssw.jp/products/',
			desc: 'INTERNET Co.,Ltd Product Page',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'itunes.apple.com/us/',
			desc: 'iTunes (US)',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'music.apple.com/us/',
			desc: 'iTunes (US)',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'itunes.apple.com/jp/',
			desc: 'iTunes (JP)',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'music.apple.com/jp/',
			desc: 'iTunes (JP)',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'itunes.apple.com/',
			desc: 'iTunes',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'music.apple.com/',
			desc: 'iTunes',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'itch.io',
			desc: 'Itch.io',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'bbs.ivocaloid.com/',
			desc: 'iVocaloid Forum',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'karent.jp/',
			desc: 'KarenT',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'kkbox.fm/',
			desc: 'KKBOX',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'kkbox.com/',
			desc: 'KKBOX',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'ko-fi.com',
			desc: 'Ko-fi',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'ep.kugou.com',
			desc: 'KuGou',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'kuwo.cn',
			desc: 'Kuwo',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'last.fm/user/',
			desc: 'Last.fm profile',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'last.fm/',
			desc: 'Last.fm',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'lastfm.jp/',
			desc: 'Last.fm (JP)',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'lenen.shoutwiki.com/',
			desc: "Len'en Wiki",
			cat: WebLinkCategory.Reference,
		} /* TouhouDB */,
		{
			url: 'line.me/',
			desc: 'LINE MUSIC',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'lin.ee/',
			desc: 'LINE MUSIC',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'linkcloud.mu/',
			desc: 'LINK CLOUD',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'linktr.ee/',
			desc: 'Linktree',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'listography.com',
			desc: 'Listography',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'listen.jp/store/',
			desc: 'Listen Japan',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'lit.link',
			desc: 'Lit.Link',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'mailto:',
			desc: 'e-mail',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'mandarake.co.jp/',
			desc: 'Mandarake',
			cat: WebLinkCategory.Other,
		},
		{
			url: 'melonbooks.co.jp/',
			desc: 'Melonbooks',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'melonbooks.com/',
			desc: 'Melonbooks',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'www.metal-archives.com/',
			desc: 'Metal Archives',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'mikumikudance.wikia.com/wiki/',
			desc: 'MikuMikuDance Wiki',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'mikumikudance.fandom.com/wiki/',
			desc: 'MikuMikuDance Wiki',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'w.atwiki.jp/hmiku/',
			desc: 'MikuWiki',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'www5.atwiki.jp/hmiku/',
			desc: 'MikuWiki',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'misskey.io',
			desc: 'Misskey',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'www.mixcloud.com/',
			desc: 'Mixcloud',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'mixi.jp',
			desc: 'Mixi',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'mmdelight.blogspot.com',
			desc: 'MMDownload',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'en.moegirl.org/',
			desc: 'Moegirlpedia (EN)',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'zh.moegirl.org',
			desc: 'Moegirlpedia (zh-cn)',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'mojim.com/',
			desc: 'Mojim',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'mora.jp/',
			desc: 'mora',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'mqube.net/user/',
			desc: 'MQube',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'sp-m.mu-mo.net/album/',
			desc: 'mu-mo album',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'sp-m.mu-mo.net/music/',
			desc: 'mu-mo track',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'musescore.com/',
			desc: 'Musescore',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'musicstore.auone.jp/',
			desc: 'Music Store',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'music-book.jp/',
			desc: 'music.jp',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'musicbrainz.org/',
			desc: 'MusicBrainz',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'muzie.ne.jp/',
			desc: 'Muzie',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'muzie.co.jp/',
			desc: 'Muzie',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'myanimelist.net/',
			desc: 'MyAnimeList',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'mysound.jp/',
			desc: 'MySound',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'myspace.com',
			desc: 'MySpace',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'myfigurecollection.net/',
			desc: 'MyFigureCollection',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'nana-music.com',
			desc: 'Nana Music',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'link-map.jp/links/',
			desc: 'narasu',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'music.163.com/#/user/home',
			desc: 'NCM User Homepage',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'music.163.com/user/home',
			desc: 'NCM User Homepage',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'music.163.com/#/artist',
			desc: 'NCM Artist Entry',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'music.163.com/artist',
			desc: 'NCM Artist Entry',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'music.163.com/#/album',
			desc: 'NCM Album Release',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'music.163.com/album',
			desc: 'NCM Album Release',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'music.163.com/#/song',
			desc: 'NCM Song Release',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'music.163.com/song',
			desc: 'NCM Song Release',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'musicstore.sg/',
			desc: 'Singtel musicStore',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'namu.wiki/',
			desc: 'Namu Wiki',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'neowing.co.jp',
			desc: 'Neowing',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'newgrounds.com',
			desc: 'Newgrounds',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'nex-tone.link/',
			desc: 'NexTone.Link',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'nicovideo.jp/watch',
			desc: 'NicoNicoDouga',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'seiga.nicovideo.jp/',
			desc: 'NicoNicoSeiga',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'commons.nicovideo.jp/',
			desc: 'Nicommons',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'chokuhan.nicovideo.jp/',
			desc: 'NicoNico Chokuhan',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'dic.nicovideo.jp/',
			desc: 'NicoNicoPedia',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'nicovideo.jp/user/',
			desc: 'NND Account',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'com.nicovideo.jp/community/',
			desc: 'NND Community',
			cat: WebLinkCategory.Official,
		},
		{
			// matching both
			// https://www.nicovideo.jp/mylist/000 (old URL style) and
   			// https://www.nicovideo.jp/user/000/mylist/000 (new URL style)
			url: '/mylist/',
			desc: 'NND MyList',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'nicovideo.jp/tag/',
			desc: 'NND Tag',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'nicovideo.jp/series/',
			desc: 'NND Series',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'ch.nicovideo.jp',
			desc: 'NND Channel',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'nicolog.jp',
			desc: 'Nicolog',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'store-jp.nintendo.com',
			desc: 'Nintendo eShop (JP)',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'nintendo.com/store/',
			desc: 'Nintendo eShop (US)',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'nodee.net/',
			desc: 'nodee',
			cat: WebLinkCategory.Official,
		},
		{
			url: '.note.com/',
			desc: 'Note',
			cat: WebLinkCategory.Official,
		},
		{
			url: '/note.com/',
			desc: 'Note',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'note.mu/',
			desc: 'Note',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'v-nyappon.net/',
			desc: 'Nyappon',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'music.oricon.co.jp/',
			desc: 'ORICON MUSIC',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'oricon.co.jp/',
			desc: 'ORICON',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'osu.ppy.sh/beatmaps/artists',
			desc: 'osu! Featured Artist Listing',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'osu.ppy.sh/beatmapsets',
			desc: 'osu! Beatmap',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'osu.ppy.sh/users',
			desc: 'osu! Profile',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'osu.ppy.sh/wiki',
			desc: 'osu! Wiki',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'otakurepublic.com',
			desc: 'Otaku Republic',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'ototoy.jp/',
			desc: 'OTOTOY',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'otoyapage.jp/user/',
			desc: 'Otoya Page',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'www.patreon.com/',
			desc: 'Patreon',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'pawoo.net/',
			desc: 'Pawoo',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'peing.net',
			desc: 'Peing',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'piapro.jp/',
			desc: 'Piapro',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'blog.piapro.net/',
			desc: 'Piapro Blog',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'www.pixiv.net/',
			desc: 'Pixiv',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'dic.pixiv.net',
			desc: 'Pixiv Encyclopedia',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'fanbox.cc',
			desc: 'Pixiv Fanbox',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'plurk.com',
			desc: 'Plurk',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'potofu.me',
			desc: 'POTOFU',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'poppro.cn/',
			desc: 'Poppro',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'potune.jp/',
			desc: 'Potune',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'profcard.info/',
			desc: 'Profcard',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'qobuz.com/',
			desc: 'Qobuz',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'y.qq.com',
			desc: 'QQ Music',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'books.rakuten.co.jp/',
			desc: 'Rakuten books',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'music.rakuten.co.jp/',
			desc: 'Rakuten music',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'rakuten.co.jp/',
			desc: 'Rakuten',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'rateyourmusic.com',
			desc: 'Rate Your Music',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'recochoku.jp/',
			desc: 'recochoku',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'remywiki.com/',
			desc: 'RemyWiki',
			cat: WebLinkCategory.Reference,
		},
		{
			url: '.theshop.jp/',
			desc: 'Shop',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'skeb.jp/',
			desc: 'Skeb',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'skima.jp',
			desc: 'Skima',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'spotify.com/',
			desc: 'Spotify',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'sonicwire.com/',
			desc: 'Sonicwire',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'soundcloud.com/',
			desc: 'SoundCloud',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'spirit-of-metal.com',
			desc: 'Spirit of Metal',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'store.steampowered.com',
			desc: 'Steam',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'streamlink.to',
			desc: 'Streamlink',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'www.suruga-ya.jp/',
			desc: 'Suruga-ya',
			cat: WebLinkCategory.Other,
		},
		{
			url: 'www.suruga-ya.com/en',
			desc: 'Suruga-ya (EN)',
			cat: WebLinkCategory.Other,
		},
		{
			url: 'suzuri.jp',
			desc: 'Suzuri',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'synthv.fandom.com/wiki/',
			desc: 'SynthV Wiki',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'tanocstore.net/',
			desc: 'TANO*C STORE',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'item.taobao.com/item.htm',
			desc: 'Taobao',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: '.taobao.com',
			desc: 'Taobao',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'www.lagoa.jp/',
			desc: 'THREE!',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'thwiki.cc/',
			desc: 'THBWiki',
			cat: WebLinkCategory.Reference,
		} /* TouhouDB */,
		{
			url: 'thwbiki.cc/',
			desc: 'THBWiki',
			cat: WebLinkCategory.Reference,
		} /* TouhouDB */,
		{
			url: 'theinterviews.jp/',
			desc: 'The Interviews',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'tidal.com/browse/album/',
			desc: 'TIDAL album',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'tidal.com/browse/artist/',
			desc: 'TIDAL artist',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'tidal.com/browse/track/',
			desc: 'TIDAL track',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'tidal.com/',
			desc: 'TIDAL',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'tiktok.com',
			desc: 'TikTok',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'tinami.com',
			desc: 'TINAMI',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'tinami.jp',
			desc: 'TINAMI',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'tmbox.net/',
			desc: 'TmBox',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'toranoana.jp',
			desc: 'Toranoana',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'toranoana.shop/',
			desc: 'Toranoana',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'touhou.arrangement-chronicle.com',
			desc: 'Touhou Arrangement Chronicle',
			cat: WebLinkCategory.Reference,
		} /* TouhouDB */,
		{
			url: 'touhoudb.com/',
			desc: 'TouhouDB',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'music.tower.jp/',
			desc: 'TOWER RECORDS MUSIC',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'tower.com/',
			desc: 'TOWER RECORDS',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'tower.jp/',
			desc: 'TOWER RECORDS',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: '.atwiki.jp/toho/',
			desc: 'Touhou Doujin CD Wiki',
			cat: WebLinkCategory.Reference,
		} /* TouhouDB */,
		{
			url: '.atwiki.jp/sagararyou/',
			desc: 'Touhou Lyrics Wiki',
			cat: WebLinkCategory.Reference,
		} /* TouhouDB */,
		{
			url: '.atwiki.jp/tohomusicdb/',
			desc: 'Touhou Music DB Wiki',
			cat: WebLinkCategory.Reference,
		} /* TouhouDB */,
		{
			url: 'en.touhouwiki.net/',
			desc: 'Touhou Wiki',
			cat: WebLinkCategory.Reference,
		} /* TouhouDB */,
		{
			url: 'touhou.wikia.com/',
			desc: 'Touhou Wikia',
			cat: WebLinkCategory.Reference,
		} /* TouhouDB */,
		{
			url: 'touhou.fandom.com/',
			desc: 'Touhou Wikia',
			cat: WebLinkCategory.Reference,
		} /* TouhouDB */,
		{
			url: 'tumblr.com',
			desc: 'Tumblr',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'linkco.re/',
			desc: 'TuneCore Japan',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'tunecore.co.jp/',
			desc: 'TuneCore Japan',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'twitch.tv',
			desc: 'Twitch',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'twitter.com/',
			desc: 'Twitter',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'twvt.me',
			desc: 'twinvite',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'twipla.jp/',
			desc: 'TwiPla',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'twpf.jp',
			desc: 'Twpf',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'ustream.tv/',
			desc: 'Ustream',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'utaitedb.net/',
			desc: 'UtaiteDB',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'www.uta-net.com/',
			desc: 'Uta-Net',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'utauchn.huijiwiki.com/',
			desc: 'UTAU China Wiki',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'utaulyrics.wikia.com/wiki/',
			desc: 'UTAU Lyrics Wiki',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'utaulyrics.fandom.com/wiki/',
			desc: 'UTAU Lyrics Wiki',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'utaudatabase.wiki.fc2.com/',
			desc: 'UTAU Visual Archive',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'w.atwiki.jp/utauuuta/',
			desc: 'UTAU Wiki (JP)',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'www24.atwiki.jp/utauuuta/',
			desc: 'UTAU Wiki (JP)',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'utau.wikia.com/wiki/',
			desc: 'UTAU Wiki (ENG)',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'utau.fandom.com/wiki/',
			desc: 'UTAU Wiki (ENG)',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'utau.wikidot.com/',
			desc: 'UTAU wiki 2.0',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'utau.wiki/',
			desc: 'UTAU wiki 2.0',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'utaite.wikia.com/',
			desc: 'Utaite Wiki',
			cat: WebLinkCategory.Reference,
		} /* UtaiteDB */,
		{
			url: 'utaite.fandom.com/',
			desc: 'Utaite Wiki',
			cat: WebLinkCategory.Reference,
		} /* UtaiteDB */,
		{
			url: 'vgmdb.net/',
			desc: 'VGMdb',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'vvstore.jp/',
			desc: 'Village Vanguard',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'vimeo.com/',
			desc: 'Vimeo',
			cat: WebLinkCategory.Official,
		},
		{
			url: '://vk.com/',
			desc: 'VK',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'vocadb.net/',
			desc: 'VocaDB',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'vocalsynth.fandom.com/',
			desc: 'Vocal Synth Wiki',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'vocaloiders.com/',
			desc: 'Vocaloiders',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'vocaloidlyrics.wikia.com/wiki/',
			desc: 'Vocaloid Lyrics Wiki',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'vocaloidlyrics.fandom.com/wiki/',
			desc: 'Vocaloid Lyrics Wiki',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'vocaloidia.com/',
			desc: 'VocaloidIA',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'vocaloidotaku.net/',
			desc: 'VocaloidOtaku.net Forums',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'vocaloid.wikia.com/wiki/',
			desc: 'Vocaloid Wiki',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'vocaloid.fandom.com/wiki/',
			desc: 'Vocaloid Wiki',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'voiceroid.wikia.com/wiki/',
			desc: 'Voiceroid Wiki',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'voiceroid.fandom.com/wiki/',
			desc: 'Voiceroid Wiki',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'www.vocaloid.com/products/',
			desc: 'VOCALOID SHOP (JPN)',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'www.vocaloid.com/en/products/',
			desc: 'VOCALOID SHOP (ENG)',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'vocalotracks.ssw.co.jp/',
			desc: 'Vocalotracks',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'www.vocallective.net/',
			desc: 'Vocallective',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'vsqx.top/',
			desc: 'vsqx.top',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'weibo.com/',
			desc: 'Weibo',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'weidian.com/',
			desc: 'Weidian',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'de.wikipedia.org/wiki/',
			desc: 'Wikipedia (DE)',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'en.wikipedia.org/wiki/',
			desc: 'Wikipedia (EN)',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'ja.wikipedia.org/wiki/',
			desc: 'Wikipedia (JP)',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'zh.wikipedia.org/wiki/',
			desc: 'Wikipedia (ZH)',
			cat: WebLinkCategory.Reference,
		},
		{
			url: 'wikipedia.org/wiki/',
			desc: 'Wikipedia',
			cat: WebLinkCategory.Reference,
		},
		{
			url: '://x.com/',
			desc: 'X (Twitter)',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'www.yesasia.com/',
			desc: 'YesAsia',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'music.youtube.com/',
			desc: 'YouTube Music',
			cat: WebLinkCategory.Commercial,
		},
		{
			url: 'youtube.com/playlist',
			desc: 'YouTube Playlist',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'youtube.com/c/',
			desc: 'YouTube Channel',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'youtube.com/@',
			desc: 'YouTube Channel',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'youtube.com/channel/',
			desc: 'YouTube Channel',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'youtube.com/user/',
			desc: 'YouTube Channel',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'youtube.com/shorts/',
			desc: 'YouTube Shorts',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'youtu.be/',
			desc: 'YouTube',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'youtube.com/',
			desc: 'YouTube',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'wixsite.com/',
			desc: 'Website',
			cat: WebLinkCategory.Official,
		},
		{
			url: '.wix.com/',
			desc: 'Website',
			cat: WebLinkCategory.Official,
		},
		{
			url: '.plala.or.jp/',
			desc: 'Website',
			cat: WebLinkCategory.Official,
		},
		{
			url: '.biglobe.ne.jp/',
			desc: 'Website',
			cat: WebLinkCategory.Official,
		},
		{
			url: '.nifty.com/',
			desc: 'Website',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'jimdofree.com',
			desc: 'Website',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'jimdosite.com',
			desc: 'Website',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'jimdo.com',
			desc: 'Website',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'weebly.com',
			desc: 'Website',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'sakura.ne.jp',
			desc: 'Website',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'sites.google.com',
			desc: 'Website',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'hp.infoseek.co.jp',
			desc: 'Website',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'iza-yoi.net',
			desc: 'Website',
			cat: WebLinkCategory.Official,
		},
		{
			url: '.xxxxxxxx.jp/',
			desc: 'Website',
			cat: WebLinkCategory.Official,
		},
		{
			url: '.web.fc2.com',
			desc: 'Website',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'fc2web.com',
			desc: 'Website',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'geocities.jp',
			desc: 'Website',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'geocities.co.jp',
			desc: 'Website',
			cat: WebLinkCategory.Official,
		},
		{
			url: '.netlify.app',
			desc: 'Website',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'wordpress.com',
			desc: 'Website',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'ameblo.jp/',
			desc: 'Blog',
			cat: WebLinkCategory.Official,
		},
		{
			url: '.amebaownd.com/',
			desc: 'Blog',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'ameba.jp/',
			desc: 'Blog',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'seesaa.net',
			desc: 'Blog',
			cat: WebLinkCategory.Official,
		},
		{
			url: '.sblo.jp/',
			desc: 'Blog',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'jugem.jp',
			desc: 'Blog',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'blog.shinobi.jp',
			desc: 'Blog',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'blog.goo.ne.jp',
			desc: 'Blog',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'blogs.yahoo.co.jp',
			desc: 'Blog',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'yaplog.jp/',
			desc: 'Blog',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'voiceblog.jp/',
			desc: 'Blog',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'blog.naver.com/',
			desc: 'Blog',
			cat: WebLinkCategory.Official,
		},
		{
			url: '.blogspot.',
			desc: 'Blog',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'blog.livedoor.jp',
			desc: 'Blog',
			cat: WebLinkCategory.Official,
		},
		{
			url: '.livedoor.blog',
			desc: 'Blog',
			cat: WebLinkCategory.Official,
		},
		{
			url: '.livedoor.jp',
			desc: 'Website',
			cat: WebLinkCategory.Official,
		}, /*blog.livedoor.jp takes precedence*/
		{
			url: 'hatenadiary.org',
			desc: 'Blog',
			cat: WebLinkCategory.Official,
		},
		{
			url: 'hatenablog.com',
			desc: 'Blog',
			cat: WebLinkCategory.Official,
		},
		{
			url: '.hatena.ne.jp',
			desc: 'Blog',
			cat: WebLinkCategory.Official,
		},
		{
			url: '.hateblo.jp',
			desc: 'Blog',
			cat: WebLinkCategory.Official,
		},
		{
			url: '.blog.eonet.jp',
			desc: 'Blog',
			cat: WebLinkCategory.Official,
		},
		{
			url: '.exblog.jp',
			desc: 'Blog',
			cat: WebLinkCategory.Official,
		},
		{
			url: '.pro.tok2.com',
			desc: 'Blog',
			cat: WebLinkCategory.Official,
		},
		
		{
			url: 'cocolog-nifty.com',
			desc: 'Blog',
			cat: WebLinkCategory.Official,
		},
		{
			"url": "sp-m.mu-mo.net",
			"desc": "mu-mo",
			"cat": WebLinkCategory.Commercial
		},
		{
			"url": "au.utapass.auone.jp",
			"desc": "au utapass / au Smart Pass Premium Music",
			"cat": WebLinkCategory.Commercial
		},
		{
			"url": "jcom.co.jp",
			"desc": "J:COM",
			"cat": WebLinkCategory.Commercial
		},
		{
			"url": "smart.usen.com",
			"desc": "SMART USEN",
			"cat": WebLinkCategory.Commercial
		},
		{
			"url": "otoraku.jp",
			"desc": "OTORAKU",
			"cat": WebLinkCategory.Commercial
		},
		{
			"url": "y.qq.com",
			"desc": "QQ Music",
			"cat": WebLinkCategory.Commercial
		},
		{
			"url": "kugou.com",
			"desc": "KuGou",
			"cat": WebLinkCategory.Commercial
		},
		{
			"url": "kuwo.cn",
			"desc": "KUWO MUSIC",
			"cat": WebLinkCategory.Commercial
		},
		{
			"url": "joox.com",
			"desc": "JOOX",
			"cat": WebLinkCategory.Commercial
		},
		{
			"url": "listen.tidal.com",
			"desc": "TIDAL",
			"cat": WebLinkCategory.Commercial
		},
		{
			"url": "music-flo.com",
			"desc": "FLO",
			"cat": WebLinkCategory.Commercial
		},
		{
			"url": "vibe.naver.com",
			"desc": "VIBE",
			"cat": WebLinkCategory.Commercial
		},
		{
			"url": "melon.com",
			"desc": "Melon",
			"cat": WebLinkCategory.Commercial
		},
		{
			"url": "music.tiktok.com",
			"desc": "TikTok Music",
			"cat": WebLinkCategory.Commercial
		},
		{
			"url": "sd.club-zion.jp",
			"desc": "Club ZION",
			"cat": WebLinkCategory.Commercial
		},
		{
			"url": "sd.reggaezion.jp",
			"desc": "Reggae ZION",
			"cat": WebLinkCategory.Commercial
		},
		{
			"url": "sd.deluxe-sound.jp",
			"desc": "DE-LUXE",
			"cat": WebLinkCategory.Commercial
		},
		{
			"url": "dhits.docomo.ne.jp",
			"desc": "dhits.docomo",
			"cat": WebLinkCategory.Commercial
		},
		{
			"url": "dmusic.docomo.ne.jp",
			"desc": "dmusic.docomo",
			"cat": WebLinkCategory.Commercial
		},
		{
			"url": "music.163.com",
			"desc": "music.163",
			"cat": WebLinkCategory.Commercial
		},
		{
			"url": "uta.573.jp",
			"desc": "uta.573",
			"cat": WebLinkCategory.Commercial
		},
		{
			"url": ".bugs.co.kr",
			"desc": "Bugs!",
			"cat": WebLinkCategory.Commercial
		},	
		{
			"url": "animelyrics.com",
			"desc": "Anime Lyrics",
			"cat": WebLinkCategory.Reference
		},	
		{
			url: '.fc2.com',
			desc: 'Blog',
			cat: WebLinkCategory.Official,
		},
	];

	private static isMatch(url: string, item: WebLinkMatcher): boolean {
		return url.indexOf(item.url) !== -1;
	}

	public static matchWebLink(url: string): WebLinkMatcher | undefined {
		return WebLinkMatcher.matchers.find((item) => this.isMatch(url, item));
	}

	public constructor(
		public url: string,
		public desc: string,
		public cat: WebLinkCategory,
	) {}
}
