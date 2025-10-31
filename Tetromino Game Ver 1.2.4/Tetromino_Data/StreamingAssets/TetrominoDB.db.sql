BEGIN TRANSACTION;
CREATE TABLE IF NOT EXISTS "UserAccState" (
	"ID"	INTEGER,
	"IsActive"	INTEGER,
	PRIMARY KEY("ID")
);
CREATE TABLE IF NOT EXISTS "userInfo" (
	"ID"	INTEGER,
	"Username"	TEXT,
	"Userpassword"	TEXT,
	"Highscore"	INTEGER,
	PRIMARY KEY("ID")
);
CREATE TABLE IF NOT EXISTS "userStatistics" (
	"ID"	INTEGER,
	"VSmatches"	INTEGER,
	"Wins"	INTEGER,
	"Losses"	INTEGER,
	"SurvivalTime"	INTEGER,
	PRIMARY KEY("ID")
);
INSERT INTO "UserAccState" VALUES (1,0);
INSERT INTO "UserAccState" VALUES (2,0);
INSERT INTO "UserAccState" VALUES (3,0);
INSERT INTO "UserAccState" VALUES (4,0);
INSERT INTO "UserAccState" VALUES (5,0);
INSERT INTO "UserAccState" VALUES (6,0);
INSERT INTO "userInfo" VALUES (1,'milky45','12345',2500);
INSERT INTO "userInfo" VALUES (2,'MikeTyson','54321',1900);
INSERT INTO "userInfo" VALUES (3,'Mk45','4200955lrm',10000);
INSERT INTO "userInfo" VALUES (4,'packhat','123',0);
INSERT INTO "userInfo" VALUES (5,'rara','wawa',0);
INSERT INTO "userInfo" VALUES (6,'fuwa','1234',0);
INSERT INTO "userStatistics" VALUES (1,1,0,1,150);
INSERT INTO "userStatistics" VALUES (2,6,6,0,0);
INSERT INTO "userStatistics" VALUES (3,1,1,0,150);
INSERT INTO "userStatistics" VALUES (4,0,0,0,0);
INSERT INTO "userStatistics" VALUES (5,0,0,0,0);
INSERT INTO "userStatistics" VALUES (6,0,0,0,0);
CREATE VIEW LeaderboardView AS
SELECT 
    userInfo.ID,
    userInfo.Username,
    userInfo.Highscore,
    userStatistics.SurvivalTime
FROM 
    userInfo
INNER JOIN 
    userStatistics ON userInfo.ID = userStatistics.ID;
COMMIT;
