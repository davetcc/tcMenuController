using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using embedCONTROL.ControlMgr;
using embedCONTROL.Services;
using tcMenuControlApi.Serialisation;
using Xamarin.Forms;

namespace embedControlTests
{
    [TestClass]
    public class PrefsAppSettingsTest
    {
        private string _tempDirectory;
        private string _uuid;

        [TestInitialize]
        public void InitTest()
        {
            _tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(_tempDirectory);
        }

        [TestCleanup]
        public void CleanUp()
        {
            Directory.Delete(_tempDirectory, true);
        }

        [TestMethod]
        public void TestLoadingAndSavingPreferences()
        {
            var settings = new PrefsAppSettings();
            settings.Load(_tempDirectory);

            _uuid = settings.UniqueId.ToString();
            Assert.AreEqual("Unnamed", settings.LocalName);
            Assert.AreEqual(1, settings.DefaultNumColumms);

            settings.LocalName = "Test 123";
            settings.DefaultNumColumms = 2;
            settings.ButtonColor = new ControlColor(PortableColors.DARK_GREY, PortableColors.DARK_BLUE);
            settings.TextColor = new ControlColor(PortableColors.BLUE, PortableColors.WHITE);
            settings.HighlightColor = new ControlColor(PortableColors.RED,  PortableColors.INDIGO);
            settings.ErrorColor = new ControlColor(PortableColors.GREY, PortableColors.CRIMSON);
            settings.PendingColor = new ControlColor(PortableColors.LIGHT_GRAY, PortableColors.ANTIQUE_WHITE);
            settings.UpdateColor = new ControlColor(PortableColors.BLACK, PortableColors.CORAL);
            settings.DialogColor = new ControlColor(PortableColors.DARK_SLATE_BLUE, PortableColors.GREEN);
            settings.Save();

            settings.Load(_tempDirectory);
            CheckAllSettings(settings);

            PrefsAppSettings clonedSettings = new PrefsAppSettings();
            clonedSettings.CloneSettingsFrom(settings);
            CheckAllSettings(clonedSettings);
        }

        private void CheckAllSettings(PrefsAppSettings settings)
        {
            Assert.AreEqual("Test 123", settings.LocalName);
            Assert.AreEqual(2, settings.DefaultNumColumms);
            Assert.AreEqual(_uuid, settings.UniqueId);
            CheckControlColor(settings.ButtonColor, PortableColors.DARK_GREY, PortableColors.DARK_BLUE);
            CheckControlColor(settings.TextColor, PortableColors.BLUE, PortableColors.WHITE);
            CheckControlColor(settings.HighlightColor, PortableColors.RED, PortableColors.INDIGO);
            CheckControlColor(settings.ErrorColor, PortableColors.GREY, PortableColors.CRIMSON);
            CheckControlColor(settings.PendingColor, PortableColors.LIGHT_GRAY, PortableColors.ANTIQUE_WHITE);
            CheckControlColor(settings.UpdateColor, PortableColors.BLACK, PortableColors.CORAL);
            CheckControlColor(settings.DialogColor, PortableColors.DARK_SLATE_BLUE, PortableColors.GREEN);
        }

        private void CheckControlColor(ControlColor actual, PortableColor expectedFg, PortableColor expectedBg)
        {
            Assert.AreEqual(expectedBg, actual.Bg);
            Assert.AreEqual(expectedFg, actual.Fg);
        }
    }
}
