using NUnit.Framework;
using RGN.Extensions;
using RGN.Impl.Firebase.Core;
using RGN.Modules.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.TestTools;

namespace RGN.Inventory.Tests.Runtime
{
    [TestFixture]
    public class InventoryTests
    {
        [OneTimeSetUp]
        public async void OneTimeSetup()
        {
            var applicationStore = ApplicationStore.I; //TODO: this will work only in editor.
            RGNCoreBuilder.AddModule(new InventoryModule());
            var appOptions = new AppOptions()
            {
                ApiKey = applicationStore.RGNMasterApiKey,
                AppId = applicationStore.RGNMasterAppID,
                ProjectId = applicationStore.RGNMasterProjectId
            };

            await RGNCoreBuilder.Build(
                new RGN.Impl.Firebase.Dependencies(
                    appOptions,
                    applicationStore.RGNStorageURL),
                appOptions,
               applicationStore.RGNStorageURL,
               applicationStore.RGNAppId);

            if (applicationStore.usingEmulator)
            {
                RGNCore rgnCore = (RGNCore)RGNCoreBuilder.I;
                var firestore = rgnCore.readyMasterFirestore;
                string firestoreHost = applicationStore.emulatorServerIp + applicationStore.firestorePort;
                bool firestoreSslEnabled = false;
                firestore.UserEmulator(firestoreHost, firestoreSslEnabled);
                rgnCore.readyMasterFunction.UseFunctionsEmulator(applicationStore.emulatorServerIp + applicationStore.functionsPort);
                //TODO: storage, auth, realtime db
            }
        }

