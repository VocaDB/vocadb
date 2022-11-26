import { Layout } from '@/Components/Shared/Layout';
import { JQueryUINavItemComponent } from '@/JQueryUI/JQueryUITabs';
import ErrorNotFound from '@/Pages/Error/ErrorNotFound';
import { contributors } from '@/Pages/Help/HelpIndex';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { useVdb } from '@/VdbContext';
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
						<Link to="/Help">VocaDBについて</Link>
					</JQueryUINavItemComponent>
					<JQueryUINavItemComponent active={tab === 'aboutvocaloid'}>
						<Link to="/Help/aboutvocaloid">VOCALOIDとは</Link>
					</JQueryUINavItemComponent>
					<JQueryUINavItemComponent active={tab === 'guidelines'}>
						<Link to="/Help/guidelines">ガイドライン</Link>
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
			<h4 id="vocadbdescription">VocaDBとは</h4>
			<p>
				Vocaloid Database (VocaDB／VOCALOIDデータベース)
				はVOCALOID関連の曲、アルバム、アーティスト情報を中心としたデータベースです。ボカロ総合情報源となることを目指しています。
			</p>
			<p>
				wiki、ブログ、フォーラムなどは、言語の異なる様々なウェブサイトに散在する情報を管理し効率的に検索したり、それらを蓄えておくには適していません。
				VocaDBは、VOCALOID音楽に関する情報のための独自のシステムとして、またその情報提供サービスとして、VOCALOID創作文化の特徴を意識しながら生み出されました。
				同時に、創作者の作品を広めることを励んでいます。
			</p>
			<p>
				当サイトではアカウント作成後誰もが情報を投稿、また編集することが出来ます。創作者ご自身の作品の情報の投稿・編集も勧めています。
			</p>
			<br />
			<h4 id="vocadbfeatures">
				このサイトでVOCALOID曲をダウンロード出来ますか？
			</h4>
			<p>
				いいえ。VocaDBには音楽ファイルは置いていませんし、ダウンロードリンクも提供していません。
			</p>
			<p>詳しくはガイドラインを御覧下さい。</p>
			<br />
			<h4 id="whatCanIDo">このサイトで何が出来ますか？</h4>
			<p>VOCALOID曲、プロデューサー、サークル、PV、歌詞の検索が出来ます。</p>
			<p>
				また、このサイトに登録しアカウントを作ると（無料で簡単に行えます）以下のことも出来るようになります。
			</p>
			<ul>
				<li>表示言語や日付の表示形式を変更</li>
				<li>VocaDBに情報を登録・編集</li>
				<li>登録されている曲をお気に入りや曲リストに追加すること</li>
				<li>VOCALOIDアルバムのコレクションリストを作ること</li>
				<li>意見交換に参加すること</li>
				<li>アルバム・アーティスト・曲にタグを付けること</li>
				<li>他</li>
			</ul>
			<br />
			<h4 id="contribute">
				情報の誤りを報告するには、情報を追加するにはどうすれば良いですか？
			</h4>
			<p>
				もしデータベースに存在する情報に誤りや不足が有ったら、レポート機能を使用して管理スタッフに知らせることが出来ますが、アカウントを作成し直接訂正することを勧めます。サイトの技術的な問題が生じた場合は管理スタッフに御連絡下さい。
			</p>
			<h4 className="withMargin">アーティストや創作者へ</h4>
			<p>
				プロデューサー、イラストレーター、アニメーター、ボーカロイド音楽や動画に関わるクリエーター、もしくはサークルの管理人ですか？
				ニコニコ、Youtube、SoundCloudなどに作品を投稿されている創作者であれば、ご自身のアーティストページと作品をを登録することをお奨めします。
				（すでに登録済みでないことを確認してください。）次のリンクで
				<Link to="/User/RequestVerification">アーティストアカウント証明</Link>
				が出来ます。これはVocaDBユーザーアカウントとVocaDBアーティストページを連携するプロセスです。
			</p>
			<br />
			<h4 id="vocadbmetadata">
				他のサイトでVocaDBからの情報を使用しても良いですか？
			</h4>
			<p>
				はい。VocaDBは他の同種のサービスにも情報を提供しやすいように作られています。詳しくは開発者にお問い合わせ下さい
			</p>
			<br />
			<h4 id="staff">運営・管理スタッフ</h4>
			<p>
				Major contributors:{' '}
				{contributors.map((contributor, index) => (
					<React.Fragment key={index}>
						{index > 0 && ', '}
						<Link to={EntryUrlMapper.details_user_byName(contributor)}>
							{contributor}
						</Link>
					</React.Fragment>
				))}
				.
			</p>
			<p>
				Japanese translation by{' '}
				<a href="https://www.youtube.com/watch?v=6Z73O2WH2jw">
					Japanese Ninja No.1
				</a>
				.
			</p>
			<br />
			<h4 id="contactus">お問い合わせ</h4>
			support[at]vocadb.net
			<br />
			<Link to="/discussion/folders/5" title="">
				VocaDB日本語話し合いページ
			</Link>
			<br />
			<a href="https://github.com/VocaDB/vocadb" title="">
				Github
			</a>
			<br />
		</HelpIndexTabs>
	);
});

