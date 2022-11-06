using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain;

// https://github.com/ycanardeau/yamatouta/blob/ba67503d3ded79c942c7240394faa1931e36f0e4/backend/src/entities/WebAddressHost.ts
public class WebAddressHost
{
	public virtual int Id { get; set; }

	public virtual string Hostname { get; set; }

	public virtual int ReferenceCount { get; set; }

	public virtual User Actor { get; set; }

	public WebAddressHost()
	{
		Hostname = null!;
		Actor = null!;
	}

	public WebAddressHost(string hostname, User actor)
	{
		Hostname = hostname;
		Actor = actor;
	}

	public virtual void IncrementReferenceCount()
	{
		ReferenceCount++;
	}

	public virtual void DecrementReferenceCount()
	{
		ReferenceCount--;
	}
}

// https://github.com/ycanardeau/yamatouta/blob/ba67503d3ded79c942c7240394faa1931e36f0e4/backend/src/entities/WebAddress.ts
public class WebAddress
{
	public virtual int Id { get; set; }

	public virtual string Url { get; set; }

	public virtual string Scheme { get; set; }

	public virtual WebAddressHost Host { get; set; }

	public virtual int Port { get; set; }

	public virtual string Path { get; set; }

	public virtual string Query { get; set; }

	public virtual string Fragment { get; set; }

	public virtual int ReferenceCount { get; set; }

	public virtual User Actor { get; set; }

	public WebAddress()
	{
		Url = null!;
		Scheme = null!;
		Host = null!;
		Path = null!;
		Query = null!;
		Fragment = null!;
		Actor = null!;
	}

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

	public virtual void IncrementReferenceCount()
	{
		Host.IncrementReferenceCount();
		ReferenceCount++;
	}

	public virtual void DecrementReferenceCount()
	{
		Host.DecrementReferenceCount();
		ReferenceCount--;
	}
}
