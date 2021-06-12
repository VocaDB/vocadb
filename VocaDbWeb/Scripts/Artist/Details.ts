import CommentContract from '@DataContracts/CommentContract';
import TagUsageForApiContract from '@DataContracts/Tag/TagUsageForApiContract';
import RepositoryFactory from '@Repositories/RepositoryFactory';
import SongRepository from '@Repositories/SongRepository';
import UrlMapper from '@Shared/UrlMapper';
import vdb from '@Shared/VdbStatic';
import { container } from '@Shared/inversify.config';
import ArtistDetailsViewModel from '@ViewModels/Artist/ArtistDetailsViewModel';
import PVPlayersFactory from '@ViewModels/PVs/PVPlayersFactory';
import { IEntryReportType } from '@ViewModels/ReportEntryViewModel';
import $ from 'jquery';
import ko from 'knockout';
import moment from 'moment';

const repoFactory = container.get(RepositoryFactory);

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
			vdb.functions.disableTabReload(ui.tab);
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
	canDeleteAllComments: boolean,
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
		moment.locale(vdb.values.culture);

		var urlMapper = new UrlMapper(vdb.values.baseAddress);

		var cultureCode = vdb.values.uiCulture;
		var loggedUserId = vdb.values.loggedUserId;
		var unknownPictureUrl = urlMapper.mapRelative('/Content/unknown.png');

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
			artistRepo,
			model.id,
			tagUsages,
			model.isAdded,
			model.emailNotifications,
			model.siteNotifications,
			hasEnglishDescription,
			unknownPictureUrl,
			vdb.values.languagePreference,
			urlMapper,
			albumRepository,
			songRepo,
			resourceRepo,
			userRepository,
			cultureCode,
			reportTypes,
			loggedUserId,
			canDeleteAllComments,
			pvPlayersFactory,
			latestComments,
		);
		ko.applyBindings(viewModel);

		initPage(model.id, saveStr, urlMapper, viewModel, songRepo);
	});
};

export default ArtistDetails;
