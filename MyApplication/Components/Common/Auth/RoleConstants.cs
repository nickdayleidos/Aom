namespace MyApplication.Components.Common.Auth;

public static class RoleConstants
{
    // Individual roles
    public const string Admin = "Admin";
    public const string OST = "OST";
    public const string WFM = "WFM";
    public const string Manager = "Manager";
    public const string Supervisor = "Supervisor";
    public const string TechLead = "Tech Lead";
    public const string Training = "Training";

    // Common combinations
    public const string AdminOst = "Admin,OST";
    public const string AdminWfm = "Admin,WFM";
    public const string AdminTraining = "Admin,Training";
    public const string AdminOstWfm = "Admin,OST,WFM";
    public const string Management = "Admin,OST,WFM,Manager,Supervisor";
    public const string FullAccess = "Admin,OST,WFM,Manager,Supervisor,Tech Lead,Training";
}
