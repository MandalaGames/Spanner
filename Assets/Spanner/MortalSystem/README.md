# The Mortal System
The Mortal System is a flexible, abstract system for tracking the state of objects that have health in your game. This README will help you get started with the Mortal System in your own games.

## The Mortal Component
The [Mortal Component](https://github.com/MandalaGames/Spanner/blob/master/Assets/Spanner/MortalSystem/Mortal.cs) is responsible for tracking an object's current health, whether it is alive or dead, and for raising appropriate events when the object receives damage, heals, etc.

## Configuring a Mortal Component
In the inspector, you will notice that the Mortal has one serialized field: a Scriptable Object called [MortalSettings](https://github.com/MandalaGames/Spanner/blob/master/Assets/Spanner/MortalSystem/MortalSettings.cs). If you're familiar with Scriptable Objects already, you know that this class may be instantiated and handed to the Mortal at runtime. However, we recommend that you create and save an asset file in your project hierarchy for each different type of mortal in your game. 

To create a new MortalSettings asset file: 
1. Navigate to the folder in your project hierarchy where you want to store the settings in the Unity Editor (We usually put ours in Assets/Data/Settings/Mortal, but you are free to put it wherever you like).
2. Right-click on the folder and click Create -> Spanner -> Mortal -> Settings
3. Give the newly created asset file a unique name (usually, we prefer to name ours something like <NameOfObject>MortalSettings).

Now that you have your settings file created, let's go over the different settings fields:
* `Randomize Health` - If checked, the maximum and starting health of the Mortal will be randomly selected when the Mortal is initialized. Otherwise, the maximum and starting health will be determined directly by the settings. Checking or unchecking this field will change the inspector for the settings.
* `Max Health` - If `Randomize Health` is unchecked, what is the maximum health this mortal can have?
* `Starting Health` - If `Randomize Health` is unchecked, how much health should this mortal start with?
* `Max Health Cap` - If `Randomize Health` is checked, what is the upper limit on the maximum health for this mortal?
* `Min Health Cap` - If `Randomize Health` is checked, what is the lower limit on the maximum health for this mortal?
* `Max Starting Health` - If `Randomize Health` is checked, what is the maximum amount of health this mortal should start with?
* `Min Starting Health` - If `Randomize Health` is checked, what is the minimum amount of health this mortal should start with?

After configuring your MortalSettings, you can drag and drop the asset file to the Mortal component in the insepctor.

## Initializing the Mortal
The Initialize() method of the Mortal Component applies the MortalSettings to the object. Initialize() is called automatically in the Start() Unity Callback method, but there are some cases where you may wish to call this method yourself, so the method has also been made public.

## How to send information to the Mortal
Currently, there are four public methods for directly interfacing with the mortal

1. `Damage` - Damage takes an integer as an argument, and subtracts that value from the Mortal's current health. Then, the `OnDamage` event is raised. If the damage causes the Mortal's current health to drop below 1, then the `Die` method is called automatically.
2. `Heal` - Heal takes an integer as an argument, and adds that value to the Mortal's current health (value is clamped to prevent healing beyond the maximum health). Then, the `OnHeal` event is raised.
3. `Die` - Sets the isDead flag to true and raises the `OnDeath` event.
4. `Revive` - There are two overloads for this method:
  1. Takes no arguments. Sets the `isDead` flag to false and raises the `OnRevive` event. Initializes the Mortal with the given settings.
  2. Takes an integer as an argument. Sets the `isDead` flag to false and raises the `OnRevive` event. Sets the Mortal's current health to the given integer value.
  
## Events raised by the Mortal
When the methods for interfacing with the Mortal are called, the Mortal will raise various events. For example, when `Heal()` is called on the Mortal, it raises the `OnHeal` event. Your development team is responsible for determining what the handling of this event will look like. For example, you may want to have two handlers for the `OnHeal` event. One is a component on the healed object that handles the event by displaying a green number above the object, showing the amount of health gained from healing. The other might be a UI object which handles the event by updating the fill amount on a health bar image. Making the Mortal System event driven in this way makes your game more modular, which is a good thing!

### A list of events and how to write handlers for them:
Each event uses delegate methods as handlers. Delegate methods are essentially method signatures that can be passed around as data. To write a handler for an event, the signature of your handler method must match the signature of the delegate method for that event. Here is an example of what the handler method would look like in a component that responds to the `OnHeal` event:

```C#
public class MyHealingHandler : MonoBehaviour {

	...
	
	public Mortal mortal;

	...

	public void OnHealHandler(Mortal mortal, int amount, int currentHealth) {
		// Handle the event here
	}

	private void OnEnable() {
		if (mortal != null) {
			// Register the OnHealHandler() method as a listener for this Mortal's OnHeal event
			mortal.OnHeal += OnHealHandler;
		}
	}

	private void OnDisable() {
		if (mortal != null) {
			// Unregister the OnHealHandler() method as a listener for this Mortal's OnHeal event
			mortal.OnHeal -= OnHealHandler;
		}
	}

	...
}
```

Here is a list of each event raised by Mortal and the signature of their handler methods:

1. `OnDamage` - `public delegate void OnDamageHandler(Mortal mortal, int amount, int currentHealth)`
2. `OnHeal` - `public delegate void OnHealHandler(Mortal mortal, int amount, int currentHealth)`
3. `OnDeath` - `public delegate void OnDeathHandler(Mortal mortal)`
4. `OnRevive` - `public delegate void OnReviveHandler(Mortal mortal, int currentHealth)`

# That's all, folks! Enjoy using the Mortal System!
