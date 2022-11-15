import { Layout } from '@/Components/Shared/Layout';
import { JQueryUINavItemComponent } from '@/JQueryUI/JQueryUITabs';
import ErrorNotFound from '@/Pages/Error/ErrorNotFound';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import React from 'react';
import { Link, Route, Routes } from 'react-router-dom';

interface HelpIndexTabsProps {
	tab: 'aboutvocadb' | 'aboutvocaloid' | 'guidelines';
	children?: React.ReactNode;
}

const HelpIndexTabs = React.memo(
	({ tab, children }: HelpIndexTabsProps): React.ReactElement => {
		return (
			<div className="js-cloak ui-tabs ui-widget ui-widget-content ui-corner-all">
				<ul
					className="ui-tabs-nav ui-helper-reset ui-helper-clearfix ui-widget-header ui-corner-all"
					role="tablist"
				>
					<JQueryUINavItemComponent active={tab === 'aboutvocadb'}>
						<Link to="/Help">VocaDB</Link>
					</JQueryUINavItemComponent>
					<JQueryUINavItemComponent active={tab === 'aboutvocaloid'}>
						<Link to="/Help/aboutvocaloid">Vocaloid</Link>
					</JQueryUINavItemComponent>
					<JQueryUINavItemComponent active={tab === 'guidelines'}>
						<Link to="/Help/guidelines">Guidelines</Link>
					</JQueryUINavItemComponent>
				</ul>

				<div
					className="ui-tabs-panel ui-widget-content ui-corner-bottom"
					role="tabpanel"
				>
					{children}
				</div>
			</div>
		);
	},
);

const AboutVocaDb = React.memo(() => {
	return (
		<HelpIndexTabs tab="aboutvocadb">
			<h4 id="vocadbdescription">概况</h4>
			<p>
				VocaDB是一个免费的互联网数据库。本网站旨在提供精确的Vocaloid艺术家及其所参与制
				作的专辑、歌曲的相关信息。同时，我们通过推广Vocaloid艺术家的作品使得他们为世人所
				知，并在条件允许的前提下提供购买他们作品的方式、渠道。
			</p>
			<br />
			<h4 id="whatCanIDo">本站用户的权利（包括但不局限于以下五项）</h4>
			<ul>
				<li>
					本站的用户（被剥夺编辑权限的用户除外）都有权在遵守编辑指南的前提下添加、编辑词条信息，还可以为词条添加标签及备注。
				</li>
				<li>设置自己喜欢的界面语言。</li>
				<li>在自定义歌单里添加自己喜欢的歌曲并为歌曲打分。</li>
				<li>使用收藏夹记录自己购买过的Vocaloid专辑。</li>
				<li>发表评论、在讨论区讨论相关问题。</li>
			</ul>
			备注：Vocaloid艺术家可以在自己的词条已被添加的前提下
			<Link to="/User/RequestVerification">申请账户认证</Link>
			，将自己的VocaDB账户与词条进行绑定。
			<br />
			<h4 id="contribute">关于错误信息</h4>
			<p>
				如果您在浏览词条的过程中发现错误信息，您可以在通过点击页面上的“报告错误信息”向我们反映。当然了，我们更建议您在发现错误之后自己对词条进行修改。
			</p>
			<h4 className="withMargin">关于转载本网站上的词条信息</h4>
			<p>
				VocaDB上的一切词条信息属于开放资料（Open
				Data），您可以在标明来源的前提下进行转载。
				如有疑问的话请联系站长或管理员。
			</p>
			<h4 className="withMargin">隐私政策</h4>
			<p>
				VocaDB尊重本站用户的隐私。我们任何情况下都不会与任何第三方分享您的个人信息：
				<ul>
					<li>电子邮箱</li>
					<li>IP地址</li>
					<li>账户密码</li>
				</ul>
				同时，我们保证您能以匿名的方式添加、编辑词条信息。
			</p>
			<br />
			<h4 id="contactus">联系我们（当前只支持用英文进行联系）</h4>
			support[at]vocadb.net
			<br />
			<a href="https://github.com/VocaDB/vocadb" title="">
				Code on Github
			</a>
			<br />
		</HelpIndexTabs>
	);
});

