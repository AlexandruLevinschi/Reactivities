import { observable, action, computed, runInAction } from "mobx";
import { SyntheticEvent } from "react";
import { IActivity } from "../models/activity";
import agent from "../api/agent";
import { history } from "../..";
import { toast } from "react-toastify";
import { RootStore } from "./rootStore";
import { setActivityProps, createAttendee } from "../common/util/util";
import {
  HubConnection,
  HubConnectionBuilder,
  LogLevel,
} from "@microsoft/signalr";

export default class ActivityStore {
  rootStore: RootStore;

  constructor(rootStore: RootStore) {
    this.rootStore = rootStore;
  }

  @observable activityRegistry = new Map();
  @observable selectedActivity: IActivity | null = null;
  @observable loadingInitial = false;
  @observable submitting = false;
  @observable target = "";
  @observable loading = false;
  @observable.ref hubConnection: HubConnection | null = null;

  @action createHubConnection = (activityId: string) => {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl("http://localhost:5000/chat", {
        accessTokenFactory: () => this.rootStore.commonStore.token!,
      })
      .configureLogging(LogLevel.None)
      .build();

    this.hubConnection
      .start()
      .then(() => {
        this.hubConnection!.invoke("AddToGroup", activityId).catch((e) =>
          console.log(e)
        );
      })
      .catch((error) => console.log(error));

    this.hubConnection.on("ReceiveComment", (comment) => {
      runInAction(() => {
        this.selectedActivity!.comments.push(comment);
      });
    });

    this.hubConnection.on("Send", (message) => {
      toast.info(message);
    });
  };

  @action stopHubConnection = () => {
    this.hubConnection!.invoke("RemoveFromGroup", this.selectedActivity!.id)
      .then(() => {
        this.hubConnection!.stop();
      })
      .catch((error) => console.log(error));
  };

  @action addComment = async (values: any) => {
    values.activityId = this.selectedActivity!.id;

    try {
      await this.hubConnection!.invoke("SendComment", values);
    } catch (error) {}
  };

  @computed get activitiesByDate() {
    return this.groupActivitiesByDate(
      Array.from(this.activityRegistry.values())
    );
  }

  groupActivitiesByDate(activities: IActivity[]) {
    const sortedActivities = activities.sort(
      (a, b) => a.date.getTime() - b.date.getTime()
    );

    return Object.entries(
      sortedActivities.reduce((activities, activity) => {
        const date = activity.date.toISOString().split("T")[0];

        activities[date] = activities[date]
          ? [...activities[date], activity]
          : [activity];

        return activities;
      }, {} as { [key: string]: IActivity[] })
    );
  }

  @action loadActivities = async () => {
    this.loadingInitial = true;

    try {
      const activities = await agent.Activities.list();

      runInAction("Loading activities...", () => {
        activities.forEach((activity) => {
          setActivityProps(activity, this.rootStore.userStore.user!);
          this.activityRegistry.set(activity.id, activity);
        });
        this.loadingInitial = false;
      });
    } catch (error) {
      runInAction("Load activities error", () => {
        console.log(error);
        this.loadingInitial = false;
      });
    }
  };

  @action loadActivity = async (id: string) => {
    let activity = this.getActivity(id);

    if (activity) {
      this.selectedActivity = activity;
      return activity;
    } else {
      this.loadingInitial = true;

      try {
        activity = await agent.Activities.details(id);

        runInAction("Getting activity...", () => {
          setActivityProps(activity, this.rootStore.userStore.user!);
          this.selectedActivity = activity;
          this.activityRegistry.set(activity.id, activity);

          this.loadingInitial = false;
        });

        return activity;
      } catch (error) {
        runInAction("Get activity error", () => {
          console.log(error);
          this.loadingInitial = false;
        });
      }
    }
  };

  @action clearActivity = () => {
    this.selectedActivity = null;
  };

  getActivity = (id: string) => {
    return this.activityRegistry.get(id);
  };

  @action createActivity = async (activity: IActivity) => {
    this.submitting = true;

    try {
      await agent.Activities.create(activity);

      const attendee = createAttendee(this.rootStore.userStore.user!);
      attendee.isHost = true;
      let attendees = [];
      attendees.push(attendee);
      activity.attendees = attendees;
      activity.comments = [];
      activity.isHost = true;

      runInAction("Creating activity...", () => {
        this.activityRegistry.set(activity.id, activity);
        this.submitting = false;
      });

      history.push(`/activities/details/${activity.id}`);
    } catch (error) {
      runInAction("Create activity error", () => {
        console.log(error);
        toast.error("Problem submitting data");
        this.submitting = false;
      });
    }
  };

  @action editActivity = async (activity: IActivity) => {
    this.submitting = true;

    try {
      await agent.Activities.update(activity);

      runInAction("Editing activity...", () => {
        this.activityRegistry.set(activity.id, activity);
        this.selectedActivity = activity;

        this.submitting = false;
      });

      history.push(`/activities/details/${activity.id}`);
    } catch (error) {
      runInAction("Edit activity error", () => {
        console.log(error);
        toast.error("Problem submitting data");
        this.submitting = false;
      });
    }
  };

  @action deleteActivity = async (
    event: SyntheticEvent<HTMLButtonElement>,
    id: string
  ) => {
    this.submitting = true;
    this.target = event.currentTarget.name;

    try {
      await agent.Activities.delete(id);

      runInAction("Deleting activity...", () => {
        this.activityRegistry.delete(id);
        this.submitting = false;
        this.target = "";
      });
    } catch (error) {
      runInAction("Delete activity error", () => {
        console.log(error);
        this.submitting = false;
        this.target = "";
      });
    }
  };

  @action attendActivity = async () => {
    const attendee = createAttendee(this.rootStore.userStore.user!);
    this.loading = true;
    try {
      await agent.Activities.attend(this.selectedActivity!.id);
      runInAction(() => {
        if (this.selectedActivity) {
          this.selectedActivity.attendees.push(attendee);
          this.selectedActivity.isGoing = true;
          this.activityRegistry.set(
            this.selectedActivity.id,
            this.selectedActivity
          );
          this.loading = false;
        }
      });
    } catch (error) {
      runInAction(() => {
        this.loading = false;
      });
      toast.error("Problem signing up to activity.");
    }
  };

  @action cancelAttendance = async () => {
    this.loading = true;
    try {
      agent.Activities.deleteAttendance(this.selectedActivity!.id);
      runInAction(() => {
        if (this.selectedActivity) {
          this.selectedActivity.attendees = this.selectedActivity.attendees.filter(
            (a) => a.username !== this.rootStore.userStore.user!.username
          );
          this.selectedActivity.isGoing = false;
          this.activityRegistry.set(
            this.selectedActivity.id,
            this.selectedActivity
          );
          this.loading = false;
        }
      });
    } catch (error) {
      runInAction(() => (this.loading = false));
      toast.error("Problem cancelling attendance.");
    }
  };
}
