import Button from '@Bootstrap/Button';
import JQueryUIDialog from '@JQueryUI/JQueryUIDialog';
import CustomNameEditStore from '@Stores/CustomNameEditStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface CustomNameEditProps {
	customNameEditStore: CustomNameEditStore;
}

const CustomNameEdit = observer(
	({ customNameEditStore }: CustomNameEditProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes']);

		return (
			<JQueryUIDialog
				title="Customize artist name" /* TODO: localize */
				autoOpen={customNameEditStore.dialogVisible}
				width={550}
				close={(): void =>
					runInAction(() => {
						customNameEditStore.dialogVisible = false;
					})
				}
				buttons={[
					{
						text: t('ViewRes:Shared.Save'),
						click: customNameEditStore.save,
					},
				]}
			>
				<div>
					{customNameEditStore.artistLink &&
						customNameEditStore.artistLink.artist && (
							<>
								<label>Default name{/* TODO: localize */}</label>
								<span>{customNameEditStore.artistLink.artist.name}</span>
								<br />
								<br />
							</>
						)}
				</div>

				<div className="input-append">
					<label>Custom name{/* TODO: localize */}</label>
					<input
						type="text"
						maxLength={255}
						value={customNameEditStore.name}
						onChange={(e): void =>
							runInAction(() => {
								customNameEditStore.name = e.target.value;
							})
						}
					/>
					<Button
						onClick={(): void =>
							runInAction(() => {
								customNameEditStore.name = '';
							})
						}
					>
						Reset{/* TODO: localize */}
					</Button>
				</div>
			</JQueryUIDialog>
		);
	},
);

export default CustomNameEdit;
