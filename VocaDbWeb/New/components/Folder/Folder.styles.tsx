import { createStyles } from '@mantine/core';

export default createStyles((theme) => ({
    folderGrid: {
        display: 'flex',
        flexDirection: 'row',
        flexWrap: 'wrap',
        justifyContent: 'space-around',
        overflow: 'hidden',
        backgroundColor: theme.colorScheme === 'dark' ? '#141517' : '#f8f9fa',
        padding: '10px',
    },
    folderGridList: {
        width: '100%',
        height: '100%',
        border: '1px solid #ccc',
        borderRadius: '8px',
        margin: '10px',
        padding: '10px',
        backgroundColor: theme.colorScheme === 'dark' ? '#25262b' : '#fff',
        '&:hover': {
            backgroundColor: theme.colorScheme === 'dark' ? '#333' : '#eee',
        },
        cursor: 'pointer',
    },
    folderName: {
        width: '100%',
        height: '40px',
        overflow: 'hidden',
        textOverflow: 'ellipsis',
        whiteSpace: 'nowrap',
        textDecoration: 'underline',
    },
    folderDescription: {
        width: '100%',
        textOverflow: 'initial',
        whiteSpace: 'normal',
        color: theme.colorScheme === 'dark' ? '#aaa' : '#666',
        fontWeight: 'normal',
    },
    folderTopics: {
        width: '100%',
        height: '20px',
        overflow: 'hidden',
        textOverflow: 'ellipsis',
        whiteSpace: 'nowrap',
        color: theme.colorScheme === 'dark' ? '#aaa' : '#666',
        fontWeight: 'normal',
    },
    folderLastTopicDate: {
        width: '100%',
        height: '20px',
        overflow: 'hidden',
        textOverflow: 'ellipsis',
        whiteSpace: 'nowrap',
        color: theme.colorScheme === 'dark' ? '#aaa' : '#666',
        fontWeight: 'normal',
    },
}));

