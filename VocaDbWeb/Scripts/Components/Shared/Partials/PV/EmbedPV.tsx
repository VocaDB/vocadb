import React from 'react';

interface EmbedPVProps {
	id?: string;
	html: string;
}

// HACK
// TODO: Replace this with React
const EmbedPV = ({ id, html }: EmbedPVProps): React.ReactElement => {
	return <div id={id} dangerouslySetInnerHTML={{ __html: html }} />;
};

export default EmbedPV;
