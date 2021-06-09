import AlbumRepository from '@Repositories/AlbumRepository';
import HttpClient from '@Shared/HttpClient';
import vdb from '@Shared/VdbStatic';
import AlbumMergeViewModel from '@ViewModels/Album/AlbumMergeViewModel';
import $ from 'jquery';
import ko from 'knockout';

const AlbumMerge = (model: { id: number }): void => {
	$(function () {
		const httpClient = new HttpClient();
		var repo = new AlbumRepository(httpClient, vdb.values.baseAddress);
		var vm = new AlbumMergeViewModel(repo, model.id);
		ko.applyBindings(vm);

		$('#mergeBtn').click(function () {
			return window.confirm('Are you sure you want to merge the albums?');
		});
	});
};

export default AlbumMerge;
