import $ from 'jquery';

const MikuDbAlbumIndex = (): void => {
	$(document).ready(function () {
		$('#importNewLink').button({
			icons: { primary: 'ui-icon-arrowthickstop-1-s' },
		});
	});
};

export default MikuDbAlbumIndex;
