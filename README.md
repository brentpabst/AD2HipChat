## Active Directory to HipChat Integration

[![Stories in Backlog](https://badge.waffle.io/brentpabst/AD2HipChat.png?label=backlog&title=Backlog)](https://waffle.io/brentpabst/AD2HipChat)
[![Build status](https://ci.appveyor.com/api/projects/status/frck4mg1x2pruh3v?svg=true)](https://ci.appveyor.com/project/brentpabst/ad2hipchat)
[![Gitter](https://badges.gitter.im/brentpabst/AD2HipChat.svg)](https://gitter.im/brentpabst/AD2HipChat?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)

Overview
--------

The AD2HipChat tool allows users to synchronize their Active Directory users with HipChat.  HipChat Server doesn't need this tool as it has the concept of User Directories baked in.  This tool enables users who don't have contracts with companies like Okta or OneLogin to still manage user accounts via Active Directory.

How it Works
------------
We suck in users from the configured directory to a SQL Server database.  The database is used to keep track of changes and what has/has not been synchronized to HipChat.  A different process then gets a list of users from HipChat and compares them with the database.  The secondary process then updates names, emails, etc. with HipChat.  Any user who has been disabled in Active Directory will cause the associated HipChat user to be deactivated.  And yes, there are configuration options to control how some of this stuff works.

Prerequisites
-------------
- .NET 4.6.1 installed on the computer/server that will run the application
- An Active Directory service account with rights to read the desired directory
- An active HipChat Cloud account and Administrative API token

Installation & Running
------------

Under the covers the application is a simple console application.  It is wrapped up in a clean Windows Service runner.  You can manually run the application from the command line/PowerShell or as a service.

< Details on how to install the Windows Service coming soon! >

Contributing
------------

Feel free to fork the repo, submit issues, ideas, and pull requests.  Getting started is pretty easy.  Clone the repo and fire up the Visual Studio solution.

A couple of notes:
- We work in Visual Studio 2015 (should work in VS2013 too)
- The project targets .NET 4.6.1
- You'll need SQL Server, we target SQL Server 2014
