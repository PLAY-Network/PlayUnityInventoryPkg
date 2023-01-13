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

            var inventoryItem = new VirtualItemInventoryData(
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

            var inventoryItem = new VirtualItemInventoryData(
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
        public IEnumerator UpdateInventoryQuantity_CanBeCalledOnlyWithAdminRights()
        {
            yield return LoginAsAdminTester();

            var virtualItemId = "b14e64d4-52c2-4f8b-be65-a0161542c010";
            var quantity = 32;

            var task = InventoryModule.I.UpdateInventoryQuantityAsync(virtualItemId, quantity);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.NotNull(result, "The result is null");
        }
        [UnityTest]
        public IEnumerator RemoveByOwnedItemId_CanBeCalledByAdminUser()
        {
            yield return LoginAsAdminTester();

            string userId = RGNCoreBuilder.I.MasterAppUser.UserId;
            string virtualItemId = "053824c3-e523-433c-9009-51367f809137";

            var task1 = InventoryModule.I.AddToInventoryAsync(
                userId,
                virtualItemId);

            yield return task1.AsIEnumeratorReturnNull();
            var result1 = task1.Result;

            var task2 = InventoryModule.I.RemoveByOwnedItemIdAsync(
                userId,
                result1.id);
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
            string virtualItemId = "92c7067d-cb58-4f3d-a545-36faf409d64c";

            var task1 = InventoryModule.I.AddToInventoryAsync(
                userId,
                virtualItemId);
            yield return task1.AsIEnumeratorReturnNull();

            var task = InventoryModule.I.RemoveByVirtualItemIdAsync(
                userId,
                virtualItemId);
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
            Assert.NotNull(result.upgradesInfoArray, "The upgradesInfoArray is null");
            Assert.IsNotEmpty(result.upgradesInfoArray);
        }
        [UnityTest]
        public IEnumerator UpgradeWithDefaultUpgradeId_ReturnsArrayOfUpgrades()
        {
            yield return LoginAsNormalTester();

            var ownedItemId = "hgy7jgfI5kuk37D0DX7j";

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

            var ownedItemId = "hgy7jgfI5kuk37D0DX7j";

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

            var ownedItemId = "cLKMibuMTFkA9m5BvBeg";
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

            var ownedItemId = "cLKMibuMTFkA9m5BvBeg";
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

            var ownedItemId = "cLKMibuMTFkA9m5BvBeg";

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

            var appId = RGNCoreBuilder.I.AppIDForRequests;

            var task = InventoryModule.I.GetByAppIdAsync(appId);
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
        public IEnumerator GetWithVirtualItemsDataByAppIdsAsync_ReturnsArrayOfOffers()
        {
            yield return LoginAsNormalTester();
            
            var appIdsToFind = new[] { "GetWithVirtualItemsDataByAppIdsAsync_ReturnsArrayOfOffers" };
            
            var getByAppIdsTask = InventoryModule.I.GetWithVirtualItemsDataByAppIdsAsync(appIdsToFind);
            yield return getByAppIdsTask.AsIEnumeratorReturnNull();
            var getByAppIdsResult = getByAppIdsTask.Result;
            
            Assert.IsNotNull(getByAppIdsResult);
            Assert.IsNotNull(getByAppIdsResult.items);
            Assert.IsNotEmpty(getByAppIdsResult.items);
            Assert.AreEqual(getByAppIdsResult.items[0].virtualItemId, getByAppIdsResult.items[0].Item.id);
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
            Assert.AreNotEqual(0, result.items.Count);
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
            Assert.AreEqual(0, result.items.Count);
            UnityEngine.Debug.Log(result);
        }
        [UnityTest]
        public IEnumerator GetByIds_ReturnsData()
        {
            yield return LoginAsNormalTester();

            var ownedItemIds = new List<string>() {
                "cLKMibuMTFkA9m5BvBeg", "kTfNdxPNN6KpPAFTZBKv" };

            var task = InventoryModule.I.GetByIdsAsync(ownedItemIds);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.NotNull(result, "The result is null");
            Assert.AreNotEqual(0, result.items.Count);
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
            Assert.AreEqual(0, result.items.Count);
            UnityEngine.Debug.Log(result);
        }
        [UnityTest]
        public IEnumerator GetByIds_ReturnsNonEmptyListForNonExistingAndExistingItemData()
        {
            yield return LoginAsNormalTester();

            var ownedItemIds = new List<string>() {
                "non_existing_owned_item_id_one", "kTfNdxPNN6KpPAFTZBKv", "non_existing_owned_item_id_two" };

            var task = InventoryModule.I.GetByIdsAsync(ownedItemIds);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.NotNull(result, "The result is null");
            Assert.AreEqual(1, result.items.Count);
            UnityEngine.Debug.Log(result);
        }

    }
}
