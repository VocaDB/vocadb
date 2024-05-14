"use client";

import React from "react";

export type AcceptsOnChange = React.ReactElement<{
	onChange: () => void;
	name: string;
}>;

export const Form: React.FC<{
	children: AcceptsOnChange[];
	submit: (key: string) => any;
}> = ({ children, submit }) => {
	// React.Children.forEach(children, (e) => {
	// 	if (!React.isValidElement(e)) return;

	// 	const { name } = e.props;
	// });
	return (
		<>
			{children.map((Input, index) => {
				return React.cloneElement(Input, {
					onChange: () => submit(Input.props.name),
					key: index,
				});
			})}
		</>
	);
};

