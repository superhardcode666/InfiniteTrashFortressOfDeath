using System;
using System.Collections.Generic;
using UnityEngine;

/*      __  _____    ____  ____ 
       / / / /   |  / __ \/ __ \
      / /_/ / /| | / /_/ / / / /
     / __  / ___ |/ _, _/ /_/ / 
    /_/_/_/_/__|_/_/_|_/_____/_ 
      / ____/ __ \/ __ \/ ____/ 
     / /   / / / / / / / __/    
    / /___/ /_/ / /_/ / /___    
    \____/\____/_____/_____/ 
        アンフェタミンを燃料
*/
namespace Hardcode.ITFOD.Items
{
    /// <summary>
    ///     Base Class for Player Inventory
    /// </summary>
    /// <remarks>
    ///     Stores list of all currently held Items and handles adding and removing
    ///     Also contains a Delegate to signal whether contents of inventory have changed
    ///     so we know when to update the Display - which isnt implemented yet
    /// </remarks>
    [Serializable]
    public class Inventory
    {
        #region Field Declarations

        public delegate void OnItemChanged();

        public event OnItemChanged onItemChangedCallback;

        public delegate void OnItemPickedUp(Item item);

        public OnItemPickedUp onItemPickedUpCallback;

        public delegate void OnItemDropped(Item item);

        public OnItemDropped onItemDroppedCallback;

        public delegate void OnItemUsed(Item item);

        public OnItemUsed onItemUsedCallback;

        public int capacity = 12;
        public List<Item> items;

        [SerializeField] private int money;
        [SerializeField] private int keys;

        public Inventory()
        {
            items = new List<Item>();
        }

        public bool AddItem(Item item)
        {
            var itemToAdd = item;

            if (items.Count >= capacity)
            {
                Debug.Log("Inventory full!");
                return false;
            }

            items.Add(itemToAdd);
            onItemChangedCallback?.Invoke();
            onItemPickedUpCallback?.Invoke(item);

            return true;
        }

        public void RemoveItem(Item item)
        {
            if (items.Contains(item))
            {
                items.Remove(item);
                onItemChangedCallback?.Invoke();
            }
            else
            {
                Debug.Log("Can't remove what isn't there, chief.");
            }
        }

        #endregion
    }
}