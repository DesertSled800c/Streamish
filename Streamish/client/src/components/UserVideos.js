import React, { useEffect, useState } from "react";
import { getVidsByUserId } from "../modules/videoManager";
import { useParams } from "react-router-dom";
import Video from "./Video";

const UserVideos = () => {
  const [userVideos, setUserVideos] = useState();
  const { id } = useParams();

  useEffect(() => {
    getVidsByUserId(id).then(setUserVideos);
  }, []);

  return (
    <div className="container">
      <div className="row justify-content-center">
        <div className="col-sm-12 col-lg-6">
          <Video video={userVideos} />
        </div>
      </div>
    </div>
  );
};

export default UserVideos;