import { functions } from '@/Shared/GlobalFunctions';
import qs from 'qs';
import React from 'react';

interface IPManageProps {
	ip: string;
}

export const IPManage = React.memo(
	({ ip }: IPManageProps): React.ReactElement => {
		return (
			<>
				<span>{ip}</span> (
				<a
					href={`https://www.geodatatool.com/?ip=${ip}`}
					target="_blank"
					rel="noreferrer"
				>
					View on GeoIPTool{/* LOC */}
				</a>
				) (
				<a
					href={`http://www.stopforumspam.com/search?q=${ip}`}
					target="_blank"
					rel="noreferrer"
				>
					View on StopForumSpam{/* LOC */}
				</a>
				) (
				<a
					href={functions.mapAbsoluteUrl(
						`/Admin/ViewAuditLog?${qs.stringify({ filter: ip })}`,
					)}
					target="_blank"
					rel="noreferrer"
				>
					View in audit log{/* LOC */}
				</a>
				)
			</>
		);
	},
);
