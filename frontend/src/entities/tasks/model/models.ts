export enum TaskTypes {
  Api = 0,
  WebsiteText = 1,
  WebsiteTags = 2
}

export enum TaskStatuses {
  Created = 1,
  InProgress = 2,
  Paused = 3,
  Error = 4,
  Finished = 5
}

export type TaskListModel = {
  id: string,
  url: string,
  name: string,
  typeId: TaskTypes,
  statusId: TaskStatuses,
  hasError: boolean,
  completedPartsNumber: number,
  allPartsNumber: number
}

export type TaskDetailsModel = {
  id: string,
  url: string,
  name: string,
  typeId: TaskTypes,
  statusId: TaskStatuses,
  inProgressUrl: string
}

export type TaskDetailsResultModel = {
  id: string,
  parserTaskId: string,
  url: string,
  statusId: number
}
