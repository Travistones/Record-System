using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Record_System.BackEnd.Constants
{
    public class ApplicationConstants
    {
        public const string ACCOUNT_APPLICATION_DATA_CONTAINER_KEY = "AccountApplicationData";

        public const string SETTINGS_APPLICATION_DATA_CONTAINER_KEY = "SettingsApplicationData";

        public const string ACCOUNT_DEFAULT_USERNAME_PASSWORD = "OriginalAdmin";

        public const string CURRENT_PRICE_PER_VOLUME_KEY = "CurrentPricePerVolume";

        public const string FIRESTORE_KEY = @"
                                                {
                                                  ""type"": ""service_account"",
                                                  ""project_id"": ""record-system-717f8"",
                                                  ""private_key_id"": ""592db452506d72aad6cf1ab910623fc373155101"",
                                                  ""private_key"": ""-----BEGIN PRIVATE KEY-----\nMIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQCWGoeW0A5aOihc\nBA0IW23TNccAXdkTzL02QqFpx4ZUt+6CRt2c1iuwPvIQtLa5zDl+JzObrjt0+3HE\ngZlBvErV7tr58Lz9m18lZONnKln1+V0otn+E7xjCrgjGYUYODu/pmyRBQz4GzSz6\n9YbmBMzt9o9e+GV34iF2DmOm8LvcPv3sQ2Rjpw4cu0MVabKIpN/nt3BBct0DOJbW\nErLgUpb+atoAGvaMZTkMdYEOy8CECsZwIRL5naYori1Q/msnnhcchnF4EYLh0cSk\nYp8KrfN6SVN2xKsw5xjZPtKjrx/qhTSyCnLsB+a0xvVFaWDcgYKt0BiiMZzhHNrd\np3ASLiVVAgMBAAECggEABZj/rXQc1OiOUMAl7EXclp/OyAi367Ea7UcW1ecG72nc\njyctZIfoIS53wH+8wLkI+9diaU/aD8bAu19J49WtxMSfjeqYYazFo5Nbl6FQsYeX\nHcmQf9iax8fNs/g0sJmGAOQ0OerUCVTldIZrfXB4Y/dp6VTCE/Ju5QQsWzFnEBOK\nra+/SdIZ4YvPCeq6pSU2g+fOH6DGHlPBBMEGtsV6bCA7lXbNRA2cYRrFIh3PhYpq\ntyx6rStHpa74i8c09qglXvIg8zC0k+Maa629qTmX2wDR+JCSZO44MHAB6nm1ixkA\nAIFLAiM64JAflM8hGw3QuvJxE2vdXgJVaOaFyOZqAQKBgQDFFYbiFn2ZugG/Grkw\nCuFtwsj0tc+IKcLIhxauNVhzbQMUBRIMQWqKY9T0un+Z4BEw9SjF95RCGP8hChe0\nMvxAp4ZxNyPJ+I6Ai9ua+IuJe4D0aebaKi3KFZcog5e8rDlg7pqzssnsAn9NL/Gw\nePfa3ez+9mrWgXIV5uwLiUTmYQKBgQDC+a1SyrZD0f9swRp0QkhMvMe5Mhc1Gcmr\n5gYhLhlwEwM/TwA/4sUBwJkw5cvFFbxindR+RoucGrjevRS5sXpzQK3kCyAGXIRi\nzzAlR2x2FVZOJVH8Bj2W+G0t2HvxMeEUjGnuoOp+ss23fReKc5J6EBQegPECguSm\ndUXLqmi7dQKBgCBfadmHUjhHGnRWomam0uKhpTq1BpLaMTmZHvucnZvQIzpNnupO\ncH6s6VUNsVoIOSquinUCNuMokffOXZhm65J/MhE0unc5kcbBsl/hRaJenA8JujbJ\nyN2x8DNicjE2pPIsH1M3If3XZyu5nVyccjIMVBqJRYFYn/HDDbDTanLhAoGAcyMX\n/DUi7ufaqzFZWuAta8trMeznkX75/d3nLr7XXLyNhVw0rIVQi1ld3WdGLstIJQBJ\nFIy6MuQA2d/Ulle9FMRUK796GOeXcfYh21HTNeQhxCH7yRUyV626Y4Fcp7Ep1oWY\npMc30rvqCXoAZc0b6aMHDoBcUnxTZA0Ku1uVX2UCgYBzhOBXH3AX1Nvd/bT8kHhM\nznU3LbTC3SPxyzjNy29yL1Ny5IvvZo5OPWHLw+Rq3bq2a1vfXbeiPOxPyHO29eJf\nkrDETnw3BgWtVYvEcDv8z6poFFe4/XTRNJ/3TdQAKnSuw4a4cGOs/ZIbQ7JA787n\nAm9LLJG5PoYcR5sdTLA6ng==\n-----END PRIVATE KEY-----\n"",
                                                  ""client_email"": ""firebase-adminsdk-khgh4@record-system-717f8.iam.gserviceaccount.com"",
                                                  ""client_id"": ""117033866453036679588"",
                                                  ""auth_uri"": ""https://accounts.google.com/o/oauth2/auth"",
                                                  ""token_uri"": ""https://oauth2.googleapis.com/token"",
                                                  ""auth_provider_x509_cert_url"": ""https://www.googleapis.com/oauth2/v1/certs"",
                                                  ""client_x509_cert_url"": ""https://www.googleapis.com/robot/v1/metadata/x509/firebase-adminsdk-khgh4%40record-system-717f8.iam.gserviceaccount.com"",
                                                  ""universe_domain"": ""googleapis.com""
                                                }
                                            ";

        public const string FIREBASE_STORAGE_BUCKET = @"record-system-717f8.appspot.com";

        public const string IMAGES_FOLDER_NAME = "Images";

        public const string NET_PROFIT_KEY = "NetProfit";

        public const string TRANSACTIONS_COLLECTION_ID = "Transactions";

        public const string TOTALS_DOCUMENT_ID = "Totals";

        public const string YEARS_DOCUMENT_ID = "Years";

        public const string MONTHS_DOCUMENT_ID = "Months";

        public const string DAYS_DOCUMENT_ID = "Days";

        public const string EMPLOYEES_COLLECTION_ID = "Employees";

        public const string SETTINGS_COLLECTION_ID = "Settings";

        public const string USERS_COLLECTION_ID = "Users";

        public const string TOTAL_REVENUES_KEY = "TotalRevenues";

        public const string TOTAL_SALES_REVENUES_KEY = "TotalSalesRevenues";

        public const string TOTAL_OTHER_REVENUES_KEY = "TotalOtherRevenues";

        public const string TOTAL_EXPENSES_KEY = "TotalExpenses";

        public const string TOTAL_CREDITS_KEY = "TotalCredits";

        public const string TOTAL_VOLUME_OF_PEBBBLES_KEY = "TotalVolumeOfPebbles";

        public const string TOTAL_AMOUNT_EARNED_FROM_PEBBLES_KEY = "TotalAmountEarnedFromPebbles";

        public const string TOTAL_VOLUME_OF_GRADE = "TotalVolumeOfGrade";

        public const string TOTAL_AMOUNT_EARNED_FROM_GRADE = "TotalAmountEarnedFromGrade";

        public const string PEBBLES_KEY = "Pebbles";

        public const string LAST_MODIFIED_KEY = "LastModified";

        public const string TOTAL_AMOUNT_USED_IN_TRUCKS_KEY = "TotalAmountUsedInTrucks";

        public const string TOTAL_AMOUNT_IN_OTHER_EXPENSES_KEY = "TotalAmountInOtherExpenses";

        public const string TOTAL_TRUCKS_IN_KEY = "TotalTrucksIn";

        public const string TOTAL_NUMBER_OF_ORDERS_KEY = "TotalNumberOfOrders";

        public const string TOTAL_AMOUNT_IN_ORDERS_KEY = "TotalAmountInOrders";

        public const string TOTAL_VOLUME_IN_ORDERS_KEY = "TotalVolumeInOrders";

        public const string TOTAL_NUMBER_OF_INCOMPLETE_ORDERS_KEY = "TotalNumberOfIncompleteOrders";

        public const string TOTAL_AMOUNT_IN_INCOMPLETE_ORDERS_KEY = "TotalAmountInIncompleteOrders";

        public const string TOTAL_VOLUME_IN_INCOMPLETE_ORDERS_KEY = "TotalVolumeInIncompleteOrders";
        
        public const string TOTAL_NUMBER_OF_COMPLETE_ORDERS_KEY = "TotalNumberOfCompleteOrders";

        public const string TOTAL_AMOUNT_IN_COMPLETE_ORDERS_KEY = "TotalAmountInCompleteOrders";

        public const string TOTAL_VOLUME_IN_COMPLETE_ORDERS_KEY = "TotalVolumeInCompleteOrders";
        
        public const string TOTAL_NUMBER_OF_CREDITS_KEY = "TotalNumberOfCredits";

        public const string TOTAL_AMOUNT_IN_CREDITS_KEY = "TotalAmountInCredits";

        public const string TOTAL_VOLUME_IN_CREDITS_KEY = "TotalVolumeInCredits";

        public const string TOTAL_NUMBER_OF_INCOMPLETE_CREDITS_KEY = "TotalNumberOfIncompleteCredits";

        public const string TOTAL_AMOUNT_IN_INCOMPLETE_CREDITS_KEY = "TotalAmountInIncompleteCredits";

        public const string TOTAL_VOLUME_IN_INCOMPLETE_CREDITS_KEY = "TotalVolumeInIncompleteCredits";
        
        public const string TOTAL_NUMBER_OF_COMPLETE_CREDITS_KEY = "TotalNumberOfCompleteCredits";

        public const string TOTAL_AMOUNT_IN_COMPLETE_CREDITS_KEY = "TotalAmountInCompleteCredits";

        public const string TOTAL_VOLUME_IN_COMPLETE_CREDITS_KEY = "TotalVolumeInCompleteCredits";

        public const string TOTAL_NUMBER_OF_PEBBLES_KEY = "TotalNumberOfPebbles";

        public const string TOTAL_AMOUNT_OF_PEBBLES_KEY = "TotalAmountOfPebbles";

        public const string TOTAL_VOLUME_OF_PEBBLES_KEY = "TotalVolumeOfPebbles";

        public const string IN_INCOMPLETE_ORDERS_KEY = "InIncompleteOrders";

        public const string IN_COMPLETE_ORDERS_KEY = "InCompleteOrders";
        
        public const string IN_INCOMPLETE_CREDITS_KEY = "InIncompleteCredits";

        public const string IN_COMPLETE_CREDITS_KEY = "InCompleteCredits";

        public const string IS_ADMIN_KEY = "IsAdmin";

        public const string IS_OWNER_KEY = "IsOwner";

        public const string COMPANY_KEY = "Company";

        public const string PRICING_KEY = "Pricing";

    }
}
