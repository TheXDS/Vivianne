# Building Vivianne
By defualt, Vivianne is being developed to support Need For Speed III. It doesn't mean however, that other games that may use the same file formats supported by Vivianne could not be edited. Such is the case for other versions of Need For Speed and other contemporary Electronic Arts games, such as Sim City.

## Extended FSH support
It's been observed that Need For Speed III uses a subset of all the available data formats supported by FSH files, using them exclusively for textures and some data related to car dashboards using the *Footer* of a texture.

Other games use varying pixel formats for textures; NFS3 uses almost exclusively 32-bit RGBA and 16-bit RGB-565 textures, with just a handful of 256 color textures with a palette. Some of the available formats observed inside FSH files for other Electronic Arts games include 24-bit color, 16-bit with alpha, varying palette color depths and DXT3/4 compressed textures. Some games don't just store texture data, and include binary and text entries in FSH files as well.

I don't know if Need For Speed III supports any of these additional FSH pixel formats (I could reasonably assume that it wouldn't support binary and text data in FSH files) or if it would be able to safely ignore anything that it could not use. Therefore, a compilation flag is required to enable support for these extra pixel and data formats.

To enable the extended FSH format support, either:
1. Edit the `./BuildTargets/GlobalDirectives.targets` file to include `EnableFullFshFormat` inside the `ExtraDefineConstants` tag
2. Include the `-p:ExtraDefineConstants=EnableFullFshFormat` parameter when building Vivianne from sources using `dotnet`.

## BNK compression support
Currently, Vivianne does not support compressed audio streams in BNK files. This is due to the relatively poor documentation on how the compression algorithms work, as well as lack of time from my part to properly investigate and implement codecs for those (I already know about the [vgmstream](https://github.com/vgmstream/vgmstream) repo).

Generally speaking, cars shipped with Need For Speed III and Need For Speed 4 do not have compressed audio streams for the engine sounds, so this would not be a problem for general car modding. Cop speech and another in-game audio is another story, but usually no one creates a car with cop speech support (not that it can't happen, but... you know, unlikely).

Anyway, a broken codec for what should've been EA ADPCM is being included in the sources. You can compile Vivianne with it if you define the `EnableBnkCompression` constant.

## Startup warning message
When compiling Vivianne with the release config, it's possible to enable a warning message to be displayed upon app startup. This is useful during early stages of development, or during a major overhaul. This warning message is usually directed at QA teams, or beta testers, not for end users.

If you want to display a message during app startup, edit `Vivianne/Resources/Embedded/EarlyAlphaNote.txt` with your custom message, and then compile Vivianne on the `Release` configuration as well as defining the `EnableStartupWarning` constant.