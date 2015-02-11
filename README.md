MvcKickstart
===========

A base site template I use to start a new asp.net mvc project.  This is a .net 4.5, asp.net mvc 5 project. 

Overview
--------
This project includes the following:
* Basic user login, registration, and forgot password screens & logic.
* A general 500 error page and 404 page are wired up.  The 404 page can easily be customized to include suggested pages.
* A basic admin area, to help directory/file naming conventions for scripts & stylesheets for areas.

Conventions
-----------

* Attribute routing - All routes are named like MyController_MyAction or MyArea_MyController_MyAction.
* There is no ~/scripts directory. The mvc team should have moved that directory to ~/content/js, where it belongs.  That is where you'll find script files in this template. 
* All actions can have their own js and less file.  These files will automatically be loaded via _ViewStart.cshtml.  If the files do not exist, the site will not fail.  The default convention is to place a file named the same as the action, inside a folder named the same as the controller.  
    * Eg. ~/content/js/Account/Login.js and ~/content/js/Admin/Home/Index.js
    * Similarly: ~/content/less/Account/Register.less and ~/content/less/Admin/Users/Index.less
* The site takes advantage of the mythical donut caching technique from the [CacheStack library](https://github.com/laughlin/laughlin_cachestack). Action results should be cached via the *DonutOutputCache* attribute.
* Only MS SQL Server is supported at this time. It should take just a few tweaks to support other databases.
* Data model objects can add CreatedBy, CreatedOn, ModifiedBy and ModifiedOn properties. The Save() method will auto update those properties, if they're present.

Optional inclusions
----------------------
* This project incorporates tracking internal metrics via [statsd](https://github.com/etsy/statsd).  It's good to know how your app is behaving.  If you don't have statsd setup, you should.  If you still don't want it, removing the "Metrics:*" keys from appSettings will disable metric tracking.
* Unit testing is built into this solution.  Please use what I have as a _starting point_.
* This project includes a basic data migration framework. Migrations are one way (no rolling back) and are handled all in code.

Technology Choices
------------------
* [Dapper](https://github.com/SamSaffron/dapper-dot-net) is the ORM of choice.
* [Spruce](https://github.com/jgeurts/spruce) compliments Dapper
* [CacheStack](https://github.com/laughlin/laughlin_cachestack) takes care of a lot of the caching throughout the project
* Asset bundling and minification is handled by [cassette](http://getcassette.net/).  The built in asp.net bundler sucks in comparison.
* This template uses [bootstrap](http://twitter.github.com/bootstrap/) for a UI starting point. Enjoy...
* [Service Stack](http://www.servicestack.net/) is used throughout this project.  Service Stack is to asp.net projects as Resharper is to Visual Studio.

Renaming Utility
----------------
The Rename Utility (/__Rename Utility/rename.bat) can be used to rename the project from 'KickstartTemplate' to whatever you need it named. 
When you run that file, it will prompt you for the new name. Once the name is entered, the utility will run through all the files to replace any occurrances of 'KickstartTemplate' (primarily the namespaces) to the name you entered. It will also rename the project files/folders, solution file, and update the solution file to reference the new project names.

Questions
-----------------
Please have a look at the [wiki](https://github.com/laughlin/MvcKickstart/wiki). If you have a question about something that isn't in the wiki, please feel free to submit an [issue](https://github.com/laughlin/MvcKickstart/issues) and I'll do my best to provide an answer.
