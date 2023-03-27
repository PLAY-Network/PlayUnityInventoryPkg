using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using RGN.Impl.Firebase;
using RGN.Modules.Inventory;
using RGN.Modules.VirtualItems;
using RGN.UI;
using RGN.Utility;
using TMPro;
using UnityEngine;

namespace RGN.Samples
{
    internal sealed class InventoryItemScreenParameters
    {
        internal InventoryItemData InventoryItemData { get; }

        internal InventoryItemScreenParameters(InventoryItemData inventoryItemData)
        {
            InventoryItemData = inventoryItemData;
        }
    }

    public sealed class InventoryItemScreen : IUIScreen
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private TextMeshProUGUI _idText;
        [SerializeField] private TextMeshProUGUI _createdAtText;
        [SerializeField] private TextMeshProUGUI _updatedAtText;
        [SerializeField] private TextMeshProUGUI _createdByText;
        [SerializeField] private TextMeshProUGUI _updatedByText;
        [SerializeField] private TextMeshProUGUI _tagsText;
        [SerializeField] private TextMeshProUGUI _appIdsText;
        [SerializeField] private TextMeshProUGUI _childIdsText;
        [SerializeField] private TextMeshProUGUI _propertiesText;
        [SerializeField] private TextMeshProUGUI _isStackableText;
        [SerializeField] private LoadingIndicator _fullScreenLoadingIndicator;
        [SerializeField] private IconImage _virtualItemIconImage;
        [SerializeField] private RectTransform _scrollRectContent;
        [SerializeField] private RectTransform _buyButtonsAnchor;
        [SerializeField] private GameObject _nftIconGameObject;

        private InventoryItemData _inventoryItemData;
        private VirtualItem _virtualItem;

        public override void PreInit(IRGNFrame rgnFrame)
        {
            base.PreInit(rgnFrame);
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
        public override async void OnWillAppearNow(object parameters)
        {
            var castedParams = parameters as InventoryItemScreenParameters;
            _inventoryItemData = castedParams.InventoryItemData;
            _virtualItem = _inventoryItemData.GetItem();
            if (_inventoryItemData == null)
            {
                Debug.LogError("The provided inventory item data is null or invalid");
                return;
            }
            if (_virtualItem == null)
            {
                Debug.LogError("The provided virtual item is null or invalid");
                return;
            }
            _titleText.text = _virtualItem.name;
            _descriptionText.text = _virtualItem.description;
            _idText.text = _virtualItem.id;
            _createdAtText.text = DateTimeUtility.UnixTimeStampToISOLikeStringNoMilliseconds(_virtualItem.createdAt);
            _updatedAtText.text = DateTimeUtility.UnixTimeStampToISOLikeStringNoMilliseconds(_virtualItem.updatedAt);
            _createdByText.text = _virtualItem.createdBy;
            _updatedByText.text = _virtualItem.updatedBy;
            _isStackableText.text = _virtualItem.isStackable ? "Item is stackable" : "Item is not stackable";
            _tagsText.text = BuildStringFromStringsList(_virtualItem.tags, "tags");
            _appIdsText.text = BuildStringFromStringsList(_virtualItem.appIds, "app ids");
            _childIdsText.text = BuildStringFromStringsList(_virtualItem.childs, "virtual item childs");
            _propertiesText.text = BuildStringFromPropertiesList(_virtualItem.properties);
            _nftIconGameObject.SetActive(_virtualItem.IsNFT());
            _fullScreenLoadingIndicator.SetEnabled(false);
            await LoadIconImageAsync(_virtualItem.id, false);
        }

        private async Task LoadIconImageAsync(string virtualItemId, bool tryToloadFromCache)
        {
            _canvasGroup.interactable = false;
            _virtualItemIconImage.SetLoading(true);
            string localPath = Path.Combine(Application.persistentDataPath, "virtual_items", virtualItemId + ".png");
            Texture2D image = null;
            if (tryToloadFromCache)
            {
                if (File.Exists(localPath))
                {
                    byte[] bytes = File.ReadAllBytes(localPath);
                    image = new Texture2D(1, 1);
                    image.LoadImage(bytes);
                    image.Apply();
                }
            }
            if (image == null)
            {
                byte[] bytes = await VirtualItemsModule.I.DownloadImageAsync(virtualItemId);

                if (bytes != null)
                {
                    image = new Texture2D(1, 1);
                    image.LoadImage(bytes);
                    image.Apply();
                    Directory.CreateDirectory(Path.GetDirectoryName(localPath));
                    File.WriteAllBytes(localPath, bytes);
                }
            }
            _virtualItemIconImage.SetProfileTexture(image);
            _canvasGroup.interactable = true;
            _virtualItemIconImage.SetLoading(false);
        }

        private string BuildStringFromStringsList(List<string> strings, string name)
        {
            if (strings == null || strings.Count == 0)
            {
                return $"No {name} set";
            }
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < strings.Count; i++)
            {
                sb.Append(strings[i]);
                if (i < strings.Count - 1)
                {
                    sb.Append(", ");
                }
            }
            return sb.ToString();
        }
        private string BuildStringFromPropertiesList(List<Properties> properties)
        {
            if (properties == null || properties.Count == 0)
            {
                return "No properties set";
            }
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < properties.Count; ++i)
            {
                var property = properties[i];
                sb.Append("Properties for apps: ");
                sb.Append(BuildStringFromStringsList(property.appIds, "app ids"));
                sb.AppendLine(string.IsNullOrWhiteSpace(property.json) ? " are not set." : " are set to: ");
                sb.AppendLine(property.json);
            }
            return sb.ToString();
        }
    }
}
