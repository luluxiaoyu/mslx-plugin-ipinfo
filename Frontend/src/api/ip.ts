import request from 'mslx-request';

export interface IpAddressDetail {
  address: string;
  prefixOrMask: string;
  type: string;
}

export interface NetworkAdapterDetail {
  name: string;
  description: string;
  type: string;
  status: string;
  macAddress: string;
  ipv4Addresses: IpAddressDetail[];
  ipv6Addresses: IpAddressDetail[];
}

export interface IpSummaryResponse {
  externalIpv4: string;
  externalIpv6: string;
  adapterCount: number;
}

export async function fetchIpSummary(forceRefresh: boolean = false): Promise<IpSummaryResponse> {
  const url = `/api/plugins/mslx-plugin-ipinfo/ip/summary?forceRefresh=${forceRefresh}`;
  const response: any = await request.get({ url });
  return response?.data ?? response;
}

export async function fetchIpDetails(): Promise<NetworkAdapterDetail[]> {
  const url = `/api/plugins/mslx-plugin-ipinfo/ip/details`;
  const response: any = await request.get({ url });
  return response?.data ?? response;
}
