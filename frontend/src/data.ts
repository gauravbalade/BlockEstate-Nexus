import type { PropertyRecord } from './types';
export const initialProperties:PropertyRecord[]=[
{id:'BEN-MH-10482',title:'Lakeview Residency · A-1204',type:'Apartment',location:'Airoli, Navi Mumbai',owner:'Ananya Mehta',area:'1,180 sq ft',status:'Verified',updated:'Today, 10:42 AM',risk:'Low',applicationId:'APP-2026-1842'},
{id:'BEN-MH-10479',title:'Greenfield Survey Plot 18',type:'Residential plot',location:'Panvel, Navi Mumbai',owner:'Rohan Deshmukh',area:'2,400 sq ft',status:'Under review',updated:'Yesterday',risk:'Review',applicationId:'APP-2026-1838'},
{id:'BEN-MH-10461',title:'Sunrise Industrial Unit 06',type:'Industrial',location:'Taloja MIDC, Navi Mumbai',owner:'Kavya Shah',area:'4,850 sq ft',status:'Action required',updated:'22 Oct 2026',risk:'Elevated',applicationId:'APP-2026-1819'},
{id:'BEN-MH-10433',title:'Orchard Agricultural Parcel',type:'Agricultural land',location:'Karjat, Raigad',owner:'Vikram Patil',area:'0.8 hectare',status:'Verified',updated:'20 Oct 2026',risk:'Low',applicationId:'APP-2026-1770'},
{id:'BEN-MH-10421',title:'Cedar Court · Shop 03',type:'Commercial shop',location:'Thane West',owner:'Neha Kulkarni',area:'620 sq ft',status:'Under review',updated:'19 Oct 2026',risk:'Review',applicationId:'APP-2026-1756'}];
export const initialApplications=[
{id:'APP-2026-1842',property:'Lakeview Residency · A-1204',applicant:'Ananya Mehta',kind:'Ownership verification',status:'Approved',documents:'5 / 5',priority:'Normal'},
{id:'APP-2026-1838',property:'Greenfield Survey Plot 18',applicant:'Rohan Deshmukh',kind:'New registration',status:'Under review',documents:'4 / 5',priority:'Normal'},
{id:'APP-2026-1819',property:'Sunrise Industrial Unit 06',applicant:'Kavya Shah',kind:'Ownership transfer',status:'Action required',documents:'3 / 5',priority:'Elevated'},
{id:'APP-2026-1807',property:'Cedar Court · Shop 03',applicant:'Neha Kulkarni',kind:'Mutation request',status:'Submitted',documents:'5 / 5',priority:'Normal'}];
