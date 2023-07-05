// Code from: https://github.com/react-bootstrap/react-bootstrap/blob/33f037ba1e9870463f1bd33a4fe66b8e2a7586f6/src/safeFindDOMNode.ts.
import ReactDOM from 'react-dom';

export default function safeFindDOMNode(
	componentOrElement: React.ComponentClass | Element | null | undefined,
): Element | Text | null {
	if (componentOrElement && 'setState' in componentOrElement) {
		// @ts-ignore
		return ReactDOM.findDOMNode(componentOrElement);
	}
	return (componentOrElement ?? null) as Element | Text | null;
}
