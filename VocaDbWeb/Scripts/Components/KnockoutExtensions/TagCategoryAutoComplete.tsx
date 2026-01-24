import JQueryUIAutocomplete from '@/JQueryUI/JQueryUIAutocomplete';
import { functions } from '@/Shared/GlobalFunctions';
import { httpClient } from '@/Shared/HttpClient';
import useMergedRefs from '@restart/hooks/useMergedRefs';
import $ from 'jquery';
import React from 'react';

interface TagCategoryAutoCompleteProps
	extends React.InputHTMLAttributes<HTMLInputElement> {
	onAcceptSelection: (categoryName: string) => void;
	clearValue: boolean;
}

export const TagCategoryAutoComplete = React.forwardRef<
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

		const source = React.useCallback(
			(ui: { term: any }, callback: (result: string[]) => void): void => {
				httpClient.get<string[]>(url, { query: ui.term }).then(callback);
			},
			[url],
		);

		const select = React.useCallback(
			(event: Event, ui: JQueryUI.AutocompleteUIParams): boolean => {
				onAcceptSelection(ui.item.label);

				if (clearValue) {
					if (inputRef.current) inputRef.current.value = '';
					return false;
				} else {
					return true;
				}
			},
			[],
		);

		return (
			<JQueryUIAutocomplete
				source={source}
				select={select}
				{...props}
				minLength={0}
				ref={mergedRef}
				onFocus={(e): void => {
					$(e.target).autocomplete('search', e.target.value);
					props.onFocus?.(e);
				}}
			/>
		);
	},
);
