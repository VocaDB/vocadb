using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain;

// https://github.com/ycanardeau/yamatouta/blob/ba67503d3ded79c942c7240394faa1931e36f0e4/backend/src/entities/WebAddressHost.ts
public class WebAddressHost
{
	public virtual int Id { get; set; }

	public virtual required string Hostname { get; set; }

	public virtual int ReferenceCount { get; set; }

	public virtual required User Actor { get; set; }

	public WebAddressHost() { }

	public WebAddressHost(string hostname, User actor)
	{
		Hostname = hostname;
		Actor = actor;
	}

	public void IncrementReferenceCount()
	{
		ReferenceCount++;
	}

	public void DecrementReferenceCount()
	{
		ReferenceCount--;
	}
}

// https://github.com/ycanardeau/yamatouta/blob/ba67503d3ded79c942c7240394faa1931e36f0e4/backend/src/entities/WebAddress.ts
public class WebAddress
{
	public virtual int Id { get; set; }

	public virtual required string Url { get; set; }

	public virtual required string Scheme { get; set; }

	public virtual required WebAddressHost Host { get; set; }

	public virtual required int Port { get; set; }

	public virtual required string Path { get; set; }

	public virtual required string Query { get; set; }

	public virtual required string Fragment { get; set; }

	public virtual int ReferenceCount { get; set; }

	public virtual required User Actor { get; set; }

	public WebAddress() { }

	public WebAddress(Uri uri, WebAddressHost host, User actor)
	{
		Url = uri.ToString();
		Scheme = uri.Scheme;
		Host = host;
		Port = uri.Port;
		Path = uri.LocalPath;
		Query = uri.Query;
		Fragment = uri.Fragment;
		Actor = actor;
	}

	public void IncrementReferenceCount()
	{
		Host.IncrementReferenceCount();
		ReferenceCount++;
	}

	public void DecrementReferenceCount()
	{
		Host.DecrementReferenceCount();
		ReferenceCount--;
	}
}
