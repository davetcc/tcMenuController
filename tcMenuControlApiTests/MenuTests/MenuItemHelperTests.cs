using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using tcMenuControlApi.MenuItems;
using tcMenuControlApi.Serialisation;

namespace tcMenuControlApiTests.MenuTests
{
    [TestClass]
    public class MenuItemHelperTests
    {
        [TestMethod]
        public void TestCreateFromExistingWithNewId()
        {
            var analog = MenuItemFixtures.AnAnalogItem(100, "Blah");
            var analogNew = MenuItemHelper.CreateFromExistingWithNewId(analog, 1000) as AnalogMenuItem;
            Assert.AreEqual(analog.MaximumValue, analogNew.MaximumValue);
            Assert.AreEqual(1000, analogNew.Id);
            Assert.AreEqual(-1, analogNew.EepromAddress);

            var enumItem = MenuItemFixtures.AnEnumItem(100, "Enum", 101);
            var newEnum = MenuItemHelper.CreateFromExistingWithNewId(enumItem, 1000) as EnumMenuItem;
            Assert.AreEqual(1000, newEnum.Id);
            Assert.AreEqual(-1, newEnum.EepromAddress);

            var boolItem = MenuItemFixtures.ABoolItem(100, "Bool");
            var newBool = MenuItemHelper.CreateFromExistingWithNewId(boolItem, 1000) as BooleanMenuItem;
            Assert.AreEqual(1000, newBool.Id);
            Assert.AreEqual(-1, newEnum.EepromAddress);

            var floatItem = MenuItemFixtures.AFloatItem(100, "Flt", 203);
            var newFloat = MenuItemHelper.CreateFromExistingWithNewId(floatItem, 1000) as FloatMenuItem;
            Assert.AreEqual(1000, newFloat.Id);
            Assert.AreEqual(-1, newFloat.EepromAddress);

            var subItem = MenuItemFixtures.ASubItem(100, "Sub", 203);
            var newSub = MenuItemHelper.CreateFromExistingWithNewId(subItem, 1000) as SubMenuItem;
            Assert.AreEqual(1000, newSub.Id);
            Assert.AreEqual(-1, newSub.EepromAddress);

            var actionItem = MenuItemFixtures.AnActionItem(100, "Act", "Fn");
            var newAction = MenuItemHelper.CreateFromExistingWithNewId(actionItem, 1000) as ActionMenuItem;
            Assert.AreEqual(1000, newAction.Id);
            Assert.AreEqual(-1, newAction.EepromAddress);

            var textItem = MenuItemFixtures.ATextItem(100, "Txt", 203);
            var newText = MenuItemHelper.CreateFromExistingWithNewId(textItem, 1000) as EditableTextMenuItem;
            Assert.AreEqual(1000, newText.Id);
            Assert.AreEqual(-1, newText.EepromAddress);

            var largeNumItem = MenuItemFixtures.ALargeNumberItem(100, "Lge", 203);
            var newLargeNum = MenuItemHelper.CreateFromExistingWithNewId(largeNumItem, 1000) as LargeNumberMenuItem;
            Assert.AreEqual(1000, newLargeNum.Id);
            Assert.AreEqual(-1, newLargeNum.EepromAddress);
        }

        [TestMethod]
        public void TestGetNextLargestIdAndEeprom()
        {
            var tree = MenuItemFixtures.LoadMenuTree(MenuItemFixtures.LARGE_MENU_TREE);

            Assert.AreEqual(20, MenuItemHelper.FindAvailableMenuId(tree));
            
            Assert.AreEqual(14, MenuItemHelper.FindAvailableEEPROMLocation(tree));

            var newTree = new MenuTree();
            Assert.AreEqual(2, MenuItemHelper.FindAvailableEEPROMLocation(newTree));
            Assert.AreEqual(1, MenuItemHelper.FindAvailableMenuId(newTree));
        }

        [TestMethod]
        public void TestGetEepromStorageRequirement()
        {
            Assert.AreEqual(2, MenuItemHelper.GetEEPROMStorageRequirement(MenuItemFixtures.AnAnalogItem(122, "hello")));
            Assert.AreEqual(2, MenuItemHelper.GetEEPROMStorageRequirement(MenuItemFixtures.AnEnumItem(211, "hello")));
            Assert.AreEqual(1, MenuItemHelper.GetEEPROMStorageRequirement(MenuItemFixtures.ABoolItem(132, "hello")));
            Assert.AreEqual(10, MenuItemHelper.GetEEPROMStorageRequirement(MenuItemFixtures.ATextItem(232, "hello", 10, EditItemType.PLAIN_TEXT)));
            Assert.AreEqual(4, MenuItemHelper.GetEEPROMStorageRequirement(MenuItemFixtures.ATextItem(243, "hello", 22, EditItemType.IP_ADDRESS)));
            Assert.AreEqual(4, MenuItemHelper.GetEEPROMStorageRequirement(MenuItemFixtures.ATextItem(242, "hello", 22, EditItemType.TIME_12H)));
            Assert.AreEqual(8, MenuItemHelper.GetEEPROMStorageRequirement(MenuItemFixtures.ALargeNumberItem(349, "Avin it large", 12)));
            Assert.AreEqual(0, MenuItemHelper.GetEEPROMStorageRequirement(MenuItemFixtures.AFloatItem(10, "Flt")));
        }
    }
}
