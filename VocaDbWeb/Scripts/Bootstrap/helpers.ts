// Code from: https://github.com/react-bootstrap/react-bootstrap/blob/d707ca3a6690b54f0990b581cfe13c44e2a2c891/src/helpers.ts

import React from "react";

export interface BsPrefixAndClassNameOnlyProps {
	bsPrefix?: string;
	className?: string;
}

export interface BsPrefixProps<As extends React.ElementType = React.ElementType> extends BsPrefixAndClassNameOnlyProps {
	as?: As;
}

export type BsPrefixPropsWithChildren<
	As extends React.ElementType = React.ElementType
> = React.PropsWithChildren<BsPrefixProps<As>>;
