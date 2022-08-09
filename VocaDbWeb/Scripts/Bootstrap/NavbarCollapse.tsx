// Code from: https://github.com/react-bootstrap/react-bootstrap/blob/c11bc67ab3105e7a1839c0dcaacc5f1099885f02/src/NavbarCollapse.tsx
import Collapse, { CollapseProps } from '@/Bootstrap/Collapse';
import NavbarContext from '@/Bootstrap/NavbarContext';
import { useBootstrapPrefix } from '@/Bootstrap/ThemeProvider';
import { BsPrefixProps } from '@/Bootstrap/helpers';
import PropTypes from 'prop-types';
import * as React from 'react';
import { useContext } from 'react';

export interface NavbarCollapseProps
	extends Omit<CollapseProps, 'children'>,
		React.HTMLAttributes<HTMLDivElement>,
		BsPrefixProps {}

const propTypes = {
	/** @default 'navbar-collapse' */
	bsPrefix: PropTypes.string,
};

const NavbarCollapse = React.forwardRef<HTMLDivElement, NavbarCollapseProps>(
	({ children, bsPrefix, ...props }, ref) => {
		bsPrefix = useBootstrapPrefix(bsPrefix, 'nav-collapse');
		const context = useContext(NavbarContext);

		return (
			<Collapse in={!!(context && context.expanded)} {...props}>
				<div ref={ref} className={bsPrefix}>
					{children}
				</div>
			</Collapse>
		);
	},
);

NavbarCollapse.displayName = 'NavbarCollapse';
NavbarCollapse.propTypes = propTypes;

export default NavbarCollapse;
