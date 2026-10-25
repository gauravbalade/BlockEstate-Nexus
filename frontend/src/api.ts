import type { PropertyRecord } from './types';
const baseUrl=import.meta.env.VITE_API_BASE_URL?.replace(/\/$/,'');
export async function fetchProperties():Promise<PropertyRecord[]|null>{if(!baseUrl)return null;const r=await fetch(`${baseUrl}/api/properties`);if(!r.ok)throw new Error(`Registry API returned ${r.status}`);return r.json() as Promise<PropertyRecord[]>;}
export const isApiConfigured=()=>Boolean(baseUrl);