const AboutVocaloid = React.memo(
	(): React.ReactElement => {
		return (
			<HelpIndexTabs tab="aboutvocaloid">
				<h4 id="vocaloidlinks">VOCALOIDとは</h4>
				<a href="https://ja.wikipedia.org/wiki/VOCALOID" title="">
					Wikipedia article
				</a>
				<br />
				<a href="https://vocaloid.fandom.com/wiki/Vocaloid_Wiki" title="">
					Vocaloid wiki
				</a>
				<br />
				<br />
				<h4 id="vocaloiddescription">概要</h4>
				<p>
					“VOCALOID”とは、人の歌声を再現するシンセサイザー及びそれらに付随するイメージキャラクターを元に、ユーザーが様々な創作（楽曲、イラスト、動画、ソフト、ゲーム、コスプレ、イベント）を行い作り上げたユニークな文化です。
				</p>
				<p>
					VOCALOID（シンセソフト）は人の歌声を作り出すことが出来るソフト音源です。つまり、誰にでも専属のバーチャルシンガーが持てるということです。
				</p>
				<p>
					最初のVOCALOIDシンガーは2004年に発売された英語のシンガー“LEON”と“LOLA”でしたが、売り上げは振るいませんでした。VOCALOIDの技術が広く世に知られるようになったのは2007年、日本語ライブラリの“初音ミク”（日本では初、世界では“Sweet
					Ann”に次いで二番目のVOCALOID2製品）が日本で発売されてからのことです。
				</p>
				<p>
					魅力的なパッケージイラストとそれにぴったりな歌声の、最高且つ新鮮なコンビネーションによって、“初音ミク”は予想を上回る空前の大ヒット商品となり、VOCALOID技術・文化の代名詞となったのでした。
				</p>
				<h4 id="whyjapan">日本でVOCALOIDが根付いた背景</h4>
				<ul>
					<li>
						日本語は音声の合成において最もシンプルな言語――日本語は音節ベースの音声で、アクセントが無く、詩における韻の概念が無いこと
					</li>
					<li>
						“萌え”要素を取り入れたアニメキャラのスタイルが浸透していること
					</li>
					<li>同人活動が盛んであること</li>
					<li>甲高い女声ボーカルに馴染みがあること</li>
					<li>
						多種多様で活発な音楽シーンと多くのインディーズアーティストが存在すること
					</li>
					<li>
						アニメにおける仮想ポップアイドルの概念（シャロン・アップル『マクロスプラス』／1994年）
					</li>
				</ul>
				<p>
					日本は偶然にもVOCALOIDがブレイクするのに最も適した国だったのです。
				</p>
				<h4 id="vocaloidfeatures">VOCALOIDの特徴</h4>
				<ul>
					<li>
						VOCALOIDのソフトは楽器として扱われ、どんな目的にも自由に使うことが出来ます。
					</li>
					<li>
						クリプトン・フューチャー・メディア株式会社のピアプロ・キャラクター・ライセンスはキャラクター画像の非営利使用を許可しています。
					</li>
					<li>
						全てのコンテンツ（楽曲・動画・イラスト等）は誰に強制されたわけでもなく、ユーザー達がそれぞれの目的（娯楽・自己表現・販売等）のために自分自身で創ったものです。
					</li>
					<li>
						作詞作曲家、ミュージシャン、イラストレーター、アニメーター、プログラマー等のコラボレーション。
					</li>
					<li>
						VOCALOIDとプロデューサーのセルフプロモーション。基本的にはコミュニティがミクを選び彼女を有名にしました（GUMIも同様）。
					</li>
					<li>
						音楽を共有し、本源を支えること。殆どのVOCALOID曲や動画はニコ動、ピアプロ、YouTubeで無料で視聴出来、更に私達はアルバムを購入することでアーティストを支援することも出来ます。それは日本以外に住む人にも可能で、実際行われていることです。楽器やハード・ソフト、そしてその扱いを習得するには多くのコストが掛かり、また、プロデューサーの多くはインディーズで、CDの売り上げは彼らにとって重要です。もしあなたが本当にVOCALOIDプロデューサー達による音楽が好きであるなら、どうか彼らを支えて下さい。
					</li>
					<li>コンサートがあります。</li>
				</ul>
				<h4 id="vocaloidpv">PV</h4>
				<p>
					PVは楽曲の一部となってリスナーをVOCALOID曲に引き込むために使用され、またPV自体の芸術性もVOCALOID文化の一端を担っています。
				</p>
				<p>
					日本の<a href="http://www.nicovideo.jp/">ニコニコ</a>
					はVOCALOID楽曲・動画を投稿するのに利用され、またコンサートやイベント等の配信を行い、日本におけるVOCALOIDの一般化に大きく貢献しました。
				</p>
				<h4 id="ytnndsoundquality">YouTubeとニコニコの音質</h4>
				<p>
					ニコニコには MP3 CBR 64 kbps（エコノミー）からAAC VBR ~370
					kpbsまで、様々な音質のファイルがありますが、概してYouTube（多くはAAC
					VBR ~128 kbps）より高音質です（解像度は同程度）。
				</p>
				<h4 id="vocaloidmmd">MMD</h4>
				<p>
					<a href="https://ja.wikipedia.org/wiki/MikuMikuDance">
						MikuMikuDance
					</a>
					は樋口M氏が制作したVOCALOID
					PVに使えるフリーで高性能な人気の3Dアニメソフトです。
				</p>
				<p>
					MMDは初音ミクの成功の一因であり、MMD自体もMMD杯が開催されたり東方・アイマス等の動画制作に用いられるなど、一つの文化を形成しています。
				</p>
				<h4 id="vocaloidp">プロデューサー</h4>
				<a href="https://vocaloid.fandom.com/wiki/Category:Producer" title="">
					Vocaloid Wikiでの説明
				</a>
				<p>
					"プロデューサー"とはVOCALOIDに関連するオーディオビジュアル――多くはオリジナル曲やPV――を制作する人を指す言葉です。
				</p>
				<h4 id="vocaloidcircle">サークルとレーベル</h4>
				<p>
					同人サークルとはコミケやボーマスのような同人イベントで一般のレーベルの様に作品を自主販売する制作者達のグループです。
				</p>
				<p>
					レコードレーベルとはプロのスタジオサービス（楽器、レコーディング、マスタリング）とCDの様々なプロモーションや著作権保護を提供するものです。VOCALOIDのレーベルとして最もよく知られているものには、クリプトン・フューチャー・メディア株式会社による
					<a href="https://karent.jp/">“KarenT”</a>があります。
				</p>
			</HelpIndexTabs>
		);
	},
);

