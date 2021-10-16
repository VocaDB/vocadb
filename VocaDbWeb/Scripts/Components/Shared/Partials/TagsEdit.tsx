import Button from '@Bootstrap/Button';
import SafeAnchor from '@Bootstrap/SafeAnchor';
import TagAutoComplete from '@Components/KnockoutExtensions/TagAutoComplete';
import JQueryUIButton from '@JQueryUI/JQueryUIButton';
import JQueryUICheckbox from '@JQueryUI/JQueryUICheckbox';
import JQueryUIDialog from '@JQueryUI/JQueryUIDialog';
import TagsEditStore from '@Stores/Tag/TagsEditStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

import AjaxLoader from './Shared/AjaxLoader';

interface TagsEditProps {
	tagsEditStore: TagsEditStore;
}

const TagsEdit = observer(
	({ tagsEditStore }: TagsEditProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes']);

		const inputRef = React.useRef<HTMLInputElement>(undefined!);

		return (
			<JQueryUIDialog
				autoOpen={tagsEditStore.dialogVisible}
				modal={true}
				width={500}
				buttons={[
					{
						text: t('ViewRes:Shared.Save'),
						click: tagsEditStore.save,
					},
				]}
				close={(): void =>
					runInAction(() => {
						tagsEditStore.dialogVisible = false;
					})
				}
				title={t('ViewRes:Shared.Tags')}
			>
				<div className="trackProperties">
					<p>{t('ViewRes:TagSelections.Info')}</p>

					<div>
						{tagsEditStore.selections.map((selection, index) => (
							<React.Fragment key={index}>
								{index > 0 && ' '}
								<span className="tag" title={selection.tag.additionalNames}>
									<JQueryUICheckbox
										id={`tagSelection${selection.tag.name.replace(
											/'/g,
											'_apos',
										)}`}
										checked={selection.selected}
										onChange={(e): void =>
											runInAction(() => {
												selection.selected = e.target.checked;
											})
										}
									>
										{selection.tag.name}
									</JQueryUICheckbox>
								</span>
							</React.Fragment>
						))}
					</div>
					<br />

					{tagsEditStore.getSuggestions && (
						<div>
							<p>{t('ViewRes:TagSelections.Suggestions')}</p>
							{tagsEditStore.suggestionsLoaded ? (
								tagsEditStore.suggestions.length > 0 ? (
									<div>
										{tagsEditStore.suggestions.map((suggestion, index) => (
											<React.Fragment key={suggestion.tag.id}>
												{index > 0 && ' '}
												<span
													className="tag"
													title={tagsEditStore.getSuggestionText(
														suggestion,
														t('ViewRes:TagSelections.Usages'),
													)}
												>
													<JQueryUIButton
														as={SafeAnchor}
														href="#"
														onClick={(): void =>
															tagsEditStore.autoCompletedTag(suggestion.tag)
														}
														icons={{ primary: 'ui-icon-plus' }}
													>
														{suggestion.tag.name}
													</JQueryUIButton>
												</span>
											</React.Fragment>
										))}
									</div>
								) : (
									<div>{t('ViewRes:TagSelections.NoSuggestions')}</div>
								)
							) : (
								<div>
									<AjaxLoader />
								</div>
							)}
							<br />
						</div>
					)}

					<div className="form-inline">
						{t('ViewRes:TagSelections.AddTag')}:{' '}
						<div className="input-append">
							<TagAutoComplete
								type="text"
								className="input-large"
								maxLength={30}
								onAcceptSelection={tagsEditStore.autoCompletedTag}
								allowAliases={true}
								tagTarget={tagsEditStore.target}
								ref={inputRef}
							/>
							<Button
								className="btn"
								onClick={(): void => {
									tagsEditStore.addTag(inputRef.current.value);
									inputRef.current.value = '';
								}}
							>
								{t('ViewRes:Shared.Add')}
							</Button>
						</div>
					</div>
				</div>
			</JQueryUIDialog>
		);
	},
);

export default TagsEdit;
