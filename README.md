# Mission Cogitator
A WH 40K KT companion app

- [Mission Cogitator](#mission-cogitator)
  - [For Users](#for-users)
  - [For Developers](#for-developers)
    - [Building Procedure](#building-procedure)
    - [Packages](#packages)
      - [Package Format](#package-format)
      - [Example: Creating a Package / Adding a KT](#example-creating-a-package--adding-a-kt)


## For Users

## For Developers

### Building Procedure
1. Install [dotnet SDK](https://dotnet.microsoft.com/en-us/download) 10+
2. Clone the repo
3. Run `dotnet build` to compile or `dotnet watch run` to open the app in a browser and be able to live-edit the app.

### Packages
Much of the content in this app are served as packages. Packages serve as bundles of content which are added to the app when loaded. 

Packages can contain:
- Definition = A description of a particular term often used to clarify a game rule
- Actions = An action that an operative can take during the course of gameplay
- Effect = Some condition that can be applied to a particular operative that will negatively or positively effect how they perform
- Equipment = Items the player can deploy that effect the battlefield or allow their operatives to take additional actions
- Ploys = Actions the player can take to effect what options they have during a turning point
- Units = Abstract descriptions of potential operatives and their capabilities
- Teams = Abstract representation of a given "Kill Team" (KT) box. Often they contain specific unique actions, rules, effects, ploys, and units.

#### Package Format
The package format is as follows:

A single file at the package root called `index.json`. This file describes the contents of the package by listing all the files that need to be loaded from the package. The file paths are relative to the package root. Files not listed in this index will be ignored. 

For instance a simple `index.json` may look something like this
```json
{
  "equipment": [
    "/my-equipment.html"
  ]
}
```

Besides the `index.json` file, the rest of the package consists of `*.html` files with optional [JSON](https://www.json.org/json-en.html) front-matter. Front-matter is enclosed within `---` blocks at the very start of the file and what follows is pure-html which is rendered within the app as the description of particular action/equipment/rule etc.  

```html
---
{
  "name": "My Equipment"
}
---
This is a custom homebrew piece of equipment that does the thing.
```

The html description supports one non-standard HTML tag which can be used to make linkages to game rules/actions/effects/equipment etc. This is the SEE tag. 

```html
<see cref='other-rule-id'>Some Text</see>
```

The cref attribute contains the unique ID of the other item to link to, and the text witihn the tag is arbitrary. This is rendered like a hyperlink within the app. 

#### Example: Creating a Package / Adding a KT
See `wwwroot/assets/packages/teams/pathfinders` for an example of how to begin to setup a team definition package.

Start your package by making a new folder with the name of your package; for instance, `my-kt`. In this folder create your `index.json` file. For now this file will be basically blank with the exception of a package name and some text indicating when it was updated. We will add capabilities to the file as we add resources to the package.

> **FILE**: index.json
```json
{
  "name": "My Custom Package",
  "updated": "01/02/2026"
}
```

Now we add resources. Start with simple ones like equipment or ploys. These are simple because they require very little front-matter and are mainly just descriptive text. For instance, say we have a equipment called `Fire Storm` well we'll create a new subfolder called `equipment` just to keep things organized and then a new file called `fire-storm.html` in that folder. The file name will become the unqiue id for that equipment. In this case the id will be `fire-storm`. Use this id if you want to link other descriptions to this one through use of the custom SEE tag.

> **FILE**: equipment/fire-storm.html
```html
---
{
  "name": "Fire Storm"
}
---
Summon the powers of Chaos to generate a whirlpool of fire within 3" of the operative. Can only be used once per game...
```

Be sure to add the file to the `index.json` or it won't be loaded when loading the package.
> **FILE**: index.json
```json
{
  "name": "My Custom Package",
  "updated": "01/02/2026",
  "equipment": [
    "/equipment/fire-storm.html"
  ]
}
```

Repeat this process for all resources in the package that you desire. Just be sure to put the correct resource into the correct spots within the index. Failing to do so may result in some things not showing up in the app as desired. The different categories within the `index.json` file include: "definitions", "actions", "effects", "equipment", "ploys", "units", and "teams".

Units and Teams contain significantly more front-matter than the other resource types. A Unit will define things like tags, attributes (apl, move, save, wounds), weapons, and special rules. A team will define what faction they are apart of, what special equipment they can use, what ploys they can deploy, and what units they can deploy as operatives. It is best to just look at the built in packages provided in this repo such as [Units](wwwroot/assets/packages/teams/pathfinders/units/) or [Teams](wwwroot/assets/packages/teams/pathfinders/teams/), for the Pathfinders team, for concrete examples of all the required fields. 

When done the package should either be committed to this repo as a *standard* package and then added to the `Program.cs` file to be auto-loaded like so `await packages.AddFromUrl("assets/packages/<package-path>");` **OR** should be compressed into a zip file to be destributed by yourself and can be loaded by users using functionality provided within the app.