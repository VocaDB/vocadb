import Alert from '@Bootstrap/Alert';
import SafeAnchor from '@Bootstrap/SafeAnchor';
import Layout from '@Components/Shared/Layout';
import useVocaDbTitle from '@Components/useVocaDbTitle';
import TagCategoryContract from '@DataContracts/Tag/TagCategoryContract';
import JQueryUIButton from '@JQueryUI/JQueryUIButton';
import JQueryUIDialog from '@JQueryUI/JQueryUIDialog';
import LoginManager from '@Models/LoginManager';
import TagRepository from '@Repositories/TagRepository';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import HttpClient from '@Shared/HttpClient';
import TagCreateStore from '@Stores/Tag/TagCreateStore';
import _ from 'lodash';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { DebounceInput } from 'react-debounce-input';
import { useTranslation } from 'react-i18next';

const loginManager = new LoginManager(vdb.values);

const httpClient = new HttpClient();

const tagRepo = new TagRepository(httpClient, vdb.values.baseAddress);

const tagCreateStore = new TagCreateStore(tagRepo);

const getFontSizePercent = (usageCount: number, avgUsage: number): number =>
	Math.min(Math.max((usageCount * 80) / avgUsage, 60), 100);

interface TagIndexLayoutProps {
	model: TagCategoryContract[];
}

const TagIndexLayout = observer(
	({ model }: TagIndexLayoutProps): React.ReactElement => {
		const { t, ready } = useTranslation(['ViewRes']);

		const title = t('ViewRes:Shared.Tags');

		useVocaDbTitle(title, ready);

		const tagCount = _.sumBy(model, (m) => m.tags.length);
		const avgUsageCount =
			_.sumBy(model, (m) => _.sumBy(m.tags, (t) => t.usageCount)) / tagCount;

		return (
			<Layout
				title={title}
				toolbar={
					<>
						{loginManager.canManageDatabase && (
							<JQueryUIButton
								as={SafeAnchor}
								href="#"
								onClick={(): void =>
									runInAction(() => {
										tagCreateStore.dialogVisible = true;
									})
								}
							>
								Create new{/* TODO: localize */}
							</JQueryUIButton>
						)}
					</>
				}
			>
				{model.map((category) => (
					<div className="tag-cloud" key={category.name}>
						<h3>{category.name || t('ViewRes:Shared.Unsorted')}</h3>

						{category.tags.map((tag, index) => (
							<React.Fragment key={tag.id}>
								{index > 0 && ' '}
								<span
									className="tag"
									style={{
										fontSize: `${getFontSizePercent(
											tag.usageCount,
											avgUsageCount,
										)}%`,
									}}
									key={tag.id}
								>
									<a
										href={EntryUrlMapper.details_tag_contract(tag)}
										title={tag.additionalNames}
										style={{ fontSize: '' }}
									>
										{tag.name}
									</a>
								</span>
							</React.Fragment>
						))}
					</div>
				))}

				<JQueryUIDialog
					title="Create tag" /* TODO: localize */
					autoOpen={tagCreateStore.dialogVisible}
					close={(): void =>
						runInAction(() => {
							tagCreateStore.dialogVisible = false;
						})
					}
					width={310}
					modal={true}
					buttons={[
						{
							text: 'Create' /* TODO: localize */,
							click: (): void => {
								tagCreateStore.createTag().then((t) => {
									// TODO: use navigate
									window.location.href = EntryUrlMapper.details_tag_contract(
										t,
									)!;
								});
							},
							disabled: !tagCreateStore.isValid,
						},
						{
							text: t('ViewRes:Shared.Cancel'),
							click: (): void =>
								runInAction(() => {
									tagCreateStore.dialogVisible = false;
								}),
						},
					]}
				>
					<DebounceInput
						type="text"
						value={tagCreateStore.newTagName}
						onChange={(e): void =>
							runInAction(() => {
								tagCreateStore.newTagName = e.target.value;
							})
						}
						maxLength={200}
						debounceTimeout={100}
					/>

					{tagCreateStore.duplicateName && (
						<Alert variant="danger">
							Tag name must be unique{/* TODO: localize */}
						</Alert>
					)}
				</JQueryUIDialog>
			</Layout>
		);
	},
);

const TagIndex = (): React.ReactElement => {
	const [model, setModel] = React.useState<TagCategoryContract[] | undefined>();

	React.useEffect(() => {
		tagRepo.getTagsByCategories().then((model) => setModel(model));
	}, []);

	return model ? <TagIndexLayout model={model} /> : <></>;
};

export default TagIndex;
