import type { PropertyRecord } from './types';

export type ApiApplication = {
  id: string; property: string; applicant: string; kind: string;
  status: string; documents: string; priority: string;
};
export type ApiAuditEvent = { time: string; actor: string; action: string; target: string; kind: string };
const baseUrl = import.meta.env.VITE_API_BASE_URL?.replace(/\/$/, '');
async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const response = await fetch(`${baseUrl}${path}`, {
    ...init, headers: { 'Content-Type': 'application/json', ...(init?.headers ?? {}) }
  });
  if (!response.ok) {
    const body = await response.text();
    throw new Error(body || `API returned ${response.status}`);
  }
  if (response.status === 204) return undefined as T;
  return response.json() as Promise<T>;
}
export const isApiConfigured = () => Boolean(baseUrl);
export async function fetchProperties(): Promise<PropertyRecord[] | null> {
  if (!baseUrl) return null;
  return request<PropertyRecord[]>('/api/properties');
}
export async function fetchApplications(): Promise<ApiApplication[] | null> {
  if (!baseUrl) return null;
  return request<ApiApplication[]>('/api/applications');
}
export async function fetchAuditEvents(): Promise<ApiAuditEvent[] | null> {
  if (!baseUrl) return null;
  return request<ApiAuditEvent[]>('/api/audit-events');
}
export async function createApplication(input: { title: string; type: string; location: string; owner: string; area: string }): Promise<{ property: PropertyRecord; application: ApiApplication }> {
  return request('/api/applications', { method: 'POST', body: JSON.stringify({
    title: input.title, propertyType: input.type, location: input.location,
    recordedOwner: input.owner, area: input.area || 'Not specified', applicantName: input.owner
  }) });
}
export async function updateApplicationStatus(code: string, status: string): Promise<ApiApplication> {
  return request(`/api/applications/${encodeURIComponent(code)}/status`, { method: 'PUT', body: JSON.stringify({ status }) });
}
