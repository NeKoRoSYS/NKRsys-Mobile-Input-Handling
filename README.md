# (NKRsys-MIH) NeKoRoSYS's Mobile Input Handling
An open-source project dedicated to providing mobile game developers a quick headstart on setting up mobile/touch controls for their Unity game.

<br>

## Before You Use
Before you use this project as a base for your mobile game, I am putting this here to remind you that this project is currently being actively developed and things are subject to change. Kindly star the repo before using, forking or cloning, please!
### Prerequisites
  - [The Input System Package](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.6/manual/index.html)
    - *To install: Editor > Window > Package Manager > Packages: Unity Registry > Input System*
### Why use the Input System package?
  - This asset also provide scripts that support Unity's Legacy Input System. However, using the new Input System package gives you more advantages than the legacy input system. It has more flexible features and settings that everyone can configure, and it helps make implementing cross-platform play a lot more do-able. This asset utilizes the `On-Screen Control` class provided by the Input System package which lets you configure the button/joystick bindings according to your liking. When combined with events subscription, you don't even have to reference the control scripts just to check if it's "pressed" just to perform an action, and it also gets checked even when not called in the `Update()` method which furthermore improves the game loop's performance.
      
<br>

| Table of Contents                     |
| ------------------------------------- |
| [Features](#features)                 |
| [Usage](#usage)                       |
| [Sponsorship](#sponsorship)           |
| [Contribution](#contribution)         |

<br>

## Features
This project extends from the [on-screen controls](https://docs.unity3d.com/Packages/com.unity.inputsystem@0.9/manual/OnScreen.html) that Unity's Input System package have provided by default. The following demos are using [(NKRsys-FPP-CC) NeKoRoSYS's First Person Perspective Character Controller](https://github.com/NeKoRoSYS/NKRsys-FPP-CC/blob/main) to showcase the mobile controls.

- ### Custom Button Implementation
  - [`ControlButton`](https://github.com/NeKoRoSYS/NKRsys-MIH/blob/main/Scripts/ControlButton.cs) is an extension of the [`On-Screen Button`](https://docs.unity3d.com/Packages/com.unity.inputsystem@0.9/manual/OnScreen.html#on-screen-buttons) component provided by the Input System package. It can have animated sprites and color fading!

<p align="center">
  <img src="https://github.com/NeKoRoSYS/NKRsys-MIH/blob/main/ControlButtonExample.gif" alt="Control Button Demo" />
</p>

- ### Custom Joystick Implementation
  - [`ControlStick`](https://github.com/NeKoRoSYS/NKRsys-MIH/blob/main/Scripts/ControlStick.cs) is a heavily-improved version of the [`On-Screen Stick`](https://docs.unity3d.com/Packages/com.unity.inputsystem@0.9/manual/OnScreen.html#on-screen-sticks) component provided by the Input System package. It has a variety of configurable settings, and it can be used for several features that your game may already have, like player movement.

<p align="center">
  <img src="https://github.com/NeKoRoSYS/NKRsys-MIH/blob/main/ControlStickExample.gif" alt="Control Stick Demo" />
</p>
    
- ### Custom Touchpad Implementation
  - [`ControlPad`](https://github.com/NeKoRoSYS/NKRsys-MIH/blob/main/Scripts/ControlPad.cs) is a component that registers input if the touch made contact within its rect bounds. This is a totally new feature, as the Input System does not provide their own rendition of what could be a touchpad. It supports multitouch and can be configured and used for a variety of features you may plan on implementing to your project!

<p align="center">
  <img src="https://github.com/NeKoRoSYS/NKRsys-MIH/blob/main/ControlPadExample.gif" alt="Control Pad Demo" />
</p>

<br>

## (New Input System) Usage
Prefabs are cool and all, but I recommend you to set up the controls from scratch like a boss!
- ### [`ControlButton`](https://github.com/NeKoRoSYS/NKRsys-MIH/blob/main/Scripts/ControlButton.cs)
  1. Create a 2D GameObject inside the canvas.
  2. Add an `Image` component and change the sprite if applicable.
  3. Add the  `ControlButton` script then customize it if necessary.
  4. Set the `Control Path` binding.
  5. Press, release, and enjoy!
- ### [`ControlStick`](https://github.com/NeKoRoSYS/NKRsys-MIH/blob/main/Scripts/ControlStick.cs)
  1. Create a parent 2D GameObject inside the canvas, make sure the rect bounds are big enough to cover a portion of the screen. (I recommend covering the entire half!)
  2. Add an `Image` component to the parent 2D GameObject and make the colors transparent.
  3. Add the  `ControlStick` script then configure it if necessary.
  4. Set the `Control Path` binding.
  5. Create two children 2D GameObjects and add an `Image` component to both of them. Rename the first child to "State," then rename the second child to "Handle" to avoid confusion.

  "State" is the background of the joystick, and one can code it so it changes visuals depending on the state of what's being controlled by the joystick (eg. make it color red if active, make it scale bigger or smaller depending on how far the joystick is pushed, etc). On the other hand, "Handle" is what you drag to move the joystick. I have provided sample sprites for you to use if you don't have any!

  6. Assign the parent 2D GameObject to the `inputArea` variable in the `ControlStick` script.
  7. Assign "State" to the `stickBounds` variable.
  8. Assign "Handle" to the `stickHandle` variable.
  9. Hold, drag, and enjoy!!
- ### [`ControlPad`](https://github.com/NeKoRoSYS/NKRsys-MIH/blob/main/Scripts/ControlPad.cs)
  1. Create an empty 2D GameObject inside the canvas and set its rect bounds so that it covers the entire right half of the screen.
  2. Add an `Image` component and set the colors to transparent.
  3. Add the  `ControlPad` script.
  4. Subscribe to the `OnTouchDrag` UnityEvent using another script, which would most likely be your camera controller.
  ```cs
  ControlPad controlPad;
  void Start() => controlPad.OnTouchDrag.AddListener(Something());
  ```
  5. Press, drag, look/pan around, and enjoy!!!
  
<br>

## Sponsorship
I am an aspiring software and game developer that currently do stuff solo, and I need funding to motivate me to do a lot better on my tasks so that I could deliver way better content. Donating is not a must, but it will be immensely cherished and appreciated!

<br>

## Contribution
Something's wrong with the code or you know better workarounds and alternatives? You can either make an issue or a pull request. It will be very much appreciated!
