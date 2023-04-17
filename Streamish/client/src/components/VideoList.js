import React, { useEffect, useState } from "react";
import Video from "./Video";
import {
  getAllVideosWithComments,
  searchVideos,
} from "../modules/videoManager";
import VideoForm from "./VideoForm";

const VideoList = () => {
  const [videos, setVideos] = useState([]);
  const [searchTerms, setSearchTerms] = useState("");
  const [sort, setSort] = useState(false);

  const getAllVidsWithComments = () => {
    getAllVideosWithComments().then((videos) => setVideos(videos));
  };

  useEffect(() => {
    getAllVidsWithComments();
  }, []);

  useEffect(() => {
    setSearchTerms("");
    setSort(false);
  }, [videos]);

  return (
    <div className="container">
      {/* <VideoForm getAllVidsWithComments={getAllVidsWithComments} /> */}
      <input
        type="text"
        className="userInput"
        placeholder="Search Videos"
        value={searchTerms}
        onChange={(event) => {
          setSearchTerms(event.target.value);
        }}
      />
      <button
        className="submit"
        onClick={() =>
          searchVideos(searchTerms, sort).then((videos) => setVideos(videos))
        }
      >
        Search
      </button>
      <label>Sort by Descending?</label>
      <input
        type="checkbox"
        className="sortButton"
        checked={sort}
        onChange={(event) => {
          setSort(event.target.checked);
        }}
      />
      <div className="row justify-content-center">
        {videos.map((video) => (
          <Video video={video} key={video.id} />
        ))}
      </div>
    </div>
  );
};

export default VideoList;
