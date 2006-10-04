***********************************************************************************************
* Piccolo Direct3D Readme file                                                                *
* Human-Computer Interaction Laboratory                                                       *
* University of Maryland                                                                      *
* http://www.cs.umd.edu/hcil/                                                                 *
***********************************************************************************************

INTRODUCTION

This package contains a Direct3D implementation of Piccolo.  Using Direct3D rather than GDI+
means that Piccolo can be hardware accelerated, offering a potentially significant performance
improvement.

This implementation resues the core of the framework, while providing specialized nodes that
render themselves with Direct3D as well as a specialized Direct3D canvas.  TO convert a standard
piccolo project to a PiccoloDirect3D project you will have to change a few of your Piccolo
references to the equivalent PiccoloDirect3D types.

PCamera => P3Camera
PCanvas =>P3Canvas
PForm => P3Form
PNode => P3Node
PImage => P3Image
PPath => P3Path
PText => P3Text
PPaintContext => P3Paintcontext


RUNNING THE CODE

If you downloaded the compiled source, open the "./Bin" directory and double click the file
named "Direct3DTester.exe."

If you checked out Piccolo.NET from CVS or downloaded the source distribution, you will need
to build the project in Visual Studio before running the code.

SOURCE

The source code is contained in the "./Source" directory.  If you would like to view the source,
double click on the "PiccoloDirect3D.sln" file to open the Visual Studio project.