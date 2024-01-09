using System.Collections.Generic;

namespace IA.Database.Data
{
    [System.Serializable]
    public class CoolAdditionalData<T> : System.Object where T : IData
    {
        public List<T> Items = new List<T>();

        public T GetData(int _id)
        {
            if (Items.Count > 0)
            {
                return Items.Find(x => x.ID == _id);
            }
            else return default;
        }
    }
}