using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IA.Database.DataType;
using IA.Database.Data;
using System.Linq;

namespace IA.Database
{
    public class LevelScriptable : ScriptableObject
    {
        /// <summary>
        /// Level Items List
        /// </summary>
        public List<LevelItemData> LevelData = new List<LevelItemData>();

        /// <summary>
        /// Level Props List
        /// </summary>
        public List<LevelPropData> LevelProps = new List<LevelPropData>();

        public LevelItemData GetLevelItem(GameItemType _itemType)
        {
            LevelItemData selectedItem = LevelData.Find(x => x.ItemType == _itemType);

            return selectedItem;
        }

        public LevelPropData GetLevelProp(PropType _propType)
        {
            LevelPropData levelPropData = LevelProps.Find(x => x.PropType == _propType);

            return levelPropData;
        }
    }
}