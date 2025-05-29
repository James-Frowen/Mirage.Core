# Mirage.Core (experimental, unlikely to get suppport)

Pure c# version of [Mirage](https://github.com/MirageNet/Mirage)

## Mirage.Core vs MirageStandalone

- [MirageStandalone](https://github.com/MirageNet/MirageStandalone) is the full unity version made to run in dotnet core without unity, it is the same codebase as Mirage with an extra csproj that contain fake versions of unity's function so that it will compile outside of unity
- [Mirage.Core](https://github.com/James-Frowen/Mirage.Core) (this repo) is a stripped down version of Mirage without any of the object features that depend on unity or other game engine.

This is currently experimental, but later on may be used as a base library for unity, standalone, and godot versions to use.

## Features

This libray has a sub-set of the feature of Mirage, it does not contain any of the object management stuff that are useful for game engines like Unity or Godot

- Reliable/Unreliable Udp transport
    - support for alternative transports like websockets
- Network message
- Automatic serialization
- BitPacking

## Install

Requires installation of .NET 8: https://dotnet.microsoft.com/en-us/download/dotnet/8.0

## Run CodeGen

1) compile Mirage.CodeGen.exe
2) run codegen
```sh
./src/Mirage.CodeGen/bin/Debug/net8.0/Mirage.CodeGen.exe "<target dll>"
```

example 
```sh
./src/Mirage.CodeGen/bin/Debug/net8.0/Mirage.CodeGen.exe "./src/Mirage.Core/bin/Debug/net8.0/Mirage.Core.dll"
```
