# EasySave
## _Your solution to backup_



[![Build Status](https://travis-ci.org/joemccann/dillinger.svg?branch=master)](https://travis-ci.org/joemccann/dillinger)

EasySave is a backup software, offline-storage compatible.


## Thanks to those who contributed in making it real

- DR. Mohammed Hindawi as supervisor
- Mohammed Abdelilah Meddahi.
- Alaize Ethan.
- Elvirand.
- Ronaldo Pradeep.



## Features
-The program detects the system language and uses it to show informations
-The program allows the user to change the languague (currently supporting only english and french).
- The program allow the creation of up to five backup jobs.
- A backup job is defined by a name, a source directory, a target directory, a type (full, differential).
- The directories (source and target) can be located on local drives, external drives, network readers.
- The backup cover all the elements of the source directory.
- The software created daily json log files.
- The user can request the running of one of the backup jobs or the sequential running of all of the jobs.

## Tech

Dillinger uses a number of open source projects to work properly:

- **[.Net]** - A versatile and robust framework for building scalable, high-performance applications across various platforms.
- **[C#]** - For expressive and type-safe development.
- **[Spectre.Console]** - For beautiful interaction https://spectreconsole.net/.

And of course EasySave itself is open source with a [public repository][dill] on GitHub. #not yet!

## Installation

EasySave requires: 
- [DotNet](https://dotnet.org/) v3+ to run.
- [to fill later] 
Install the dependencies and devDependencies and start the server.

```sh
git clone EasySave
cd EasySave
dotnet run .
```

For production environments...

```sh
git clone EasySave
cd EasySave
code .
start adding your functionnalities
```





