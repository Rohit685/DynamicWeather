# DynamicWeather

This mod gives GTA its own realistic weather system! Weathers transition as the day continues in the game giving _more variety in the sky_. Read below to see what this mod has to offer.

> [!NOTE] 
> This mod is open-source and includes APIs for developers.

# Features:
- Weather Sync to a real-life location (Requires API key and some setup to use (API is free, **DO NOT SHARE YOUR API KEY**))
- Custom realistic forecast
  - Temperature and weather conditions included
  - Predicted (in-game) time when the weather will occur
- On-screen UI
  - Forecast
  - Current Weather
    - For real-life weather sync and forecast
  - Ability to customize the position and scale of UI
- Customizable temperature for different weather conditions
  - _yes there is a way to switch to Celsius but the default is Fahrenheit cuz 'Murica!_

# APIs:
``` csharp
bool IsRealLifeWeatherSyncRunning();
void PauseWeatherSystem()
void ResumeWeatherSystem()
List<Weather> GetCurrentForecast(out int currentWeatherIndex) // returns an empty list if real-life weather sync is running
```

# Events:
``` csharp
OnWeatherChanged
```
 
# Installation:

Drag and drop the Plugins folder into the main game directory. Install the OIV file by dragging and dropping it into OpenIV. The OIV file is <ins>mandatory</ins>.

The time interval in the <ins>INI file</ins> is representative of in-game time. A conversion from in-game time to real-life time was kindly made by Krazy Manuel and is provided down below.

# IMPORTANT THINGS TO NOTE:
- The Severe Weather Event from ImmersiveAmbientEvents by [@Echooo](https://www.lcpdfr.com/profile/482664-echooo/) is not compatible yet with DynamicWeather. Please <ins>disable that event</ins> if you want to use DynamicWeather.
- You <ins>cannot have</ins> freeze time OR force weather enabled in any trainer. Otherwise, DynamicWeather will not transition between weather. 
- I do not log the location from the INI in the RagePluginHook.log. However, if you have trouble with the API working, I will need to know what the location is so please choose something more generic so any identifying information is not revealed while I am trying to debug.
- The time interval in the INI is measured by IN-GAME HOURS.
- I am not providing support. I’m not available enough to fix issues.

# Customization:
All textures can be customized. Just make sure the weather texture is optimized for 96x96 and the notification texture is optimized for 64x64.

# Console Commands:
- PauseForecast 
- ResumeForecast
- RegenerateForecast
- RefreshWeather
- SwitchToIRLWeather
- SwitchToForecast
- ReloadDynamicWeatherINISettings

# Credits to:
- [@Haze Studio](https://www.lcpdfr.com/profile/393830-haze-studio/) for the fire notification texture
- [@Scorpionfam](https://www.lcpdfr.com/profile/430253-scorpionfam/) for the fire thumbnail
- [@HeyPalu](https://www.lcpdfr.com/profile/533866-heypalu/) for the fire-on-screen textures
- [@BlueLine Vibes](https://www.lcpdfr.com/profile/250460-blueline-vibes/) for making [a showcase video](https://www.youtube.com/watch?v=FWaytHG6Jy0) of a beta (it is outdated but gives a good idea of what the mod does) and providing [his discord](https://discord.com/invite/cxVZwSC) as a platform to help beta test (Thank you all the beta testers in the discord!).
- [@khorio](https://www.lcpdfr.com/profile/99784-khorio/) for the coding help with time and API integration
- [@Echooo](https://www.lcpdfr.com/profile/482664-echooo/) for weather transition code
- Krazy Manuel for the conversion chart below

![A chart with conversion from in-game time to real-life time made by Krazy Manuel.](https://s3-attachments.int-cdn.lcpdfrusercontent.com/monthly_2024_08/image.png.3e05b22e4206e6a380fb920ce5f0fea4.png)
