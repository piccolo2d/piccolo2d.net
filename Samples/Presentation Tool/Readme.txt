***********************************************************************************************
* Presentation Tool Readme file                                                               *
* Human-Computer Interaction Laboratory                                                       *
* University of Maryland                                                                      *
* http://www.cs.umd.edu/hcil/                                                                 *
***********************************************************************************************

INTRODUCTION
	
This example builds an animated slide viewing programing.  The program reads a directory of
slide images and tiles them in a row of thumbnails at the bottom of the screen.  The user may
then click on any slide to have it animate to fill the screen while the previous slide shrinks
back to the thumbnail view at the bottom.  The user can also press the spacebar to move through
the slides sequentially.  In addition, the slide bar at the bottom of the screen will enlarge
whichever slide the mouse moves over, creating a fisheye effect something like the Apple Dock
in the OS X operating system.  Of course, this program does not have to be used as presentation
tool.  It's a pretty nifty image viewing program as well.


RUNNING THE CODE

If you downloaded the compiled source, open the "./Bin" directory and double click the file
named "PresentationTool.exe."

If you checked out Piccolo.NET from CVS or downloaded the source distribution, you will need
to build the project in Visual Studio before running the code.

Select the File->Open menu and specify a directory that contains images.

Special shortcut keys:
  * F11:    Enters full-screen mode
  * Escape: Exits full-screen mode
  * M:      Exits full-screen mode and minimizes window


SOURCE

The source code is contained in the "./Source" directory.  If you would like to view the source,
double click on the "PresentationTool.sln" file to open the Visual Studio project.