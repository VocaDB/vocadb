import React from 'react';

interface EmbedPVProps {
	html: string;
}

// HACK
// TODO: Replace this with React
const EmbedPV = ({ html }: EmbedPVProps): React.ReactElement => {
	return <div dangerouslySetInnerHTML={{ __html: html }} />;
};

export default EmbedPV;
