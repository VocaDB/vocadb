import { Center, Paper, Slider, createStyles } from '@mantine/core';
import PlayerControls from './PlayerControls';
import { usePlayerStore } from '@/nostalgic-darling/stores/usePlayerStore';
import { useInterval } from '@mantine/hooks';
import React, { useRef, useState } from 'react';

const useStyles = createStyles((theme) => ({
	base: {
		position: 'fixed',
		height: 65,
		right: 0,
		bottom: 0,
		left: 300,
		[theme.fn.smallerThan('lg')]: {
			left: 220,
		},
		[theme.fn.smallerThan('sm')]: {
			left: 0,
		},
	},
	footer: {
		display: 'flex',
		flexDirection: 'column',
		alignContent: 'center',
		justifyContent: 'flex-start',
		height: '100%',
	},
}));

const CustomFooter = () => {
	const [playerApi] = usePlayerStore((set) => [set.playerApi]);
	const styles = useStyles();
	const [progress, setProgress] = useState<number>(0);
	// https://github.com/mantinedev/mantine/issues/2840
	const currentState = useRef(playerApi);
	currentState.current = playerApi;

	const interval = useInterval(() => {
		if (playerApi === undefined) return;
		setProgress(playerApi.getCurrentTime() / playerApi.getDuration());
	}, 500);

	React.useEffect(() => {
		interval.start();
		return interval.stop();
	}, [playerApi]);

	return (
		<div className={styles.classes.base}>
			<Paper<'footer'> className={styles.classes.footer} component="footer">
				<Slider
					styles={() => ({
						trackContainer: {},
					})}
					w="100%"
					size="sm"
					value={progress * 100}
					showLabelOnHover={false}
					onChange={(newProgress) => {
						if (interval.active) interval.stop();
						setProgress(newProgress / 100);
					}}
					label={null}
					onChangeEnd={(progress) => {
						interval.start();
						if (currentState.current === undefined) return;
						currentState.current.setCurrentTime(
							(progress / 100) * currentState.current.getDuration()
						);
					}}
				/>
				{/* TODO: Use flex container instead of Center */}
				<Center h="100%" mb={6}>
					<PlayerControls />
				</Center>
			</Paper>
		</div>
	);
};

export default CustomFooter;

