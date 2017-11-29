# GearVR Simulator

This is derived from the Simulator, simplified and adapted to make it more "GearVR friendly".
This is not needed to develop on GearVR, but it implements an interaction similar to the GearVR,
useful for debugging.
It is designed to work with the `Touchpad Touch` set as button for interaction (e.g. grab, teleport, ...)

## Instructions for using the GearVR Simulator

 * Follow the initial [Getting Started](/GETTING_STARTED.md) steps and then add the `VRTK/Source/SDK/Simulator/[GearVRSimulator_CameraRig]` prefab as a child of the SDK Setup GameObject.
 * Look around:
   * move the mouse to look around (Mouse Movement Input set to `Always`) or
   * press the right mouse button (`Mouse 1`) to look around (Mouse Movement Input set to `Requires Button Press`).
 * Use the left mouse button `Mouse 0` to interact (mapped to `Touchpad Touch`).
 * Press `Backspace` to simulate GearVR "back" button.
 * Additional button mappings can be found on `SDK_InputGearVRSimulator` on the prefab.
 * All button mappings can be remapped using the inspector on the prefab.
 