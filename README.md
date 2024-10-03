# Testing out Godot VR

## Requirements

* Godot 4.3
* https://docs.godotengine.org/en/stable/tutorials/export/exporting_for_android.html
  * OpenJDK (17+)
  * Android SDK (35)
  * Android NDK
  * CMake

## Running locally on Quest

* https://docs.godotengine.org/en/stable/tutorials/xr/deploying_to_android.html
* https://github.com/GodotVR/godot_openxr_vendors
* Have to enable Developer Mode through the Meta Quest app
  * Make sure to allow access to files in the headset (notification may be in the notifications tab, it goes away kinda quick)
* If not using the same signing across PCs, have to adb uninstall first
* Have to set XR mode to OpenXR (defaults to Regular) and include the Meta OpenXR vendor plugin
* Deploy with Remote Debug is useful to get logs in the editor
* Small Deploy with Network Filesystem seems to crash the app when used
* Have to use test users until a first build can be uploaded (and this is a long process to kick off)
  * https://developers.meta.com/horizon/resources/test-users/

## Misc

* https://docs.godotengine.org/en/stable/tutorials/xr/setting_up_xr.html
* https://decacis.github.io/godot_oculus_platform/
* https://godotvr.github.io/godot-xr-tools/
* Reference Space is set to Local (don't follow player height)

## TODO

* https://docs.godotengine.org/en/stable/tutorials/xr/xr_next_steps.html
* https://github.com/godotengine/godot-demo-projects/tree/master/viewport/gui_in_3d
* Multiplayer
  * https://forum.godotengine.org/t/multiplayer-xr-players-that-join-follow-player-ones-position/68593
  * Probably easiest thing is to move the character out of the XR piece entirely and have the XR piece just update the one character, which can then be sync'd across the network
