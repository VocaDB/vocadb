import React from 'react';
import { useRootClose } from 'react-overlays';

const useContextMenu = (
	ref: React.MutableRefObject<HTMLElement>,
): {
	show: boolean;
	setShow: (value: boolean) => void;
	position: { x: number; y: number };
	handleContextMenu: (e: React.MouseEvent<HTMLElement, MouseEvent>) => void;
} => {
	const [show, setShow] = React.useState(false);
	const [position, setPosition] = React.useState({ x: 0, y: 0 });

	const handleRootClose = (): void => setShow(false);
	// This enables the popup to click anywhere to dismiss.
	// See https://react-bootstrap.github.io/react-overlays/api/useRootClose.
	useRootClose(ref, handleRootClose, { disabled: !show });

	// Close the popup when the window is scrolled or resized.
	const handleScroll = React.useCallback(() => setShow(false), []);
	React.useEffect(() => {
		window.addEventListener('scroll', handleScroll);
		window.addEventListener('resize', handleScroll);

		return (): void => {
			window.removeEventListener('scroll', handleScroll);
			window.removeEventListener('resize', handleScroll);
		};
	}, [handleScroll]);

	const handleContextMenu = React.useCallback(
		(e: React.MouseEvent<HTMLElement, MouseEvent>) => {
			e.preventDefault();

			setPosition({
				x: e.pageX - window.scrollX,
				y: e.pageY - window.scrollY,
			});

			setShow(true);
		},
		[],
	);

	return { show, setShow, position, handleContextMenu };
};

export default useContextMenu;
