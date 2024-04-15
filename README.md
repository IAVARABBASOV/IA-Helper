# IA-Helper Unity Package

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Unity](https://img.shields.io/badge/Unity_Pack-IA-blue)](https://unity3d.com/get-unity/download)

## Overview

Helper Pack to speed things up.

Here are some solutions to the problem.

- Add Seperator
  ![Add Seperator](https://github.com/IAVARABBASOV/IA-Helper/assets/155958627/a0c6fba6-a171-46b7-b72b-a7a368a34248)

- Easy Scene Changer - Helps you switch between scenes fast.
  ![Uploading Easy Scene Loader.gif…]()

- Channel/Listener System - This system allows to create ScriptableObject Event and listener.
 Multiple listeners can listen to the same value and execute it for different tasks.

- For example: Let's have a Float Event Channel of the player's health, 
a Listener to see it as the UI changes, and another system to check if the health is 0 or not. At this point, 
we separate the UI and the life-checking system. But our value comes from the same place.
![Uploading Event Channel System.gif…]()

- Channel && Listener Class Generator - As the name suggests, it is a tool for creating Channel and Listener classes. 
This is so that we can create a Channel and listener at the same time according to any type. And make it easy to add them to the project.

- Json Scriptable Save System - This is the save system,
Converts Data -> to Json -> Encrypts it -> Creates a file named Game_Data in the Assets folder and writes it into it.
Reads game_data -> Decodes -> Loads values. (its working on Android/IOS/Windows, tested before)

- Utils - Utils includes many helpful, functions for speed up coding. 
Ex: myList.GetRandomItem() - it is give you random item from List or 
"Hello, World".Log(); it is increase your speed when write Log Message

- All codes is under the IA namespace, ex: IA.Utils


## Installation

### Unity Package Manager (UPM)

You can install this package through the Unity Package Manager. Follow these steps:

1. Open the Package Manager in Unity.
2. Add a new package by selecting "Add package from git URL..."
3. Enter the following URL: `https://github.com/IAVARABBASOV/IA-Helper.git`

## Usage

Examples and code snippets to demonstrate how to use IA-Helper package.

```csharp
// Example code
using IA.Utils;

// Short Codes
List<GameObject> goList = new List<GameObject>();
goList.GetRandomItem(); // it gives you random item from list


// Easy Coroutine
new WaitForSeconds(1f).EventRoutine(() =>
{
    "Do Something after 1 sec.".LogError(_color: Color.green, _context: this);
}).
StartCoroutine(this); // Also it can create/destroy temporary GameObject on scene for run Coroutine, 
//this mean it can also work on Editor Mode


// It makes button on Inspector, but function must be public
// void and IEnumerator functions you can call.
[IA.Attributes.InspectorButton("Do Something", 
    isIEnumerator: true, 
    enableDialogBox: true, 
    dialogTitle: "Title", 
    dialogMessage: "Do Something ?", 
    dialogOk: "Yes", dialogCancel: "No")]
public IEnumerator DoSomething()

// Check String is Null or Not
bool isMyStringNull = myString.IsNullOrWhiteSpace();

// Compare Strings
bool isMyStringOther = myString.AreThisStringEqualTo(otherString);

// Destroy All GameObjects in list
goList.DestroyList();

// Calculate Median Value
int[] ints = new int[] { 1, 2, 3, 4, 5 };
double medianValue = ints.CalculateMedian();

// Check default struct
Vector3 pos = default;
pos.IsNull();

// Return Random Vector3 in Range
pos.GetRandomValue(range: 1);

// Shuffle the Array or List
ints.Shuffle();

// Clear Empty List
goList.ClearEmptyItems(out goList);

```

License
This project is licensed under the MIT License - see the LICENSE file for details.

Acknowledgments
- Newtonsoft.Json 
- Cinemachine
- Addressables

```csharp

// Deserialize the JSON data into a dictionary
JObject dataObject = JObject.Parse(jsonString);

// Remove the "targetTier" property from the object, if it exists
JProperty targetKeywordProperty = dataObject.Property(keyword);

```


Contact
iavarabbasov@gmail.com

Changelog
See the Changelog for details on version history.


If you have code you'd like to add or a feature you'd like to improve, please add it. The reason I share this project publicly is very simple. I would appreciate it if you could make your own additions to make our work easier, to solve those little problems that we "don't have time" to solve. Game Developers Join and add or change your simplified suggestions.

