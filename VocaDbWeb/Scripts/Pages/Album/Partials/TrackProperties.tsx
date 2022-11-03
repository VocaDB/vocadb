import { TrackPropertiesStore } from '@/Stores/Album/AlbumEditStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';

interface TrackPropertiesProps {
	trackPropertiesStore: TrackPropertiesStore;
}

const TrackProperties = observer(
	({ trackPropertiesStore }: TrackPropertiesProps): React.ReactElement => {
		return (
			<>
				<div>
					{trackPropertiesStore.song && (
						<h3>{
							`Artists for song ${trackPropertiesStore.song.songName}` /* LOCALIZE */
						}</h3>
					)}
				</div>

				<h5>
					Selected artists{/* LOCALIZE */}{' '}
					<small>(click to remove{/* LOCALIZE */})</small>
				</h5>
				<div>
					{trackPropertiesStore.artistSelections.map(
						(artistSelection, index) => (
							<React.Fragment key={index}>
								{artistSelection.selected && (
									<>
										{index > 0 && ' '}
										<div
											onClick={(): void =>
												runInAction(() => {
													artistSelection.selected = false;
												})
											}
											title={artistSelection.artist.additionalNames}
											className="label label-artistSelection"
										>
											<button type="button" className="close">
												&times;
											</button>
											<span>{artistSelection.artist.name}</span>
										</div>
									</>
								)}
							</React.Fragment>
						),
					)}
				</div>
				<span className="extraInfo">
					{
						!trackPropertiesStore.somethingSelected &&
							'No artists selected.' /* LOCALIZE */
					}
					<br />
				</span>
				<br />

				<h4>
					Album artists{/* LOCALIZE */}{' '}
					<small>(click to add{/* LOCALIZE */})</small>
				</h4>
				<div className="form-inline">
					Filter{/* LOCALIZE */}{' '}
					<input
						value={trackPropertiesStore.filter}
						onChange={(e): void =>
							runInAction(() => {
								trackPropertiesStore.filter = e.target.value;
							})
						}
						type="text"
						maxLength={128}
					/>
				</div>
				<br />

				<div>
					{trackPropertiesStore.artistSelections.map(
						(artistSelection, index) => (
							<React.Fragment key={index}>
								{!artistSelection.selected && artistSelection.visible && (
									<>
										{index > 0 && ' '}
										<div
											onClick={(): void =>
												runInAction(() => {
													artistSelection.selected = true;
												})
											}
											title={artistSelection.artist.additionalNames}
											className="label label-artistSelection"
											key={index}
										>
											<span>{artistSelection.artist.name}</span>
										</div>
									</>
								)}
							</React.Fragment>
						),
					)}
				</div>
				<span className="extraInfo">
					{
						!trackPropertiesStore.somethingSelectable &&
							'No selectable artists.' /* LOCALIZE */
					}
				</span>
			</>
		);
	},
);

export default TrackProperties;
