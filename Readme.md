Resquirrelly
====================

> A Squirrel sample app for app updates and restarts

This sample app uses [Squirrel.Windows](https://github.com/Squirrel/Squirrel.Windows)
for its installation and automatic updates. The app will run for the first time
immediately after installation (like most Squirrel installed apps) and wait for
an update. Once it detects an update (using a 20 second polling interval), it will
show a **Restart** button, which will restart the app when selected. The rest of
this document will walk you through how to set up this sample and run it. Although,
**keep reading before cloning this repo**.

I built this sample because I was struggling to get Squirrel to restart the app
after it detected and downloaded an update. Part of the solution was to comment
out a line of Squirrel code. That's why Squirrel project is linked as a submodule.
I hope to change this after I get more guidance from the community. More info below.

## Prerequisites

* Hosting. You will need a place to host the deployment updates with a
public URL that Resquirrelly can poll. Azure Blob storage was used in putting
this together, but most types of HTTP hosting (Amazon S3, GitHub Pages, etc.)
should work fine.

* NuGet. You'll need to have the [NuGet Command Line Utility](http://nuget.org/nuget.exe)
installed and in your %PATH%. This is used by a script thats been included to make
doing the deployments faster/easier. You'll also need NuGet installed with Visual
Studio (VS) [2013].

## Setup

* Clone this repository with `git clone --recursive https://github.com/locksmithdon/Resquirrelly.git`
This will also clone the Squirrel.Windows submodule. If you cloned before reading
this, run `git submodule init` and then `git submodule update` to bring in Squirrel.Windows.
* Restore Squirrel's NuGet packages by running `nuget restore Squirrel.Windows/Squirrel.sln`
from Resquirrelly directory.
* Open `Resquirrelly.sln` in VS and open `UpdateHelper.cs`.
* Edit line 6 to be the address where Resquirrelly can find the deployments
you're going to upload.
* Open `Squirrel/UpdateManager.cs` and comment out the last line in `RestartApp()`
(`Environment.Exit(0);`) at around line 184.
* Build the solution. _NuGet Package Restore_ is enabled so the packages should
be downloaded and installed when you build. Be sure they do.

## The first build

* Back in your command line tool (in the Resquirrelly folder), enter the following
command: `release 1.0` and let it finish.
* From the `Releases` folder, upload `RELEASES` and `Resquirrelly-1.0-full.nupkg` to the
HTTP location you're hosting your deployment packages. In a real world scenario,
you would probably upload `ResquirrellyInstaller.exe` also.
* After the upload finishes, run `ResquirrellyInstaller.exe` to install and run
Resquirrelly.
* Leave it running.

![Version 1.0](Images/Version-1.0.png)

## Deployments

* Using VS, open `MainWindow.xaml` and change line 16 to "Version 1.1" so it will
be obvious when the update has been applied.
* Build the solution.
* Back on the command line, enter: `release 1.1`
* From the Releases folder, upload `Resquirrelly-1.1-delta.nupkg`,
`Resquirrelly-1.1-full.nupkg` to your HTTP deployment location (hold off on
`RELEASES` for the moment). In a real world scenario, you would probably upload
an update of the installer also.
* Only after the other files have finished uploading, upload `RELEASES`. This
is the file Squirrel is polling for, so you want the other files in place first.
* In less than 20 seconds, Resquirrelly will detect the update, download it, and
display the **Restart** button.

![Version 1.0 updates ready](Images/Version-1.0-restart.png)

* Select it to restart and launch Resquirrelly 1.1.

![Version 1.1](Images/Version-1.1.png)

* Deploying new versions is just a matter of repeating these steps.

## More info

* The line of the Squirrel source code I commented out is the last line of
`UpdateManager.RestartApp()` which closes the currently running app. I think this
line may be called too soon, which might not be giving the call to `Update.exe`
enough time to grab the PID of the app before shutting it down. Before
Resquirrelly closes itself in, the button handler it is just doing an
`await Task.Delay(1000)` to give it more time. Not at all elegant, but hopefully
it's only temporary until I get better guidance from the community.

* The script that does the builds (release.bat) is currently using a debug build.
If you need to use a release build for whatever reason, be sure to change the
script to use Resquirrelly.Release.nuspec. It doesn't include the symbols.

Let me know if you have any questions.




