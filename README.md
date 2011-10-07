DS3Dbugger
==========

3D Debugger for NDS

NDS
---

###Build Prerequisites

You need the following in order to build the NDS server

 * [devkitARM](http://sourceforge.net/projects/devkitpro/files/devkitARM/)
 * [libnds](http://sourceforge.net/projects/devkitpro/files/libnds/)
 * [dswifi](http://sourceforge.net/projects/devkitpro/files/dswifi/)
 * [libz](http://sourceforge.net/projects/devkitpro/files/portlibs/arm/)

###Usage

 * Ensure you have set up a WFC connection on your DS via a retail game. DS3Dbugger uses your WFC data to connect to an AP.
 * Run DS3Dbugger on your DS.
 * It will connect to your AP and display an IP address.
 * Use this IP address in the PC client.
 * The DS does not require user intervention.

PC
--

###Build Prerequisites

You need the following in order to build the PC client

 * [Visual C#](http://www.microsoft.com/visualstudio/en-us/products/2010-editions/visual-csharp-express)

###Usage

 * Run the C# program.
 * Enter the IP address displayed on the DS and click 'Connect'.
 * Click "Capture" to capture the DS output to be displayed on your PC.
