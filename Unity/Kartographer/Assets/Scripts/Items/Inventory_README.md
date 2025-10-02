# Inventory System

## Overview
This system handles picking up, storing, and dropping items.  
Items are represented by `ItemData` ScriptableObjects, and a generic pickup prefab spawns meshes based on the assigned ItemData.

## Components
- **ItemData**: ScriptableObject that defines item stats, icon, prefab mesh, etc.
- **PickUp**: Generic prefab with Rigidbody + Collider. Spawns the mesh assigned using ItemData's `prefab` GameObject at runtime.
- **Inventory**: Holds a list of `ItemData` for the player (capped at five items).
- **InventoryIconGenerator**: Generates icons in the UI.
- **PlayerPickUp**: Handles input for pickup/drop.
- **ItemDatabase**: Holds categorized `ItemData` arrays.
- **ItemSpawner**: Spawns items at designated spawn points.
- **SpawnPoint**: Empty GameObject that defines spawn chance and item type.

---

## How to Add a New Item
1. Right-click → `Create/Inventory/Item` to create a new `ItemData`.
2. Assign the 3D model prefab to its `prefab` field.
3. Choose `itemType`.
4. Add to `inventoryDatabase` on the player.

---

## ItemData.cs
- **Description**: Stores item attributes, mesh prefab, sprite, and other metadata.
- **Attributes**:

| Type        | Variable         | Description                           |
|-------------|-----------------|---------------------------------------|
| `string`    | `itemId`        | Unique identifier                      |
| `string`    | `displayName`   | Name displayed in UI                   |
| `Sprite`    | `icon`          | Icon for UI                            |
| `GameObject`| `prefab`        | Mesh/pickup prefab                     |
| `int`       | `value`         | Item worth/value                        |
| `string`    | `description`   | Tooltip description                    |
| `bool`      | `isStackable`   | Can multiple copies stack?             |
| `int`       | `scale`         | Default scale multiplier (100 = 1x)   |
| `ItemType`  | `itemType`      | Enum: FloorItem, ShelfItem, TableItem, OutdoorItem |
| `Vector3`   | `defaultScale`  | Computed automatically in `OnValidate`|

- **Functions**:
  - `private void OnValidate()` → updates `defaultScale` when `scale` changes.

---

## ItemDatabase.cs
- **Description**: Stores categorized `ItemData` arrays for spawners.
- **Attributes**:

| Type        | Variable         | Description                         |
|-------------|-----------------|-------------------------------------|
| `ItemData[]`| `floorItems`    | Items that spawn on floors           |
| `ItemData[]`| `shelfItems`    | Items that spawn on shelves          |
| `ItemData[]`| `tableItems`    | Items that spawn on tables           |
| `ItemData[]`| `outdoorItems`  | Items that spawn outdoors            |

- **Functions**:
  - `void Awake()` → Ensures singleton pattern.

---

## ItemSpawner.cs
- **Description**: Place on building prefab, spawns items at child `SpawnPoint`s objects based on chance.
- **Attributes**:

| Type         | Variable      | Description                 |
|--------------|---------------|-----------------------------|
| `ItemDatabase`| `database`   | Reference to the item database |
| `GameObject` | `itemPrefab` | Generic pickup prefab to spawn |

- **Functions**:
  - `void Start()` → Loops through spawn points, checks `spawnChance`, and instantiates items.

---

## PickUp.cs
- **Description**: Represents a pickupable object; handles mesh instantiation and adding to inventory.
- **Attributes**:

| Type       | Variable       | Description                |
|------------|----------------|----------------------------|
| `ItemData` | `itemData`     | Data describing this item  |

- **Functions**:
  - `void Start()` → Instantiates prefab, applies `itemData.defaultScale`, sets position.
  - `void OnPickup(GameObject player)` → Adds item to player's inventory, generates icon in UI, destroys pickup object.

---

## PlayerPickUp.cs
- **Description**: Handles pickup/drop input and displays prompt.
- **Attributes**:

| Type                      | Variable           | Description                           |
|----------------------------|------------------|---------------------------------------|
| `PickUp`                   | `nearbyPickup`    | Currently triggerable pickup object   |
| `InputActionReference`     | `pickUpAction`    | Input action for picking up           |
| `InputActionReference`     | `dropAction`      | Input action for dropping             |
| `GameObject`               | `pickUpPrompt`    | UI prompt to show when near item      |
| `Inventory`                | `playerInventory` | Player's inventory                    |
| `InventoryIconGenerator`   | `iconGenerator`   | Reference to generate UI icons        |
| `GameObject`               | `pickupPrefab`    | Generic pickup prefab to spawn        |

- **Functions**:
  - `void Start()` → Disables pick up prompt initially.
  - `void OnTriggerEnter(Collider other)` → Sets `nearbyPickup` and enables prompt.
  - `void OnTriggerExit(Collider other)` → Clears `nearbyPickup` and disables prompt.
  - `void Update()` → Handles input:
    - Pickup:
      - Calls `nearbyPickup.OnPickup(gameObject)` and clears `nearbyPickup`.
    - Drop:
      | Step | Description |
      |------|-------------|
      | 1    | Get last item from `playerInventory.GetLastItem()` |
      | 2    | Instantiate generic `pickupPrefab` in front of player |
      | 3    | Assign `PickUp.itemData` on prefab to the dropped `ItemData` |
      | 4    | Remove item from inventory using `playerInventory.Remove(item)` |
      | 5    | Clear UI slot using `iconGenerator.ClearSlot()` |

---

## SpawnPoint.cs
- **Description**: Empty GameObject marking a spawn location.
- **Attributes**:

| Type      | Variable      | Description                     |
|-----------|---------------|---------------------------------|
| `float`   | `spawnChance` | Chance (0–1) to spawn an item  |
| `ItemType`| `itemType`    | What type of item can spawn     |

- **Functions**: None.

---

## Inventory.cs
- **Description**: Stores `ItemData` objects in a list and provides helper functions.
- **Attributes**:

| Type             | Variable | Description                   |
|------------------|---------|-------------------------------|
| `List<ItemData>`  | `items` | Dynamic list of inventory items |

- **Functions**:
  - `void Add(ItemData item)` → Adds an item.
  - `bool Remove(ItemData item)` → Removes an item.
  - `ItemData GetLastItem()` → Returns the last item, or null if empty.

---

## InventoryIconGenerator.cs
- **Description**: Generates UI icons for inventory slots.
- **Attributes**:

| Type       | Variable         | Description                              |
|------------|-----------------|------------------------------------------|
| `Image[]`  | `inventorySlots` | Predetermined UI slots for item icons    |

- **Functions**:
  - `void GenerateIcon(ItemData itemData, int slotIndex)` → Sets sprite in slot.
  - `int GetNextAvailableSlot()` → Returns index of next free slot, or -1 if none.
  - `void ClearSlot(int slotIndex)` → Clears the sprite at a given slot.
