using System.Reflection;
using ProjectsNow.Database;

namespace ProjectsNow.Controllers
{
    public static class SupplierController
    {
        public static void Update(this Supplier oldSupplier, Supplier newSupplier)
        {
            foreach (PropertyInfo property in typeof(Supplier).GetProperties())
            {
                if (property.SetMethod != null)
                    oldSupplier.GetType().GetProperty(property.Name).
                    SetValue(oldSupplier, typeof(Supplier).GetProperty(property.Name).GetValue(newSupplier));
            }
        }
    }
}
