# Mission Cogitator
A WH 40K KT companion app

## Building Procedure
1. Install [dotnet SDK](https://dotnet.microsoft.com/en-us/download) 10+
2. Clone the repo
3. Run `dotnet build` to compile or `dotnet watch run` to open the app in a browser and be able to live-edit the app.

## Building Teams
See `wwwroot/assets/packages/teams/pathfinders` for an example of how to begin to setup a team definition package.

Each package contains
- an index.json which describes what is in the package (the team, the units, the rules etc)
- files for each including thing which are HTML+Json front-matter
- Of which, one file is used to represent which units are in the team (this allows for a package to ship multiple teams)

Then add the package to the `Program.cs` like so `await packages.AddFromUrl("assets/packages/teams/pathfinders");`
