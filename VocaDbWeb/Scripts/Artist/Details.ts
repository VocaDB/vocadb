import CommentContract from '@DataContracts/CommentContract';
import TagUsageForApiContract from '@DataContracts/Tag/TagUsageForApiContract';
import LoginManager from '@Models/LoginManager';
import RepositoryFactory from '@Repositories/RepositoryFactory';
import SongRepository from '@Repositories/SongRepository';
import functions from '@Shared/GlobalFunctions';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import ArtistDetailsViewModel from '@ViewModels/Artist/ArtistDetailsViewModel';
import PVPlayersFactory from '@ViewModels/PVs/PVPlayersFactory';
import { IEntryReportType } from '@ViewModels/ReportEntryViewModel';
import $ from 'jquery';
import ko from 'knockout';
import moment from 'moment';

function initPage(
	artistId: number,
	saveStr: string,
	urlMapper: UrlMapper,
	viewModel: ArtistDetailsViewModel,
	songRepo: SongRepository,
): void {
	$('#addToUserLink').button({
		disabled: $('#addToUserLink').hasClass('disabled'),
		icons: { primary: 'ui-icon-heart' },
	});
	$('#removeFromUserLink')
		.button({
			disabled: $('#removeFromUserLink').hasClass('disabled'),
			icons: { primary: 'ui-icon-close' },
		})
		.next()
		.button({ text: false, icons: { primary: 'ui-icon-triangle-1-s' } })
		.parent()
		.buttonset();
	$('#editArtistLink').button({
		disabled: $('#editArtistLink').hasClass('disabled'),
		icons: { primary: 'ui-icon-wrench' },
	});
	$('#viewVersions').button({ icons: { primary: 'ui-icon-clock' } });
	$('#reportEntryLink').button({ icons: { primary: 'ui-icon-alert' } });
	$('#manageTags').button({ icons: { primary: 'ui-icon-wrench' } });
	$('#viewCommentsLink').click(function () {
		$('#tabs').tabs('option', 'active', 1);
		return false;
	});

	$('#tabs').tabs({
		load: function (event, ui) {
			functions.disableTabReload(ui.tab);
		},
		activate: function (event, ui) {
			if (ui.newTab.data('tab') === 'Discussion') {
				viewModel.comments.initComments();
			}
		},
	});

	if (window.location.hash === '#mainAlbumsTab') {
		viewModel.initMainAlbums();
	}
	if (window.location.hash === '#collaborationAlbumsTab') {
		viewModel.initCollaborationAlbums();
	}
	if (window.location.hash === '#songsTab') {
		viewModel.initSongs();
	}

	$('#newAlbums img').vdbAlbumToolTip();
	$('#topAlbums img').vdbAlbumToolTip();
	$('#baseVoicebank a').vdbArtistToolTip();
	$('#childVoicebanks a').vdbArtistToolTip();
	$('#groups a').vdbArtistToolTip();
	$('.artistLink').vdbArtistToolTip();
}

const ArtistDetails = (
	hasEnglishDescription: boolean,
	model: {
		emailNotifications: boolean;
		id: number;
		isAdded: boolean;
		latestComments: CommentContract[];
		siteNotifications: boolean;
		tags: TagUsageForApiContract[];
	},
	reportTypes: IEntryReportType[],
	saveStr: string,
): void => {
	$(function () {
		const loginManager = new LoginManager(vdb.values);
		const canDeleteAllComments = loginManager.canDeleteComments;

		moment.locale(vdb.values.culture);

		var urlMapper = new UrlMapper(vdb.values.baseAddress);

		var unknownPictureUrl = urlMapper.mapRelative('/Content/unknown.png');

		const httpClient = new HttpClient();
		var repoFactory = new RepositoryFactory(httpClient, urlMapper);
		var artistRepo = repoFactory.artistRepository();
		var albumRepository = repoFactory.albumRepository();
		var songRepo = repoFactory.songRepository();
		var resourceRepo = repoFactory.resourceRepository();
		var userRepository = repoFactory.userRepository();
		var pvPlayerElem = $('#pv-player-wrapper')[0];
		var pvPlayersFactory = new PVPlayersFactory(pvPlayerElem);
		var tagUsages = model.tags;
		var latestComments = model.latestComments;

		var viewModel = new ArtistDetailsViewModel(
			vdb.values,
			artistRepo,
			model.id,
			tagUsages,
			model.isAdded,
			model.emailNotifications,
			model.siteNotifications,
			hasEnglishDescription,
			unknownPictureUrl,
			urlMapper,
			albumRepository,
			songRepo,
			resourceRepo,
			userRepository,
			reportTypes,
			canDeleteAllComments,
			pvPlayersFactory,
			latestComments,
		);
		ko.applyBindings(viewModel);

		initPage(model.id, saveStr, urlMapper, viewModel, songRepo);
	});
};

export default ArtistDetails;
