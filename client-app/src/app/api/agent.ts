import axios, { AxiosResponse } from "axios";
import { IActivity, IActivitiesEnvelope } from "../models/activity";
import { history } from "../..";
import { toast } from "react-toastify";
import { IUser, IUserFormValues } from "../models/user";
import { IProfile, IPhoto } from "../models/profile";

axios.defaults.baseURL = process.env.REACT_APP_API_URL;

axios.interceptors.request.use(
  (config) => {
    const token = window.localStorage.getItem("jwt");
    if (token) config.headers.Authorization = `Bearer ${token}`;
    return config;
  },
  (error) => Promise.reject(error)
);

axios.interceptors.response.use(undefined, (error) => {
  if (error.message === "Network Error" && !error.response) {
    toast.error("Network error - make sure API is running!");
  }

  const { status, data, config, headers } = error.response;

  if (status === 404) {
    history.push("/404");
  }

  if (status === 401 && headers["www-authenticate"].indexOf(`Bearer error="invalid_token"`) !== -1) {
    window.localStorage.removeItem("jwt");
    history.push("/");
  }

  if (status === 400 && config.method === "get" && data.errors.hasOwnProperty("id")) {
    history.push("/404");
  }

  if (status === 500) {
    toast.error("Server error");
  }

  throw error.response;
});

const responseBody = (response: AxiosResponse) => response.data;

// const sleep = (ms: number) => (response: AxiosResponse) => new Promise<AxiosResponse>((resolve) => setTimeout(() => resolve(response), ms));

const requests = {
  get: (url: string) => axios.get(url).then(responseBody),
  post: (url: string, body: {}) => axios.post(url, body).then(responseBody),
  put: (url: string, body: {}) => axios.put(url, body).then(responseBody),
  delete: (url: string) => axios.delete(url).then(responseBody),
  postForm: (url: string, file: Blob) => {
    let formData = new FormData();
    formData.append("File", file);
    return axios
      .post(url, formData, {
        headers: { "Content-type": "miltipart/form-data" },
      })
      .then(responseBody);
  },
};

const Activities = {
  list: (axiosParams: URLSearchParams): Promise<IActivitiesEnvelope> => axios.get(`/activities`, { params: axiosParams }).then(responseBody),
  details: (id: string) => requests.get(`/activities/${id}`),
  create: (activity: IActivity) => requests.post("/activities", activity),
  update: (activity: IActivity) => requests.post(`/activities/${activity.id}`, activity),
  delete: (id: string) => requests.delete(`/activities/${id}`),
  attend: (id: string) => requests.post(`/activities/attend/${id}`, {}),
  deleteAttendance: (id: string) => requests.delete(`/activities/deleteAttendance/${id}`),
};

const User = {
  current: (): Promise<IUser> => requests.get("/user"),
  login: (user: IUserFormValues): Promise<IUser> => requests.post("/user/login", user),
  register: (user: IUserFormValues): Promise<IUser> => requests.post("/user/register", user),
  fbLogin: (accessToken: string) => requests.post(`/user/facebook`, {accessToken})
};

const Profile = {
  get: (username: string): Promise<IProfile> => requests.get(`/profiles/${username}`),
  uploadPhoto: (photo: Blob): Promise<IPhoto> => requests.postForm(`/photos`, photo),
  setMainPhoto: (id: string) => requests.post(`/photos/setMain/${id}`, {}),
  deletePhoto: (id: string) => requests.delete(`/photos/${id}`),
  follow: (username: string) => requests.post(`/profiles/follow/${username}`, {}),
  unfollow: (username: string) => requests.delete(`/profiles/unfollow/${username}`),
  listFollowings: (username: string, predicate: string) => requests.get(`/profiles/getFollowings?username=${username}&predicate=${predicate}`),
  listActivities: (username: string, predicate: string) => requests.get(`/profiles/${username}/activities?predicate=${predicate}`),
};

export default {
  Activities,
  User,
  Profile,
};