const AboutVocaloid = React.memo(
	(): React.ReactElement => {
		return (
			<HelpIndexTabs tab="aboutvocaloid">
				<h4 id="vocaloidlinks">Main description</h4>
				<a href="https://en.wikipedia.org/wiki/Vocaloid" title="">
					Wikipedia article
				</a>
				<br />
				<a href="http://vocaloid.wikia.com/wiki/Vocaloid_Wiki" title="">
					Vocaloid wiki
				</a>
				<br />
				<br />
				<h4 id="vocaloiddescription">Short description</h4>
				<p>
					Vocaloid is a unique subculture of user-created content (music,
					illustrations, videos, software, games, cosplay, events) based on
					voice synthesizers and their mascots.
				</p>
				<p>
					Vocaloid synthesizer software is a digital musical instrument to
					create human vocals, allowing anyone to have their personal virtual
					singer.
				</p>
				<p>
					The first Vocaloid "singers," Leon and Lola, were released in 2004 in
					the English language, but weren't successful. Vocaloid technology
					became popular in 2007 in Japan with the Japanese voicebank "Hatsune
					Miku" (first for the VOCALOID2 engine in Japan, second in the world
					after Sweet Ann).
				</p>
				<p>
					Being a great and fresh combination of a decent V2 voicebank and a
					snazzy boxart illustration, Hatsune Miku gained unexpected,
					unprecedented and massive success and became the symbol of all
					Vocaloid technology and subculture.
				</p>
				<h4 id="whyjapan">Why in Japan?</h4>
				<ul>
					<li>
						The simplest language for synthesis in the world, Japanese has
						syllable-based phonetics as well as no accents and no rhyme concept
						in poetry
					</li>
					<li>Popular anime style (moe catalyst)</li>
					<li>
						A lot of{' '}
						<a href="https://en.wikipedia.org/wiki/D%C5%8Djin" title="">
							dōjin
						</a>{' '}
						(self-published) activity
					</li>
					<li>Traditional high-pitched female vocal</li>
					<li>Diverse and active musical scene--a lot of indie musicians</li>
					<li>
						Pop-idols and virtual pop-star concepts in anime (Sharon Apple in
						"Macross Plus", 1994)
					</li>
				</ul>
				<p>
					Japan happened to be the best country for a popularity breakthrough of
					the Vocaloid technology.
				</p>
				<h4 id="vocaloidfeatures">Features of Vocaloid</h4>
				<ul>
					<li>
						Vocaloid synthesizer software is treated as a musical instrument, so
						it is free to use for any purpose.
					</li>
					<li>
						Piapro license initiative from Crypton Future Media permits the
						non-commercial usage of their mascot images.
					</li>
					<li>
						Decentralization. No one commands the community what to do and how
						to do it. All content (music, videos, illustrations) is created by
						users themselves for their own purposes (fun, self-expression,
						selling).
					</li>
					<li>
						Collaboration of song writers, musicians, illustrators, animators
						and programmers.
					</li>
					<li>
						Self-promotion of Vocaloids and producers. Basically, the community
						chose Miku and made her popular (as well as GUMI).
					</li>
					<li>
						Sharing of music and support principles. Most Vocaloid music and
						videos are freely available on NND, Piapro and YouTube, and still
						the community supports producers by buying their albums. It is
						possible for and being done by people who live outside Japan, too.
						Musical instruments, hardware, software and learning cost a lot, and
						producers are mostly indie musicians, so every sold CD is important
						for them. If you really like the music that was done by a Vocaloid
						producer, please support him.
					</li>
					<li>
						Concerts are also a large part of the Vocaloid subculture. Vocaloid
						mascots are projected onto a transparent screen and perform "live"
						with a real band.
					</li>
				</ul>
				<h4 id="vocaloidpv">PV</h4>
				<p>
					Promotional Videos are being used to attract listeners to Vocaloid
					songs, becoming part of the songs, the form of art by itself and a
					part of Vocaloid subculture.
				</p>
				<p>
					Japanese flash video streaming service{' '}
					<a href="http://www.nicovideo.jp/">Niconico</a> (originally called
					Nico Nico Douga or NND) greatly helped in the popularization of
					Vocaloid in Japan and is used for uploading new Japanese Vocaloid
					songs and videos, live streaming of concerts, presentations and other
					events.
				</p>
				<h4 id="ytnndsoundquality">
					Sound quality of videos on YouTube and Niconico
				</h4>
				<p>
					NND videos have various sound quality - from MP3 CBR 64 kbps in
					"economy" mode to AAC VBR ~370 kpbs, but generally it is higher than
					on YouTube, that has AAC VBR ~128 kbps in most videos (same for all
					video resolutions).
				</p>
				<h4 id="vocaloidmmd">MMD</h4>
				<p>
					<a href="https://en.wikipedia.org/wiki/MikuMikuDance">
						MikuMikuDance
					</a>{' '}
					(MMD for short) is a free, popular and powerful 3D animation software
					made by Yu Higuchi (HiguchiM) for Vocaloid Promotional Videos.
				</p>
				<p>
					MMD is a part of the success of Hatsune Miku and is also a subculture
					itself, having MMD Cup championships and being used to produce
					animation for memes, Touhou, IDOLM@STER, etc.
				</p>
				<h4 id="vocaloidp">Producer</h4>
				<a href="http://vocaloid.wikia.com/wiki/Category:Producer" title="">
					Description on Vocaloid Wiki
				</a>
				<p>
					"Producer" is the term used for any person that produces
					Vocaloid-related audiovisual material, generally original music or
					fanmade PVs.
				</p>
				<h4 id="vocaloidcircle">Circles and Labels</h4>
				<p>
					<a href="https://en.wikipedia.org/wiki/D%C5%8Djin" title="">
						Dōjinshi circle
					</a>{' '}
					is a group of authors of self-published works that may act as a label
					for dōjin events (Comiket or Vocaloid M@ster).
				</p>
				<p>
					<a href="https://en.wikipedia.org/wiki/Record_label" title="">
						Record label
					</a>{' '}
					is a company that provides professional studio services (instruments,
					recording, mastering), production of CDs, various promotion and
					copyright protection. The most well known Vocaloid label is{' '}
					<a href="https://karent.jp/">KarenT</a>, created by Crypton Future
					Media.
				</p>
			</HelpIndexTabs>
		);
	},
);

