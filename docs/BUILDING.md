# Building Vivianne
By defualt, Vivianne is being developed to support Need For Speed III. It doesn't mean however, that other games that may use the same file formats supported by Vivianne could not be edited. Such is the case for other versions of Need For Speed and other contemporary Electronic Arts games, such as Sim City.

## Extended FSH support
It's been observed that Need For Speed III uses a subset of all the available data formats supported by FSH files, using them exclusively for textures and some data related to car dashboards using the *Footer* of a texture.

Other games use varying pixel formats for textures; NFS3 uses almost exclusively 32-bit RGBA and 16-bit RGB-565 textures, with just a handful of 256 color textures with a palette. Some of the available formats observed inside FSH files for other Electronic Arts games include 24-bit color, 16-bit with alpha, varying palette color depths and DXT3/4 compressed textures. Some games don't just store texture data, and include binary and text entries in FSH files as well.

I don't know if Need For Speed III supports any of these additional FSH pixel formats (I could reasonably assume that it wouldn't support binary and text data in FSH files) or if it would be able to safely ignore anything that it could not use. Therefore, a compilation flag is required to enable support for these extra pixel and data formats.

To enable the extended FSH format support, either:
1. Edit the `./BuildTargets/GlobalDirectives.targets` file to include `EnableFullFshFormat` inside the `ExtraDefineConstants` tag
2. Include the `-p:ExtraDefineConstants=EnableFullFshFormat` parameter when building Vivianne from sources using `dotnet`.