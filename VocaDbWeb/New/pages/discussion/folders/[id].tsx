import { useRouter } from 'next/router';

export default function FolderPage() {
	const router = useRouter();
	return <p>Folder: {router.query.id}</p>;
}

