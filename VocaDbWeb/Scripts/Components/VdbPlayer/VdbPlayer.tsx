import Button from '@Bootstrap/Button';
import Container from '@Bootstrap/Container';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import { css } from '@emotion/react';
import classNames from 'classnames';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { Link } from 'react-router-dom';

import ButtonGroup from '../../Bootstrap/ButtonGroup';
import { useVdbPlayer } from './VdbPlayerContext';

const VdbPlayer = observer(
	(): React.ReactElement => {
		const vdbPlayer = useVdbPlayer();

		const handleEntryLinkClick = React.useCallback(() => {
			vdbPlayer.collapse();
		}, [vdbPlayer]);

		return (
			<div
				css={{
					position: 'fixed',
					left: 0,
					right: 0,
					top: vdbPlayer.expanded ? 0 : undefined,
					bottom: 0,
					zIndex: 3939,
					backgroundColor: 'rgb(39, 39, 39)',
					display: 'flex',
					flexDirection: 'column',
				}}
			>
				<div
					css={{
						display: vdbPlayer.expanded ? undefined : 'none',
						flexGrow: 1,
						backgroundColor: 'black',
					}}
				></div>

				<div>
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

							<div css={{ width: 220 }}>
								<div css={{ display: 'flex', alignItems: 'center' }}>
									{vdbPlayer.entry && (
										<Link
											to={EntryUrlMapper.details_entry(vdbPlayer.entry.entry)}
											onClick={handleEntryLinkClick}
											css={{ marginRight: 8 }}
										>
											<div
												css={{
													width: 64,
													height: 36,
													backgroundColor: 'rgb(28, 28, 28)',
													backgroundImage: `url(${vdbPlayer.entry.entry.mainPicture?.urlThumb})`,
													backgroundSize: 'cover',
													backgroundPosition: 'center',
												}}
											/>
										</Link>
									)}

									<div
										css={{
											flexGrow: 1,
											display: 'flex',
											minWidth: 0,
											flexDirection: 'column',
										}}
									>
										{vdbPlayer.entry && (
											<>
												<Link
													to={EntryUrlMapper.details_entry(
														vdbPlayer.entry.entry,
													)}
													onClick={handleEntryLinkClick}
													css={css`
														color: white;
														&:hover {
															color: white;
														}
														&:visited {
															color: white;
														}
														font-weight: bold;
														overflow: hidden;
														text-overflow: ellipsis;
														white-space: nowrap;
													`}
												>
													{vdbPlayer.entry.entry.name}
												</Link>
												<div css={{ display: 'flex' }}>
													<span
														css={{
															color: '#999999',
															overflow: 'hidden',
															textOverflow: 'ellipsis',
															whiteSpace: 'nowrap',
														}}
													>
														{vdbPlayer.entry.entry.artistString}
													</span>
												</div>
											</>
										)}
									</div>
								</div>
							</div>

							<ButtonGroup css={{ marginLeft: 8 }}>
								{vdbPlayer.expanded ? (
									<Button
										variant="inverse"
										title="Collapse" /* TODO: localize */
										onClick={vdbPlayer.collapse}
									>
										<i className="icon-resize-small icon-white" />
									</Button>
								) : (
									<Button
										variant="inverse"
										title="Expand" /* TODO: localize */
										onClick={vdbPlayer.expand}
									>
										<i className="icon-resize-full icon-white" />
									</Button>
								)}
							</ButtonGroup>
						</div>
					</Container>
				</div>
			</div>
		);
	},
);

export default VdbPlayer;
