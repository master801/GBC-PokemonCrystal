# GBC-PokemonCrystal

## Info
Crowd Control script for the GameBoy Color game "Pokemon Crystal"

## How to develop/use locally.
- Download the Crowd Control SDK from [here](https://forum.warp.world/t/how-to-setup-and-use-the-crowd-control-sdk/5121) and unzip it.
- Download Bizhawk from [here](http://tasvideos.org/BizHawk.html) and unzip it.
- Open the Bizhawk emulator
- Go to the `Config` tab at the top and select the option `Customize...`
- Click on the `Advanced` tab and under `Lua Core`, make sure `Lua+LuaInterface` is selected instead of `NLua+KopiLua`.
- Close the `Customize...` window
- Go to the `Tools` tab at the top again and click on `Lua Console`.
- In the folder where you unzipped the Crowd Control SDK (should be a folder called `SDK`) open the sub-directory `ConnectorScripts`.
- Click and drag the Lua file `connector.lua` from the `ConnectorScripts` folder, into BizHawk's Lua Console window.<br/>
The connector script should automatically be loaded without any errors. <i>(A warning about the emulator not running is nothing to worry about.)</i><br/>
<b>Note: Do not close the `Lua Console` window or else the connector script will <i>NOT</i> be loaded anymore.</b>
- Then in BizHawk again, load either a Vanilla (unmodified) or a randomized version.<br/>
<b>Note: Do not worry if the game does not automatically start. This is normal because the lua script is waiting for the SDK to be connected.</b>
- Open the Crowd Control SDK program (`CrowdControl.SDK.exe`), click the button `Load Pack Source` and select the `GBC-PokemonCrystal.cs` file.
- Click on `PokemonCrystal` in the list on the tab pane to the left and then click the button `Select Pack`.
- Then click the `Connect` button below the `Include Debug` checkbox.
