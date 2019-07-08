# FmvMaker
Video data for the FmvMaker is within the LocalVideoFilePath folder (FmvMakerDemoData.json) and consists of a JSON list of single video elements, which are linked via their names. This means that there is no complex hierarchy. Yes, this leads to a long list of elements, but it also helps to keep things simple. Please always choose simple names for your video elements. There is a field for the displayed name available.

## How do I build a JSON list?
For those who are not familiar with JSON everything you need to know is, that there is a certain structure that has to be followed. Everything where a “VideoElement” belongs, is represented by …

```
[{...},{...},{...},{...},{...},{...},{...}]
```

So there is one pair of square brackets, which represents an array. Curly brackets represent an item of this array. Each item is a “VideoElement”. It´s simple right? Usually there is some formatting done, to keep the overview. Like this:
```
[{
        ...
    },{
        ...
    },{
        ...
    },{
        ...
    }
]
```
I´ll skip the brackets for the upcoming explanations. Please make sure, you don`t forget them in your configuration file!!!

## What does a “VideoElement” look like?
```javascript
"Name": "VideoFileNameWithoutExtension",
"NavigationTargets": [{
        "NextVideo": "FileNameOfTheNextVideoWithoutExtension"
    }
]
```
This is the minimum configuration for a single video elememt. It has a name (VideoFileNameWithoutExtension) and so called “NavigationTargets”, which are videos the player wants to load from his current state. Both are separated with a ,

Like in a Point and Click adventure, where the player can “navigate” from one screen to another (e.g. when clicking a door), the player can decide which video he wants to load next. It´s possible to have multiple “NavigationTargets” with the same array structure, that was already mentioned at the beginning, but the replacement is no nnn.
```
"NavigationTargets": [{
        nnn
    },{
        nnn
    },{
        nnn
    },{
        nnn
    }
]
```
## What does a “NavigationTarget” look like?
```
"NextVideo": "FileNameOfTheNextVideoWithoutExtension"
```
The minimum configuration is at least one target (otherwise there will be no navigation possible). The “NextVideo” is the filename of the designated “VideoElement” name.

So in the next step you only have to replace the … with the previous video element content, the nnn with “NavigationTargets” and your data will look something like this (working example with all brackets included):
```javascript
[{
        "Name": "VideoFileNameWithoutExtension",
        "NavigationTargets": [{
                "NextVideo": "FileNameOfTheNextVideoWithoutExtension"
            }
        ]
    },{
        "Name": "FileNameOfTheNextVideoWithoutExtension",
        "NavigationTargets": [{
                "NextVideo": "VideoFileNameWithoutExtension"
            }
        ]
    }
]
```
## Enough explanation, can't we start already?
YES, we can. With the example from the previous section we already created a simple prototype, which will allow us to navigate from “VideoFileNameWithoutExtension” to “FileNameOfTheNextVideoWithoutExtension” and back. Just copy the designated .mp4 video files into the LocalVideoFilePath and replace the “VideoFileNameWithoutExtension” to “FileNameOfTheNextVideoWithoutExtension” with their video file names (without extension).

There are a few more properties can be used to create “VideoElements” and their “NavigationTargets”.
Like this example of a more detailed “VideoElement”:
```javascript
"Name": "VideoFileNameWithoutExt",
"IsLooping": false,
"NavigationTargets": [{
        "NextVideo": "FileNameOfTheNextVideoWithoutExt",
        "DisplayText": "To the left",
        "RelativeScreenPosition": {
            "x": 0.2,
            "y": 0.5
        }
    }
]
```
- The “IsLooping” property can be used to create a looped video state, when set to true. This gives the player a better idea of the scenes by showing various impressions. E.g. wind movement, lively places, traffic, etc… Pls make sure to use loopable videos when using this property.
- “DisplayText” does what it name says. It shows a set text for a “NavigationTarget”.
- As you may have noticed already, when starting the simple example from above, the buttons for “NavigationTargets” are always located at the center of the screen. This is where “RelativeScreenPosition” comes to the rescue. This Vector2 property lets you freely set the screen position relative to the lower left corner. Please be sure to only use values between 0 and 1.

Expanding the simple example from before, we can now create e.g. a movement between two rooms. Room B has a loopable background video. The navigation buttons are either on the left or right side (working example with all brackets included).
```javascript
[{
        "Name": "RoomA",
        "IsLooping": false,
        "NavigationTargets": [{
                "NextVideo": "RoomB",
                "DisplayText": "To the left",
                "RelativeScreenPosition": {
                    "x": 0.2,
                    "y": 0.5
                }
            }
        ]
    },{
        "Name": "RoomB",
        "IsLooping": true,
        "NavigationTargets": [{
                "NextVideo": "RoomA",
                "DisplayText": "To the right",
                "RelativeScreenPosition": {
                    "x": 0.8,
                    "y": 0.5
                }
            }
        ]
    }
]
```
A more sophisticated example is shipped with the demo project.
