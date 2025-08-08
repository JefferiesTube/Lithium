This mod is inspired by mods like ValheimPlus that give you the possibility to adjust almost every setting to your preference. I wanted to make the start of a game slower and make a lot of gameplay options (like smashing of ATMs etc.) more valuable.

### **Enabling required**

Lithium consists of several modules that can be toggled by setting the value of `Enabled` in the corresponding configuration file. All configuration files are saved in `/UserData/Lithium/` in separate JSON files. All setting files are created with default values when first starting a game with this mod. All modules are disabled per default and you need to enable them by changing the configuration file.

### **Value types**

#### **Weighted Values**

You can provide lots of values and give each of them a weight. They are put in a random picker, but when picking an item, items with a higher weight are preferred, according to their relative weight. You can use this to make something like: 80% of the times give me option A, 15% option B and 5% option C. The weights do not have to add up to 100%, this is calculated internally automatically.

#### **Weighted Normalized Values**

Similar to WeightedValues, but only works for numeric values. Internally, they are used to create a curve using the given weights and then pick a random value on this curve. Allows you to get values in between individual weighted values.

---

### **Module "Customers"**

*ConfigFile: `customers.json`*

**Description**  
Make customers' preferred effects more relevant. Once you've reached a rank of "Hoolum II", customers will only order products from you or your dealers, if at least one of their desired effects is provided by the product. They will send you a message listing their desired effects (and the name of their dealer if applicable) if there is no suitable offer. Products providing multiple of their effects are preferred (using a weighted random).  
When providing samples to customers, the same requirements apply, but you get a percentage bonus for each level of quality you exceed their required quality standards. Makes getting new customers a challenge and feels much more rewarding.

**Config settings**  
_None_

---

### **Module "DryingRacks"**

*ConfigFile: `DryingRacks.json`*

**Description**  
Allows you to set a custom duration for the drying operation per Quality. Increase the value to make drying take more time and prevent you from easily getting loads of heavenly product.

**Config settings**  
**DryTimePerQuality**  
The amount of ingame minutes required to increase a product quality by one level on a drying rack  
- DefaultValue: 720  
- Recommended Value: 1440  

---

### **Module "LabOven"**

*ConfigFile: `LabOven.json`*

**Description**  
Allows you to set custom values for lab ovens.

**Config settings**  
**Speed**  
The factor the working speed of lab ovens are multiplied with. Higher values result in faster operations.  
- DefaultValue: 1  
- Recommended Value: 2  

---

### **Module "MixingStations"**

*ConfigFile: `MixingStations.json`*

**Description**  
Allows you to set custom values for mixing stations.

**Config settings**  
**SpeedFactor**  
The factor the working speed of mixing stations are multiplied with. Higher values result in faster operations  
- DefaultValue: 1  
- Recommended Value: 2  

**InputCapacity**  
The maximum amount of items you can put into the input for a mixing operation. Useful if you increased the stack sizes as well.  
- Default Value: 20  
- Recommended Value: 60  

---

### **Module "StoryLine"**

*ConfigFile: `Storyline.json`*

**Description**  
Allows you to set custom options that alter or affect the quests and storyline.

**Config settings**  
**PreventRVExplosion**  
If active, the RV will not explode and the quest will be skipped. To allow further progression, the cartel node is activated (but hovers in the air, because the wreckage below is missing).  
- DefaultValue: true  
- Recommended Value: true  

---

### **Module "PlantGrowth"**

*ConfigFile: `Plants.json`*

**Description**  
Allows you to alter the growing and harvesting behavior of plants. Also affects botanists working on plants.

**Config settings**  
**GrowthModifier**  
Controls the grow speed of all plants. Higher values = faster growth.  
- DefaultValue: 1  
- Recommended Value: 0.25  

**WaterDrainModifier**  
Controls the water loss of pots. Higher values = faster water loss.  
- DefaultValue: 1  
- Recommended Value: 0.15  

**RandomYieldPerBudPicker**  
Weighted values for amount of product from each harvested bud.  
- DefaultValue: {1,1}  
- Recommended Value: {10, 1}, {2, 2}, {1, 3}  

**RandomYieldQualityPicker**  
Weighted values for quality of harvested product.  
- DefaultValue: {1,0}  
- Recommended Value: {1.5, -0.24}, {5.0, 0}, {1.5, 0.4}, {0.1, 1}  

**RandomYieldModifierPicker**  
Allows plants to have varied numbers of buds.  
- DefaultValue: {1,1}  
- Recommended Value: {8.0, 1.0}, {1.0, 0.25}, {2.0, 1.5}, {1.0, 2.0}, {0.5, 3.0}  

---

### **Module "PropertyPrices"**

*ConfigFile: `Property Prices.json`*

**Description**  
Allows you to set custom prices for all properties and businesses.

**Config settings**  
**PropertyPrices**  
A list of all properties and their respective prices:  
- RV: 0  
- Motel Room: 750  
- Sweatshop: 8000  
- Storage Unit: 15000  
- Bungalow: 30000  
- Barn: 100000  
- Docks Warehouse: 250000  
- Laundromat: 5000  
- Post Office: 25000  
- Car Wash: 50000  
- Taco Ticklers: 100000  
- Manor: 100000  

---

### **Module "StackSizes"**

*ConfigFile: `StackSizes.json`*

**Description**  
Allows you to set a custom maximum stack size per category. You can also set specific items to separate values and ignore specific items.

**Config settings**  
**CategorySizes**  
- Product: 40  
- Packaging: 60  
- Agriculture: 20  
- Tools: 10  
- Furniture: 10  
- Lighting: 10  
- Cash: 10000  
- Consumable: 40  
- Equipment: 20  
- Ingredient: 40  
- Decoration: 10  
- Clothing: 10  
- Storage: 10  

**ItemOverrides**  
Set specific override stack sizes for individual items by ID.

**IgnoredItems**  
Specify item IDs to keep original stack sizes.

**DryingRack**  
Configure the capacity of a drying rack.  
- DefaultValue: 20  

> ⚠️ CAUTION: This value will probably be moved into the DryingRack module in a future release

---

### **Module "TrashGrabber"**

*ConfigFile: `TrashGrabber.json`*

**Description**  
Allows you to set a custom capacity for the TrashGrabber.

**Config settings**  
**CustomCapacity**  
Overrides the maximum capacity of the TrashGrabber. Allows exceeding 100%.  
- DefaultValue: 20  
- Recommended Value: 100  

---

### **Inspired by**

I tried and played a lot of mods in Schedule I and some have inspired me to do things for Lithium:

- **BetterStacks** by zarnes — great mod and has more options (like working for deliveries etc.). Give it a try!  
- **TrashGrabberPlus** by Elio — Great idea and updates percentage on the item and gives the TrashGrabber infinite capacity.  
- **Ovens Cook Faster** by lasersquid — Inspired the modification of station usage times.

### **Thank you**

This work would not have been possible without the help of the squad from the Schedule 1 Modding Discord. So a big thank you goes to the great and supporting folks on the Unofficial Schedule One Modding Server:

- OnlyMurdersSometimes  
- Bars  
- Max  
- k0  
- hiemdallh  
- coolpaca  
- and others (in case I missed some, which I sincerely hope I did not)
