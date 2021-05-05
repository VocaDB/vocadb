import axios from 'axios';

export default class HttpClient {
  public delete = async <T>(url: string): Promise<T> => {
    const response = await axios.delete<T>(url);
    return response.data;
  };

  public get = async <T>(url: string, data?: any): Promise<T> => {
    const response = await axios.get<T>(url, {
      params: data,
      // HACK: This is required for advanced search filters.
      paramsSerializer: (params) => {
        // HACK: Removes undefined.
        // Code from: https://stackoverflow.com/questions/286141/remove-blank-attributes-from-an-object-in-javascript/30386744#30386744
        return $.param(JSON.parse(JSON.stringify(params)));
      },
    });
    return response.data;
  };

  public post = async <T>(url: string, data?: any): Promise<T> => {
    const response = await axios.post<T>(url, data);
    return response.data;
  };
}
