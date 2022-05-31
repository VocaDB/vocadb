import JQueryUIAutocomplete from '@JQueryUI/JQueryUIAutocomplete';
import functions from '@Shared/GlobalFunctions';
import HttpClient from '@Shared/HttpClient';
import useMergedRefs from '@restart/hooks/useMergedRefs';
import React from 'react';

const httpClient = new HttpClient();

interface TagCategoryAutoCompleteProps
	extends React.InputHTMLAttributes<HTMLInputElement> {
	onAcceptSelection: (categoryName: string) => void;
	clearValue: boolean;
}

const TagCategoryAutoComplete = React.forwardRef<
	HTMLInputElement,
	TagCategoryAutoCompleteProps
>(
	(
		{ onAcceptSelection, clearValue, ...props }: TagCategoryAutoCompleteProps,
		ref,
	): React.ReactElement => {
		const url = functions.mapAbsoluteUrl('/api/tags/categoryNames');
		const inputRef = React.useRef<HTMLInputElement>(undefined!);
		const mergedRef = useMergedRefs<HTMLInputElement | null>(inputRef, ref);

		return (
			<JQueryUIAutocomplete
				source={(
					ui: { term: any },
					callback: (result: string[]) => void,
				): void => {
					httpClient
						.get<string[]>(url, { query: ui.term })
						.then(callback);
				}}
				select={(event: Event, ui): boolean => {
					onAcceptSelection(ui.item.label);

					if (clearValue) {
						if (inputRef.current) inputRef.current.value = '';
						return false;
					} else {
						return true;
					}
				}}
				{...props}
				ref={mergedRef}
			/>
		);
	},
);

export default TagCategoryAutoComplete;
