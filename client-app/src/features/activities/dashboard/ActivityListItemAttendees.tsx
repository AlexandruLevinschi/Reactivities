import React from "react";
import { List, Image, Popup } from "semantic-ui-react";
import { IAttendee } from "../../../app/models/activity";
import { Link } from "react-router-dom";

interface IProps {
  attendees: IAttendee[];
}

const styles = {
  borderColor: "orange",
  borderWidth: 2
}

const ActivityListItemAttendees: React.FC<IProps> = ({ attendees }) => {
  return (
    <List horizontal>
      {attendees.map((attendee) => (
        <List.Item key={attendee.username}>
          <Popup
            size="mini"
            header={attendee.displayName}
            trigger={
              <Image as={Link} to={`/profile/${attendee.username}`}
                size="mini"
                circular
                src={attendee.image || "/assets/user.png"}
                bordered
                style={attendee.following ? styles : null}
              />
            }
          />
        </List.Item>
      ))}
    </List>
  );
};

export default ActivityListItemAttendees;
