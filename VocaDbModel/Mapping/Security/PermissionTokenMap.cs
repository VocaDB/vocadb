using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Mapping.Security {

	public class PermissionTokenMap : ClassMap<PermissionToken> {

		public PermissionTokenMap() {

			Id(m => m.Id);

		}
	}
}