const Guidelines = React.memo(
	(): React.ReactElement => {
		return (
			<HelpIndexTabs tab="guidelines">
				<h3 id="glmain">General principles</h3>
				<ul>
					<li>
						Avoid submitting entries that already exist in the database.
						Duplicates have to be deleted or merged by the staff.
					</li>
					<li>
						When editing entries that are finalized or approved (not drafts),
						make sure you have a good reason, and preferably state that reason
						in the edit notes unless it's obvious. If you're unsure of whether
						your edit is appropriate and necessary, ask in the comments, on
						Twitter, in discussions, or by email.
					</li>
					<li>
						This is a database for Vocaloid content and related voice
						synthesizers such as UTAU. Instrumental songs and human-sung covers
						are allowed when they appear on albums together with Vocaloid songs,
						or there's a significant number of Vocaloid covers of those songs.
						Albums without Vocaloid vocals are generally not relevant to our
						focus. Completely unrelated content will be deleted.
					</li>
					<li>
						Download links to illegal file-sharing sites that provide no useful
						information (like Mediafire/4Shared/Torrents) are NOT allowed. Links
						to informative blogs/forums/wikis are allowed but should be
						categorized as "Unofficial" or "Reference". The staff members
						reserve the right to remove any links deemed inappropriate.
					</li>
					<li>
						Direct download links to media files (such as .mp3) are generally
						not allowed, even if they're official and legal. If the song/video
						is officially distributed, link to the artist's official download
						page (for example on piapro) instead.
					</li>
					<li>
						Creating multiple user accounts is not allowed. Any kind of abuse,
						manipulation of ratings or spamming will not be tolerated.
					</li>
					<li>
						VocaDB strives for objectivity. Advertising in comments or entry
						descriptions is forbidden without prior approval by a staff member.
						Verified artists are allowed to post advertisements in the "personal
						description" field for entries they manage.
					</li>
				</ul>
				<h4>Language</h4>
				<p>
					The official language of the site is English. Please prefer using
					English for all non-translateable information. For example, when
					adding external links, the descriptions of those links should be in
					English. Discussions and tags should also be mainly in English. Other
					languages are allowed with a good reason, for example, if a specific
					piece of information cannot be properly translated into English.
					<br />
					We're looking into the possibility of allowing other languages in the
					future, at least for descriptions and discussions.
				</p>
				<h4>How can I add new producers, albums and songs?</h4>
				<p>
					Register and use links in main navigation panel or buttons in
					producer, album and song list pages.
				</p>
				<h4>Names and translations</h4>
				<p>
					All names should primarily be written in the original language, which
					usually means Japanese, but it can also be English or Korean, for
					example. The system supports 4 language options:
				</p>
				<ul>
					<li>
						"Non-English" covers original names in all languages that aren't
						English or romanized, for example Japanese.
					</li>
					<li>
						"Romanized" is for foreign original names written in the Latin
						(western) alphabet. Usually this means Romaji.
					</li>
					<li>
						"English" is for official names in English and known translations of
						foreign names.
					</li>
					<li>
						"Unspecified" is for names where the language doesn't matter or it's
						unknown. Unspecified names will never be used as display names of an
						entry if a name of a different option is provided.
					</li>
				</ul>
				<p>
					If the original language is English, the name should be written in the
					English language field. Otherwise, it's written to the "Non-English"
					language field. If those names have commonly used romanizations or
					English translations, those can be entered as well to make it easier
					for the international audience to find and understand those names.
					Other names, for example translations to other languages, should be
					marked as "Unspecified" according to current rules. Sometimes, an
					entry may have multiple names in one language, for example a
					producer's real name and artist name. In this case, the additional
					names can be marked as "Unspecified" so that they won't be used as
					display names regardless of the entry's display option.
					<br />
					Sometimes, the Romanization or English name of the artist is always
					used, even though the artist also has a name in Japanese. In this
					case, the Japanese name should be marked as Unspecified so that it
					won't be used as a display name.
				</p>
				<p>
					For more guidelines regarding Romanization,{' '}
					<a href="http://wiki.vocadb.net/wiki/1/romanization-guidelines">
						check the wiki article
					</a>
					.
				</p>
				<p>
					If the entry has multiple names in one language and it's not clear
					which one of those should be the primary one, you should refer to
					official information, such as product packaging. If the name in the
					album's cover doesn't conflict with the other rules on the site, that
					name should be preferred as the primary one.
					<br />
					However, keep in mind that sometimes the official information contains
					the same name in multiple languages combined into one. In this case
					you should enter the name separately for each language. For example,
					if the song is uploaded to Niconico with the name 秘密警察 - Himitsu
					Keisatsu, you should enter both names separately: 秘密警察 in the
					Non-English field and Himitsu Keisatsu in the Romanized field.
				</p>
				<p>
					Registered users can set their display language so that they will
					always see the names in that language, if available. Unregistered
					users and users who haven't set their display language will see the
					original names, which may be either English or something else. For
					example, if the non-English name of an artist is ナナホシ, the
					romanized name is nanahoshi, and the default language option is set to
					"Original", then by default, users will see ナナホシ unless they have
					chosen to prefer romanized names instead.
				</p>
				<p>
					Generally, there is no need to add composite names, and those names
					should be broken into separate fields.
				</p>
				<p>
					<input
						type="text"
						className="incorrect-example-input"
						value="cosMo@Bousou-P(暴走P)"
						size={23}
						readOnly
					/>{' '}
					is incorrect: every field should contain only one name.
				</p>
				<h4>Entry statuses</h4>
				<p>
					Entries that are marked as drafts have missing or incomplete
					information. The draft status is indicated on the entry's page. All
					entries should meet certain requirements before they can be marked as
					finished/completed. For now, these requirements are only suggestions,
					but in the future, they may be enforced more strictly. Of course, when
					editing an entry that meets these requirements, you can still mark it
					as draft if you feel that the entry needs further attention.
					<br />
					<br />
					After the entry has been finished, it can be approved by a trusted
					user or staff member. Most properties of approved entries cannot be
					edited by users with normal permissions. Tags can still be edited by
					anyone and comments can be added. Trusted users are also able to
					change the status back from Approved to Finished or even Draft.
					<br />
					<b>A note for trusted users regarding entry approval: </b>You're
					encouraged to mark the entry as Approved when you've checked that all
					the necessary information is provided and it's correct. However, songs
					shouldn't be marked as approved until they have lyrics. When approving
					albums, make sure that ALL the artists have been added.
				</p>
				<br />
				<h3 id="glproducers">Artists, producers and Vocaloids</h3>
				<h4 id="glartisttypes">Artist types</h4>
				<p>
					Every artist has a classification which also determines the artist's
					default roles. Roles can be overridden per album and per song-basis,
					but in many cases the artist is involved only in one role.
				</p>
				<ul>
					<li>
						"Producer" - the person who created the song (and in the case of a
						Vocaloid song, usually provided the vocals as well using Vocaloid).
						Not necessarily the original composer.
					</li>
					<li>"Animator" - person who primarily animates PVs.</li>
					<li>
						"Illustrator" - person who primarily draws static illustrations.
					</li>
					<li>"Lyricist" - person who primarily composes lyrics for songs.</li>
					<li>
						"Circle" - group that self-publishes and sells albums only at doujin
						events (such as Comiket or Vocaloid Master).
					</li>
					<li>
						"Label" - a commercial company that publishes albums for other
						artists.
					</li>
					<li>"Other group" - group that releases albums via Labels.</li>
					<li>
						"Other vocalist" - human singers (NND vocalists/Utaite) and fan-made
						voice configurations (derivatives).
					</li>
					<li>
						"Other voice synthesizer" - voice synthesizer voicebanks (machine
						voices) that aren't Vocaloid, UTAU or CeVIO.
					</li>
					<li>"Other individual" - other people such as instrumentalists.</li>
				</ul>
				<h4>Artist pictures</h4>
				<p>
					When adding pictures to artists, keep in mind all the pictures should
					be related to the artist himself, not his works. Do not upload album
					covers for artist entries.
					<br />
					That said, there is no common rule for choosing the main picture of an
					artist. Photos of the artist himself as well as official logos are
					preferred. You may upload any number of these pictures as additional
					pictures for the artist, provided that they're relevant enough.
					Copyrighted pictures or pictures of artists may be taken down if the
					copyright holder requests it.
					<br />
					If no better picture is provided, any picture found on one of the
					artist's official profiles, for example on Twitter, is acceptable as
					well.
				</p>
				<h4>Should I create an entry for artist's personal circle?</h4>
				<p>
					Many Vocaloid artists have a personal band/circle through which they
					publish their albums. Often there are no other members in this circle.
					In this case, it's not necessary to create a separate entry for that
					personal circle, but it's not wrong either, and might be a good idea
					if the artist has separate websites/blogs for himself and his circle.
					If there is no separate entry for the circle, the circle name can be
					included as an alias for that artist. In that case, it would be
					advisable to mention this in the notes.
				</p>
				<h4>Completed artist entries should meet the following criteria:</h4>
				<ul>
					<li>
						The artist has at least one name whose language option isn't
						"Unspecified".
					</li>
					<li>Artist type isn't "Unspecified".</li>
					<li>
						The artist has a description OR at least one external link (to a
						wiki, artist's NND MyList etc.)
					</li>
				</ul>
				<br />
				<h3 id="glalbums">Albums</h3>
				<p>
					To assign producers and vocalists to songs in an album, add artists to
					the album first and then click on each added song in the song list to
					show a quick selection dialog.
				</p>
				<p>
					Please note that the album must have at least one song with
					synthesized vocals (Vocaloid, UTAU and other voice synthesizers all
					count). Cover albums consisting only of human-sung covers of Vocaloid
					songs are unfortunately not allowed. Consider checking{' '}
					<a href="https://utaitedb.net/">UtaiteDB</a>. Unrelated albums may be
					deleted from VocaDB.
				</p>
				<h4>A note about fanmade compilations</h4>
				<p>
					Unofficial, fanmade compilation albums (bootlegs) where the authors
					don't have permissions to use the songs are generally not allowed.
					There can be some exceptions if the albums are widely known, for
					example the Hatsune Miku 1st Song Album. In any case, the staff
					members reserve the right to remove these entries should they deem it
					necessary.
				</p>
				<h4>Adding artists for albums</h4>
				<p>
					When linking artists to albums, at the very least you should add the
					responsible circle (usually there's only one) and vocalists.
					Individual producers and the associated record label (if any) can be
					added as well. At the moment, all artist types may be linked to
					albums.
					<br />
					Always enter artists individually. Artists such as "producer feat.
					vocalist", meaning "cosMo feat. Hatsune Miku", are not needed. The
					system will produce these "artists strings" automatically.
					<br />
					Sometimes, it's necessary to credit people that aren't in the
					database, and it doesn't make sense to add an entry for them. For
					example, if the song in the database is a remix/cover and the original
					isn't a Vocaloid song, the original composer doesn't need to be added
					to the database, but should be credited nevertheless. These "extra
					artists" can then be assigned into roles just like artists that are in
					the database. However, for all Vocaloid-related artists, it's
					necessary to create an entry for that artist.
				</p>
				<h4>Album types</h4>
				The possible album types on VocaDB are:
				<ul>
					<li>
						"Original album" - Album that consists mostly of previously
						unpublished songs.
					</li>
					<li>
						"Single" - Contains one or two individual tracks. Alternate versions
						(instrumentals and remixes) are usually not counted.
					</li>
					<li>
						"EP" - Meaning extended play. Contains 3-4 individual tracks.
						Alternate versions (instrumentals and remixes) are usually not
						counted.
					</li>
					<li>
						"Split album" - Collaboration between two or more (but usually just
						two) equal artists, where all artists have roughly the same number
						of songs.
					</li>
					<li>
						"Compilation" - Collection of previously published songs, gathered
						from one or more earlier albums. For example, "best of" collections.
					</li>
					<li>
						"Video" - Disc containing mostly music videos, usually a DVD or
						Blu-ray.
					</li>
					<li>"Other" - For albums that don't fit into anything above.</li>
				</ul>
				<p>
					These are only suggestions; use common sense when determining album
					type. For example, an album with a single song that by itself is as
					long as a regular album (the definition on Wikipedia says over 25
					minutes) can be counted as "original album" instead of single. If
					unsure of which type to use, try to find out how the artists
					themselves call the album. For more elaborate descriptions, please
					refer to Wikipedia, Discogs or Musicbrainz.
				</p>
				<h4>Completed album entries should meet the following criteria:</h4>
				<ul>
					<li>
						The album has at least one name whose language option isn't
						"Unspecified".
					</li>
					<li>
						All artists responsible for the album are specified. Usually there
						should be at least one producer and one vocalist. Whenever it makes
						sense, each of those artists should have an entry in the database.
						If you can't locate some of the artists, please leave the entry as
						draft.
					</li>
					<li>Album type isn't "Unspecified". Try to find the correct type.</li>
					<li>
						The album's release date, at least release year, is specified.
					</li>
					<li>The album has a complete tracklist.</li>
				</ul>
				<br />
				<h3 id="glsongs">Songs</h3>
				<p>
					Song entries contain information about authors and vocalists (see
					"Artists" tab), PVs and lyrics.
				</p>
				<p>
					<input
						type="text"
						className="incorrect-example-input"
						value="『ミク×リン×レン』 ReAct 『オリジナル曲PV』"
						size={40}
						readOnly
					/>{' '}
					is incorrect: all additional info (vocalists, authors, PV) should be
					added to appropriate fields. A song title should contain nothing more
					than the name itself.
				</p>
				<h4>Adding artists for songs</h4>
				<p>
					Most Vocaloid songs have one producer and one or more vocalists (i.e.
					Vocaloids). Circles, labels and other groups are generally NOT
					credited for individual songs, unless it's clear that the whole group
					worked on that song. Always prefer adding individual people to songs
					over adding circles or groups if possible. Note that different sources
					may swap producer and group names.
				</p>
				<p>
					When tagging Vocaloids, you should use the information provided by the
					artist, not guess yourself. If the artist says the singer is Hatsune
					Miku, you should add Miku's original voicebank, even if you think the
					singer is actually an Append. Sometimes, the artist says the singer is
					Append, but doesn't specify which Append. In this case, you should
					choose Append (unknown). Do not guess unless you are sure.
				</p>
				<p>
					In the case of a remix or cover, the original composer/lyricist of the
					song does not need to be credited if the original song is in the
					database.
					<br />
					If the original is a Vocaloid song, or a song featuring another voice
					synthesizer, add the original song to the database and link the
					original to the derived version, or if the original is not a Vocaloid
					song, add the composer as an "extra artist" (see album guide for more
					information). Remember to set this artist's role properly to
					Composer/Lyricist.
				</p>
				<h4>Song types</h4>
				<p>
					The most important distinction is between Original and others.
					Original always means that the song is a completely original
					production. If the song uses material from existing sources, it's a
					derived work and not original.
				</p>
				<p>
					For instrumental songs, you should use the original song type if the
					instrumental version is the original. In this case, you should
					indicate that the song is an instrumental by tagging it with the{' '}
					<Link
						to={EntryUrlMapper.details_tag(
							vdb.values.instrumentalTagId,
							'instrumental',
						)}
					>
						instrumental tag
					</Link>
					. The instrumental song type is for instrumental versions of original
					songs. Usually, if the song is an original work and not a derivative,
					it should be marked as original. One exception to this rule is drama
					PVs. Because drama PVs are not songs, they should be separated from
					songs by using the Drama PV song type, even if the PV is a completely
					original production, like drama PVs usually are.
				</p>
				<h4 id="glpv">Song embeds and PVs</h4>
				<p>
					Songs may contain any number of embedded media files, such as
					Promotional Videos (PVs) on Niconico (NND), YouTube, Bilibili or
					Vimeo, or audio streams on Piapro or SoundCloud. All embeds should
					have the same version of the song, meaning the audio should be the
					same. Shortened versions are sometimes accepted. Do not add karaoke
					versions as embeds to the original song: either add the karaoke
					version as a link or create a new entry. Remixes should always be
					separate entries.
					<br />
					Very often, Vocaloid artists themselves upload their songs to NND, and
					sometimes to YouTube or SoundCloud as well, in which case these
					uploads are called "original". Original uploads, if available, are
					highly preferred to any others.
					<br />
					If a PV made for a song is remarkable or well-known in some way,
					you're encouraged to create a separate entry for that PV and link it
					to the original song.
				</p>
				<ul>
					<li>
						"Original" - the first video with that song, usually uploaded by the
						author himself (may lack any animation). There may be multiple
						Original PVs if all are uploaded by the producer.
					</li>
					<li>
						"Reprint" - should be identical to "Original"; most often it's a
						YouTube reprint of an original NND video, uploaded by someone other
						than the artist.
					</li>
					<li>
						"Other" - any following PV with animation, translated subtitles, MMD
						versions, etc. May be better known than the original upload. Note
						that the audio should still be the same as the original.
					</li>
				</ul>
				<p>
					<strong>Important note about raw file embeds:</strong> VocaDB supports
					embedding raw links to .mp3 files. For security reasons, the usage of
					this feature is limited to trusted users only. Only mp3 files are
					supported for now because it's the most widely supported format, but
					in the future we might allow other audio and video files as well.
					<br />
					To do that (assuming, you're a trusted user), simply input a URL
					pointing to a .mp3 file. The file must be publicly accessible and
					authorized by the artist - we do not support illegal distribution. If
					possible, try to make sure the artist has allowed embedding the file
					on other sites. Whenever adding a raw file as media, be sure to
					include a link to the official webpage where the link is from. If
					there is no such webpage, it's better not to add the media.
					<br />
					Finally, raw file embeds should mostly be used as fallback when the
					song isn't available on any other service. Especially if the song is
					officially on YouTube or SoundCloud, raw file embeds should{' '}
					<em>not</em> be added. Services like YouTube and SoundCloud have
					worldwide content delivery networks (CDNs) that most likely offer
					better performance than the artist's own server hosting the file.
				</p>
				<h4>Completed song entries should meet the following criteria:</h4>
				<ul>
					<li>
						The song has at least one name whose language option isn't
						"Unspecified".
					</li>
					<li>
						The song has at least one artist (usually, there should be at least
						one producer and one vocalist).
					</li>
					<li>Song type isn't "Unspecified".</li>
				</ul>
				<br />
				<h3>Tags</h3>
				<p>
					Tags are free-form metadata that can be added to all entry types. Tags
					allow users to link entries together by any properties they can come
					up with, not being limited to the options provided by the system.
					Examples of tags are genres, presentation, languages and themes. Tags
					can be edited more freely than other properties and some of them may
					even be considered subjective. Therefore, tags are based on voting.
					Any user may add tags to an entry and vote on existing tags. Only the
					most popular tags will be displayed for that entry.
				</p>
				<h4>What to tag then?</h4>
				<p>
					Generally, you should avoid tagging with information that is already
					provided by more specialized fields. For example, albums or songs with
					artist names is redundant because the artists list already handles
					this better. Likewise, tagging cover songs with "cover" is useless if
					the song classification is already cover. Of course, albums have no
					such classification, so that tag might be relevant for an album (or
					artist, if that artist is known for making covers).
				</p>
				<p>
					Most tags should be <b>objective</b>, meaning their validity isn't
					based on the listener's subjective opinion. For example, "beautiful"
					is a poor tag because beauty is highly subjective. "Calm" is a
					slightly better tag, since that can be defined by some objective
					characteristics, although it's still mostly subjective. Please prefer
					creating private song lists for highly subjective properties.
				</p>
				<h4>Tags versus pools</h4>
				<p>
					Trusted users are able to create public songlists called pools. Pools
					can be used for largely the same purpose as tags, grouping songs
					together based on some common theme. There are a few differences
					between tags and songlists.
				</p>
				<ul>
					<li>Pools are not voted on, unlike tags. All pools are equal.</li>
					<li>
						Only trusted users are able to edit pools. Any user can vote on
						tags. Thus tags are easier to use, but also more unreliable.
					</li>
					<li>
						Pools may contain only songs. Tags can be applied to albums and
						artists as well.
					</li>
					<li>
						Songs in a pool can be ordered. Songs with a specified tag are
						unordered.
					</li>
					<li>You can add notes to songs in a pool.</li>
				</ul>
				<br />
				<h3>Other guidelines</h3>
				<ul>
					<li>
						Applies to trusted users: when merging duplicate entries, prefer
						merging the newer entry to the older one, unless the new entry is
						finalized while the older one is draft.
					</li>
				</ul>
			</HelpIndexTabs>
		);
	},
);

const HelpIndex = (): React.ReactElement => {
	const title = 'Help / About';

	return (
		<Layout pageTitle={title} ready={true} title={title}>
			<Routes>
				<Route path="" element={<AboutVocaDb />} />
				<Route path="aboutvocaloid" element={<AboutVocaloid />} />
				<Route path="guidelines" element={<Guidelines />} />
				<Route path="*" element={<ErrorNotFound />} />
			</Routes>
		</Layout>
	);
};

export default HelpIndex;
