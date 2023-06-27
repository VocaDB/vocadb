import { Paper, Progress, createStyles } from '@mantine/core';
import PlayerControls from './PlayerControls';
import { usePlayerStore } from '@/nostalgic-darling/stores/usePlayerStore';
import { useInterval } from '@mantine/hooks';
import React, { useState } from 'react';

const useStyles = createStyles((theme) => ({
	base: {
		position: 'fixed',
		flexDirection: 'column',
		display: 'flex',
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
		alignContent: 'center',
		justifyContent: 'center',
		height: '100%',
	},
}));

const CustomFooter = () => {
	const [playerApi] = usePlayerStore((set) => [set.playerApi]);
	const styles = useStyles();
	const [progress, setProgress] = useState<number>(0);

	const interval = useInterval(() => {
		if (playerApi?.current === undefined) return;
		setProgress(playerApi.current.getCurrentTime() / playerApi.current.getDuration());
	}, 500);

	React.useEffect(() => {
		interval.start();
		return interval.stop();
	}, []);
	console.log(progress);

	return (
		<div className={styles.classes.base}>
			<Progress w="100%" size="sm" value={progress * 100} />
			<Paper<'footer'> className={styles.classes.footer} component="footer">
				<PlayerControls />
			</Paper>
		</div>
	);
};

export default CustomFooter;

