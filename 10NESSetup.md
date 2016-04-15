# Introduction #

Prerequisites, and a rundown of the various projects which make up this mess.


# Details #

## Prerequisites, and locations ##

There should be a folder named Libs in the top level of the tree (alongside the dotnet, mono folders).

Libs
  * ` SharpZipLib (ICSharpCode.SharpZipLib.dll) `,
  * lame\_enc.dll, yeti.mmedia.dll, yeti.mp3.dll (required for sound capture to file)

Libs\Unit
  * ` Microsoft's Unity dlls  (Microsoft.Practices.*.dll) `

Libs\WinAPI
  * Microsoft.WindowsAPICodePack.DirectX.dll
  * Microsoft.WindowsAPICodePack.DirectX.Controls.dll (From the Windows 7 API Code Pack on CodePlex)

Other Prerequisites:
> SlimDX August 2009, DirectX SDK August 2009

Optional:
> Tao Framework, for OpenGL and OpenAL components

Projects in the winbulb solution

There is a lot of junk floating around, though it's getting cleaner, it is part of this projects legacy as a late-night scratchpad for wild ideas.

  * 10NES (deprecated, original test project)
  * 10NES2 - The main UI project for the Windows build, should be startup
  * InstiBulbWPFUI - WPF UI Components and view models
  * SlimDXBindings - SlimDX code, audio, video, graphics, input
  * InstiBulb (deprecated, pure wpf project)
  * OpenALWavStreamer - OpenAL bound wav streamer, optional and out of date
  * OpenGLNesViewer - OpenGL bound viewer, optional and out of date

winbulb - original test project, deprecated