using Amazon.DynamoDBv2.Model;
using Common.Auth.Models;
using Common.DynamoDB.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Auth.Services
{
    public class AuthService : DynamoDBServiceBase
    {
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

        /// <summary>
        /// ユーザー ID からユーザー情報を取得する
        /// </summary>
        /// <param name="userId">ユーザー ID</param>
        /// <returns>ユーザー情報</returns>
        public async Task<Dictionary<string, AttributeValue>> GetUserByUserId<T>(Guid userId) where T : UserAuthBase
        {
            var items = await GetItems(tableName, null, nameof(UserAuthBase.UserId), userId.ToString());

            if (items.Count == 0)
            {
                return null;
            }

            return items.FirstOrDefault();
        }

        /// <summary>
        /// Google ユーザー ID からユーザー ID を取得する
        /// </summary>
        /// <param name="googleUserId">Google ユーザー ID</param>
        /// <returns>ユーザー ID</returns>
        public async Task<string> GetUserIdByGoogle(string googleUserId)
        {
            var items = await GetItems(tableName, indexName, nameof(UserAuthBase.GoogleUserId), googleUserId);

            if (items.Count == 0)
            {
                return null;
            }

            items.FirstOrDefault().TryGetValue(nameof(UserAuthBase.UserId), out var userId);

            return userId.S;
        }

        /// <summary>
        /// Google ユーザーが存在するかどうかを取得する
        /// </summary>
        /// <param name="googleUserId">Google ユーザー ID</param>
        /// <returns>Google ユーザーが存在する場合は true、それ以外は false</returns>
        public async Task<bool> IsExistUserByGoogle(string googleUserId)
        {
            var items = await GetItems(tableName, indexName, nameof(UserAuthBase.GoogleUserId), googleUserId);

            return items.Count > 0;
        }

        /// <summary>
        /// Google ユーザーを追加する
        /// </summary>
        /// <param name="googleUserId">Google ユーザー ID</param>
        public async Task<Guid> AddUserByGoogle(string googleUserId)
        {
            var userId = Guid.NewGuid();

            var user = new UserAuthBase
            {
                UserId = userId,
                GoogleUserId = googleUserId
            };

            await Update(tableName, user);

            return userId;
        }

        public async Task UpdateUser<T>(T user) where T : UserAuthBase
        {
            try
            {
                await Update(tableName, user);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while updating user: {ex.Message}");
                throw;
            }
        }
    }
}
