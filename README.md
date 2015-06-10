Armok Vision
============

[Forum thread](http://www.bay12forums.com/smf/index.php?topic=146473) | [Screenshots](http://imgur.com/a/bPmeo)

A 3d realtime visualizer for Dwarf Fortress. 

## To Use

1. Install Dwarf Fortress and a current DFHack.
2. Download Armok Vision from the forum thread.
3. Load into a fortress mode map.
4. Start up Armok Vision.
5. Enjoy!

## To Contribute

Want to help out? We love contributions!

#### Bugs
If you've run into any bugs: [report them!](https://github.com/JapaMala/armok-vision/issues) (You'll need a [github account](https://github.com/).) Make sure to describe the issue in as much detail as you can, and to mention what system & Armok Vision version you're using. Also, check if what you're reporting has been reported before.

#### Artists
If you're an artist and want to contribute 3D models, sounds, concept art:

- There should be a folder called `StreamingAssets` somewhere around the armok vision executable (or inside, if you're on a mac.) If you edit the files inside and restart Armok Vision, it will use your modified assets. Be careful editing the .xml files, they're finnicky. You can post your edited resources in the forum thread and we can try to integrate them with the project.
- Alternatively, load things up in Unity and edit them there (see the following instructions).
- Check the [issues](https://github.com/JapaMala/armok-vision/issues); there may be something open about things that need prettifying.

#### Developers
If you know how to code and want to hack on the engine:

1. Install [Unity 5](http://unity3d.com/get-unity). (We're using the Personal Edition.)
2. `$ git clone --depth 1 https://github.com/JapaMala/armok-vision.git` (or without `--depth 1` if you want the full history, but it's pretty big).
3. Load the `armok-vision` folder in the Unity editor.
4. Hack around. Check out the [issues](https://github.com/JapaMala/armok-vision/issues) to find things that need fixing / ideas that could be implemented.
5. Submit a [pull request](https://github.com/JapaMala/armok-vision/pulls) with your changes!

##### Structural Notes
(Some short notes for anyone getting started with the codebase.)

- Armok Vision is an application built with the [Unity engine](https://unity3d.com/). It connects to the [remotefortressreader](https://github.com/DFHack/dfhack/blob/master/plugins/remotefortressreader.cpp) DFHack plugin over TCP and exchanges [protobuf-formatted messages](https://github.com/DFHack/dfhack/blob/master/plugins/proto/RemoteFortressReader.proto). (You don't need to be familiar with DFHack to work with Armok Vision.)
- On the Unity side, [Assets/RemoteClientDF](https://github.com/JapaMala/armok-vision/tree/master/Assets/RemoteClientDF) contains the generated C# protobuf files, as well as classes for managing the network connection. The script [Assets/MapGen/DFConnection.cs](https://github.com/JapaMala/armok-vision/blob/master/Assets/MapGen/DFConnection.cs) runs the connection on a separate thread and exposes data collected from DF.
- The script that actually manages the onscreen map is [Assets/MapGen/GameMap.cs](https://github.com/JapaMala/armok-vision/blob/master/Assets/MapGen/GameMap.cs), which stores the `GameObject`s representing different map chunks. It calls the scripts in [Assets/MapGen/Meshing](https://github.com/JapaMala/armok-vision/tree/master/Assets/MapGen/Meshing) to build the actual meshes (on separate threads).
- Most assets - textures, 3d models, sprites, etc. - are loaded at runtime from [Assets/StreamingAssets](https://github.com/JapaMala/armok-vision/tree/master/Assets/StreamingAssets), which is copied directly to folder containing the generated app. The script that handles this is [Assets/MapGen/ContentLoader.cs](https://github.com/JapaMala/armok-vision/blob/master/Assets/MapGen/ContentLoader.cs).

There's a lot of other stuff but hopefully it'll be reasonably self-explanatory. Alternatively, you can ask in the forum thread, or the #dfhack IRC channel on freenode; somebody might be lurking who can help.
