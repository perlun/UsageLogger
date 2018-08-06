# UsageLogger

Logs computer usage of a Windows workstation/desktop machine.

What if you are saying to your children "You are sitting by the computer too much", and they reply
"You sit at the computer just as much yourself"? You want _proof_, hard facts instead of a meaningless
debate around _feelings_ or _perceived truths_. :-)

If so, `UsageLogger` might be for you. It's a simple .NET/C#-based application written during the course
of a few evenings. It's very Windows-centric - the APIs being used are Win32 APIs and I don't think
something like this could easily be done in a platform-agnostic way. If you want a Linux or macOS port,
it's probably easiest to look at the code and be inspired by the approach, and then rewrite it using
platform-specific APIs for your target platform.

## Prerequisites

- A MySQL database. (Could easily be rewritten to use SQLite or some other DB instead; I chose MySQL
  simply because I wanted to be able to query the database easily, `GROUP BY` etc., and also because
  it's the database being used at work.)
- A Windows 10 computer. Probably works on Windows 8 or 7 also, but untested.

## Creating the database and database user

Connect to your MySQL user as a root user (or some other account with superuser privileges) and run
the following queries (replace `the_password` with a randomized password of your choice):

```sql
CREATE DATABASE usage_log;
CREATE USER 'usage_log'@'%' IDENTIFIED BY 'the_password';
GRANT ALL PRIVILEGES ON usage_log.* TO 'usage_log'@'%';
```

## How to install the program

- After building the project (I've used VS2017 Community), copy `bin/x64/Release/UsageLogger.exe*` to
  a folder of your choice.
- Modify `UsageLogger.exe.config` to refer to your MySQL server.
- Add a shortcut to the `UsageLogger.exe` file to `shell:common startup`. The program should now start
  when any user logs on to Windows. (To be able to add a shortcut there, you might have to open up
  the security permissions for the folder in question and grant your own user Full Control.)

## Security model

Obviously, to make this much more secure, it should run as a Windows Service with `SYSTEM` privileges,
and all the children accounts should be non-privileged. However, certain Win32 APIs being used by this
program are unavailable when being run from a background service.

This means that any skillful user could easily open up the Task Manager and kill the `UsageLogger.exe`
process. But, if they've managed to be clever enough to figure that out, maybe your users deserve their
hard-earned privacy after all? :-)

## Development

- The program runs as a Windows app without a user interface. All `Console.WriteLine` statements get
  redirected to the Output window in Visual Studio, if running with debugger attached.

## Useful SQL queries

```sql
-- Usage per user, per day.
SELECT
	SUM(duration) / 60 AS minutes_used,
	DATE(created) log_date,
	login_name
FROM log_entry
GROUP BY log_date, login_name;

-- Find the most used programs, sorted by total usage.
SELECT
	SUM(duration) / 3600 AS hours_used,
	process_name
FROM log_entry
GROUP BY process_name
ORDER BY hours_used DESC;
```

# LICENSE

[MIT](LICENSE)
