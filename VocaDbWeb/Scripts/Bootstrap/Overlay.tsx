// Code from: https://github.com/react-bootstrap/react-bootstrap/blob/33f037ba1e9870463f1bd33a4fe66b8e2a7586f6/src/Overlay.tsx.
import safeFindDOMNode from '@/Bootstrap/safeFindDOMNode';
import { Placement, PopperRef, RootCloseEvent } from '@/Bootstrap/types';
import useOverlayOffset from '@/Bootstrap/useOverlayOffset';
import useCallbackRef from '@restart/hooks/useCallbackRef';
import useEventCallback from '@restart/hooks/useEventCallback';
import useIsomorphicEffect from '@restart/hooks/useIsomorphicEffect';
import useMergedRefs from '@restart/hooks/useMergedRefs';
import BaseOverlay, {
	OverlayProps as BaseOverlayProps,
	OverlayArrowProps,
} from '@restart/ui/Overlay';
import { State } from '@restart/ui/usePopper';
import classNames from 'classnames';
import * as React from 'react';
import { useRef } from 'react';

export interface OverlayInjectedProps {
	ref: React.RefCallback<HTMLElement>;
	style: React.CSSProperties;
	'aria-labelledby'?: string;

	arrowProps: Partial<OverlayArrowProps>;

	show: boolean;
	placement: Placement | undefined;
	popper: PopperRef;
	[prop: string]: any;
}

export type OverlayChildren =
	| React.ReactElement<OverlayInjectedProps>
	| ((injected: OverlayInjectedProps) => React.ReactNode);

export interface OverlayProps
	extends Omit<BaseOverlayProps, 'children' | 'transition' | 'rootCloseEvent'> {
	children: OverlayChildren;
	placement?: Placement;
	rootCloseEvent?: RootCloseEvent;
}

const defaultProps: Partial<OverlayProps> = {
	rootClose: false,
	show: false,
	placement: 'top',
};

function wrapRefs(props: any, arrowProps: any): void {
	const { ref } = props;
	const { ref: aRef } = arrowProps;

	props.ref =
		ref.__wrapped || (ref.__wrapped = (r: any): any => ref(safeFindDOMNode(r)));
	arrowProps.ref =
		aRef.__wrapped ||
		(aRef.__wrapped = (r: any): any => aRef(safeFindDOMNode(r)));
}

const Overlay = React.forwardRef<HTMLElement, OverlayProps>(
	({ children: overlay, popperConfig = {}, ...outerProps }, outerRef) => {
		const popperRef = useRef<Partial<PopperRef>>({});
		const [firstRenderedState, setFirstRenderedState] = useCallbackRef<State>();
		const [ref, modifiers] = useOverlayOffset(outerProps.offset);
		const mergedRef = useMergedRefs(
			outerRef as React.MutableRefObject<HTMLElement>,
			ref,
		);

		const handleFirstUpdate = useEventCallback((state) => {
			setFirstRenderedState(state);
			popperConfig?.onFirstUpdate?.(state);
		});

		useIsomorphicEffect(() => {
			if (firstRenderedState) {
				popperRef.current.scheduleUpdate?.();
			}
		}, [firstRenderedState]);

		return (
			<BaseOverlay
				{...outerProps}
				ref={mergedRef}
				popperConfig={{
					...popperConfig,
					modifiers: modifiers.concat(popperConfig.modifiers || []),
					onFirstUpdate: handleFirstUpdate,
				}}
			>
				{(
					overlayProps,
					{ arrowProps, popper: popperObj, show },
				): React.ReactNode => {
					wrapRefs(overlayProps, arrowProps);
					// Need to get placement from popper object, handling case when overlay is flipped using 'flip' prop
					const updatedPlacement = popperObj?.placement;
					const popper = Object.assign(popperRef.current, {
						state: popperObj?.state,
						scheduleUpdate: popperObj?.update,
						placement: updatedPlacement,
						outOfBoundaries:
							popperObj?.state?.modifiersData.hide?.isReferenceHidden || false,
					});

					if (typeof overlay === 'function')
						return overlay({
							...overlayProps,
							placement: updatedPlacement,
							show,
							...(show && { className: 'show' }),
							popper,
							arrowProps,
						});

					return React.cloneElement(overlay as React.ReactElement, {
						...overlayProps,
						placement: updatedPlacement,
						arrowProps,
						popper,
						className: classNames(
							(overlay as React.ReactElement).props.className,
							show && 'show',
						),
						style: {
							...(overlay as React.ReactElement).props.style,
							...overlayProps.style,
						},
					});
				}}
			</BaseOverlay>
		);
	},
);

Overlay.displayName = 'Overlay';
Overlay.defaultProps = defaultProps;

export default Overlay;
