import React from "react";
import { Card, CardBody } from "reactstrap";
import { Link } from "react-router-dom";

// ...

const Video = ({ video }) => {
  return (
    <Card>
      <p className="text-left px-2">
        Posted by:
        <Link to={`/users/${video.userProfileId}`}>
          {video.userProfile.name}
        </Link>
      </p>
      <CardBody>
        <iframe
          className="video"
          src={video.url}
          title="YouTube video player"
          frameBorder="0"
          allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
          allowFullScreen
        />

        <p>{video.description}</p>
        <Link to={`/videos/${video.id}`}>
          <strong>{video.title}</strong>
        </Link>
        <ul>
          {video.comments
            ? video.comments.map((c) => <li key={c.id}>{c.message}</li>)
            : null}
        </ul>
      </CardBody>
    </Card>
  );
};

export default Video;
