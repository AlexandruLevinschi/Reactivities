import React, { useContext, useEffect, useState } from "react";
import { Grid, Loader } from "semantic-ui-react";
import { observer } from "mobx-react-lite";
import ActivityList from "./ActivityList";
import { LoadingComponent } from "../../../app/layout/LoadingComponent";
import { RootStoreContext } from "../../../app/stores/rootStore";
import InfiniteScroll from "react-infinite-scroller";
import ActivityFilters from "./ActivityFilters";

const ActivityDashboard: React.FC = () => {
  const rootStore = useContext(RootStoreContext);
  const { activityStore } = rootStore;
  const [loadingNext, setLoadingNext] = useState(false);

  const handleGetNext = () => {
    setLoadingNext(true);
    activityStore.setPage(activityStore.page + 1);
    activityStore.loadActivities().then(() => setLoadingNext(false));
  };

  useEffect(() => {
    activityStore.loadActivities();
  }, [activityStore]);

  if (activityStore.loadingInitial && activityStore.page === 0)
    return <LoadingComponent content="Loading activities..." />;

  return (
    <Grid>
      <Grid.Column width={10}>
        <InfiniteScroll
          pageStart={0}
          loadMore={handleGetNext}
          hasMore={
            !loadingNext && activityStore.page + 1 < activityStore.totalPages
          }
          initialLoad={false}
        >
          <ActivityList />
        </InfiniteScroll>
      </Grid.Column>
      <Grid.Column width={6}>
        <ActivityFilters />
      </Grid.Column>
      <Grid.Column width={10}>
        <Loader active={loadingNext} />
      </Grid.Column>
    </Grid>
  );
};

export default observer(ActivityDashboard);
