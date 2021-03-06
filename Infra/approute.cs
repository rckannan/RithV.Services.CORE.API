namespace RithV.Services.CORE.API.Infra
{
    public static class Approute
    { 
        public const string root = "/svc/";
        public static class BBFMC {
            public const string customer_getOrder = root + "Customer/GetCustomer";
            public const string customer_create = root + "Customer/Create";
            public const string home_index = root + "Home/Index";
        }
        
    }
}
