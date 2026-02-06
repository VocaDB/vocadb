import Accordion from '@/Bootstrap/Accordion';
import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { UserLanguageCultureDropdownList } from '@/Components/Shared/Partials/Knockout/DropdownList';
import { HelpLabel } from '@/Components/Shared/Partials/Shared/HelpLabel';
import { CULTURE_CODE_UNKNOWN } from '@/Components/userLanguageCultures';
import { useCultureCodes } from '@/CultureCodesContext';
import { useLoginManager } from '@/LoginManagerContext';
import {
	LyricsForSongEditStore,
	LyricsForSongListEditStore,
} from '@/Stores/Song/LyricsForSongListEditStore';
import { franc } from 'franc-min';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React, { useState } from 'react';
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
		const [languageOptionsExtended, setLanguageOptionsExtended] = useState(
			lyricsForSongEditStore.cultureCodes.filter(
				(c) => c.length > 2 && c !== 'fil',
			).length > 0,
		);
		const { getCodeDescription, iso639to1, codes } = useCultureCodes();
		const loginManager = useLoginManager();

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
								{lyricsForSongEditStore.cultureCodes
									.map(
										(code) =>
											getCodeDescription(code)
												? `${getCodeDescription(code)?.nativeName} (${
														getCodeDescription(code)?.englishName
												  })`
												: 'Other/Unknown' /* LOC */,
									)
									.join(' / ')}
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
										dangerouslySetInnerHTML={{
											__html:
												'If multiple languages match, click "Add" to add more languages. If none of the options match, select \'Other/Unknown\'.' /* LOC */,
										}}
									/>{' '}
									<tbody>
										{lyricsForSongEditStore.cultureCodes.map(
											(cultureCode, index) => (
												<tr>
													<td>
														<UserLanguageCultureDropdownList
															key={index}
															value={cultureCode || CULTURE_CODE_UNKNOWN}
															extended={languageOptionsExtended}
															onChange={(e): void =>
																runInAction(() => {
																	lyricsForSongEditStore.replaceCultureCode(
																		index,
																		e.target.value,
																	);
																})
															}
														/>
													</td>
													{lyricsForSongEditStore.allowMultipleLanguages && (
														<td>
															<SafeAnchor
																onClick={(): void =>
																	lyricsForSongEditStore.removeCultureCode(
																		index,
																	)
																}
																href="#"
																className="nameDelete textLink deleteLink"
															>
																{t('ViewRes:Shared.Delete')}
															</SafeAnchor>
														</td>
													)}
												</tr>
											),
										)}
									</tbody>
									{lyricsForSongEditStore.allowMultipleLanguages &&
										(lyricsForSongEditStore.cultureCodes.length < 3 ||
											loginManager.canApproveEntries) && (
											<SafeAnchor
												onClick={(): void =>
													lyricsForSongEditStore.addCultureCode(
														CULTURE_CODE_UNKNOWN,
													)
												}
												href="#"
												className="textLink addLink"
											>
												{t('ViewRes:Shared.Add')}
											</SafeAnchor>
										)}
									{!languageOptionsExtended && (
										<SafeAnchor
											onClick={(): void => setLanguageOptionsExtended(true)}
											href="#"
											className="textLink addLink"
										>
											{t('ViewRes.Song:Edit.LyExtendLanguages')}{' '}
										</SafeAnchor>
									)}
								</p>
							)}
							<div>
								<HelpLabel
									label="Source" /* LOC */
									dangerouslySetInnerHTML={{
										__html: 'You can fill either label, URL or both' /* LOC */,
									}}
								/>{' '}
								<div className="inline input-prepend">
									<span className="add-on" title="Label" /* LOC */>
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
										placeholder="Label" /* LOC */
									/>
								</div>{' '}
								<div className="inline input-prepend">
									<span className="add-on" title="URL" /* LOC */>
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
										placeholder="URL" /* LOC */
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
							onBlur={(): void => {
								if (
									lyricsForSongEditStore.cultureCodes.length === 1 &&
									lyricsForSongEditStore.cultureCodes[0] === '' &&
									iso639to1 !== undefined &&
									codes !== undefined
								) {
									const detectedLang = franc(lyricsForSongEditStore.value);
									if (
										detectedLang in iso639to1 &&
										iso639to1[detectedLang] in codes
									) {
										lyricsForSongEditStore.cultureCodes[0] =
											iso639to1[detectedLang];
									}
								}
							}}
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
										Change to original{/* LOC */}
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
										Change to translation{/* LOC */}
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
