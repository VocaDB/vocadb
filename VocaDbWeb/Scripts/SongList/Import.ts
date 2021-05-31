import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import vdb from '@Shared/VdbStatic';
import ImportSongListViewModel from '@ViewModels/SongList/ImportSongListViewModel';
import $ from 'jquery';
import ko from 'knockout';

const SongListImport = (): void => {
	ko.punches.enableAll();

	$(function () {
		const httpClient = new HttpClient();
		var urlMapper = new UrlMapper(vdb.values.baseAddress);
		var viewModel = new ImportSongListViewModel(httpClient, urlMapper);
		ko.applyBindings(viewModel);
	});
};

export default SongListImport;
