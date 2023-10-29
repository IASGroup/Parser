export interface Task {
  id: string;
  url: string;
  type: {
    id: number;
    name: string;
    description: string;
  };
  name: string;
  status: Status;
  statusId: number;
  // parserTaskWebsiteTagsOptions: {
  //   id: string,
  //   parserTaskWebsiteTags: Array<{
  //     id: string,
  //     findOptions : {
  //       name: string,
  //       atributs: Array<{
  //         name: string,
  //         value: string
  //       }>
  //     }

  //   }>
  // }
}

export interface Status {
  id: number;
  key: string;
  description: string;
}
