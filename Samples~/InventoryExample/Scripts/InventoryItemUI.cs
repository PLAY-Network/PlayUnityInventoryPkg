using RGN.Modules.Inventory;
using RGN.Modules.VirtualItems;
using RGN.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RGN.Samples
{
    internal sealed class InventoryItemUI : MonoBehaviour, System.IDisposable
    {
        public string Id { get => _virtualItem.id; }

        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private TextMeshProUGUI _idText;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _createdAtText;
        [SerializeField] private TextMeshProUGUI _updatedAtText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private GameObject _nftIconGameObject;

        [SerializeField] private Button _openVirtualItemScreenButton;

        private Impl.Firebase.IRGNFrame _rgnFrame;
        private InventoryItemData _inventoryItemData;
        private VirtualItem _virtualItem;
        private bool _disposed = false;

        internal void Init(
            Impl.Firebase.IRGNFrame rgnFrame,
            int index,
            InventoryItemData inventoryItemData)
        {
            _rgnFrame = rgnFrame;
            _inventoryItemData = inventoryItemData;
            _virtualItem = inventoryItemData.GetItem();
            _rectTransform.localPosition = new Vector3(0, -index * GetHeight(), 0);
            _idText.text = _virtualItem.id;
            _nameText.text = _virtualItem.name;
            _createdAtText.text = DateTimeUtility.UnixTimeStampToISOLikeStringNoMilliseconds(_virtualItem.createdAt);
            _updatedAtText.text = DateTimeUtility.UnixTimeStampToISOLikeStringNoMilliseconds(_virtualItem.updatedAt);
            _descriptionText.text = _virtualItem.description;
            _nftIconGameObject.SetActive(_virtualItem.IsNFT());
            if (!_virtualItem.IsNFT())
            {
                _nameText.rectTransform.anchoredPosition = Vector2.right * 4;
            }
            _openVirtualItemScreenButton.onClick.AddListener(OnOpenVirtualItemScreenButtonClick);
        }
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            Destroy(gameObject);
        }
        private void OnDestroy()
        {
            _openVirtualItemScreenButton.onClick.RemoveListener(OnOpenVirtualItemScreenButtonClick);
            _disposed = true;
        }

        internal float GetHeight()
        {
            return _rectTransform.sizeDelta.y;
        }

        private void OnOpenVirtualItemScreenButtonClick()
        {
            Debug.Log("Open Virtual Item Screen");
            //_rgnFrame.OpenScreen<VirtualItemScreen>(
            //    new VirtualItemScreenParameters(
            //        _virtualItem,
            //        _virtualItemsExampleClient));
        }
    }
}
