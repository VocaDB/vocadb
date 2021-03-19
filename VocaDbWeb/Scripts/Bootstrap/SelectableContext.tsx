// Code from: https://github.com/react-bootstrap/react-bootstrap/blob/d707ca3a6690b54f0990b581cfe13c44e2a2c891/src/SelectableContext.tsx

import React from 'react';

// TODO (apparently this is a bare "onSelect"?)
type SelectableContextType = (key: string | null, event: any) => void;

const SelectableContext = React.createContext<SelectableContextType | null>(
	null,
);

export const makeEventKey = (
	eventKey: string | null,
	href: string | null = null,
): string | null => {
	if (eventKey != null) return String(eventKey);
	return href || null;
};

export default SelectableContext;
