using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using RGN.Extensions;
using RGN.Modules.Inventory;
using RGN.Tests;
using UnityEngine.TestTools;

namespace RGN.Inventory.Tests.Runtime
{
    [TestFixture]
    public class InventoryTests : BaseTests
    {
        [UnityTest]
        public IEnumerator AddToInventory_StackableItemShouldIncreaseCount()
        {
            yield return LoginAsAdminTester();

            var inventoryItem = new InventoryItemData(
                "b14e64d4-52c2-4f8b-be65-a0161542c010",
                new List<string>() { RGNCoreBuilder.I.AppIDForRequests });

            var task = InventoryModule.I.AddToInventoryAsync(
                "bb4717dd1bca471e9641afba1d428147",
                inventoryItem);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.NotNull(result, "The result is null");
            Assert.IsTrue(result.quantity >= 1);
            UnityEngine.Debug.Log(result);
        }
        [UnityTest]
        public IEnumerator AddToInventory_NonStackableItemShouldCreateNewDocument()
        {
            yield return LoginAsAdminTester();

            var inventoryItem = new InventoryItemData(
                "053824c3-e523-433c-9009-51367f809137",
                new List<string>() { RGNCoreBuilder.I.AppIDForRequests });

            var task = InventoryModule.I.AddToInventoryAsync(
                "bb4717dd1bca471e9641afba1d428147",
                inventoryItem);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.NotNull(result, "The result is null");
            Assert.IsTrue(result.quantity == 1);
            UnityEngine.Debug.Log(result);
        }
        [UnityTest]
        public IEnumerator RemoveByInventoryItemId_CanBeCalledByAdminUser()
        {
            yield return LoginAsAdminTester();

            string userId = RGNCoreBuilder.I.MasterAppUser.UserId;
            string virtualItemId = "053824c3-e523-433c-9009-51367f809137";

            var task1 = InventoryModule.I.AddToInventoryAsync(virtualItemId);

            yield return task1.AsIEnumeratorReturnNull();
            var result1 = task1.Result;

            var task2 = InventoryModule.I.RemoveByInventoryItemIdAsync(result1.id, 1);
            yield return task2.AsIEnumeratorReturnNull();
            var result2 = task2.Result;

            Assert.NotNull(result2, "The result is null");
            UnityEngine.Debug.Log(result2);
        }
        [UnityTest]
        public IEnumerator RemoveByVirtualItemId_CanBeCalledByAdminUser()
        {
            yield return LoginAsAdminTester();

            string userId = RGNCoreBuilder.I.MasterAppUser.UserId;
            string virtualItemId = "7dNN81eLr8XsMxFlgsbH";

            var task1 = InventoryModule.I.AddToInventoryAsync(
                userId,
                virtualItemId);
            yield return task1.AsIEnumeratorReturnNull();
            
            var task = InventoryModule.I.RemoveByVirtualItemIdAsync("7dNN81eLr8XsMxFlgsbH", 1);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.NotNull(result, "The result is null");
            UnityEngine.Debug.Log(result);
        }

