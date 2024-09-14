# Testing out Godot VR

## Requirements

* Godot 4.3
* https://docs.godotengine.org/en/stable/tutorials/export/exporting_for_android.html
  * OpenJDK (11+)
  * Android SDK (35)
  * Android NDK
  * CMake
* https://docs.godotengine.org/en/stable/tutorials/scripting/gdextension/gdextension_cpp_example.html
  * Scons
  * clang

## Building Extension

* git submodule update --init
* scons

## Running locally on Quest

* https://www.reddit.com/r/godot/comments/zsluh8/comment/j1a9iai/
* https://forum.godotengine.org/t/how-can-i-connect-my-android-phone-to-godot-and-live-preview/22744/2

## TODO

* The GDExtension example recommends putting the Godot project in its own folder and I think that would be a lot cleaner here
  * Probaby put project in a directory and then each extension in its own?

## Misc

* https://docs.godotengine.org/en/stable/tutorials/xr/setting_up_xr.html
