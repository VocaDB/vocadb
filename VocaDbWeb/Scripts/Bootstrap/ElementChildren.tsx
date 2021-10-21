// Code from: https://github.com/react-bootstrap/react-bootstrap/blob/c11bc67ab3105e7a1839c0dcaacc5f1099885f02/src/ElementChildren.tsx
import * as React from 'react';

/**
 * Iterates through children that are typically specified as `props.children`,
 * but only maps over children that are "valid elements".
 *
 * The mapFunction provided index will be normalised to the components mapped,
 * so an invalid component would not increase the index.
 *
 */
function map<P = any>(
	children: any,
	func: (el: React.ReactElement<P>, index: number) => any,
): any {
	let index = 0;

	return React.Children.map(children, (child) =>
		React.isValidElement<P>(child) ? func(child, index++) : child,
	);
}

/**
 * Iterates through children that are "valid elements".
 *
 * The provided forEachFunc(child, index) will be called for each
 * leaf child with the index reflecting the position relative to "valid components".
 */
function forEach<P = any>(
	children: any,
	func: (el: React.ReactElement<P>, index: number) => void,
): any {
	let index = 0;
	React.Children.forEach(children, (child) => {
		if (React.isValidElement<P>(child)) func(child, index++);
	});
}

export { map, forEach };
