<Query Kind="Statements">
  <Connection>
    <ID>4eb94a33-ed3f-4dfb-8755-87614302b79b</ID>
    <Persist>true</Persist>
    <Server>.</Server>
    <Database>VocaloidSite</Database>
  </Connection>
  <Output>DataGrids</Output>
  <Reference>&lt;RuntimeDirectory&gt;\System.Web.dll</Reference>
  <Namespace>System.Globalization</Namespace>
  <Namespace>System.Web</Namespace>
</Query>

// Migrates user messages into inboxes

var messages = UserMessages.Where(m => m.User == null).ToArray();

("Migrating " + messages.Length + " messages").Dump();

foreach (var msg in messages) {

	("Migrating message " + msg.Id).Dump();
	msg.User = msg.Receiver;
		
	// Create copy for sender		
	if (msg.Sender != null) {
	
		"Creating copy for sender".Dump();
		msg.Inbox = "Received";
	
		var copy = new UserMessages();
		copy.Inbox = "Sent";
		copy.User = msg.Sender;
		copy.Created = msg.Created;
		copy.Message = msg.Message;
		copy.Receiver = msg.Receiver;
		copy.Sender = msg.Sender;
		copy.Subject = msg.Subject;
		UserMessages.InsertOnSubmit(copy);
	
	} else {
	
		// No sender, it's a notification
		msg.Inbox = "Notifications";
	
	}
	
	("Migrated message into inbox " + msg.Inbox).Dump();
	
}

"Commit".Dump();
SubmitChanges();
"Done".Dump();