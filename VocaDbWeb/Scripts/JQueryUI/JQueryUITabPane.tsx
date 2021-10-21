// Code from: https://github.com/react-bootstrap/react-bootstrap/blob/8e3f3c2b5f232e17829df3474b7b6e398f22ff3d/src/TabPane.tsx
import {
	BsPrefixProps,
	BsPrefixRefForwardingComponent,
} from '@Bootstrap/helpers';
import NoopTransition from '@restart/ui/NoopTransition';
import SelectableContext from '@restart/ui/SelectableContext';
import TabContext from '@restart/ui/TabContext';
import { useTabPanel } from '@restart/ui/TabPanel';
import { EventKey, TransitionCallbacks } from '@restart/ui/types';
import classNames from 'classnames';
import * as React from 'react';

export interface JQueryUITabPaneProps
	extends TransitionCallbacks,
		BsPrefixProps,
		React.HTMLAttributes<HTMLElement> {
	eventKey?: EventKey;
	active?: boolean;
	mountOnEnter?: boolean;
	unmountOnExit?: boolean;
}

const JQueryUITabPane: BsPrefixRefForwardingComponent<
	'div',
	JQueryUITabPaneProps
> = React.forwardRef<HTMLElement, JQueryUITabPaneProps>(({ ...props }, ref) => {
	const [
		{
			className,
			// Need to define the default "as" during prop destructuring to be compatible with styled-components github.com/react-bootstrap/react-bootstrap/issues/3595
			as: Component = 'div',
			...rest
		},
		{
			isActive,
			onEnter,
			onEntering,
			onEntered,
			onExit,
			onExiting,
			onExited,
			mountOnEnter,
			unmountOnExit,
			transition: Transition = NoopTransition,
		},
	] = useTabPanel({
		...props,
	} as any);

	// We provide an empty the TabContext so `<Nav>`s in `<TabPanel>`s don't
	// conflict with the top level one.
	return (
		<TabContext.Provider value={null}>
			<SelectableContext.Provider value={null}>
				<Transition
					in={isActive}
					onEnter={onEnter}
					onEntering={onEntering}
					onEntered={onEntered}
					onExit={onExit}
					onExiting={onExiting}
					onExited={onExited}
					mountOnEnter={mountOnEnter}
					unmountOnExit={unmountOnExit as any}
				>
					<Component
						{...rest}
						ref={ref}
						className={classNames(
							className,
							isActive && 'active',
							'ui-tabs-panel',
							'ui-widget-content',
							'ui-corner-bottom',
						)}
						style={!isActive ? { display: 'none' } : undefined}
					/>
				</Transition>
			</SelectableContext.Provider>
		</TabContext.Provider>
	);
});

export default JQueryUITabPane;
