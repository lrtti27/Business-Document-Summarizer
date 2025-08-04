import React, {use, useState} from 'react'
import axios from 'axios'
import {Line} from 'react-chartjs-2'
import {
    Chart as ChartJS,
    CategoryScale,
    LinearScale,
    PointElement,
    LineElement,
    Legend,
    Title,
    Tooltip,
} from "chart.js";

ChartJS.register(
    CategoryScale,
    LinearScale,
    PointElement,
    LineElement,
    Legend,
    Title,
    Tooltip
);

function DocumentSummarizer() {
    const [text , setText] = useState('');
    const [summary , setSummary] = useState('');
    const [loading , setLoading] = useState(false);
    const [file , setFile] = useState(null);
    const [fileLoading , setFileLoading] = useState(false);
    const [chartData , setChartData] = useState(null);
    
    const handleSummarize = async () => {
        setLoading(true);
        try {
            const response = await axios.post('http://localhost:5003/api/businesssearch/uploadText' , {
                text: text,
            });
            setSummary(response.data.summary);
        } catch (error) {
            console.error('Error summarizing:' , error);
            setSummary('Error summarizing document.');
        }
        setLoading(false);
    };
    
    const handleFileChange = (e) => {
        if(e.target.files) {
            setFile(e.target.files[0]);
        }
    };
    
    const handleFileUpload = async () => {
        if(!file)   return;
        setFileLoading(true);
        
        const formData = new FormData();
        formData.append('file' , file);
        formData.append('description' , 'Test Description');
        
        try {
            const response = await axios.post(
                'http://localhost:5003/api/businesssearch/uploadFile',
                formData,
                {
                    headers: {
                        'Content-Type': 'multipart/form-data',
                    },
                }
            );
            console.log("Original chart data form OpenAI : ");
            console.log(response.data.chartData);
            const chartDataJSON = JSON.parse(response.data.chartData);

            console.log("JSON chart data form OpenAI : ");
            console.log(chartDataJSON);
            setChartData(chartDataJSON);
        } catch (error) {
            console.error('Error uploading file:', error);
            setSummary('Error uploading file.');
            setChartData(null);
        }

        setFileLoading(false);
    }

    return (
        <div className="p-4 max-w-4xl mx-auto">
            {/* Title on top */}
            <h1 className="text-2xl font-bold mb-6">Document Summarizer</h1>

            {/* Flex container for left and right sides */}
            <div className="flex flex-row space-x-6">
               
                {/* Left side: textarea + summarize text button */}
                <div className="flex flex-col flex-1">
                    <h1 className="text-2xl font-bold mb-6">Text Upload</h1>
        <textarea
            className="w-full border p-2 mb-4 flex-grow resize-none"
            rows={10}
            placeholder="Paste your document here..."
            value={text}
            onChange={(e) => setText(e.target.value)}
        />
                    <button
                        className="bg-blue-500 text-white px-4 py-2 rounded self-center"
                        onClick={handleSummarize}
                        disabled={loading}
                    >
                        {loading ? 'Summarizing...' : 'Summarize Text'}
                    </button>
                </div>

                {/* Right side: file input + summarize file button */}
                <div className="flex flex-col flex-1">
                    <h1 className="text-2xl font-bold mb-6">File Upload</h1>
                    <input
                        type="file"
                        //Accept excel files
                        accept=".xls,.xlsx,application/vnd.ms-excel,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                        onChange={handleFileChange}
                        className="mb-4"
                    />
                    <p className="text-sm text-gray-600 mb-4">
                        Accepted File Types : .xls , .xlsx
                    </p>
                    <button
                        className="bg-blue-500 text-white px-4 py-2 rounded self-center"
                        onClick={handleFileUpload}
                        disabled={!file}
                    >
                        {fileLoading ? 'Summarizing...' : 'Summarize File'}
                    </button>
                </div>
            </div>
            {/* Summary Output */}
            {chartData && (
                <div className="mt-8">
                    <h2 className="text-xl font-semibold mb-2">Financial Chart</h2>
                    <div className="bg-white rounded p-4 shadow">
                        <Line data={chartData} />
                    </div>
                </div>
            )}
        </div>
    );


}

export default DocumentSummarizer;