export const parserTasksHubUrl = "http://localhost:5140/parser-tasks-hub";
export enum parserTasksHubMessages {
  NewParserTaskCollectMessage = "NewParserTaskCollectMessage"
}

export enum ParserTaskCollectMessageTypes {
  StatusChanged,
  Progress
}

export type ParserTaskCollectMessage = {
  parserTaskId: string,
  type: ParserTaskCollectMessageTypes,
  parserTaskStatusChangedMessage: {
    newTaskStatus: number
  } | null,
  parserTaskErrorMessage: {
    url: string | null,
    errorMessage: string
  }| null,
  parserTaskProgressMessage: {
    completedPartUrl: string,
    nextPartUrl: string | null,
    completedPartsNumber: number,
    completedPartId: string,
    completedPartStatusId: number
  }
}
