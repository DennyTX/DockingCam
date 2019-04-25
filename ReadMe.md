This mod adds a camera to all docking ports using a ModuleManager patch, as well as a part which can act as a mobile camera.

It looks ahead by the vertical axis of the vessel's and displays a minimum of necessary info for the docking maneuver 
- namely, range, speed, angle, alignment and when the correct trajectory occurred 
- time prior to docking with the mark that such trajectory will lead our vessel to dock without additional actions (small lamp will become green and will show time to docking). 
The camera window has three size presets. 

It also has three viewing modes:
	Color
	Black and white
	Infrared

Available options/abilities include:
	A powered zoom function, using a slider at the top of the window.
	A button to remove flight data from the screen.
	A button to add television interference 

The range of the camera parameters of night vision (RSMA) and presence of noise can be configured through CFG. 
The camera requires a target to operate.

The included external camera can show videostream only, has three modes of vision, and is able to rotate on two axes. 
For cameras installed on the side or under the belly of the aircraft (which gets inverted 
upside down) there is the mode of rotation of the image.  The camera can now be rotated in 90 degree increments.

In the CFG of the partcamera some meshes can be configured for use with other camera models.

All cameras have a title with its number
Multiple cameras can be working at the same time. 
Docking camera reports its target on its window title.

There is an experiment on the part camera:
A try to reproduce a surveillance (spy) activity. You need to be at a distance less than 1000 m near any targetable thingie, catch it on camera's screen and targeted. Then press the "⦿" button. 
A ray will be shot. If all requirements have met and there aren’t any obstacles on the ray's path, you'll sget experiment results  If something was wrong, then you just spent one bullet.  Experiments is limited by bullets (4 yellow balls around cam, disappearing one by one each time). 

Camera has 3 presets of shaders:
	noisy TV 1960th style
	TV 1980th style
	Standard:
		color, 
		b/w
		nightvision
Available via button. 

Also, using the toolbar button you can find info about nearby cameras installed on other vessels within transmission range.  If a camera has been activated on another vessel then a new window from the distant camera will appear when distance from first one will become less than 2500 (or 10k (customizable)). 

There is a "look at me" mode by activating it on distant camera. 
"Targetcam"  and "follow me" modes on your active vessel (set camera position using scrollers).

The cameras can transmit the videostream to nearby vessels, but only at a distance of up to 2,500 or 10,000 meters, configurable via the toolbar window
