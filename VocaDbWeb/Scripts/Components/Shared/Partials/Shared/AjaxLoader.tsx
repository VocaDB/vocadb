import React from 'react';

interface AjaxLoaderProps {
	id?: string;
}

const AjaxLoader = ({ id }: AjaxLoaderProps): React.ReactElement => {
	return id ? (
		<img
			id={id}
			src="/Content/ajax-loader.gif"
			alt="loading..." /* TODO: localize */
		/>
	) : (
		<img src="/Content/ajax-loader.gif" alt="loading..." /* TODO: localize */ />
	);
};

export default AjaxLoader;
