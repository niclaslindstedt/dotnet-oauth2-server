using System.Collections.Generic;
using System.Linq;

namespace Etimo.Id.Service.Utilities
{
    public class RedirectUriHelper
    {
        public static bool UriMatches(
            string uriToCompare,
            List<string> validRedirectUris,
            bool allowCustomQueryParameters)
        {
            // Only compare uris with actual values.
            if (uriToCompare == null) { return true; }

            foreach (string validRedirectUri in validRedirectUris)
            {
                if (allowCustomQueryParameters && IsMatchWithoutQueryParameters(uriToCompare, validRedirectUri)) { return true; }

                if (uriToCompare == validRedirectUri) { return true; }
            }

            return false;
        }

        private static bool IsMatchWithoutQueryParameters(string uriToCompare, string validRedirectUri)
        {
            if (validRedirectUri.EndsWith("?")) { return uriToCompare.Split("?").First() == validRedirectUri.TrimEnd('?'); }

            if (validRedirectUri.EndsWith("&")) { return uriToCompare.StartsWith(validRedirectUri.TrimEnd('&')); }

            return false;
        }
    }
}
