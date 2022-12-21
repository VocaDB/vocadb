import { VideoServiceHelper } from '@/Helpers/VideoServiceHelper';
import { EntryType } from '@/Models/EntryType';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import { PlayQueueEntryContract } from '@/Stores/VdbPlayer/PlayQueueRepository';
import { Link } from 'react-router-dom';

interface VdbPlayerEntryLinkProps
	extends Omit<React.AnchorHTMLAttributes<HTMLAnchorElement>, 'href'> {
	entry: PlayQueueEntryContract;
	children?: React.ReactNode;
}

export const VdbPlayerEntryLink = ({
	entry,
	children,
	...props
}: VdbPlayerEntryLinkProps): React.ReactElement => {
	return entry.entryType === EntryType.PV ? (
		<a
			{...props}
			href={VideoServiceHelper.getUrlById(entry.pvs[0])}
			target="_blank"
			rel="noreferrer"
		>
			{children}
		</a>
	) : (
		<Link {...props} to={EntryUrlMapper.details_entry(entry)}>
			{children}
		</Link>
	);
};
