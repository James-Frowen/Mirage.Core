# Mirage.Core

Pure c# version of [Mirage](https://github.com/MirageNet/Mirage)

## Features

This libray has a sub-set of the feature of Mirage, it does not contain any of the object management stuff that are useful for game engines like Unity or Godot

- Reliable/Unreliable Udp transport
    - support for alternative transports like websockets
- Network message
- Automatic serialization
- BitPacking

## Run CodeGen

1) compile Mirage.CodeGen.exe
2) run codegen
```sh
./src/Mirage.CodeGen/bin/Debug/net6.0/Mirage.CodeGen.exe "<target dll>"
```

example 
```sh
./src/Mirage.CodeGen/bin/Debug/net6.0/Mirage.CodeGen.exe "./src/Mirage.Core/bin/Debug/net6.0/Mirage.Core.dll"
```
