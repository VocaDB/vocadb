import axios from 'axios';

export default class HttpClient {
  public delete = async <T>(url: string): Promise<T> => {
    const response = await axios.delete<T>(url);
    return response.data;
  };

  public get = async <T>(url: string, data?: any): Promise<T> => {
    const response = await axios.get<T>(url, { params: data });
    return response.data;
  };

  public post = async <T>(url: string, data?: any): Promise<T> => {
    const response = await axios.post<T>(url, data);
    return response.data;
  };
}
