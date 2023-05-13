using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tcMenuControlApi.MenuItems
{
    /// <summary>
    /// A series of helper methods for menu items
    /// </summary>
    public static class MenuItemHelper
    {
        /// <summary>
        /// provides a means to visit menu items and get a result back from the visit
        /// </summary>
        /// <typeparam name="T">The return type required</typeparam>
        /// <param name="item">The item to be visited</param>
        /// <param name="visitor">The class extending from AbstractMenuItemVisitor</param>
        /// <returns>The last item stored in Result</returns>
        public static T VisitWithResult<T>(MenuItem item, AbstractMenuItemVisitor<T> visitor)
        {
            item.Accept(visitor);
            return visitor.Result;
        }

        /// <summary>
        /// Find the next available menu ID that's available.
        /// </summary>
        /// <param name="tree">the tree to evaluate</param>
        /// <returns>A non-conflicting menu id</returns>    
        public static int FindAvailableMenuId(MenuTree tree)
        {
            return tree.GetAllMenuItems()
                .Select(item => item.Id)
                .Max() + 1;
        }

        /// <summary>
        /// Finds the next available EEPROM location
        /// </summary>
        /// <param name="tree">the tree to evaluate</param>
        /// <returns>an available eeprom location</returns>
        public static int FindAvailableEEPROMLocation(MenuTree tree)
        {
            var loc = tree.GetAllMenuItems()
                .Where(item => item.EepromAddress != -1)
                .Select(item => item.EepromAddress + GetEEPROMStorageRequirement(item))
                .DefaultIfEmpty(2)
                .Max();
            return loc;
        }

        /// <summary>
        /// Returns the amount of EEPROM storage needed for the menu item
        /// </summary>
        /// <param name="item">the item to find the size of</param>
        /// <returns>the size as an int</returns>
        public static int GetEEPROMStorageRequirement(MenuItem item)
        {
            switch(item)
            {
                case AnalogMenuItem _: return 2;
                case BooleanMenuItem _: return 1;
                case EnumMenuItem _: return 2;
                case LargeNumberMenuItem _: return 8;
                case Rgb32MenuItem _: return 4;
                case ScrollChoiceMenuItem _: return 2;
                case EditableTextMenuItem txt:
                    if (txt.EditType == EditItemType.IP_ADDRESS) return 4;
                    else if (txt.EditType == EditItemType.PLAIN_TEXT) return txt.TextLength;
                    else return 4; // time always 4
                default: return 0;
            }
        }

        public static MenuItem CreateFromExistingWithNewId(MenuItem existing, int id)
        {
            switch (existing)
            {
                case AnalogMenuItem i: return new AnalogMenuItemBuilder().WithExisting(i).WithId(id).WithEepromLocation(-1).Build();
                case EnumMenuItem i: return new EnumMenuItemBuilder().WithExisting(i).WithId(id).WithEepromLocation(-1).Build();
                case BooleanMenuItem i: return new BooleanMenuItemBuilder().WithExisting(i).WithId(id).WithEepromLocation(-1).Build();
                case ActionMenuItem i: return new ActionMenuItemBuilder().WithExisting(i).WithId(id).WithEepromLocation(-1).Build();
                case SubMenuItem i: return new SubMenuItemBuilder().WithExisting(i).WithId(id).WithEepromLocation(-1).Build();
                case FloatMenuItem i: return new FloatMenuItemBuilder().WithExisting(i).WithId(id).WithEepromLocation(-1).Build();
                case EditableTextMenuItem i: return new EditableTextMenuItemBuilder().WithExisting(i).WithId(id).WithEepromLocation(-1).Build();
                case LargeNumberMenuItem i: return new LargeNumberMenuItemBuilder().WithExisting(i).WithId(id).WithEepromLocation(-1).Build();
                case RuntimeListMenuItem i: return new RuntimeListMenuItemBuilder().WithExisting(i).WithId(id).WithEepromLocation(-1).Build();
                case Rgb32MenuItem rgb: return new Rgb32MenuItemBuilder().WithExisting(rgb).WithId(id).WithEepromLocation(-1).Build();
                case ScrollChoiceMenuItem sc: return new ScrollChoiceMenuItemBuilder().WithExisting(sc).WithId(id).WithEepromLocation(-1).Build();
                default: return null;
            }
        }

        public static bool IsRuntimeStructureNeeded(MenuItem item)
        {
            switch (item)
            {
                case RuntimeListMenuItem _: return true;
                case EditableTextMenuItem _: return true;
                case LargeNumberMenuItem _: return true;
                case SubMenuItem _: return true;
                default: return false;
            }
            
        }
    }
}
