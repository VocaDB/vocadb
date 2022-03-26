import Button from '@Bootstrap/Button';
import Container from '@Bootstrap/Container';
import classNames from 'classnames';
import { observer } from 'mobx-react-lite';
import React from 'react';

import ButtonGroup from '../../Bootstrap/ButtonGroup';
import { useVdbPlayer } from './VdbPlayerContext';

const VdbPlayer = observer(
	(): React.ReactElement => {
		const vdbPlayer = useVdbPlayer();

		return (
			<div
				css={{
					position: 'fixed',
					left: 0,
					right: 0,
					bottom: 0,
					zIndex: 3939,
					backgroundColor: 'rgb(39, 39, 39)',
				}}
			>
				<Container>
					<div css={{ display: 'flex', height: 50, alignItems: 'center' }}>
						<ButtonGroup>
							<Button
								variant="inverse"
								title={
									`Shuffle: ${
										vdbPlayer.shuffle ? 'On' : 'Off'
									}` /* TODO: localize */
								}
								onClick={vdbPlayer.toggleShuffle}
								className={classNames(vdbPlayer.shuffle && 'active')}
							>
								<i className="icon-random icon-white" />
							</Button>
							<Button
								variant="inverse"
								title="Previous" /* TODO: localize */
								disabled={!vdbPlayer.hasPreviousSong}
							>
								<i className="icon-step-backward icon-white" />
							</Button>
							{vdbPlayer.playing ? (
								<Button
									variant="inverse"
									title="Pause" /* TODO: localize */
									onClick={vdbPlayer.pause}
								>
									<i className="icon-pause icon-white" />
								</Button>
							) : (
								<Button
									variant="inverse"
									title="Play" /* TODO: localize */
									onClick={vdbPlayer.play}
								>
									<i className="icon-play icon-white" />
								</Button>
							)}
							<Button
								variant="inverse"
								title="Next" /* TODO: localize */
								disabled={!vdbPlayer.hasNextSong}
							>
								<i className="icon-step-forward icon-white" />
							</Button>
							<Button
								variant="inverse"
								title={`Repeat: ${vdbPlayer.repeat}` /* TODO: localize */}
								onClick={vdbPlayer.toggleRepeat}
							>
								<i className="icon-repeat icon-white" />
							</Button>
						</ButtonGroup>

						<div css={{ flexGrow: 1 }}></div>

						<div css={{ width: 360 }}>
							<div css={{ display: 'flex', alignItems: 'center' }}>
								<div css={{ flexGrow: 1 }}>
									<div
										css={{
											width: 64,
											height: 36,
											backgroundColor: 'rgb(28, 28, 28)',
										}}
									/>
								</div>

								<ButtonGroup>
									<Button variant="inverse">
										<i className="icon-info-sign icon-white" />
									</Button>
								</ButtonGroup>
							</div>
						</div>
					</div>
				</Container>
			</div>
		);
	},
);

export default VdbPlayer;
