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
                string uriWithoutQueryParams = uriToCompare.Split("?").First();
                if (allowCustomQueryParameters && IsMatchWithoutQueryParameters(uriWithoutQueryParams, validRedirectUri)) { return true; }

                if (uriToCompare == validRedirectUri) { return true; }
            }

            return false;
        }

        private static bool IsMatchWithoutQueryParameters(string uriToCompare, string validRedirectUri)

            // The registered redirect uri needs to end with ? to allow custom query parameters.
            => validRedirectUri.EndsWith("?") && uriToCompare == validRedirectUri.TrimEnd('?');
    }
}
