# STOMT Unity-SDK [![STOMT API](https://img.shields.io/badge/stomt-v2.4.X-brightgreen.svg)](https://rest.stomt.com/)

<img alt="STOMT unity feedback integration" src="https://cdn.stomt.com/uploads/Dh1x/origin/Dh1xzkpSoHXH2UGuh3rNX35WR4DSjiqq4TLeu9Ag_origin.gif" />

This Widget allows the easy integration of the feedback solution [www.stomt.com](https://www.stomt.com/) in your Unity apps and games. Of course you can find the STOMT feedback widget for Unity3D in the [Unity Asset Store](https://www.assetstore.unity3d.com/en/#!/content/64669).

## Installation

1. Register as normal user on [www.stomt.com](https://www.stomt.com/) 
2. [Create a target page on STOMT](https://www.stomt.com/createTarget).
3. Go to [Settings > My Apps](https://www.stomt.com/dev/my-apps) and create an application.
4. Download this repository and [or get it from the AssetStore](https://www.assetstore.unity3d.com/en/#!/content/64669) and copy the assets into your project.
5. Add the ```StomtPopup``` prefab to your main UI canvas.
6. Enter all necessary data into the ```StomtAPI``` component on the prefab.     
6.1 Enter the ```Rest Server Url```: rest.stomt.com (Or use our Sandbox for testing - See below)     
6.2 Enter the ```AppId``` you obtained in the third step     
6.3 Enter your ```page username (as Target Id)``` (you find it in your profile-url. E.g. stomt.com/stomt => stomt)     
7. Finished! *Regularly communicate your page on social channels and checkout our [Website-Widget](https://www.stomt.com/dev/js-sdk) for your websites to collect feedback from anywhere.*     

<img alt="Configure STOMT Unity plugin" src="http://schukies.io/images/stomt/config.gif" />

## Usage

The Widget can be enabled by using a toggle key or calling the API Methods.

StomtPopup Class
* Enable:	ShowWidget()
* Disable:	HideWidget()

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

We would love to see you contributing to this project. Feel free to fork it and send in your pull requests! Visit the [project on STOMT](https://www.stomt.com/stomt-unity) to support with your ideas, wishes and feedback.

## Authors

[Daniel Schukies](https://github.com/daniel-schukies) | [Follow Daniel Schukies on STOMT](https://www.stomt.com/danielschukies)    
[Patrick Mours](https://github.com/crosire) | [Follow Patrick Mours on STOMT](https://www.stomt.com/crosire)

## More about STOMT

* On the web [www.stomt.com](https://www.stomt.com)
* [STOMT for iOS](http://stomt.co/ios)
* [STOMT for Android](http://stomt.co/android)
