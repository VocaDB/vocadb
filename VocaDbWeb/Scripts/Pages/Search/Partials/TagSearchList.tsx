import React, { ReactElement } from 'react';
import { useTranslation } from 'react-i18next';
import { EntryUrlMapper } from '../../../../wwwroot/Scripts/App';
import PartialFindResultContract from '../../../../wwwroot/Scripts/DataContracts/PartialFindResultContract';
import TagApiContract from '../../../../wwwroot/Scripts/DataContracts/Tag/TagApiContract';
import SafeAnchor from '../../../Bootstrap/SafeAnchor';
import DraftIcon from '../../../Shared/Partials/DraftIcon';
import EntryCountBox from '../../../Shared/Partials/EntryCountBox';
import ServerSidePaging from '../../../Shared/Partials/ServerSidePaging';

interface TagSearchListProps {
	onPageChange: (page: number) => void;
	onPageSizeChange: (pageSize: number) => void;
	page: number;
	pageSize: number;
	result: PartialFindResultContract<TagApiContract>;
}

const TagSearchList = ({
	onPageChange,
	onPageSizeChange,
	page,
	pageSize,
	result,
}: TagSearchListProps): ReactElement => {
	const { t } = useTranslation(['ViewRes', 'ViewRes.Search']);

	return (
		<div>
			<EntryCountBox onPageSizeChange={onPageSizeChange} pageSize={pageSize} totalItems={result.totalCount} />

			<div /* TODO */>
				<ServerSidePaging onPageChange={onPageChange} page={page} pageSize={pageSize} totalItems={result.totalCount} />
			</div>

			<table className="table table-striped" /* TODO */>
				<thead>
					<tr>
						<th colSpan={2}>
							<a /* TODO */ href="#">
								{t('ViewRes:Shared.Name')}
								{' '}
								{/* TODO */}
							</a>
						</th>
						<th>
							{t('ViewRes.Search:Index.Category')}
						</th>
						<th>
							<a /* TODO */ href="#">
								{t('ViewRes.Search:Index.TagUsages')}
								{' '}
								{/* TODO */}
							</a>
						</th>
					</tr>
				</thead>
				<tbody>
					{result.items.map(item => (
						<tr key={item.id}>
							<td style={{ width: '80px' }}>
								{item.mainPicture?.urlSmallThumb && (
									<a href={EntryUrlMapper.details_tag(item.id, item.urlSlug)} title={item.additionalNames}>
										<img src={item.mainPicture.urlSmallThumb} title="Cover picture" className="coverPicThumb img-rounded" />
									</a>
								)}
							</td>
							<td>
								<a href={EntryUrlMapper.details_tag(item.id, item.urlSlug)}>{item.name}</a>
								{' '}
								<DraftIcon status={item.status} /* TODO */ />
								<br />
								<small className="extraInfo">{item.additionalNames}</small>
							</td>
							<td>
								<p>{item.categoryName}</p>
							</td>
							<td>
								<p>{item.usageCount}</p>
							</td>
						</tr>
					))}
				</tbody>
			</table>

			<div /* TODO */>
				<ServerSidePaging onPageChange={onPageChange} page={page} pageSize={pageSize} totalItems={result.totalCount} />
			</div>
		</div>
	);
};

export default TagSearchList;