        [UnityTest]
        public IEnumerator AddToInventory_StackableItemShouldIncreaseCount()
        {
            var inventoryItem = new VirtualItemInventoryData(
                "b14e64d4-52c2-4f8b-be65-a0161542c010",
                new List<string>() { RGNCoreBuilder.I.AppIDForRequests });

            var task = RGNCoreBuilder.I.GetModule<InventoryModule>().AddToInventory(
                RGNCoreBuilder.I.masterAppUser.UserId,
                inventoryItem);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.NotNull(result, "The result is null");
            Assert.IsTrue(result.quantity > 1);
            UnityEngine.Debug.Log(result);
        }
        [UnityTest]
        public IEnumerator AddToInventory_NonStackableItemShouldCreateNewDocument()
        {
            var inventoryItem = new VirtualItemInventoryData(
                "053824c3-e523-433c-9009-51367f809137",
                new List<string>() { RGNCoreBuilder.I.AppIDForRequests });

            var task = RGNCoreBuilder.I.GetModule<InventoryModule>().AddToInventory(
                RGNCoreBuilder.I.masterAppUser.UserId,
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
            var virtualItemId = "b14e64d4-52c2-4f8b-be65-a0161542c010";
            var quantity = 32;

            var task = RGNCoreBuilder.I.GetModule<InventoryModule>().UpdateInventoryQuantity(virtualItemId, quantity);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.NotNull(result, "The result is null");
        }
        [UnityTest]
        public IEnumerator RemoveByOwnedItemId_CanBeCalled()
        {
            string userId = RGNCoreBuilder.I.masterAppUser.UserId;
            string virtualItemId = "053824c3-e523-433c-9009-51367f809137";

            var task1 = RGNCoreBuilder.I.GetModule<InventoryModule>().AddToInventory(
                userId,
                virtualItemId);

            yield return task1.AsIEnumeratorReturnNull();
            var result1 = task1.Result;

            var task2 = RGNCoreBuilder.I.GetModule<InventoryModule>().RemoveByOwnedItemId(
                userId,
                result1.id);
            yield return task2.AsIEnumeratorReturnNull();
            var result2 = task2.Result;

            Assert.NotNull(result2, "The result is null");
            UnityEngine.Debug.Log(result2);
        }
        [UnityTest]
        public IEnumerator RemoveByVirtualItemId_CanBeCalled()
        {
            string userId = RGNCoreBuilder.I.masterAppUser.UserId;
            string virtualItemId = "92c7067d-cb58-4f3d-a545-36faf409d64c";

            var task = RGNCoreBuilder.I.GetModule<InventoryModule>().RemoveByVirtualItemId(
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
            var virtualItemId = "92c7067d-cb58-4f3d-a545-36faf409d64c";

            var task = RGNCoreBuilder.I.GetModule<InventoryModule>().GetUpgrades(virtualItemId);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.NotNull(result, "The result is null");
            Assert.NotNull(result.upgradesInfoArray, "The upgradesInfoArray is null");
            Assert.IsNotEmpty(result.upgradesInfoArray);
        }
        [UnityTest]
        public IEnumerator UpgradeWithDefaultUpgradeId_ReturnsArrayOfUpgrades()
        {
            var ownedItemId = "SvTYsvVFuwNbytn7CJ7t";

            var task = RGNCoreBuilder.I.GetModule<InventoryModule>().Upgrade(ownedItemId, 33);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.NotNull(result, "The result is null");
            UnityEngine.Debug.Log(result);
        }
        [UnityTest]
        public IEnumerator UpgradeWithCustomUpgradeId_ReturnsArrayOfUpgrades()
        {
            var ownedItemId = "SvTYsvVFuwNbytn7CJ7t";

            var task = RGNCoreBuilder.I.GetModule<InventoryModule>().Upgrade(ownedItemId, 42, "my_custom_upgrade_level");
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.NotNull(result, "The result is null");
            UnityEngine.Debug.Log(result);
        }
        [UnityTest]
        public IEnumerator SetProperties_ReturnsPropertiesThatWasSet()
        {
            var ownedItemId = "SvTYsvVFuwNbytn7CJ7t";
            var propertiesToSet = "{}";

            var task = RGNCoreBuilder.I.GetModule<InventoryModule>().SetProperties(ownedItemId, propertiesToSet);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.NotNull(result, "The result is null");
            Assert.AreEqual(propertiesToSet, result);
            UnityEngine.Debug.Log(result);
        }
        [UnityTest]
        public IEnumerator GetProperties_ReturnsPropertiesThatWasSetBeforeInDB()
        {
            var ownedItemId = "SvTYsvVFuwNbytn7CJ7t";
            var expectedProperties = "{}";

            var task = RGNCoreBuilder.I.GetModule<InventoryModule>().GetProperties(ownedItemId);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.NotNull(result, "The result is null");
            Assert.AreEqual(expectedProperties, result);
            UnityEngine.Debug.Log(result);
        }
        [UnityTest]
        public IEnumerator GetById_ReturnsData()
        {
            var ownedItemId = "SvTYsvVFuwNbytn7CJ7t";

            var task = RGNCoreBuilder.I.GetModule<InventoryModule>().GetById(ownedItemId);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.NotNull(result, "The result is null");
            UnityEngine.Debug.Log(result);
        }
        [UnityTest]
        public IEnumerator GetByAppId_ReturnsData()
        {
            var appId = RGNCoreBuilder.I.AppIDForRequests;

            var task = RGNCoreBuilder.I.GetModule<InventoryModule>().GetByAppId(appId);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.NotNull(result, "The result is null");
            UnityEngine.Debug.Log(result);
        }
        [UnityTest]
        public IEnumerator GetByAppIds_ReturnsData()
        {
            var appIds = new List<string>() { RGNCoreBuilder.I.AppIDForRequests };

            var task = RGNCoreBuilder.I.GetModule<InventoryModule>().GetByAppIds(appIds);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.NotNull(result, "The result is null");
            UnityEngine.Debug.Log(result);
        }

        [UnityTest]
        public IEnumerator GetByVirtualItemIds_ReturnsData()
        {
            var virtualItemIds = new List<string>() { "92c7067d-cb58-4f3d-a545-36faf409d64c" };

            var task = RGNCoreBuilder.I.GetModule<InventoryModule>().GetByVirtualItemIds(virtualItemIds);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.NotNull(result, "The result is null");
            Assert.AreNotEqual(0, result.items.Count);
            UnityEngine.Debug.Log(result);
        }
        [UnityTest]
        public IEnumerator GetByVirtualItemIds_ReturnsEmptyListForNonExistingItemData()
        {
            var virtualItemIds = new List<string>() { "non_existing_virtual_item_id" };

            var task = RGNCoreBuilder.I.GetModule<InventoryModule>().GetByVirtualItemIds(virtualItemIds);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.NotNull(result, "The result is null");
            Assert.AreEqual(0, result.items.Count);
            UnityEngine.Debug.Log(result);
        }
        [UnityTest]
        public IEnumerator GetByIds_ReturnsData()
        {
            var ownedItemIds = new List<string>() {
                "NAhoR9u9wc1NcQy3SnNh", "lckU5Lf3xKpDE7SKI5MN" };

            var task = RGNCoreBuilder.I.GetModule<InventoryModule>().GetByIds(ownedItemIds);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.NotNull(result, "The result is null");
            Assert.AreNotEqual(0, result.items.Count);
            UnityEngine.Debug.Log(result);
        }
        [UnityTest]
        public IEnumerator GetByIds_ReturnsEmptyListForNonExistingItemData()
        {
            var ownedItemIds = new List<string>() {
                "non_existing_owned_item_id_one", "non_existing_owned_item_id_two" };

            var task = RGNCoreBuilder.I.GetModule<InventoryModule>().GetByIds(ownedItemIds);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.NotNull(result, "The result is null");
            Assert.AreEqual(0, result.items.Count);
            UnityEngine.Debug.Log(result);
        }
        [UnityTest]
        public IEnumerator GetByIds_ReturnsNonEmptyListForNonExistingAndExistingItemData()
        {
            var ownedItemIds = new List<string>() {
                "non_existing_owned_item_id_one", "SvTYsvVFuwNbytn7CJ7t", "non_existing_owned_item_id_two" };

            var task = RGNCoreBuilder.I.GetModule<InventoryModule>().GetByIds(ownedItemIds);
            yield return task.AsIEnumeratorReturnNull();
            var result = task.Result;

            Assert.NotNull(result, "The result is null");
            Assert.AreEqual(1, result.items.Count);
            UnityEngine.Debug.Log(result);
        }

    }
}
