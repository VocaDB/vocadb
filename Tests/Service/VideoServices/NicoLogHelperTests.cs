using System;
using System.Linq;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Tests.Service.VideoServices {

	/// <summary>
	/// Unit tests for <see cref="NicoLogHelper"/>.
	/// </summary>
	[TestClass]
	public class NicoLogHelperTests {

		[TestMethod]
		public void ParsePage_Test() {
			string samplePage = @"<!DOCTYPE html>
									<html lang=""ja"">
									<head>
									<meta charset=""utf-8"">
									<meta name=""viewport"" content=""width=device-width, initial-scale=1"">
									<title>sm40268860 - 琴葉茜の闇ゲー#185 「木シミュレーター（2023年版）」｜ニコログ</title>
									<base href=""https://www.nicolog.jp/"">
									<link rel=""icon"" type=""image/vnd.microsoft.ico"" href=""favicon.ico"">
									<link href=""css/bootstrap.min.css"" rel=""stylesheet"">
									<link href=""css/nicolog.css?2021061216"" rel=""stylesheet"">
									<script type=""text/javascript"" src=""js/amcharts.js""></script>
									<script type=""text/javascript"" src=""js/serial.js""></script>
									</head>
									<body>
									<script>
									  (function(i,s,o,g,r,a,m){i['GoogleAnalyticsObject']=r;i[r]=i[r]||function(){
									  (i[r].q=i[r].q||[]).push(arguments)},i[r].l=1*new Date();a=s.createElement(o),
									  m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m)
									  })(window,document,'script','//www.google-analytics.com/analytics.js','ga');
									  ga('create', 'UA-8910583-7', 'auto');
									  ga('send', 'pageview');
									</script>
									<nav class=""navbar navbar-default navbar-inverse navbar-fixed-top"">
										<div class=""container"">
											<div class=""navbar-header"">
												<a class=""navbar-brand"" href="""">ホーム</a>
												<button type=""button"" class=""navbar-toggle"" data-toggle=""collapse"" data-target=""#top-nav"">
													<span class=""icon-bar""></span>
													<span class=""icon-bar""></span>
													<span class=""icon-bar""></span>
												</button>
											</div>
											<div class=""collapse navbar-collapse"" id=""top-nav"">
											<ul class=""nav navbar-nav visible-xs"">
												<li class=""dropdown""><a href=""#"" class=""dropdown-toggle"" data-toggle=""dropdown"" role=""button"">最新ランキング<span class=""caret""></span></a>
												<ul class=""dropdown-menu"" role=""menu"">
													<li><a href=""ranking/now/"">全ジャンル</a></li>
													<li><a href=""ranking/now/hot-topic"">話題</a></li>
													<li><a href=""ranking/now/entertainment"">エンターテイメント</a></li>
													<li><a href=""ranking/now/radio"">ラジオ</a></li>
													<li><a href=""ranking/now/music_sound"">音楽・サウンド</a></li>
													<li><a href=""ranking/now/dance"">ダンス</a></li>
													<li><a href=""ranking/now/animal"">動物</a></li>
													<li><a href=""ranking/now/nature"">自然</a></li>
													<li><a href=""ranking/now/cooking"">料理</a></li>
													<li><a href=""ranking/now/traveling_outdoor"">旅行・アウトドア</a></li>
													<li><a href=""ranking/now/vehicle"">乗り物</a></li>
													<li><a href=""ranking/now/sports"">スポーツ</a></li>
													<li><a href=""ranking/now/society_politics_news"">社会・政治・時事</a></li>
													<li><a href=""ranking/now/technology_craft"">技術・工作</a></li>
													<li><a href=""ranking/now/commentary_lecture"">解説・講座</a></li>
													<li><a href=""ranking/now/anime"">アニメ</a></li>
													<li><a href=""ranking/now/game"">ゲーム</a></li>
													<li><a href=""ranking/now/other"">その他</a></li>
													<li><a href=""ranking/now/r18"">R-18</a></li>
												</ul>
												</li>
												<li><a href=""ranking/"">過去ランキング</a></li>
												<li><a href=""anime"">最新チャンネル配信無料アニメ</a></li>
												<li><a href=""million/"">ミリオン達成動画</a></li>
												<li><a href=""about"">このサイトについて</a></li>
												<li><a href=""news"">お知らせ</a></li>
												<li><a href=""contact"">お問い合わせ</a></li>
											</ul>
											<ul class=""nav navbar-nav navbar-right"">
											<li>
											<form class=""navbar-form"" action=""q.php"" method=""POST"">
												<div class=""form-group"">
													<select name=""target"" class=""form-control"">
													<option value=""video"" selected>動画ID</option>
													<option value=""user"" >ユーザーID</option>
													<option value=""ch"" >チャンネルID</option>
													</select>
												</div>
												<div class=""form-group"">
													<input type=""text"" name=""id"" class=""form-control"" value=""sm40268860"">
												</div>
												<button type=""submit"" class=""btn btn-info"">検索</button>
											</form>
											</li>
											</ul>
											</div>
										</div>
									</nav>
										<div class=""container"">
											<div id=""header"">
												<a href=""""><img src=""img/logo.png""></a>
												<small>ニコニコ動画のタグ履歴情報サイト</small>
											</div>
											<hr>
											<div class=""row"">
												<div class=""col-sm-3 hidden-xs"">
													<h4>ジャンル別24時間ランキング</h4>
													<ul>
														<li><a href=""ranking/now/"">全ジャンル</a></li>
														<li><a href=""ranking/now/hot-topic"">話題</a></li>
														<li><a href=""ranking/now/entertainment"">エンターテイメント</a></li>
														<li><a href=""ranking/now/radio"">ラジオ</a></li>
														<li><a href=""ranking/now/music_sound"">音楽・サウンド</a></li>
														<li><a href=""ranking/now/dance"">ダンス</a></li>
														<li><a href=""ranking/now/animal"">動物</a></li>
														<li><a href=""ranking/now/nature"">自然</a></li>
														<li><a href=""ranking/now/cooking"">料理</a></li>
														<li><a href=""ranking/now/traveling_outdoor"">旅行・アウトドア</a></li>
														<li><a href=""ranking/now/vehicle"">乗り物</a></li>
														<li><a href=""ranking/now/sports"">スポーツ</a></li>
														<li><a href=""ranking/now/society_politics_news"">社会・政治・時事</a></li>
														<li><a href=""ranking/now/technology_craft"">技術・工作</a></li>
														<li><a href=""ranking/now/commentary_lecture"">解説・講座</a></li>
														<li><a href=""ranking/now/anime"">アニメ</a></li>
														<li><a href=""ranking/now/game"">ゲーム</a></li>
														<li><a href=""ranking/now/other"">その他</a></li>
														<li><a href=""ranking/now/r18"">R-18</a></li>
													</ul>
													<hr>
													<h4><a href=""ranking/"">過去ランキング</a></h4>
													<hr>
													<h4>その他</h4>
													<p><a href=""sp/summary2021/"">年間人気動画ランキング #2021</a></p>
													<p><a href=""https://www.tsujino-akari.jp/sp/taberungo/"">たべるんごのうた 特設ページ</a></p>
													<p><a href=""anime"">最新チャンネル配信無料アニメ</a></p>
													<p><a href=""about"">このサイトについて</a></p>
													<p><a href=""news"">お知らせ</a></p>
													<p><a href=""million"">ミリオン達成動画</a></p>
													<p><a href=""contact"">お問い合わせ</a></p>
												</div>
											<div class=""col-sm-9 col-xs-12"">
											<ol class=""breadcrumb"">
											<li><a href="""">Home</a></li>
											<li>動画情報</li>
											<li class=""active"">sm40268860</li>
											</ol>
											<div class=""col-sm-2 center-block""><img src=""https://nicovideo.cdn.nimg.jp/thumbnails/40268860/40268860.66068857"" class=""center-block img-thumbnail"" /></div><div class=""col-sm-10""><dl class=""dl-horizontal""><dt>動画ID</dt><dd><a href=""https://www.nicovideo.jp/watch/sm40268860"">sm40268860</a></dd><dt>動画タイトル</dt><dd>琴葉茜の闇ゲー#185 「木シミュレーター（2023年版）」</dd><dt>投稿日時</dt><dd>2022年4月3日 18時00分00秒</dd><dt>長さ</dt><dd>0:03:59</dd><dt>投稿者</dt><dd><a href=""user/1594318"">moco78 (ID:1594318)</a></dd><dt>動画説明</dt><dd>ゆかりん「この世から花粉を出す植物が消滅しますように」茜ちゃん「生態系が終わりそうやな」次＞◇マイリスト 闇リスト：mylist/62445927Part1リスト：mylist/62982810 ◇投稿者 twitter：https://twitter.com/moco7800</dd></dl></div>		<hr>
											<ul class=""nav nav-tabs"">
												<li class=""active""><a href=""#tag-his"" data-toggle=""tab"">タグ履歴</a></li>
												<li><a href=""#ran-his"" data-toggle=""tab"">ランキング履歴</a></li>
												<li><a href=""#title-his"" data-toggle=""tab"">タイトル変更履歴</a></li>
												<li><a href=""#descr-his"" data-toggle=""tab"">動画説明変更履歴</a></li>
												<li><a href=""#chart"" data-toggle=""tab"">グラフ表示</a></li>
											</ul>
											<div id=""myTabContent"" class=""tab-content"">
												<div class=""tab-pane fade in active"" id=""tag-his""><div class=""table-responsive""><table class=""table table-condensed table-bordered""><tr><th class=""col-xs-2"">取得日時<a href=""watch/sm40268860?order=ASC"">▼</a></th><th class=""col-xs-1"">再生数</th><th class=""col-xs-1"">コメント数</th><th class=""col-sm-1"">マイリスト数</th><th class=""col-xs-7"">登録タグ</th></tr><tr><td class=""date col-xs-2"">2022年4月4日 0:40</td><td class=""counter col-xs-1"">23310</td><td class=""counter col-xs-1"">931</td><td class=""counter col-xs-1"">93</td><td class=""tdtag col-xs-7""><ul class=""list-inline""><li class=""genre"" title=""ジャンル"">ゲーム</li><li class=""category"" title=""カテゴリタグ"">ゲーム</li><li class=""lock"" title=""タグロック"">VOICEROID実況プレイ</li><li class=""lock"" title=""タグロック"">琴葉茜実況プレイ</li><li class=""lock"" title=""タグロック"">琴葉茜・葵実況プレイ</li><li class=""lock"" title=""タグロック"">琴葉茜</li><li class=""lock"" title=""タグロック"">結月ゆかり実況プレイ</li><li class=""lock"" title=""タグロック"">Tree_Simulator</li><li class=""lock"" title=""タグロック"">tree_simulator_2023</li><li class=""tag"">虚無ゲー</li><li class=""tag"">ゲー無</li><li class=""tag"">マリオとワリオ</li></ul></td></tr><tr><td class=""date col-xs-2"">2022年4月3日 23:40</td><td class=""counter col-xs-1"">20186</td><td class=""counter col-xs-1"">844</td><td class=""counter col-xs-1"">83</td><td class=""tdtag col-xs-7""><ul class=""list-inline""><li class=""genre"" title=""ジャンル"">ゲーム</li><li class=""category"" title=""カテゴリタグ"">ゲーム</li><li class=""lock"" title=""タグロック"">VOICEROID実況プレイ</li><li class=""lock"" title=""タグロック"">琴葉茜実況プレイ</li><li class=""lock"" title=""タグロック"">琴葉茜・葵実況プレイ</li><li class=""lock"" title=""タグロック"">琴葉茜</li><li class=""lock"" title=""タグロック"">結月ゆかり実況プレイ</li><li class=""lock"" title=""タグロック"">Tree_Simulator</li><li class=""lock"" title=""タグロック"">tree_simulator_2023</li><li class=""tag"">虚無ゲー</li><li class=""tag"">ゲー無</li><li class=""tag"">マリオとワリオ</li></ul></td></tr><tr><td class=""date col-xs-2"">2022年4月3日 22:40</td><td class=""counter col-xs-1"">16902</td><td class=""counter col-xs-1"">752</td><td class=""counter col-xs-1"">76</td><td class=""tdtag col-xs-7""><ul class=""list-inline""><li class=""genre"" title=""ジャンル"">ゲーム</li><li class=""category"" title=""カテゴリタグ"">ゲーム</li><li class=""lock"" title=""タグロック"">VOICEROID実況プレイ</li><li class=""lock"" title=""タグロック"">琴葉茜実況プレイ</li><li class=""lock"" title=""タグロック"">琴葉茜・葵実況プレイ</li><li class=""lock"" title=""タグロック"">琴葉茜</li><li class=""lock"" title=""タグロック"">結月ゆかり実況プレイ</li><li class=""lock"" title=""タグロック"">Tree_Simulator</li><li class=""lock"" title=""タグロック"">tree_simulator_2023</li><li class=""tag"">虚無ゲー</li><li class=""tag"">ゲー無</li><li class=""tag"">マリオとワリオ</li></ul></td></tr><tr><td class=""date col-xs-2"">2022年4月3日 21:40</td><td class=""counter col-xs-1"">13460</td><td class=""counter col-xs-1"">652</td><td class=""counter col-xs-1"">66</td><td class=""tdtag col-xs-7""><ul class=""list-inline""><li class=""genre"" title=""ジャンル"">ゲーム</li><li class=""category"" title=""カテゴリタグ"">ゲーム</li><li class=""lock"" title=""タグロック"">VOICEROID実況プレイ</li><li class=""lock"" title=""タグロック"">琴葉茜実況プレイ</li><li class=""lock"" title=""タグロック"">琴葉茜・葵実況プレイ</li><li class=""lock"" title=""タグロック"">琴葉茜</li><li class=""lock"" title=""タグロック"">結月ゆかり実況プレイ</li><li class=""lock"" title=""タグロック"">Tree_Simulator</li><li class=""lock"" title=""タグロック"">tree_simulator_2023</li><li class=""tag"">虚無ゲー</li><li class=""tag"">ゲー無</li><li class=""tag"">マリオとワリオ</li></ul></td></tr><tr><td class=""date col-xs-2"">2022年4月3日 20:41</td><td class=""counter col-xs-1"">9875</td><td class=""counter col-xs-1"">547</td><td class=""counter col-xs-1"">61</td><td class=""tdtag col-xs-7""><ul class=""list-inline""><li class=""genre"" title=""ジャンル"">ゲーム</li><li class=""category"" title=""カテゴリタグ"">ゲーム</li><li class=""lock"" title=""タグロック"">VOICEROID実況プレイ</li><li class=""lock"" title=""タグロック"">琴葉茜実況プレイ</li><li class=""lock"" title=""タグロック"">琴葉茜・葵実況プレイ</li><li class=""lock"" title=""タグロック"">琴葉茜</li><li class=""lock"" title=""タグロック"">結月ゆかり実況プレイ</li><li class=""lock"" title=""タグロック"">Tree_Simulator</li><li class=""lock"" title=""タグロック"">tree_simulator_2023</li><li class=""tag"">虚無ゲー</li><li class=""tag"">ゲー無</li><li class=""tag"">マリオとワリオ</li></ul></td></tr><tr><td class=""date col-xs-2"">2022年4月3日 19:41</td><td class=""counter col-xs-1"">5935</td><td class=""counter col-xs-1"">395</td><td class=""counter col-xs-1"">55</td><td class=""tdtag col-xs-7""><ul class=""list-inline""><li class=""genre"" title=""ジャンル"">ゲーム</li><li class=""category"" title=""カテゴリタグ"">ゲーム</li><li class=""lock"" title=""タグロック"">VOICEROID実況プレイ</li><li class=""lock"" title=""タグロック"">琴葉茜実況プレイ</li><li class=""lock"" title=""タグロック"">琴葉茜・葵実況プレイ</li><li class=""lock"" title=""タグロック"">琴葉茜</li><li class=""lock"" title=""タグロック"">結月ゆかり実況プレイ</li><li class=""lock"" title=""タグロック"">Tree_Simulator</li><li class=""lock"" title=""タグロック"">tree_simulator_2023</li><li class=""tag"">虚無ゲー</li><li class=""tag"">ゲー無</li><li class=""tag"">マリオとワリオ</li></ul></td></tr><tr><td class=""date col-xs-2"">2022年4月3日 19:06</td><td class=""counter col-xs-1"">3565</td><td class=""counter col-xs-1"">303</td><td class=""counter col-xs-1"">42</td><td class=""tdtag col-xs-7""><ul class=""list-inline""><li class=""genre"" title=""ジャンル"">ゲーム</li><li class=""category"" title=""カテゴリタグ"">ゲーム</li><li class=""lock"" title=""タグロック"">VOICEROID実況プレイ</li><li class=""lock"" title=""タグロック"">琴葉茜実況プレイ</li><li class=""lock"" title=""タグロック"">琴葉茜・葵実況プレイ</li><li class=""lock"" title=""タグロック"">琴葉茜</li><li class=""lock"" title=""タグロック"">結月ゆかり実況プレイ</li><li class=""lock"" title=""タグロック"">Tree_Simulator</li><li class=""lock"" title=""タグロック"">tree_simulator_2023</li><li class=""tag"">虚無ゲー</li><li class=""tag"">ゲー無</li><li class=""tag"">moco78</li></ul></td></tr><tr><td class=""date col-xs-2"">2022年4月3日 18:05</td><td class=""counter col-xs-1"">390</td><td class=""counter col-xs-1"">22</td><td class=""counter col-xs-1"">12</td><td class=""tdtag col-xs-7""><ul class=""list-inline""><li class=""genre"" title=""ジャンル"">ゲーム</li><li class=""category"" title=""カテゴリタグ"">ゲーム</li><li class=""lock"" title=""タグロック"">VOICEROID実況プレイ</li><li class=""lock"" title=""タグロック"">琴葉茜実況プレイ</li><li class=""lock"" title=""タグロック"">琴葉茜・葵実況プレイ</li><li class=""lock"" title=""タグロック"">琴葉茜</li><li class=""lock"" title=""タグロック"">結月ゆかり実況プレイ</li><li class=""lock"" title=""タグロック"">Tree_Simulator</li><li class=""lock"" title=""タグロック"">tree_simulator_2023</li></ul></td></tr></table></div><div class=""text-center""><ul class=""pagination""><li class=""active""><a href=""watch/sm40268860?page=1"">1</a></li></ul></div></div>
												<div class=""tab-pane fade"" id=""ran-his""><ul class=""nav nav-tabs""><li class=""active""><a href=""#ran-his-24h"" data-toggle=""tab"">24時間ランキング履歴</a></li><li><a href=""#ran-his-1h"" data-toggle=""tab"">毎時ランキング履歴</a></li></ul><div id=""myTabContent2"" class=""tab-content""><div class=""tab-pane in active"" id=""ran-his-24h""><div class=""table-responsive""><p>最新の7日間でのランキング履歴を表示しています。</p><table class=""table table-condensed table-bordered text-nowrap""><tr><th>日時</th><th>総合</th><th colspan=""2"">ジャンル総合</th><th colspan=""4"">ジャンル > 人気のタグ</th><th>話題総合</th></tr><tr><td>2022年04月04日 00時</td><td class=""counter"">1位</td><td class=""r-tag"">ゲーム</td><td class=""r-val"">1位</td><td class=""r-tag"">VOICEROID実況プレイ</td><td class=""r-val"">1位</td><td class=""r-tag"">ゲーム</td><td class=""r-val"">1位</td><td class=""counter"">-</td></tr><tr><td>2022年04月03日 23時</td><td class=""counter"">3位</td><td class=""r-tag"">ゲーム</td><td class=""r-val"">3位</td><td class=""r-tag"">VOICEROID実況プレイ</td><td class=""r-val"">2位</td><td class=""r-tag"">ゲーム</td><td class=""r-val"">2位</td><td class=""counter"">-</td></tr><tr><td>2022年04月03日 22時</td><td class=""counter"">5位</td><td class=""r-tag"">ゲーム</td><td class=""r-val"">5位</td><td class=""r-tag"">VOICEROID実況プレイ</td><td class=""r-val"">4位</td><td class=""r-tag"">ゲーム</td><td class=""r-val"">3位</td><td class=""counter"">-</td></tr><tr><td>2022年04月03日 21時</td><td class=""counter"">8位</td><td class=""r-tag"">ゲーム</td><td class=""r-val"">6位</td><td class=""r-tag"">VOICEROID実況プレイ</td><td class=""r-val"">4位</td><td class=""r-tag"">ゲーム</td><td class=""r-val"">3位</td><td class=""counter"">-</td></tr><tr><td>2022年04月03日 20時</td><td class=""counter"">15位</td><td class=""r-tag"">ゲーム</td><td class=""r-val"">10位</td><td class=""r-tag"">VOICEROID実況プレイ</td><td class=""r-val"">4位</td><td class=""r-tag"">ゲーム</td><td class=""r-val"">4位</td><td class=""counter"">-</td></tr><tr><td>2022年04月03日 19時</td><td class=""counter"">53位</td><td class=""r-tag"">ゲーム</td><td class=""r-val"">25位</td><td class=""r-tag"">VOICEROID実況プレイ</td><td class=""r-val"">12位</td><td class=""r-tag"">ゲーム</td><td class=""r-val"">12位</td><td class=""counter"">-</td></tr></table></div></div><div class=""tab-pane"" id=""ran-his-1h""><div class=""table-responsive""><p>最新の7日間でのランキング履歴を表示しています。</p><table class=""table table-condensed table-bordered text-nowrap""><tr><th>日時</th><th>総合</th><th colspan=""2"">ジャンル総合</th><th colspan=""4"">ジャンル > 人気のタグ</th><th>話題総合</th></tr><tr><td>2022年04月04日 00時</td><td class=""counter"">1位</td><td class=""r-tag"">ゲーム</td><td class=""r-val"">1位</td><td class=""r-tag"">VOICEROID実況プレイ</td><td class=""r-val"">1位</td><td class=""r-tag"">ゲーム</td><td class=""r-val"">1位</td><td class=""counter"">-</td></tr><tr><td>2022年04月03日 23時</td><td class=""counter"">1位</td><td class=""r-tag"">ゲーム</td><td class=""r-val"">1位</td><td class=""r-tag"">VOICEROID実況プレイ</td><td class=""r-val"">1位</td><td class=""r-tag"">ゲーム</td><td class=""r-val"">1位</td><td class=""counter"">-</td></tr><tr><td>2022年04月03日 22時</td><td class=""counter"">1位</td><td class=""r-tag"">ゲーム</td><td class=""r-val"">1位</td><td class=""r-tag"">VOICEROID実況プレイ</td><td class=""r-val"">1位</td><td class=""r-tag"">ゲーム</td><td class=""r-val"">1位</td><td class=""counter"">-</td></tr><tr><td>2022年04月03日 21時</td><td class=""counter"">1位</td><td class=""r-tag"">ゲーム</td><td class=""r-val"">1位</td><td class=""r-tag"">VOICEROID実況プレイ</td><td class=""r-val"">1位</td><td class=""r-tag"">ゲーム</td><td class=""r-val"">1位</td><td class=""counter"">-</td></tr><tr><td>2022年04月03日 20時</td><td class=""counter"">1位</td><td class=""r-tag"">ゲーム</td><td class=""r-val"">1位</td><td class=""r-tag"">VOICEROID実況プレイ</td><td class=""r-val"">1位</td><td class=""r-tag"">ゲーム</td><td class=""r-val"">1位</td><td class=""counter"">-</td></tr><tr><td>2022年04月03日 19時</td><td class=""counter"">1位</td><td class=""r-tag"">ゲーム</td><td class=""r-val"">1位</td><td class=""r-tag"">VOICEROID実況プレイ</td><td class=""r-val"">1位</td><td class=""r-tag"">ゲーム</td><td class=""r-val"">1位</td><td class=""counter"">-</td></tr></table></div></div></div></div>
												<div class=""tab-pane fade"" id=""title-his""><p>履歴はありません。</p></div>
												<div class=""tab-pane fade"" id=""descr-his""><p>履歴はありません。</p></div>
												<div class=""tab-pane fade"" id=""chart""><p>※最新の800件を表示しています。</p><div id=""chartdiv"" style=""width: 100%; height: 500px;"" ></div></div>
											</div>
										</div>
										</div></div>
									</body>
									</html>";

			HtmlDocument samplePageHtml = new HtmlDocument();
			samplePageHtml.LoadHtml(samplePage);
			VideoTitleParseResult result = NicoLogHelper.ParsePage(samplePageHtml);

			Assert.AreEqual("琴葉茜の闇ゲー#185 「木シミュレーター（2023年版）」", result.Title);
			Assert.AreEqual("moco78", result.Author);
			Assert.AreEqual("1594318", result.AuthorId);
			Assert.AreEqual(239, result.LengthSeconds);
			Assert.AreEqual(new DateTime(2022, 4, 3, 18, 0, 0), result.UploadDate);
			foreach (var tag in result.Tags.Zip(new[]{"ゲーム", "ゲーム", "VOICEROID実況プレイ", "琴葉茜実況プレイ", "琴葉茜・葵実況プレイ", "琴葉茜", "結月ゆかり実況プレイ", "Tree_Simulator", "tree_simulator_2023", "虚無ゲー", "ゲー無", "マリオとワリオ"}, (s, s1) => (s, s1))) {
				Assert.AreEqual(tag.s1, tag.s);
			}
			Assert.AreEqual("https://nicovideo.cdn.nimg.jp/thumbnails/40268860/40268860.66068857", result.ThumbUrl);
		}

	}

}
