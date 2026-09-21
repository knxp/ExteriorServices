namespace ExteriorServices.Web;

// Single source of truth for admin page URLs so links, redirects, and @page routes stay in sync.
public static class AppRoutes
{
    public const string Home = "/";
    public const string AddCustomer = "/admin/add-customer";
    public const string Records = "/admin/records";
    public const string Generate = "/admin/generate";
    public const string Images = "/admin/images";

    public const string ModeAdd = "add";
    public const string ModeEdit = "edit";

    public static string AddCustomerWithMode(string mode) => $"{AddCustomer}?mode={mode}";
}
