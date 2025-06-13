# Building Vivianne
By defualt, Vivianne is being developed to support Need For Speed III. It doesn't mean however, that other games that may use the same file formats supported by Vivianne could not be edited. Such is the case for other versions of Need For Speed and other contemporary Electronic Arts games, such as Sim City.

Some of the flags being mentioned here might be outdated or deprecated already, and I just haven't removed them yet from the docs.

## Extended FSH support
It's been observed that Need For Speed III uses a subset of all the available data formats supported by FSH files, using them exclusively for textures and some data related to car dashboards using the *Footer* of a texture.

Other games use varying pixel formats for textures; NFS3 uses almost exclusively 32-bit RGBA and 16-bit RGB-565 textures, with just a handful of 256 color textures with a palette. Some of the available formats observed inside FSH files for other Electronic Arts games include 24-bit color, 16-bit with alpha, varying palette color depths and DXT3/4 compressed textures. Some games don't just store texture data, and include binary and text entries in FSH files as well.

I don't know if Need For Speed III supports any of these additional FSH pixel formats (I could reasonably assume that it wouldn't support binary and text data in FSH files) or if it would be able to safely ignore anything that it could not use. Therefore, a compilation flag is required to enable support for these extra pixel and data formats.

To enable the extended FSH format support, either:
1. Edit the `./BuildTargets/GlobalDirectives.targets` file to include `EnableFullFshFormat` inside the `ExtraDefineConstants` tag
2. Include the `-p:ExtraDefineConstants=EnableFullFshFormat` parameter when building Vivianne from sources using `dotnet`.

## Audio compression support
Currently, Vivianne has only partial support for compressed audio files. This is due to the relatively poor documentation on how the compression algorithms work, as well as lack of time from my part to properly investigate and implement codecs for those (I already know about the [vgmstream](https://github.com/vgmstream/vgmstream) repo and some others).

Generally speaking, cars shipped with Need For Speed III and Need For Speed 4 do not have compressed audio streams for the engine sounds, so this would not be a problem for general car modding. Cop speech and another in-game audio is another story, but usually no one creates a car with cop speech support (not that it can't happen, but... you know, unlikely).

Also, as far as I know, there's been little to no interest in creating music mods for Need For Speed III, so the lack of compression support for music files is not a big problem either. The music files are stored in the MUS format, which is a superset of the ASF audio format that generally uses EA's ADPCM codec. The compression algorithm is giving me a hard time, but I hope to implement it in the future.

As far as audio files that use the MicroTalk codec goes, there's no support for it in Vivianne yet. The MicroTalk codec is used for the cop speech and some other audio files, but I don't know how it works. I've seem some docs there on the internet, but implementing audio compression codecs is not my strong suit, so I haven't even tried to implement it yet. If you want to help with that, please do!

Anyway, a partially implemented codec for EA-ADPCM exists in the sources (for MUS files), but it does not include compression yet. The MicroTalk codec is not implemented yet.

## Startup warning message
When compiling Vivianne with the release config, it's possible to enable a warning message to be displayed upon app startup. This is useful during early stages of development, or during a major overhaul. This warning message is usually directed at QA teams, or beta testers, not for end users.

If you want to display a message during app startup, edit `Vivianne/Resources/Embedded/EarlyAlphaNote.txt` with your custom message, and then compile Vivianne on the `Release` configuration as well as defining the `EnableStartupWarning` constant.