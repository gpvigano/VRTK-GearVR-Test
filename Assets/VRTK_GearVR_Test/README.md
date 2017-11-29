# GearVR Test

Some examples from VRTK were modified to work with GearVR, included here in `VRTK_GearVR_Test\Examples`.
A custom "GearVR-friendly" simulator is included in a proper SDK, see `Source/SDK/GearVRSimulator/README.md` for details.
Everything here works with the GearVR D-Pad only, it is designed just to enable interaction without a gamepad.

## Instructions for building on GearVR

Set up libraries:
* Install OculusUtilities.unitypackage
* Delete OVR/Plugins folder
* Install VRTK version 3.3.0 (branch release/3.3.0-alpha)

Set up Build for GearVR:
* In Build settings set `Android` as platform
* set `Player Settings`/`Other Settings`:
	- `Color Space` = `Gamma`
	- `GPU Skinning` = unchecked
	- `Virtual Reality Supported` = checked
	-`Virtual Reality SDKs` = add `Oculus`
	- `Stereo Rendering Method` = `Multi Pass`
	- `Minimum API Level` = `API Level 22`

Set up the scene:
* load one of the provided examples
* test the example in Unity Editor (the GearVR Simulator should be automatically enabled)

In Build Settings remove all the scenes (or add only the current one)
  
==> Build and Run...

** Note that some examples originally could not work, because D-Pad is not mapped to touch axis (e.g. for the `015_Controller_TouchPadAxisControl` example, after the applying the changes suggested by this video [<https://www.youtube.com/watch?v=ma2AetALN_k>] a new example `015_Controller_TouchPadAxisControl_GearVR` was provided with a modified script `RC_Car_Controller_DPad` attached to `RightController`) **

## Instructions for making VRTK examples work on GearVR


Set up libraries and build settings as above.

Set up scene:
* load an example
* in the scene expand `[VRTK_SDKManager]`/`SDKSetups` and delete all the attached game objects except `OculusVR`
* select `OculusVR` and in Inspector set `SDK Selection`/`Quick select` to `GearVR (Android:Oculus)`
* select `[VRTK_SDKManager]` and press `Auto Populate` in Inspector
* delete `[VRTK_Scripts]`/`LeftController` (or the controller you won't use)
* select `[VRTK_SDKManager]` and set `Left Controller` to `None`
* select `[VRTK_SDKManager]`/`SDKSetups`/`OculusVR`/`OVRCameraRig` and set `Transform`/`Position`/`Y` to a desired height (e.g. 1.5, 1.6, ... depending on the desired camera height)
* delete `[VRTK_SDKManager]`/`SDKSetups`/`OculusVR`/`LocalAvatar` (if you have not Oculus drivers installed)
* to enable gaze-based interaction choose one of these two ways (****):
  - change the existing configuration:
    - add a new game object named `HeadsetFollower` to `[VRTK_Scripts]`
	- add a `VRTK_SDKObjectAlias` script to `HeadsetFollower` and set `Sdk Object` to `Headset`
    - rename the main controller (e.g. `RightController`) to `Headset`
	- add a `VRTK_TransformFollow` script to it and set `Game Object To Follow` to `HeadsetFollower` and `Game Object To Change` to `Headset`
	- change settings in interaction scripts, e.g. interaction button set in `VRTK_InteractGrab` to `Touchpad Touch`, or even `Button Two Press` for using back button press for a secondary interaction in `VRTK_InteractUse` (warning: holding down the back button opens the system menu)
  - replace the existing `[VRTK_Scripts]` with the provided prefab
    - add the prefab in `VRTK_GearVR_Test\Assets\VRTK_GearVR_Test\Prefabs\[VRTK_Scripts_GearVR]` to the scene
	- change pointer and interaction settings (e.g. Pointer settings in `Headset` or Policy List in `PlayArea`)
    - remove unneeded components
	- delete the original `[VRTK_Scripts]` object
* enable GearVR Simulator (optional):
  * add to `[VRTK_SDKManager]`/`SDKSetups` a new game object `GearVRSimulator`
  * add a `VRTK_SDKSetup` script to it and set `SDK Selection`/`Quick select` to `GearVRSimulator (Standalone)`
  * add the `[GearVRSimulator_CameraRig]` prefab to the `GearVRSimulator` object
  * select `[VRTK_SDKManager]` and press `Remove All Symbols` in Inspector
  * select `[VRTK_SDKManager]` and press `Auto Populate` in Inspector
  * select `GearVRSimulator` and press `Populate Now` or check `Auto Populate`
