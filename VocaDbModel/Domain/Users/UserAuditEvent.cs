using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VocaDb.Model.Domain.Users {

	public class UserAuditEvent {



	}

	public enum UserAuditEventType {
		EditGroup,
		EditName,
		AddNote
	}

}
