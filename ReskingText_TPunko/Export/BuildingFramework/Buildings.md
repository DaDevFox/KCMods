BuildingFramework Model Reskinning Guide | Kingdoms and Castles Modding
=
Buildings in Kingdoms and Castles don't have a standard format that can be used to generically reskin any building in the game. Becuase of this, every building has different models and specifications to be built to. This makes reskinning buildings in KC more difficult than it needs to be, because of this, We've brought a framework for people with ranging levels of skill in either coding or art, so that someone less experienced in either shouldn't have to be deterred from reskinning anything!

![Image](https://media.discordapp.net/attachments/294162953337307138/704799796325515315/Sky_Over_View.png "Amazing Models by TPunko")
*This wonderful scene was made by TPunko from the Kingdoms and Castles Discord*

How the Framework works
=
Every building in the game has different specifications, so in order to create a model for any, you first have to do research and find out what kinds of models it uses, how many models it uses, and how to change those models, which is a different process for each building. 
This is what the Framework takes care of for you, we've already gone ahead and done the research and the coding so you can simply put in your models with little effort. 
To reskin a building through the framework, first a model, or multiple models, depending on the buliding, must be made to be used in the reskinning, those models will then be processed by the framework and injected into the building, doing its best to preserve its tree structure so animations (yes, some buildings have animations), funcitonality, and certain visual elements can stay unbroken. 

How to use the **Building Models Information** section
-
The **Building Models Information** section is organized into entries of information which usually look like this, sometimes with a little bit of extra information. 
```
-- BuildingName --
Name: ...
UniqueName: ...
Models: ...
```
Every entry in the Building Models Information corresponds to a building in Kingdoms and Castles.
Entries usually contain multiple elements, which can include:
```
[Not Supported]: If this is present, it means the building is not supported by the Framework 
Name: The friendly name shown in the build menu, what you likely know it by *
UniqueName: The name of the building used in many places in code *
Models: A list of the different models the building incorporates and their types and descriptions *
[Dynamic Stacks]: If this is present, it means Resource Stacks can be added or removed from the building
Stacks: A list of all the Stacks a building uses
ResourceStacks: A list of all the ResourceStacks a building uses
```
*Items with a * are always present*


Step by Step Guide
=
1 | Model
=
The first step is, somewhat obviously, to create a model to use. 

To start, pick a building to begin modelling, and look it up in the **Building Models Information** section. Once you've found it, look at the all corresponding information below its name, most of it is important and could affect how you need to make your art, so you should know what it means and how it will affect you.  

Know what to model
-

The first piece of information you need to look at is the *Models* section, this will tell you what and how to model for a reskin of your chosen building, below is a highlight of all the important information you'll need to understand while reading the **Building Models Information** section. 









*Models*: The different 3D Models the building uses, these can be *Instance* models or *Modular* models, you will have to model differently according to the type
            **Instance Models**:
            An *Instance* model is one that will be instanced inside of the original building, meaning that model, and all of its children will be *added* to the building, so you can add extra things to the model, like lights or doors, and even add code to make it move or do something during game. With this type of model, all you need to make sure to do is make sure that your scaling and translation is correct, and that it's in a GameObject form!
			**Modular Models**: 
			A *Modular* model isn't the same as an *Instance* model, certain buildings interchange their models in a different way, in which they use *only* the mesh data involved with the base GameObject, excluding it's children, and any transformations applied in the Unity Editor, including scale and rotation. This means that if you have a model that was rotated 90 degrees before being imported into Unity, and then you corrected it in the editor, it will disregard this, so all scaling and rotation must be applied *before* being imported into Unity. It will also disregard the children of your model, so this version cannot have arbitrary elements added on to it. 
			These types of models are usually used on modular pieces, hence the name, such as Castle Block variations or Roads. 
		  **Information Format**:
		  	(model name) | (model type, either *Instance* or *Modular*): (model description)

To Make a Model
----------------
First, as said above already, look up your building in the **Building Models Information** section of this guide and gather your information for how and what to model. 

Once that is done, make the model in your chosen art program! I reccomend using Blender because it's free, although I'm not much of an artist, so you can use whatever, but make sure that it can export files in a format supported by unity: [here](https://docs.unity3d.com/Manual/3D-formats.html)'s a list of all those formats

After it is modelled, import it into Unity, I reccomend importing it into the Kingdoms and Castles Toolkit project so you can see how it looks in the game world and next to other buildings. 

> If you don't have the Unity Editor or the Kingdoms and Castle Toolkit, there's a guide to download both in the Modding Tutorial [here](https://modtutorial.kingdomsandcastles.com/), as well as a more detailed desription on how to import models into Unity

Once it's imported, you have to convert it into a Prefab, the mesh alone won't work, in order to do this, find your model in the Project window, and drag it into the SampleScene if your using the toolkit, or your scene of choice if you're not. You can drag it into either the Hierarchy window, or into the Scene View. Once it's in, find it in the Hierarchy, and once you do, drag it back into the Project window. 

> If using a newer version of Unity, you might be faced with dialog saying 
    `Would you like to make new original prefab or a prefab variant?`,
    create a new original 

![Image](https://i.ibb.co/g9WDkMG/Unity-2018-4-20f1-Personal-PREVIEW-PACKAGES-IN-USE-Sample-Scene-unity-Kingdoms-and-Castles-Toolkit-master-PC-Mac-Linux-Standalone-DX11-5-8-2020-1-08-53-PM.png "Unity Editor Diagram")

> This might seem useless at first, but how Unity works is that when you drag a mesh (file in your original format, .fbx, .obj, or whatever you got after modelling it) into the scene view, it automatically converts that into a GameObject, but that GameObject doesn't exist anywhere other than in that specific scene, when you drag it back into the project, it gives the GameObject a file that can be shared, opened, modified, and duplicated, this is known as a Prefab, and is what the BuildingFramework uses to instantiate your meshes.  

Now you should have 2 files, your original model, with your original file extension, I'll say .fbx for the sake of this example, and your new Prefab version, which should have the extension .prefab

If you're using a more recent version of Unity, you should be able to double-click the prefab and open it in the relatively new **Prefab Editor**, if not, then you can just drag it into the Hierarchy ***again*** (also make sure to delete the old GameObject in the Hierarchy, we don't need it anymore)

![Image](https://i.ibb.co/59b7sqJ/Unity-2018-4-20f1-Personal-PREVIEW-PACKAGES-IN-USE-Sample-Scene-unity-Kingdoms-and-Castles-Toolkit-master-PC-Mac-Linux-Standalone-DX11-5-8-2020-3-02-58-PM.png "new Prefab View")
*The Prefab Editor in newer versions of Unity*

 2 | Additional Setup (optional)
=
Lots of buildings have more than just a base model, the Market, for example has a row of ham that is hung outside the shop, and all of the houses have chimneys that emit smoke. 

![Image](https://i.ibb.co/SXXQcSV/KC-Small-Markets.png "Small Market with fish, apples, wheat and pork all arranged nicely on stands")

These effects might make sense with the original models, but they might be misplaced, or just not neccessary with the newer models, for example, in a model for a house where the chimney no longer exists, smoke would continues to spew out where the chimney used to be, or with a reskin for the market, maybe the stacks aren't on the new counter, rather where it used to be. 

The BuildingFramework allows you to change these things optionally, but note that these things do require more code than just simply changing or adding a model. 

> Important: Only ResourceStacks and Stacks are supported at present time

**Changing Resource Stacks**
-

**Creating a Resource Stack**

> Note: ResourceStack and Stack are **not** the same thing, more on that later

ResourceStacks/Stacks have two types, *Normal*, or *Instanced*. 

*Normal* stacks are simple resource stacks that stack resources in a specified order, usually stacking upward. These are the kind that form in stockpiles. 
*Instanced* stacks are a bit more complex, in an Instanced stack, instead of stacking each resource on top of eachother with predefined rules, it actually has a formation that it fills, an example of this would be the ham on a string outside of the market, or the fish in a Fishmonger. 
**Tree Structure**
> **Important**: The only way to use any model or prefab in a mod, as will be explained later, is to package it into an AssetBundle. AssetBundles can hold Meshes, GameObjects, Prefabs, Materials, anything that Unity should recognize, except scripts. 
This means that any components you add to your GameObject will have to be added through **code**. 

> Know that it is possible to make a new look for an existing resource, which is how the Baker transforms wheat into bread, it doesn't actually transform anything by the game's definition, it only produces 2 wheat for every 1 wheat and 1 charcoal that it recieves, but the wheat that it sends has a different look, and thus is designed to look like bread, but it's actually wheat. 


Depending on the type of stack, the tree structure will be different. 

A *Normal* stack will consist of a single GameObject with a ResourceStack or Stack component (see below for more information) and a ResourcePrefab component. 
You will also need to design a Resource prefab, which will be the resource shown in the ResourceStack/Stack. 
* The Stack/ResourceStack component will create and format the resource stack, and will make the resource look like it's designated ResourcePrefab. 
* The ResourcePrefab component will need a reference to a prefab of a resource and will need a FreeResourceType, which can be wheat, apples, or whatever you wish. 

An *Instanced* stack is more complicated, there's really only 1 new requirement, that it has a child somewhere, be it a child of a child of a child, or just a direct child, that contains X number of children GameObjects, each being a single resource in the stack, where X is the maximum amount of resource in the stack, you can see this at play in the market's prefab, specifically for the Ham stack:

![Image](https://i.ibb.co/gm8JFmF/Unity-2018-4-20f1-Personal-PREVIEW-PACKAGES-IN-USE-Sample-Scene-unity-Kingdoms-and-Castles-Toolkit-master-PC-Mac-Linux-Standalone-DX11-5-8-2020-3-30-30-PM.png "Market Prefab")

For the rest, follow the same procedure as a *Normal* stack

This piece will be revisited in the code, due to the fact that the components will have to be added via code and not through the editor. 

**ResourceStacks vs Stacks**
Here, it's important to know the distinction between Stacks and ResourceStacks. 
A *Stack* is a purely graphical stack of resources, like the stacks of wood you see form in stockpiles, but it doesn't have any actual functionality. A *ResourceStack* is a Stack that also has functionality and will actually be used as storage when in game. Usually, changing a ResourceStack's values will actually affect gameplay, as well as making things look different, so be careful to make sure you don't change any of the default values

**Changing existing Resource Stacks**

This piece will maingly be covered in the **code** section, but this is how to identify which buildings have a ResourceStack vs. a Stack and what kind in the **Building Model Information** section.  

Usually it will be done like this:
```
-- my building --
Name: my building
UniqueName: mybuilding
Models:
    .....
[Dynamic Stacks] (if this is present, it means stacks can be added or removed, otherwise, you can only replace existing stacks)
ResourceStacks:
    ....
    here all the ResourceStack will be listed
Stacks:
    ....
    here all the Stacks will be listed
```
**Format**:
(stack name) | (stack type): (stack description) | (stack default value and resource type)








Code
=








Building Models Information
===========================

 -- Castle --
--------------

 -- Keep --
Name: Keep
UniqueName: keep
Models: 
	keepUpgrade1 | Instance: The base upgrade for the keep
	keepUpgrade2 | Instance: The second upgrade for the keep
	keepUpgrade3 | Instance: The third upgrade for the keep
	keepUpgrade4 | Instance: The final upgrade for the keep
ResourceStacks:
	foodStack:  Normal    | A ResourceStack that contains wheat  | Wheat: Unknown
	appleStack: Normal    | A ResourceStack that contains apples | Apples: Unknown
	stoneStack: Instanced | A ResourceStack that contains stone  | 35
	woodStack:  Instanced | A ResourceStack that contains wood   | Wood: 35

-- Wooden Castle Block --
Name: Wood Castle Block
UniqueName: woodcastleblock
Models: 
	doorPrefab | Instance: The door that appears on a castleblock when it connects to other castleblocks
	------------
	Open       | Modular: The flat piece without crenelations for a castle block
	Closed     | Modular: The piece of a castleblock with all crenelations at the top and no connections
	Threeside  | Modular: The piece of a castleblock with crenelations on 3 sides
	Opposite   | Modular: The straight piece of a castle block
	Adjacent   | Modular: The corner piece for a castle block
	Single     | Modular: The piece of a castleblock that only has crenelations on one side

-- Stone Castle Block --
Name: Castle Block
UniqueName: castleblock
Models: 
	doorPrefab | Instance: The door that appears on a castleblock when it connects to other castleblocks
	------------
	Open       | Modular: The flat piece without crenelations for a castle block
	Closed     | Modular: The piece of a castleblock with all crenelations at the top and no connections
	Threeside  | Modular: The piece of a castleblock with crenelations on 3 sides
	Opposite   | Modular: The straight piece of a castle block
	Adjacent   | Modular: The corner piece for a castle block
	Single     | Modular: The piece of a castleblock that only has crenelations on one side
	


woodengate
gate
castlestairs
archer
ballista
throneroom
chamberofwar
greathall
moat
barracks
archerschool
road
stoneroad
smallhouse
well
largehouse
manorhouse
townsquare
tavern
firehouse
cemetarydummy
cemetarykeeper
garden
church
library
clinic
hospital
fountain
largefountain
bathhouse
statue_levi
statue_barbara
statue_sam
greatlibrary
cathedral
farm
smallgranary
largegranary
windmill
baker
orchard
producestand
fishinghut
fishmonger
swineherd
butcher
smallmarket
market
noria
aqueduct
reservoir
quarry
forester
smallstockpile
largestockpile
charcoalmaker
ironmine
blacksmith
Mason
destructioncrew
outpost
dock
transportship
trooptransportship
pier
bridge
drawbridge
stonebridge
cemetary
cemetaryCircle
cemetaryDiamond
cemetary44

