# stomt Unity-SDK [![Stomt API](https://img.shields.io/badge/stomt-v2.4.X-brightgreen.svg)](https://rest.stomt.com/)

<img alt="stomt unity feedback integration" src="https://cdn.stomt.com/uploads/Dh1x/origin/Dh1xzkpSoHXH2UGuh3rNX35WR4DSjiqq4TLeu9Ag_origin.gif" />

This SDK allows the easy integration of the feedback solution [www.stomt.com](https://www.stomt.com/) in your Unity apps and games.

## Installation

1. Register on [www.stomt.com](https://www.stomt.com/) 
2. [Create a target page on stomt](https://www.stomt.com/createTarget).
3. Go to [Settings > My Apps](https://www.stomt.com/dev/my-apps) and create an application.
4. Download this repository and copy the assets into your project.
5. Add the ```StomtPopup``` prefab to your main UI canvas.
6. Enter the AppId you obtained in the second step and your target username into the ```StomtAPI``` component on the prefab.

<img alt="Configure stomt Unity plugin" src="http://schukies.io/images/stomt/config.gif" />

## Use our Sandbox!
If you want to test the integration please feel free to do what you want on [test.stomt.com](https://test.stomt.com/) 

* Just go through the installation steps again on [test.stomt.com](https://test.stomt.com/)
* Enter "https://test.rest.stomt.com" as Rest Server URL in the widget.

## Common Issues

* Error (401) Unauthorized: Is your application ID right? ```test.stomt.com``` and ```stomt.com``` use different ID's.
* Error (500) Internal Server Error: [Report] (https://www.stomt.com/dev/unity-sdk) us the problem.
* Target Name doesn't fit: you can easily adjust the width. 

<img alt="Adjust target bubble" src="http://schukies.io/images/stomt/targetname.gif" />

## Contribution

We would love to see you contributing to this project. Feel free to fork it and send in your pull requests! Visit the [project on stomt](https://www.stomt.com/stomt-unity) to support with your ideas, wishes and feedback.

## Authors

[Daniel Schukies](https://github.com/daniel-schukies) | [Follow me on stomt](https://www.stomt.com/danielschukies)    
[Patrick Mours](https://github.com/crosire) | [Follow me on stomt](https://www.stomt.com/crosire)

## More about stomt

* On the web [www.stomt.com](https://www.stomt.com)
* [stomt for iOS](http://stomt.co/ios)
* [stomt for Android](http://stomt.co/android)