const Guidelines = React.memo(
	(): React.ReactElement => {
		const vdb = useVdb();

		return (
			<HelpIndexTabs tab="guidelines">
				<h3 id="glmain">General principles</h3>
				<ul>
					<li>
						既にデータベースに存在する事柄を新規に登録することは避けて下さい。同じ物について複数の項目が出来てしまっている場合はスタッフが削除あるいは一つの項目に統合します。
					</li>
					<li>
						完成もしくは承認された（下書きの状態ではない）記事を編集する際は、あなたがそれに手を加える適切な理由が有るのか、よく考えて下さい。そして、敢えて手を加える理由が誰の目にも明らかな場合を除いて、なるべく記事を編集した理由をはっきり示して下さい。
					</li>
					<li>
						これはVOCALOIDに関連するコンテンツのためのデータベースです。UTAUやVOCALOIDの関わるコラボレーションは承認されますが、全くVOCALOIDに関係の無いコンテンツは削除されます。
					</li>
					<li>
						有用な情報をもたらさない不正なファイル共有サイト（例：Mediafire、Megaupload、Torrents）へのダウンロードリンクを貼ることは“認められていません”。有益な情報源であるブログ、フォーラム、wiki等へのリンクは“認められています”。
					</li>
					<li>
						メディアファイル（mp3等）へ直接リンクすることは、それが公式・合法のものであっても基本的に認められていません。もし公式に楽曲や動画を配布しているのであれば、直リンクの代わりにそのアーティストの使用している正規のダウンロードページ（例：ピアプロ）へのリンクを貼って下さい。
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
				<h4>
					どうすれば新しくアーティスト（プロデューサー）、アルバム、曲を追加出来るのですか？
				</h4>
				<p>
					このサイトに登録し、トップページ左のメニューにあるリンク、または「アーティスト」、「アルバム」、「曲」それぞれのリストページにあるボタンから追加出来ます。
				</p>
				<h4>名前と訳</h4>
				<p>
					全ての名前は第一にオリジナルの言語――多くの場合日本語で記載されているのでしょうが、例えば英語や韓国語の場合もあるでしょう。このデータベースには３つの言語オプションを用意してあります。：
				</p>
				<ul>
					<li>
						"英語以外の言語"はオリジナルのタイトルが英語とローマ字以外の全ての言語、例えば日本語等の場合に使用します。
					</li>
					<li>
						"ローマ字表記"は、どの国の人にも読めるようにオリジナルの名前をアルファベットで記すものです。
					</li>
					<li>
						"英語"は正式名が英語である場合や正式名の英訳が判っている場合に使用します。
					</li>
					<li>
						"不明な言語"は意味を持たない言葉や未知の言語に対応しています。不明な言語による名称は他の言語の名称が設定されている場合は項目名として表示されることはありません。
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
				{/* Artists, producers and Vocaloids */}
				<h3 id="glproducers">アーティスト、プロデューサー、そしてVOCALOID</h3>
				{/* Artist types */}
				<h4 id="glartisttypes">アーティストの種類</h4>
				<p>
					全てのアーティストは、彼もしくは彼女が作品の中で何の役割を担う人なのか判断するための分類がなされています。
					その役割はアルバムや曲ごとに違ってくる可能性は有りますが、多くの場合、アーティストは一つの役割に深く関わります。
				</p>
				<ul>
					<li>
						"プロデューサー" -
						曲（VOCALOID曲の場合、通常はボーカルにVOCALOIDを使用します）を作る人。必ずしもオリジナルの作曲家とは限りません。
					</li>
					<li>"アニメーター" - 主にアニメーションPVを制作する人。</li>
					<li>
						"サークル" -
						同人イベント(コミケやボーマス等)で自主出版・自主販売を行うグループ。
					</li>
					<li>
						"レーベル" - その他のアーティスト達のアルバムを出版する営利会社。
					</li>
					<li>
						"その他のグループ" - レーベルからアルバムをリリースするグループ。
					</li>
					<li>
						"その他のボーカル" - 人間のシンガー（ニコニコにおける歌い手）。
					</li>
					<li>"その他の個人" - 作詞家やイラストレーターなど、その他の人々。</li>
				</ul>
				{/* Artist pictures */}
				<h4>アーティスト画像</h4>
				<p>
					アーティストに画像を追加する時は、その画像が彼もしくは彼女の作品（曲）にではなくアーティスト自身に関連深いものを選ぶよう心掛けて下さい。アーティストの項目にアルバムのジャケット画像をアップロードしないこと。とは言え、アーティストのメイン画像を選ぶに当たって共通のルールがあるわけではありません。アーティスト自身の写真や公式ロゴが望ましいです。アーティストに十分関連する画像であればいくつ追加しても構いません。
					<br />
					画像の使用に問題がある場合は削除します。
					<br />
					もし適した画像が無ければ、アーティストの公式プロフィールにあるどんな画像でも――例えばツイッター等の画像でも可能です。
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
				{/* Completed artist entries should meet the following criteria: */}
				<h4>
					アーティストの項目が完成されるには、以下の基準を満たしていなければなりません：
				</h4>
				<ul>
					<li>
						アーティストは少なくとも一つは不明な言語によらない名前を持っていること
					</li>
					<li>アーティストの種類が“不明”ではないこと</li>
					<li>
						アーティストに説明、または一つ以上の外部リンク（wiki、ニコニコのマイリスト等）が記載されていること
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
