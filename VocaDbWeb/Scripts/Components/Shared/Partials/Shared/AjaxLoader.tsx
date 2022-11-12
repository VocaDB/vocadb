import React from 'react';

interface AjaxLoaderProps {
	id?: string;
}

export const AjaxLoader = ({ id }: AjaxLoaderProps): React.ReactElement => {
	return id ? (
		<img id={id} src="/Content/ajax-loader.gif" alt="loading..." /* LOC */ />
	) : (
		<img src="/Content/ajax-loader.gif" alt="loading..." /* LOC */ />
	);
};
