# Testing out Godot VR

## Requirements

* Godot 4.3
* https://docs.godotengine.org/en/stable/tutorials/export/exporting_for_android.html
  * OpenJDK (17+)
    * sudo apt install openjdk-17-jdk openjdk-17-jre
  * Android SDK (35)
  * Android NDK
  * CMake

## Running locally on Quest

* https://docs.godotengine.org/en/stable/tutorials/xr/deploying_to_android.html
* https://github.com/GodotVR/godot_openxr_vendors
* Have to enable Developer Mode through the Meta Quest app
  * Make sure to allow access to files in the headset (notification may be in the notifications tab, it goes away kinda quick)
* Have to set XR mode to OpenXR (defaults to Regular) and include the Meta OpenXR vendor plugin
* Deploy with Remote Debug is useful to get logs in the editor
* Small Deploy with Network Filesystem seems to crash the app when used
* Have to use test users until a first build can be uploaded (and this is a long process to kick off)
  * https://developers.meta.com/horizon/resources/test-users/

## Misc

* https://docs.godotengine.org/en/stable/tutorials/xr/setting_up_xr.html
* https://decacis.github.io/godot_oculus_platform/
* Reference Space is set to Local Floor

## TODO

* https://docs.godotengine.org/en/stable/tutorials/xr/a_better_xr_start_script.html
* https://docs.godotengine.org/en/stable/tutorials/xr/xr_next_steps.html
* https://github.com/godotengine/godot-demo-projects/tree/master/viewport/gui_in_3d
