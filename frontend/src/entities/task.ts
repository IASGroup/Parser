export interface ITask {
  id: string,
  url: string,
  type: {
    id: number,
    name: string,
    description: string
  },
  name: string,
  status: {
    id: number,
    key: string,
    description: string
  },
  statusId: number,
  parserTaskWebsiteTagsOptions: {
    id: string,
    parserTaskWebsiteTags: Array<{
      id: string,
      
    }>
  }
}
