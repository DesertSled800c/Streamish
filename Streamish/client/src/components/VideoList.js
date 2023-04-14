import React, { useEffect, useState } from "react";
import Video from './Video';
import { getAllVideosWithComments, searchVideos } from "../modules/videoManager";

const baseUrl = '/api/video';

const VideoList = () => {
    const [videos, setVideos] = useState([]);
    const [searchTerms, setSearchTerms] = useState('');
    const [sort, setSort] = useState(false);
    const [send, triggerSend] = useState(false)

    const getAllVidsWithComments = () => {
        getAllVideosWithComments().then(videos => setVideos(videos));
    };

    const searchVids = () => {
        searchVideos(searchTerms, sort).then(videos => setVideos(videos));
    }

    useEffect(() => {
        getAllVidsWithComments();
    }, []);

    useEffect(() => {
        searchVids();
    }, [send]);

    useEffect(() => {
        triggerSend(false);
    }, [videos]);



    return (
        <div className="container">
            <input
                type="text"
                className="userInput"
                placeholder="Search Videos"
                onChange={
                    (event) => {
                        setSearchTerms(event.target.value)
                    }
                } />
            <button
                className="submit"
                onClick={() => triggerSend(true)}
            >
                Search
            </button>
            <label>Sort by Descending?</label>
            <input
                type="radio"
                className="sortButton"
                onChange={
                    () => {
                        setSort(true)
                    }
                } />
            <div className="row justify-content-center">
                {videos.map((video) => (
                    <Video video={video} key={video.id} />
                ))}
            </div>
        </div>
    );
};

export default VideoList;