        [UnityTest]
        public IEnumerator GetUpgrades_ReturnsArrayOfUpgrades()
        {
            yield return LoginAsNormalTester();

            var virtualItemId = "053824c3-e523-433c-9009-51367f809137";

            var task = InventoryModule.I.GetUpgradesAsync(virtualItemId);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.NotNull(result, "The result is null");
            Assert.IsNotEmpty(result);
        }
        [UnityTest]
        public IEnumerator UpgradeWithDefaultUpgradeId_ReturnsArrayOfUpgrades()
        {
            yield return LoginAsNormalTester();

            var ownedItemId = "yRT1RXlePdlmDehgPiKf";

            var task = InventoryModule.I.UpgradeAsync(ownedItemId, 33);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.NotNull(result, "The result is null");
            UnityEngine.Debug.Log(result);
        }
        [UnityTest]
        public IEnumerator UpgradeWithCustomUpgradeId_ReturnsArrayOfUpgrades()
        {
            yield return LoginAsNormalTester();

            var ownedItemId = "8298Sa57hYHdakmbRIv7";

            var task = InventoryModule.I.UpgradeAsync(ownedItemId, 42, "my_custom_upgrade_level");
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.NotNull(result, "The result is null");
            UnityEngine.Debug.Log(result);
        }
        [UnityTest]
        public IEnumerator SetProperties_ReturnsPropertiesThatWasSet()
        {
            yield return LoginAsNormalTester();

            var ownedItemId = "lFY2aQhWfjf9EiJZsyI1";
            var propertiesToSet = "{}";

            var task = InventoryModule.I.SetPropertiesAsync(ownedItemId, propertiesToSet);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.NotNull(result, "The result is null");
            Assert.AreEqual(propertiesToSet, result);
            UnityEngine.Debug.Log(result);
        }
        [UnityTest]
        public IEnumerator GetProperties_ReturnsPropertiesThatWasSetBeforeInDB()
        {
            yield return LoginAsNormalTester();

            var ownedItemId = "lFY2aQhWfjf9EiJZsyI1";
            var expectedProperties = "{}";

            var task = InventoryModule.I.GetPropertiesAsync(ownedItemId);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.NotNull(result, "The result is null");
            Assert.AreEqual(expectedProperties, result);
            UnityEngine.Debug.Log(result);
        }
        [UnityTest]
        public IEnumerator GetById_ReturnsData()
        {
            yield return LoginAsNormalTester();

            var ownedItemId = "yRT1RXlePdlmDehgPiKf";

            var task = InventoryModule.I.GetByIdAsync(ownedItemId);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.NotNull(result, "The result is null");
            UnityEngine.Debug.Log(result);
        }
        [UnityTest]
        public IEnumerator GetByAppId_ReturnsData()
        {
            yield return LoginAsNormalTester();

            var task = InventoryModule.I.GetAllForCurrentAppAsync();
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.NotNull(result, "The result is null");
            UnityEngine.Debug.Log(result);
        }
        [UnityTest]
        public IEnumerator GetByAppIds_ReturnsData()
        {
            yield return LoginAsNormalTester();

            var appIds = new List<string>() { RGNCoreBuilder.I.AppIDForRequests };

            var task = InventoryModule.I.GetByAppIdsAsync(appIds);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.NotNull(result, "The result is null");
            UnityEngine.Debug.Log(result);
        }
        [UnityTest]
        public IEnumerator GetWithVirtualItemsDataByAppIdsAsync_ReturnsNotNullVirtualItem()
        {
            yield return LoginAsNormalTester();
            
            var appIdsToFind = new List<string> { "io.getready.rgntest" };
            
            var getByAppIdsTask = InventoryModule.I.GetWithVirtualItemsDataByAppIdsAsync(appIdsToFind);
            yield return getByAppIdsTask.AsIEnumeratorReturnNull();
            var getByAppIdsResult = getByAppIdsTask.Result;

            Assert.IsNotNull(getByAppIdsResult);
            Assert.IsNotEmpty(getByAppIdsResult);
            Assert.IsNotNull(getByAppIdsResult[0].GetItem());
            Assert.AreEqual(getByAppIdsResult[0].virtualItemId, getByAppIdsResult[0].GetItem().id);
        }
        [UnityTest]
        public IEnumerator GetByVirtualItemIds_ReturnsData()
        {
            yield return LoginAsNormalTester();

            var virtualItemIds = new List<string>() { "053824c3-e523-433c-9009-51367f809137" };

            var task = InventoryModule.I.GetByVirtualItemIdsAsync(virtualItemIds);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.NotNull(result, "The result is null");
            Assert.AreNotEqual(0, result.Count);
            UnityEngine.Debug.Log(result);
        }
        [UnityTest]
        public IEnumerator GetByVirtualItemIds_ReturnsEmptyListForNonExistingItemData()
        {
            yield return LoginAsNormalTester();

            var virtualItemIds = new List<string>() { "non_existing_virtual_item_id" };

            var task = InventoryModule.I.GetByVirtualItemIdsAsync(virtualItemIds);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.NotNull(result, "The result is null");
            Assert.AreEqual(0, result.Count);
            UnityEngine.Debug.Log(result);
        }
        [UnityTest]
        public IEnumerator GetByIds_ReturnsData()
        {
            yield return LoginAsNormalTester();

            var ownedItemIds = new List<string>() {
                "Cqxfvx5fBAuqcY9IINBW", "lFY2aQhWfjf9EiJZsyI1" };

            var task = InventoryModule.I.GetByIdsAsync(ownedItemIds);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.NotNull(result, "The result is null");
            Assert.AreNotEqual(0, result.Count);
            UnityEngine.Debug.Log(result);
        }
        [UnityTest]
        public IEnumerator GetByIds_ReturnsEmptyListForNonExistingItemData()
        {
            yield return LoginAsNormalTester();

            var ownedItemIds = new List<string>() {
                "non_existing_owned_item_id_one", "non_existing_owned_item_id_two" };

            var task = InventoryModule.I.GetByIdsAsync(ownedItemIds);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.NotNull(result, "The result is null");
            Assert.AreEqual(0, result.Count);
            UnityEngine.Debug.Log(result);
        }
        [UnityTest]
        public IEnumerator GetByIds_ReturnsNonEmptyListForNonExistingAndExistingItemData()
        {
            yield return LoginAsNormalTester();

            var ownedItemIds = new List<string>() {
                "non_existing_owned_item_id_one", "lFY2aQhWfjf9EiJZsyI1", "non_existing_owned_item_id_two" };

            var task = InventoryModule.I.GetByIdsAsync(ownedItemIds);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.NotNull(result, "The result is null");
            Assert.AreEqual(1, result.Count);
            UnityEngine.Debug.Log(result);
        }

    }
}
