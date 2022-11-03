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
					href={`http://www.geoiptool.com/?IP=${ip}`}
					target="_blank"
					rel="noreferrer"
				>
					View on GeoIPTool{/* LOCALIZE */}
				</a>
				) (
				<a
					href={`http://www.stopforumspam.com/search?q=${ip}`}
					target="_blank"
					rel="noreferrer"
				>
					View on StopForumSpam{/* LOCALIZE */}
				</a>
				) (
				<a
					href={functions.mapAbsoluteUrl(
						`/Admin/ViewAuditLog?${qs.stringify({ filter: ip })}`,
					)}
					target="_blank"
					rel="noreferrer"
				>
					View in audit log{/* LOCALIZE */}
				</a>
				)
			</>
		);
	},
);
