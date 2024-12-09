using Common.Auth.Models;
using Common.DynamoDB.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Common.Auth.Services
{
    public class AuthService : DynamoDBServiceBase
    {
        /// <summary>
        /// アプリケーション設定
        /// </summary>
        private readonly IConfiguration configuration;

        /// <summary>
        /// テーブル名
        /// </summary>
        private readonly string tableName;

        /// <summary>
        /// インデックス名
        /// </summary>
        private readonly string indexName;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AuthService(IConfiguration configuration) : base(configuration)
        {
            tableName = configuration["Authentication:TableName"];
            indexName = configuration["Authentication:Google:IndexName"];
        }

        public async Task<bool> IsExistUserByGoogle(string googleUserId)
        {
            return await IsExist(tableName, indexName, nameof(UserAuthBase.GoogleUserId), googleUserId);
        }

        /// <summary>
        /// Google ユーザーを追加する
        /// </summary>
        /// <param name="googleUserId">Google ユーザー ID</param>
        public async Task AddUserByGoogle(string googleUserId)
        {
            var user = new UserAuthBase
            {
                UserId = Guid.NewGuid(),
                GoogleUserId = googleUserId
            };

            await Add(tableName, user);
        }
    }
}
