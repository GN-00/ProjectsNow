using System.Reflection;
using ProjectsNow.Database;

namespace ProjectsNow.Controllers
{
    public static class ItemTransactionController
    {
        public static void Update(this ItemTransaction oldItem, ItemTransaction newItem)
        {
            foreach (PropertyInfo property in typeof(ItemTransaction).GetProperties())
            {
                if (property.SetMethod != null)
                    oldItem.GetType().GetProperty(property.Name).
                    SetValue(oldItem, typeof(ItemTransaction).GetProperty(property.Name).GetValue(newItem));
            }
        }
    }
}
