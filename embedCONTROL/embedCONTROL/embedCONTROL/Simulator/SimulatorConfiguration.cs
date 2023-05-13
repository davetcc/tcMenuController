using System;
using tcMenuControlApi.MenuItems;
using tcMenuControlApi.RemoteCore;
using tcMenuControlApi.RemoteSimulator;
using tcMenuControlApi.Serialisation;
using embedCONTROL.Models;

namespace embedCONTROL.Simulator
{
    public class SimulatorConfiguration : IConnectionConfiguration
    {
        public const string SIMULATOR_NAME = "Simulator";
        public const string DEFAULT_MENU_TREE = "{\"items\":[{\"parentId\":0,\"type\":\"analogItem\",\"item\":{\"name\":\"Voltage\",\"eepromAddress\":2,\"id\":1,\"readOnly\":false,\"localOnly\":false,\"functionName\":\"onVoltageChange\",\"maxValue\":255,\"offset\":-128,\"divisor\":2,\"unitName\":\"V\"}},{\"parentId\":0,\"type\":\"analogItem\",\"item\":{\"name\":\"Current\",\"eepromAddress\":4,\"id\":2,\"readOnly\":false,\"localOnly\":false,\"functionName\":\"onCurrentChange\",\"maxValue\":255,\"offset\":0,\"divisor\":100,\"unitName\":\"A\"}},{\"parentId\":0,\"type\":\"enumItem\",\"item\":{\"name\":\"Limit\",\"eepromAddress\":6,\"id\":3,\"readOnly\":false,\"localOnly\":false,\"functionName\":\"onLimitMode\",\"enumEntries\":[\"Current\",\"Voltage\"]}},{\"parentId\":0,\"type\":\"subMenu\",\"item\":{\"name\":\"Settings\",\"eepromAddress\":-1,\"id\":4,\"readOnly\":false,\"localOnly\":false,\"secured\":false}},{\"parentId\":4,\"type\":\"boolItem\",\"item\":{\"name\":\"Pwr Delay\",\"eepromAddress\":-1,\"id\":5,\"readOnly\":false,\"localOnly\":false,\"naming\":\"YES_NO\"}},{\"parentId\":4,\"type\":\"actionMenu\",\"item\":{\"name\":\"Save all\",\"eepromAddress\":-1,\"id\":10,\"readOnly\":false,\"localOnly\":false,\"functionName\":\"onSaveRom\"}},{\"parentId\":4,\"type\":\"subMenu\",\"item\":{\"name\":\"Advanced\",\"eepromAddress\":-1,\"id\":11,\"readOnly\":false,\"localOnly\":false,\"secured\":false}},{\"parentId\":11,\"type\":\"boolItem\",\"item\":{\"name\":\"S-Circuit Protect\",\"eepromAddress\":8,\"id\":12,\"readOnly\":false,\"localOnly\":false,\"naming\":\"ON_OFF\"}},{\"parentId\":11,\"type\":\"boolItem\",\"item\":{\"name\":\"Temp Check\",\"eepromAddress\":9,\"id\":13,\"readOnly\":false,\"localOnly\":false,\"naming\":\"ON_OFF\"}},{\"parentId\":0,\"type\":\"subMenu\",\"item\":{\"name\":\"Status\",\"eepromAddress\":-1,\"id\":7,\"readOnly\":false,\"localOnly\":false,\"secured\":false}},{\"parentId\":7,\"type\":\"floatItem\",\"item\":{\"name\":\"Volt A0\",\"eepromAddress\":-1,\"id\":8,\"readOnly\":true,\"localOnly\":false,\"numDecimalPlaces\":2}},{\"parentId\":7,\"type\":\"floatItem\",\"item\":{\"name\":\"Volt A1\",\"eepromAddress\":-1,\"id\":9,\"readOnly\":true,\"localOnly\":false,\"numDecimalPlaces\":2}},{\"parentId\":7,\"type\":\"largeNumItem\",\"item\":{\"name\":\"RotationCounter\",\"eepromAddress\":-1,\"id\":16,\"readOnly\":true,\"localOnly\":false,\"decimalPlaces\":4,\"digitsAllowed\":8}},{\"parentId\":0,\"type\":\"subMenu\",\"item\":{\"name\":\"Connectivity\",\"eepromAddress\":-1,\"id\":14,\"readOnly\":false,\"localOnly\":true,\"secured\":true}},{\"parentId\":14,\"type\":\"textItem\",\"item\":{\"name\":\"Ip Address\",\"eepromAddress\":10,\"id\":15,\"readOnly\":false,\"visible\":\"false\",\"localOnly\":false,\"itemType\":\"IP_ADDRESS\",\"textLength\":20}}]}";

        public string Name { get; set; }
        public string JsonObjects { get; set; }

        public SimulatorConfiguration()
        {
            this.Name = "Unknown";
            this.JsonObjects = "";
        }

        public SimulatorConfiguration(string name, string json)
        {
            this.Name = name ?? "";
            this.JsonObjects = json ?? "";
        }

        public IRemoteController Build()
        {
            var persistor = new JsonMenuItemPersistor();
            var menuTree = new MenuTree();
            var json = string.IsNullOrWhiteSpace(JsonObjects) ? DEFAULT_MENU_TREE : JsonObjects;
            foreach(var itemAndParent in persistor.DeSerialiseItemsFromJson(json))
            {
                menuTree.AddMenuItem(menuTree.GetMenuById(itemAndParent.ParentId) as SubMenuItem, itemAndParent.Item);
            }
            var connector = new SimulatedRemoteConnection(menuTree, Name);
            var controller = new RemoteController(connector, menuTree, new tcMenuControlApi.Protocol.SystemClock());
            controller.Start();
            return controller;

        }

        public bool Pair(PairingUpdateEventHandler handler)
        {
            throw new NotImplementedException();
        }
    }
}
