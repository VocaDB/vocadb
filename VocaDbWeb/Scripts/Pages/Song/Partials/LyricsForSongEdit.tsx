import Accordion from '@/Bootstrap/Accordion';
import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { UserLanguageCultureDropdownList } from '@/Components/Shared/Partials/Knockout/DropdownList';
import { HelpLabel } from '@/Components/Shared/Partials/Shared/HelpLabel';
import { userLanguageCultures } from '@/Components/userLanguageCultures';
import {
	LyricsForSongEditStore,
	LyricsForSongListEditStore,
} from '@/Stores/Song/LyricsForSongListEditStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface LyricsForSongEditProps {
	lyricsForSongListEditStore: LyricsForSongListEditStore;
	lyricsForSongEditStore: LyricsForSongEditStore;
	eventKey: string;
}

const LyricsForSongEdit = observer(
	({
		lyricsForSongListEditStore,
		lyricsForSongEditStore,
		eventKey,
	}: LyricsForSongEditProps): React.ReactElement => {
		const { t } = useTranslation([
			'ViewRes',
			'ViewRes.Song',
			'VocaDb.Web.Resources.Domain.Globalization',
		]);

		return (
			<Accordion.Item eventKey={eventKey}>
				<Accordion.Header>
					<i className="icon icon-chevron-down" />{' '}
					<span>{lyricsForSongEditStore.translationType}</span>
					{lyricsForSongEditStore.showLanguageSelection && (
						<>
							{' '}
							<span>
								(
								{
									userLanguageCultures[lyricsForSongEditStore.cultureCode]
										? `${
												userLanguageCultures[lyricsForSongEditStore.cultureCode]
													.nativeName
										  } (${
												userLanguageCultures[lyricsForSongEditStore.cultureCode]
													.englishName
										  })`
										: 'Other/Unknown' /* TODO: localize */
								}
								)
							</span>
						</>
					)}
					{(lyricsForSongEditStore.source || lyricsForSongEditStore.url) && (
						<>
							{' '}
							<span>
								from{' '}
								{lyricsForSongEditStore.source || lyricsForSongEditStore.url}
							</span>
						</>
					)}
				</Accordion.Header>
				<Accordion.Body>
					<div className="accordion-inner">
						<div>
							{lyricsForSongEditStore.showLanguageSelection && (
								<p>
									<HelpLabel
										label={t('ViewRes.Song:Edit.LyLanguage')}
										title="If multiple languages match, select the one that best represents the lyrics. If none of the options match, select 'Other/Unknown'." /* TODO: localize */
									/>{' '}
									<UserLanguageCultureDropdownList
										placeholder={t(
											'VocaDb.Web.Resources.Domain.Globalization:InterfaceLanguage.Other',
										)}
										value={lyricsForSongEditStore.cultureCode}
										onChange={(e): void =>
											runInAction(() => {
												lyricsForSongEditStore.cultureCode = e.target.value;
											})
										}
									/>
								</p>
							)}
							<div>
								<HelpLabel
									label="Source" /* TODO: localize */
									title="You can fill either label, URL or both" /* TODO: localize */
								/>{' '}
								<div className="inline input-prepend">
									<span className="add-on" title="Label" /* TODO: localize */>
										<i className="icon-pencil" />
									</span>
									<input
										type="text"
										value={lyricsForSongEditStore.source}
										onChange={(e): void =>
											runInAction(() => {
												lyricsForSongEditStore.source = e.target.value;
											})
										}
										className="input-large"
										size={45}
										maxLength={255}
										placeholder="Label" /* TODO: localize */
									/>
								</div>{' '}
								<div className="inline input-prepend">
									<span className="add-on" title="URL" /* TODO: localize */>
										<i className="icon-globe" />
									</span>
									<input
										type="text"
										value={lyricsForSongEditStore.url}
										onChange={(e): void =>
											runInAction(() => {
												lyricsForSongEditStore.url = e.target.value;
											})
										}
										className="input-xlarge"
										size={45}
										maxLength={500}
										placeholder="URL" /* TODO: localize */
									/>
								</div>
							</div>
						</div>

						<textarea
							value={lyricsForSongEditStore.value}
							onChange={(e): void =>
								runInAction(() => {
									lyricsForSongEditStore.value = e.target.value;
								})
							}
							cols={65}
							rows={30}
							className="input-xxlarge withMargin"
						/>
						<br />

						{lyricsForSongEditStore.translationType === 'Translation' && (
							<SafeAnchor
								onClick={(): void =>
									lyricsForSongListEditStore.remove(lyricsForSongEditStore)
								}
								href="#"
								className="textLink deleteLink"
							>
								{t('ViewRes:Shared.Delete')}
							</SafeAnchor>
						)}

						{lyricsForSongEditStore.translationType === 'Translation' &&
							!!lyricsForSongEditStore.id &&
							!lyricsForSongListEditStore.original.id && (
								<>
									{' '}
									<SafeAnchor
										onClick={(): void =>
											lyricsForSongListEditStore.changeToOriginal(
												lyricsForSongEditStore,
											)
										}
										href="#"
										className="textLink editLink"
									>
										Change to original{/* TODO: localize */}
									</SafeAnchor>
								</>
							)}
						{lyricsForSongEditStore.translationType === 'Original' &&
							!!lyricsForSongEditStore.id && (
								<>
									{' '}
									<SafeAnchor
										onClick={(): void =>
											lyricsForSongListEditStore.changeToTranslation(
												lyricsForSongEditStore,
											)
										}
										href="#"
										className="textLink editLink"
									>
										Change to translation{/* TODO: localize */}
									</SafeAnchor>
								</>
							)}
					</div>
				</Accordion.Body>
			</Accordion.Item>
		);
	},
);

export default LyricsForSongEdit;
