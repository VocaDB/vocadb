// Code from: https://github.com/react-bootstrap/react-bootstrap/blob/8a7e095e8032fdeac4fd1fdb41e6dfb452ae4494/src/types.tsx
import { State } from '@restart/ui/usePopper';

export type Variant =
	| 'primary'
	| 'danger'
	| 'warning'
	| 'success'
	| 'info'
	| 'inverse'
	| string;
export type ButtonVariant = Variant;

export type EventKey = string | number;

export type AlignDirection = 'start' | 'end';

export type ResponsiveAlignProp =
	| { sm: AlignDirection }
	| { md: AlignDirection }
	| { lg: AlignDirection }
	| { xl: AlignDirection }
	| { xxl: AlignDirection };

export type AlignType = AlignDirection | ResponsiveAlignProp;

export type Placement = import('@restart/ui/usePopper').Placement;

export type RootCloseEvent = 'click' | 'mousedown';

export interface PopperRef {
	state: State | undefined;
	outOfBoundaries: boolean;
	placement: Placement | undefined;
	scheduleUpdate?: () => void;
}
