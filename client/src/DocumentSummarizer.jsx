import React, {use, useState} from 'react'
import axios from 'axios'

function DocumentSummarizer() {
    const [text , setText] = useState('');
    const [summary , setSummary] = useState('');
    const [loading , setLoading] = useState(false);
    
    const handleSummarize = async () => {
        setLoading(true);
        try {
            const response = await axios.post('http://localhost:5003/api/businesssearch/summarize' , {
                text: text,
            });
            setSummary(response.data.summary);
        } catch (error) {
            console.error('Error summarizing:' , error);
            setSummary('Error summarizing document.');
        }
        setLoading(false);
    };
    
    return (
        <div className="p-4">
            <h1 className="text-2xl font-bold mb-2">Document Summarizer</h1>
            <textarea
                className="w-full border p-2 mb-2"
                rows={8}
                placeholder="Paste your document here..."
                value={text}
                onChange={(e) => setText(e.target.value)}
            />
            <button
                className="bg-blue-500 text-white px-4 py-2 rounded"
                onClick={handleSummarize}
                disabled={loading}
            >
                {loading ? 'Summarizing...' : 'Summarize'}   
            </button>
            {summary && (
                <div className="mt-4">
                    <h2 className="text-xl font-semibold"></h2>
                    <p>{summary}</p>
                </div>
            )}
        </div>
    );
}

export default DocumentSummarizer;