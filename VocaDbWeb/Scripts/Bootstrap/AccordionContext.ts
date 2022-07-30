// Code from: https://github.com/react-bootstrap/react-bootstrap/blob/f62da57493a63e40bd67b74f1414ac025c54d553/src/AccordionContext.ts.
import * as React from 'react';

export type AccordionEventKey = string | string[] | null | undefined;

export declare type AccordionSelectCallback = (
	eventKey: AccordionEventKey,
	e: React.SyntheticEvent<unknown>,
) => void;

export interface AccordionContextValue {
	activeEventKey?: AccordionEventKey;
	onSelect?: AccordionSelectCallback;
	alwaysOpen?: boolean;
}

export function isAccordionItemSelected(
	activeEventKey: AccordionEventKey,
	eventKey: string,
): boolean {
	return Array.isArray(activeEventKey)
		? activeEventKey.includes(eventKey)
		: activeEventKey === eventKey;
}

const context = React.createContext<AccordionContextValue>({});
context.displayName = 'AccordionContext';

export default context;
