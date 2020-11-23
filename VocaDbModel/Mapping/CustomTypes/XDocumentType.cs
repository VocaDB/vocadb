using System;
using System.Data;
using System.Data.Common;
using System.Xml.Linq;
using NHibernate;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

namespace VocaDb.Model.Mapping.CustomTypes
{

	public class XDocumentType : IUserType
	{

		public int GetHashCode(object x)
		{
			return x.GetHashCode();
		}

		public object Replace(object original, object target, object owner)
		{
			return original;
		}

		public object Assemble(object cached, object owner)
		{
			return DeepCopy(cached);
		}

		public object Disassemble(object value)
		{
			return DeepCopy(value);
		}

		public SqlType[] SqlTypes
		{
			get { return new[] { new SqlXmlType() }; }
		}
		public Type ReturnedType { get { return typeof(XDocument); } }

		public new bool Equals(object x, object y)
		{

			if (ReferenceEquals(x, y)) return true;
			if (x == null || y == null) return false;

			return x.Equals(y);

		}

		public object DeepCopy(object value)
		{

			var source = value as XDocument;

			if (source == null)
				return null;

			return new XDocument(source);

		}

		public bool IsMutable
		{
			get { return true; }
		}

		public object NullSafeGet(DbDataReader dr, string[] names, ISessionImplementor session, object owner)
		{

			var content = dr[names[0]] as string;

			if (content != null)
				return XDocument.Parse(content);
			else
				return null;

		}

		public void NullSafeSet(DbCommand cmd, object obj, int index, ISessionImplementor session)
		{

			var parameter = (DbParameter)cmd.Parameters[index];

			if (obj == null)
			{
				parameter.Value = DBNull.Value;
			}
			else
			{
				var doc = (XDocument)obj;
				var content = doc.ToString();
				parameter.Value = content;
			}
		}

	}

	public class SqlXmlType : SqlType
	{
		public SqlXmlType()
			: base(DbType.Xml)
		{
		}
	}

}
