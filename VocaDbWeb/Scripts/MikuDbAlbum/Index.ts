import $ from 'jquery';

export const MikuDbAlbumIndex = (): void => {
	$(document).ready(function () {
		$('#importNewLink').button({
			icons: { primary: 'ui-icon-arrowthickstop-1-s' },
		});
	});
};
