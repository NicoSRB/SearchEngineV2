export interface SearchResultItem {
  query: string[];
  hits: number;
  documentHits: {
    document: {
      mId: number;
      mUrl: string;
      mIdxTime: string;
      mCreationTime: string;
    };
    noOfHits: number;
    missing: string[];
    includeTimeStamp: boolean;
    termnet: string[];
  }[];
  ignored: string[];
  timeUsed: string;
}
