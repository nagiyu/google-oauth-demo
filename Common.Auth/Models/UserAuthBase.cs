using Amazon.DynamoDBv2.DataModel;
using System;

namespace Common.Auth.Models
{
    public class UserAuthBase
    {
        /// <summary>
        /// ユーザー ID
        /// </summary>
        [DynamoDBHashKey]
        public Guid UserId { get; set; }

        /// <summary>
        /// Google ユーザー ID
        /// </summary>
        [DynamoDBProperty]
        public string GoogleUserId { get; set; }
    }
}
