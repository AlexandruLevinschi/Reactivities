import React, { useEffect, Fragment, useContext } from "react";
import { Container } from "semantic-ui-react";
import ActivityDashboard from "../../features/activities/dashboard/ActivityDashboard";
import { LoadingComponent } from "./LoadingComponent";
import ActivityStore from "../stores/activityStore";
import { observer } from "mobx-react-lite";
import NavBar from "../../features/nav/NavBar";

const App = () => {
  const activityStore = useContext(ActivityStore);

  useEffect(() => {
    activityStore.loadActivities();
  }, [activityStore]);

  if (activityStore.loadingInitial)
    return <LoadingComponent content="Loading activities..." />;

  return (
    <Fragment>
      <NavBar />
      <Container className="main-container">
        <ActivityDashboard />
      </Container>
    </Fragment>
  );
};

export default observer(App);