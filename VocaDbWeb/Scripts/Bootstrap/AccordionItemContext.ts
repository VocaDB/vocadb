// Code from: https://github.com/react-bootstrap/react-bootstrap/blob/f62da57493a63e40bd67b74f1414ac025c54d553/src/AccordionItemContext.ts.
import * as React from 'react';

export interface AccordionItemContextValue {
	eventKey: string;
}

const context = React.createContext<AccordionItemContextValue>({
	eventKey: '',
});
context.displayName = 'AccordionItemContext';

export default context;
