using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;

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

        public UserAuthBase()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="keyValuePairs">キーと値のペア</param>
        public UserAuthBase(Dictionary<string, AttributeValue> keyValuePairs)
        {
            if (keyValuePairs.TryGetValue(nameof(UserId), out var userIdValue) && !string.IsNullOrEmpty(userIdValue.S))
            {
                UserId = Guid.Parse(userIdValue.S);
            }
            else
            {
                UserId = Guid.Empty;
            }

            if (keyValuePairs.TryGetValue(nameof(GoogleUserId), out var googleUserIdValue))
            {
                GoogleUserId = googleUserIdValue.S;
            }
            else
            {
                GoogleUserId = string.Empty;
            }
        }
    }
}
