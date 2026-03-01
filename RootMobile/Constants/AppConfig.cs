using System;
namespace RootMobile.Constants
{
    public static class AppConfig
    {
        // keytool -genkeypair -v -keystore avocadokey.keystore -alias AvocadoAlias -keyalg RSA -keysize 2048 -validity 10000 -sigalg SHA256withRSA -storetype PKCS12

        public const string SUPABASE_URL = "https://mwisinljhgjagygojlky.supabase.co";
        public const string SUPABASE_KEY = "sb_publishable_09Elna1NiyOoAFqoGQsF7w_wInlCrcy";
        public const string SupabaseFunctionUrl = "https://bgrmftzpmzwahkxooxxf.supabase.co/functions/v1/openai_request";

        public const string PRODUCTS_SUPABASE_URL = "https://api.valentineos.pp.ua";
        public const string PRODUCTS_SUPABASE_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJyb2xlIjoiYW5vbiIsImlzcyI6InN1cGFiYXNlIiwiaWF0IjoxNzQ3MTcwMDAwLCJleHAiOjE5MDQ5MzY0MDB9.lrHQMIcBdhYdQREUWyEARE3iFR9YzrLRFQ7IsOmm_6I";

        //test ad
        //public const string AdmobInterstitial = "ca-app-pub-3940256099942544/1033173712";
        //public const string AdmobBannerAndroid = "ca-app-pub-3940256099942544/6300978111";

        //real ad
        public const string AdmobInterstitial = "ca-app-pub-2108598265596673/2267385312";
        public const string AdmobBannerAndroid = "ca-app-pub-2108598265596673/7118791706";
    }
}

