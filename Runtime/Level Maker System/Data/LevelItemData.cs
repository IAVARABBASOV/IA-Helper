using IA.Database.DataType;
using UnityEngine;

namespace IA.Database.Data
{
    [System.Serializable]
    public class LevelItemData : BaseData
    {
        public GameItemType ItemType;

        [Space]

        public string AdditionalData;

        public LevelItemData(GameItemType _itemType)
        {
            ItemType = _itemType;
        }
    }

    [System.Serializable]
    public class LevelPropData : BaseData
    {
        public PropType PropType;

        [Space]

        public string AdditionalData;

        public LevelPropData(PropType _propType)
        {
            PropType = _propType;
        }
    }
}