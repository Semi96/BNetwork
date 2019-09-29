using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace BnetworkManagement.Views.Admin
{
    public static class AdminNavPages
    {
        public static string ActivePageKey => "ActivePage";

        public static string Index => "Index";

        public static string RPCs => "RPCs";

        public static string Batches => "Batches";

        public static string AvailableCapacity => "AvailableCapacity";

        public static string Accounting => "Accounting";

        public static string DiscountCodes => "DiscountCodes";

        public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);

        public static string RPCsNavClass(ViewContext viewContext) => PageNavClass(viewContext, RPCs);

        public static string BatchesNavClass(ViewContext viewContext) => PageNavClass(viewContext, Batches);

        public static string AvailableCapacityNavClass(ViewContext viewContext) => PageNavClass(viewContext, AvailableCapacity);

        public static string AccountingNavClass(ViewContext viewContext) => PageNavClass(viewContext, Accounting);

        public static string DiscountCodesNavClass(ViewContext viewContext) => PageNavClass(viewContext, DiscountCodes);
    
        public static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string;
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }

        public static void AddActivePage(this ViewDataDictionary viewData, string activePage) => viewData[ActivePageKey] = activePage;
    }
}
