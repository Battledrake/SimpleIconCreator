<b> Simple Icon Creator </b>

<br> Video showcasing the tools use. <br>

https://youtu.be/ZXYyX80F6CU

![SimpleIconCreator](https://github.com/Battledrake/SimpleIconCreator/assets/37988801/c9402bf2-90de-48d0-a5e3-296db859ea11)

An easy to use icon creator for Unity.

<b>Steps:</b>
<br>-Download the files or clone them to your computer.
<br>-Add project files to existing project.
<br>Ready to go!

To use, you can click the BattleDrakeCreations dropdown and simpleiconcreator.

<b>Creator Options:</b>
<br>-Use any asset gameobject. Don't need to enter play mode to use.
<br>-Change material for the icon.
<br>-Change light settings of preview window.
<br>-Make the background transparent (You'll see the window go magenta. This is the trick to making the icon transparent, as it converts all magenta pixels into 0 alpha ones. If your object has a magenta color in it, you can change the transparency color used right from the OnEnable in the IconCreatorWindow.cs).
<br>-Background and foreground textures. (Unity's URP/HDRP broke background textures. An issue that has been noted and Unity has acknowledged it as something they might re-allow in the future. Until then, if you want background textures, use Built-In)
<br>-Save the icon in the resolution of your choice. Supports 32x32 up to 4096x4096. If you want an IconResolution that's not squared, you can go to the CreatePngFromTexture() method and just set the resolution in the ResizeTexture function.
  <br>-Saving using an existing filename will automatically overwrite it.
<br>-Save preview settings to reuse for rapid-fire icon creation. Preview settings will save the gameobject position/orientation/zoom and all the other visible settings.
  <br>-Preview settings will save new or overwrite an existing if they share the same filename and location.

  <b>Window Controls:</b>
  <br>Left-click drag horizontally for rotation on the y-axis(Yaw). Vertically for the x-axis(Pitch).
  <br>Right-click drag horizontally for rotation on the z-axis(Roll).
  <br>Middle-mouse down to drag the gameobject around the window.
  <br>Scroll-wheel to zoom in and out.
