

CREATE TABLE IF NOT EXISTS MessageLog
(id INTEGER PRIMARY KEY AUTOINCREMENT,
messageTemplateId INTEGER,
status varchar,
senderTuser varchar,
recipientsCpr varchar,
recipientsAdresse varchar,
title varchar,
text varchar,
timeStamp varchar);


CREATE TABLE IF NOT EXISTS MessageTemplates
(id INTEGER PRIMARY KEY AUTOINCREMENT,
title varchar,
text varchar,
messageType integer);

CREATE TABLE IF NOT EXISTS Attachments
(id INTEGER PRIMARY KEY AUTOINCREMENT,
messageId integer,
fileName varchar,
fileData BLOB);

