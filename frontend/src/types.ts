export type PropertyStatus = 'Verified' | 'Under review' | 'Action required';
export type PropertyRecord = { id:string; title:string; type:string; location:string; owner:string; area:string; status:PropertyStatus; updated:string; risk:'Low'|'Review'|'Elevated'; applicationId:string };
