﻿namespace DamaNoJornal.Core
{
    public class GlobalSetting
    {
        public const string AzureTag = "Azure";
        public const string MockTag = "Mock";
        public const string DefaultEndpoint = "http://localhost:50390"; // i.e.: "http://YOUR_IP" or "http://YOUR_DNS_NAME"
        public const string JueAuthToken = "AAABBBCCC_JUE";
        public const string SueAuthToken = "AAABBBCCC_SUE";
        public const string SoniaAuthToken = "AAABBBCCC_SONIA";

        private string _baseEndpoint;
        private static readonly GlobalSetting _instance = new GlobalSetting();

        public GlobalSetting()
        {
            AuthToken = "";
            BaseEndpoint = DefaultEndpoint;
        }

        public static GlobalSetting Instance
        {
            get { return _instance; }
        }

        public string BaseEndpoint
        {
            get { return _baseEndpoint; }
            set
            {
                _baseEndpoint = value;
                UpdateEndpoint(_baseEndpoint);
            }
        }

        public string ClientId { get { return "xamarin"; }}

        public string ClientSecret { get { return "secret"; }}

        public string AuthToken { get; set; }
        

        public string RegisterWebsite { get; set; }

        public string IdentityEndpoint { get; set; }

        public string UserInfoEndpoint { get; set; }

        public string TokenEndpoint { get; set; }

        public string LogoutEndpoint { get; set; }

        public string IdentityCallback { get; set; }

        public string LogoutCallback { get; set; }

        private void UpdateEndpoint(string baseEndpoint)
        {
            //var identityBaseEndpoint = $"{baseEndpoint}/identity";
            var identityBaseEndpoint = "http://localhost:5000";
            RegisterWebsite = $"{identityBaseEndpoint}/Account/Register";
            LogoutCallback = $"{identityBaseEndpoint}/Account/Redirecting";

            var connectBaseEndpoint = $"{identityBaseEndpoint}/connect";
            IdentityEndpoint = $"{connectBaseEndpoint}/authorize";
            UserInfoEndpoint = $"{connectBaseEndpoint}/userinfo";
            TokenEndpoint = $"{connectBaseEndpoint}/token";
            LogoutEndpoint = $"{connectBaseEndpoint}/endsession";
			
            IdentityCallback = $"{identityBaseEndpoint}/xamarincallback";
        }
    }
}