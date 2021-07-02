using Grpc.Core;
using System;

namespace DawnQuant.Passport
{
    public static class GrpcMetadataExtension
    {

        public static void AddAuthorization(this Metadata headers, string accessToken)
        {
            if (!string.IsNullOrEmpty(accessToken))
            {
                headers.Add("Authorization", $"Bearer {accessToken}");
            }

        }
    }
}
