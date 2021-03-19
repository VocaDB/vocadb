import classNames from 'classnames';
import React, { ReactElement } from 'react';
import { useTranslation } from 'react-i18next';
import SafeAnchor from '../../Bootstrap/SafeAnchor';

interface ServerSidePagingProps {
	onPageChange: (page: number) => void;
	page: number;
	pageSize: number;
	totalItems: number;
}

const ServerSidePaging = ({
	onPageChange,
	page: currentPage,
	pageSize,
	totalItems,
}: ServerSidePagingProps): ReactElement => {
	const { t } = useTranslation(['VocaDb.Web.Resources.Other']);

	const totalPages = (): number => Math.ceil(totalItems / pageSize);

	const isFirstPage = (): boolean => currentPage <= 1;

	const isLastPage = (): boolean => currentPage >= totalPages();

	const pages = (): number[] => {
		var start = Math.max(currentPage - 4, 1);
		var end = Math.min(currentPage + 4, totalPages());

		return _.range(start, end + 1);
	};

	const showMoreBegin = (): boolean => currentPage > 5;

	const showMoreEnd = (): boolean => currentPage < totalPages() - 4;

	const goToFirstPage = (): void => onPageChange?.(1);

	const goToLastPage = (): void => onPageChange?.(totalPages());

	const nextPage = (): void => {
		if (!isLastPage()) {
			onPageChange?.(currentPage + 1)
		}
	}

	const previousPage = (): void => {
		if (!isFirstPage()) {
			onPageChange?.(currentPage - 1);
		}
	};

	return (
		<div className="pagination" /* TODO */>
			<ul>
				<li className={classNames(isFirstPage() && 'disabled')}>
					<SafeAnchor onClick={goToFirstPage}>&laquo;&laquo; {t('VocaDb.Web.Resources.Other:PagedList.First')}</SafeAnchor>
				</li>
				<li className={classNames(isFirstPage() && 'disabled')}>
					<SafeAnchor onClick={previousPage}>&laquo; {t('VocaDb.Web.Resources.Other:PagedList.Previous')}</SafeAnchor>
				</li>

				{showMoreBegin() && (
					<li className="disabled">
						<SafeAnchor>&hellip;</SafeAnchor>
					</li>
				)}

				{pages().map(page => (
					<li className={classNames(page == currentPage && 'active')} key={page}>
						<SafeAnchor onClick={() => onPageChange?.(page)}>{page}</SafeAnchor>
					</li>
				))}

				{showMoreEnd() && (
					<li className="disabled">
						<SafeAnchor>&hellip;</SafeAnchor>
					</li>
				)}

				<li className={classNames(isLastPage() && 'disabled')}>
					<SafeAnchor onClick={nextPage}>{t('VocaDb.Web.Resources.Other:PagedList.Next')} &raquo;</SafeAnchor>
				</li>
				<li className={classNames(isLastPage() && 'disabled')}>
					<SafeAnchor onClick={goToLastPage}>{t('VocaDb.Web.Resources.Other:PagedList.Last')} &raquo;&raquo;</SafeAnchor>
				</li>
			</ul>
		</div>
	);
};

export default ServerSidePaging;
