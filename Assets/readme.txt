3rd person controller with orbit camera and move, aim, fly modes

Version: 2.0

Description:

This package provides a basic setup for a third person player controller. Includes scripts for player movement, camera orbit and a Mecanim animator controller, containing basic locomotion - walk, run, sprint, strafe, and also an extra fly mode. The scripts also implements camera collision detection and aim mode. A basic demo scene is included.

Important:

If you are importing just the needed assets on an existing project, it is necessary to define Input Manager settings for the following custom keys:

Run (default: mouse 0)
Aim (default: mouse 1)
Sprint (default: left shift)
Fly (default: e)

The package inputSettings, included in this project, contains the custom keys settings listed above.

Usage:

Add the BasicBehaviour, MoveBehaviour, AimBehaviour and FlyBehaviour scripts to the player game object.
Drop the camera game object to the Player Camera parameter of the BasicBehaviour script, on the inspector.
Add the ThirdPersonOrbitCam script to the camera.
Drop the player reference to the Player parameter of the ThirdPersonOrbitCam script, on the inpector.
Add the CharacterController to the player animator controller.
Attach a capsule collider, and a rigidbody component to the player. 
Set up the collider measures properly to fit the player.
It mandatory that the player's mesh never surpass the bottom of its capsule collider, otherwise the movement script will not work properly and the player may not move.
An image for the crosshair in Crosshair parameter of the AimBehaviour script is also necessary to work properly.

Note that the ThirdPersonOrbitCam script is tied to the PlayerControl script. Modifications are necessary in this scripts to work independently.

Reference:

Author's page: www.dcc.ufmg.br/~allonman
