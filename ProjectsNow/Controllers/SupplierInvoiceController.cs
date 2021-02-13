using System.Reflection;
using ProjectsNow.Database;

namespace ProjectsNow.Controllers
{
    public static class SupplierInvoiceController
    {
        public static void Update(this SupplierInvoice oldInvoice, SupplierInvoice newInvoice)
        {
            foreach (PropertyInfo property in typeof(SupplierInvoice).GetProperties())
            {
                if (property.SetMethod != null)
                    oldInvoice.GetType().GetProperty(property.Name).
                    SetValue(oldInvoice, typeof(SupplierInvoice).GetProperty(property.Name).GetValue(newInvoice));
            }
        }
    }
}
