import React, { useContext } from "react";
import { Segment, Item, Image, Header, Button, Dropdown } from "semantic-ui-react";
import { IActivity } from "../../../app/models/activity";
import { observer } from "mobx-react-lite";
import { Link } from "react-router-dom";
import { format } from "date-fns";
import { RootStoreContext } from "../../../app/stores/rootStore";

const activityImageStyle = {
  filter: "brightness(30%)",
};

const activityImageTextStyle = {
  position: "absolute",
  bottom: "5%",
  left: "5%",
  width: "100%",
  height: "auto",
  color: "white",
};

const ActivityDetailedHeader: React.FC<{ activity: IActivity }> = ({ activity }) => {
  const rootStore = useContext(RootStoreContext);
  const { attendActivity, cancelAttendance, deleteActivity, submitting, loading } = rootStore.activityStore;
  const { user } = rootStore.userStore;
  const host = activity.attendees.filter((a) => a.isHost)[0];

  return (
    <Segment.Group>
      <Segment basic attached="top" style={{ padding: "0" }}>
        <Image src={`/assets/categoryImages/${activity.category}.jpg`} fluid style={activityImageStyle} />
        <Segment basic style={activityImageTextStyle}>
          <Item.Group>
            <Item>
              <Item.Content>
                <Header size="huge" content={activity.name} style={{ color: "white" }} />
                <p>{format(activity.date, "eeee do MMMM")}</p>
                <p>
                  Hosted by{" "}
                  <Link to={`/profile/${host.username}`}>
                    <strong>{host.displayName}</strong>
                  </Link>
                </p>
              </Item.Content>
            </Item>
          </Item.Group>
        </Segment>
      </Segment>
      <Segment clearing attached="bottom">
        {user?.isAdmin ? (
          <Dropdown pointing="top left" text="Actions">
            <Dropdown.Menu>
              <Dropdown.Item as={Link} to={`/activities/edit/${activity.id}`} text="Manage event" />
              <Dropdown.Item onClick={() => deleteActivity(activity.id)} text="Delete event" />
              {activity.isGoing ? (
                <Dropdown.Item onClick={cancelAttendance} text="Cancel attendance" />
              ) : (
                <Dropdown.Item onClick={attendActivity} text="Join Activity" />
              )}
            </Dropdown.Menu>
          </Dropdown>
        ) : activity.isHost ? (
          <>
            <Button as={Link} to={`/activities/edit/${activity.id}`} color="orange" floated="right">
              Manage Event
            </Button>
            <Button loading={submitting} onClick={() => deleteActivity(activity.id)} color="red" floated="right">
              Delete Event
            </Button>
          </>
        ) : activity.isGoing ? (
          <Button loading={loading} onClick={cancelAttendance}>
            Cancel attendance
          </Button>
        ) : (
          <Button loading={loading} onClick={attendActivity} color="teal">
            Join Activity
          </Button>
        )}
      </Segment>
    </Segment.Group>
  );
};

export default observer(ActivityDetailedHeader);
