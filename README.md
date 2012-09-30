Backer Upper
============

Backer Upper is a small, simple, and fast backup program for Windows.
It was created because I needed something which would backup my documents to Amazon S3.
It also supports mirroring to a specified folder.

Apart from the S3 integration, main features are:
 - Super-fast operation. With no changes, it'll blast through my documents folder of 160,000 files in under a minute.
 - Rename/copy detection, so it doesn't have to transfer unnecessary data.
 - Supports backing up to multiple places *at once*.
 - Good integration with Windows Scheduler.

See the [changelog](https://github.com/canton7/backer_upper/blob/master/CHANGELOG.md) for version-to-version changes.


Getting Backer Upper
--------------------

### Downloading

Grab the latest installer from the [downloads page](https://github.com/canton7/backer_upper/downloads).

### Buildling

Clone the repo, and fire up VS2010.
Packages are handled with NuGet, so there are a few steps you need to do to download the deps:

1. Install NuGet. [See here](http://docs.nuget.org/docs/start-here/installing-nuget) for instructions.
2. Allow NuGet to download missing packages: Tools -> Options -> Package Manager -> General -> Check "Allow NuGet to download missing packages during build"
3. Try to build. It *should* work.
4. If not, go to Tools -> Library Package Manager -> Manage NuGet Packages for Solution..., and click "Restore" in the orange bar at the top.

To build the installer build in the release configuration (into bin/Release) and use [NSIS](http://nsis.sourceforge.net/Download) to compile and install `installer/installer.nsi`.


Using It
--------

**WARNING: This is experimental software. While I trust it, always check and verify your backups. I take absolutely no responsibility for any data loss as a result of anything to do with this program**


### Creating and modifying backup profiles

So, fire up Backer Upper.
In the middle is a big list of all of your backup profiles, which is empty.
Let's rectify that.
Hit 'Create'.

#### Backup Name

This is the name of this backup profile.
Other than avoiding some forbidden characters (the backup name maps onto a filename), do what you like.

#### Source Folder

This is the folder you're backing up from.
All files from in here will be backed up, unless they're ignored (see below).
The ability to exclude subfolders will come one day...

#### Source Filter

Filters exclude certain types of files from being backed up.
This field is a series of globs (so '\*' matches everything, while '?' matches a single character), separated by pipes.

#### Source Advanced

This button opens a form, allowing you to select precisely which folders and files do and don't get backed up.
You can also edit your filters from inside this window.

#### Mirror backend

This is a simple backend which takes your source folder, and mirrors it into the destination folder.
The destination folder is kept in sync with the source folder: file deletions and modifications are copied across as necessary.

#### S3 Backend

This backend mirrors your source folder to a specific S3 bucket.
Fill the `Location` box with your destination folder (e.g. `my_bucket_name/my_folder`), add your credentials (there's even a handy link if you've forgotten them) and you're good to go!

Reduced Redendancy Storage, or RRS, provides cheaper storage at a higher failure rate.
The RRS setting for individual files can be changed through your AWS console later if you want.
This setting only applies to new files.

Backer Upper stores each file's checksum in a custom header on S3, and can optionally check this when performing a backup.
This requires a HEAD request per file, so is slow and potentially (slightly) expensive, but could be handy.

#### Scheduler

The scheduler is pretty self-explanatory.
It just talks to the Windows Task Scheduler, so if you're curious, fire that up and open the `BackerUpper` folder.
Don't add new conditions, trigger, or actions to existing items, but feel free to add your own.

If "start when available" is checked, and your computer isn't on when the backup is scheduled to start, the backup will run when the computer is next turned on.
If "start on batteries" is checked, the backup will run even when your computer is running on battery power.
If "close when finished" is checked, Backer Upper will close when the backup is complete.
If "ignore warnings" is *not* checked, Backer Upper will display a dialog box if warnings occurred during the backup, which will stop the program from automatically closing.


### Running Backups

This bit's the easy bit: hit the big 'Backup' button, watch the files and time tick away along the bottom, and inspect the log at the end.

Cancelling backups is pretty easy: hit the cancel button.
An ongoing S3 transfer will finish before cancelling (if anyone can figure out the API for cancelling uploads, tell me!), but everything else cancels pretty quickly.
Be careful about force-closing the application: records of the operations performed are kept in memory for speed, and only synced to disk every 5 minutes, so you could lose 5 minutes' worth of backing up (for the mirror backend), or a single file (for the S3 backend).

You can only run one backup from one instance of the program at once time.
However, feel free to fire up another instance to do another backup simultaneously -- Backer Upper has enough locking to stop you doing anything silly (I think: never underestimate human stupidity and all that).

### Managing Backup Profiles

Your backup profiles are stored in AppData, and are copied to the backup destination after a successful backup (the profile being uploaded to S3 has most of its data stripped).
If you want to import a profile from where else, hit "Import".
To delete something, hit "Delete".

No metadata is kept about the profile other than the file on the filesystem in AppData, and maybe a Task in the Task Scheduler.

### Restoring a backup

Step 1 to restoring a backup, if you've lost the backup profile as well, is to grab it and import it into Backer Upper.
Then hit "Restore".

#### Restore from Backend

If a backup uses multiple backends, which one to use to restore the backup.

#### Restore To

Which folder to restore to.
Defaults to the folder which is normally backed up, but feel free to override.

#### Overwrite files which are already present

Pretty self-explanatory.
If Backer Upper tries to restore a file which already exists in your "Restore To" folder, it would normally give up and move on.
With this option ticked, it will go ahead and overwrite that file.

#### But only if the file present is older than the one being restored

Again, explains itself.
If Backer Upper tries to restore a file which already exists in your "Restore To" folder, it will only overwrite said file if it's older than the file it's trying to restore.
"Age" here refers to the mtime (last modified time) of the file in question, not when the last backup was performed.

#### Delete files which aren't present in the backup

If files in your "Restore To" folder exist which aren't present in the backup, Backer Upper would normally leave them alone.
With this option ticket it'll nuke them. From orbit. With fire. And Nukes. And you get the idea: beware!


Under The Hood
--------------

If you're seeing unexplained behaviour, look in here.
I'll try to outline how Backer Upper works, so you can see if it fits in with your expectations.

A backup profile consists of an sqlite database, in which are a files table and a folder table.
The folders table tracks just the name of the folder, but the files table also tracks the MD5 and last modified time of each file.

When you run a backup, Backer Upper iterates over every file and folder in the source directory.
For each folder, it checks to see whether the folder exists in the database.
If not, it creates it on the backends, and adds it to the database.
If it does exist, it checks to make sure it's also present on the backend (these things do go missing).
If it's gone, it re-creates it.

For each file, it checks to see whether the file exists in the database.
If not it finds the MD5, and searches for other files on the backend with the same MD5 ("alternates").
If it finds something, it'll ascertain whether that alternate needs to be deleted this run (so whether the operation is a copy or move), then give each backend the choice of whether or not to use that alternate: moving a file on an external disk is cheap while copying is expensive, for example, while copying and moving are both cheap on S3.
If it doesn't find any alternates, or if the backend refuses all alternates, it'll copy the file over.
Either way, an entry is added to the database.

If the file does exist in the database, it checks the file's last modified time against that recorded in the database.
If it differs (either way), the file's MD5 is calculated.
If this differs, then the backends need to be updated.
The same procedure as with new files is followed: first alternates, then copy the file as a last resort.
The S3 backend does an additional check: it checks the file's MD5 against the MD5 of the file stored on S3 (stored in a custom header).
If this matches (meaning the database is out of date, or something) the file won't be copied.
(It's too expensive to do this with the mirror backend: might as well just write the file than hash then probably write it).

If the file's mtime matches the database record, the file on the backend is checked.
(This only occurs on the S3 backend if the option in properties is ticked, due to its cost).
The mirror backend verifies that the file exists and has the right last modified time.
The S3 backend will check that the file exists and, if enabled, also that the MD5 matches.

Next, a list of all files and folders in the database is obtained, and checked against the source filesystem.
Any files/folders missing on the source filesystem are therefore deletions, and are removed from the database and backends.

Finally, the backends are checked for any files which aren't present in the database.
Such files are removed.


Reporting Bugs
--------------

I've tried very hard to cover every possible code path with lots of exception handling.
However, it's possible you might still hit a bug.

If this happens, you'll be presented with a window including a stack trace.
Please copy the *whole* of this and paste it, along with exactly what you did to cause it, into an issue.
If you can, please try and figure out how to reproduce the issue.

Also record which version you're using.
