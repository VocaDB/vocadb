// Code from: https://github.com/react-bootstrap/react-bootstrap/blob/8e3f3c2b5f232e17829df3474b7b6e398f22ff3d/src/Tab.tsx
import Tabs from '@restart/ui/Tabs';
import * as React from 'react';

import JQueryUITabContent from './JQueryUITabContent';
import JQueryUITabPane, { JQueryUITabPaneProps } from './JQueryUITabPane';

export interface JQueryUITabProps extends Omit<JQueryUITabPaneProps, 'title'> {
	title: React.ReactNode;
	disabled?: boolean;
	tabClassName?: string;
}

const JQueryUITab: React.FC<JQueryUITabProps> = () => {
	throw new Error(
		'ReactBootstrap: The `Tab` component is not meant to be rendered! ' +
			"It's an abstract component that is only valid as a direct Child of the `Tabs` Component. " +
			'For custom tabs components use TabPane and TabsContainer directly',
	);
	// Needed otherwise docs error out.
	// eslint-disable-next-line no-unreachable
	return <></>;
};

export default Object.assign(JQueryUITab, {
	Container: Tabs,
	Content: JQueryUITabContent,
	Pane: JQueryUITabPane,
});
