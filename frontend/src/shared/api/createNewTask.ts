export enum TaskTypes {
  Api = 0,
  WebsiteText = 1,
  WebsiteTags = 2
}

export type CreateTaskRequest = {
  url: string,
  typeId: TaskTypes,
  name: string,
  parserTaskWebsiteTagsOptions: {
    parserTaskWebsiteTags: Array<{
      findOptions: {
        name: string,
        attributes: Array<{
          name: string,
          value: string
        }> | null
      }
    }>
  } | null,
  parserTaskUrlOptions: {
    requestMethod: string,
    postMethodOptions: {
      requestBody: string
    } | null,
    queries: Array<{
      name: string,
      valueOptions: {
        range: {
          start: number,
          end: number
        } | null,
        values: Array<{
          value: string
        }> | null,
        value: string | null
      }
    }> | null,
    paths: Array<{
      name: string,
      valueOptions: {
        range: {
          start: number,
          end: number
        } | null,
        values: Array<{
          value: string
        }> | null,
        value: string | null
      }
    }> | null,
    headers: Array<{
      name: string,
      value: string
    }> | null
  }
}

export enum TaskStatuses {
  Created = 1,
  InProgress = 2,
  Paused = 3,
  Error = 4,
  Finished = 5
}

export type CreateTaskResponse = {
  id: string,
  url: string,
  typeId: TaskTypes,
  name: string,
  statusId: TaskStatuses
  parserTaskWebsiteTagsOptions: {
    parserTaskWebsiteTags: Array<{
      findOptions: {
        name: string,
        attributes: Array<{
          name: string,
          value: string
        }> | null
      }
    }>
  } | null,
  parserTaskUrlOptions: {
    requestMethod: string,
    postMethodOptions: {
      requestBody: string
    } | null,
    queries: Array<{
      name: string,
      valueOptions: {
        range: {
          start: number,
          end: number
        } | null,
        values: Array<{
          value: string
        }> | null,
        value: string | null
      }
    }> | null,
    paths: Array<{
      name: string,
      valueOptions: {
        range: {
          start: number,
          end: number
        } | null,
        values: Array<{
          value: string
        }> | null,
        value: string | null
      }
    }> | null,
    headers: Array<{
      name: string,
      value: string
    }> | null
  }
}

import { axiosApi } from "@/shared/api/axiosApi";
import { Result } from "@/shared/api/result"

export async function CreateTaskAsync(request: CreateTaskRequest): Promise<Result<CreateTaskResponse>> {
  const response = await axiosApi.post('/api/manage/parser-tasks', request)
  return response.status === 200
    ? { isSuccess: true, result: response.data as CreateTaskResponse, errorMessage: null }
    : { isSuccess: false, result: null, errorMessage: response.data }
}
