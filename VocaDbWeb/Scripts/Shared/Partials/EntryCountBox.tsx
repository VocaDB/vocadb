import React, { ReactElement } from 'react';
import { useTranslation } from 'react-i18next';
import Dropdown from '../../Bootstrap/Dropdown';
import SafeAnchor from '../../Bootstrap/SafeAnchor';

interface EntryCountBoxProps {
	onPageSizeChange: (pageSize: number) => void;
	pageSize: number;
	totalItems: number;
}

const EntryCountBox = ({
	onPageSizeChange,
	pageSize,
	totalItems,
}: EntryCountBoxProps): ReactElement => {
	const { t } = useTranslation(['ViewRes.Search']);

	const selections = [10, 20, 40, 100];

	return (
		// TODO
		<Dropdown className="pull-right">
			<Dropdown.Toggle href="#">
				{t('ViewRes.Search:Index.ShowingItemsOf', { 0: pageSize, 1: totalItems })}
			</Dropdown.Toggle>
			<Dropdown.Menu>
				{selections.map(selection => (
					<li key={selection}><SafeAnchor onClick={() => onPageSizeChange?.(selection)}>{t('ViewRes.Search:Index.ItemsPerPage', { 0: selection })}</SafeAnchor></li>
				))}
			</Dropdown.Menu>
		</Dropdown>
	);
};

export default EntryCountBox;
