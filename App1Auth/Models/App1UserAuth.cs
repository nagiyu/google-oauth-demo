using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Common.Auth.Models;
using System.Collections.Generic;

namespace App1Auth.Models
{
    public class App1UserAuth : UserAuthBase
    {
        /// <summary>
        /// App1 ユーザーのロール
        /// </summary>
        [DynamoDBProperty]
        public string App1Role { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public App1UserAuth()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="keyValuePairs">キーと値のペア</param>
        public App1UserAuth(Dictionary<string, AttributeValue> keyValuePairs) : base(keyValuePairs)
        {
            if (keyValuePairs.TryGetValue(nameof(App1Role), out var app1RoleValue))
            {
                App1Role = app1RoleValue.S;
            }
            else
            {
                App1Role = string.Empty;
            }
        }
    }
}
