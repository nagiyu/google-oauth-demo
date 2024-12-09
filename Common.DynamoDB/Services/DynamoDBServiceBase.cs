using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.DynamoDB.Services
{
    /// <summary>
    /// DynamoDB サービスの基底クラス
    /// </summary>
    public abstract class DynamoDBServiceBase
    {
        /// <summary>
        /// アプリケーション設定
        /// </summary>
        private readonly IConfiguration configuration;

        /// <summary>
        /// DynamoDB クライアント
        /// </summary>
        protected readonly AmazonDynamoDBClient client;

        /// <summary>
        /// DynamoDB コンテキスト
        /// </summary>
        protected readonly DynamoDBContext context;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DynamoDBServiceBase(IConfiguration configuration)
        {
            this.configuration = configuration;

            var region = configuration["AWS:Region"];
            var accessKey = configuration["AWS:AccessKey"];
            var secretKey = configuration["AWS:SecretKey"];

            client = new AmazonDynamoDBClient(accessKey, secretKey, RegionEndpoint.GetBySystemName(region));
            context = new DynamoDBContext(client);
        }

        /// <summary>
        /// アイテムを取得する
        /// </summary>
        /// <param name="tableName">テーブル名</param>
        /// <param name="indexName">インデックス名</param>
        /// <param name="keyName">キー名</param>
        /// <param name="keyValue">キー値</param>
        /// <returns>アイテムのリスト</returns>
        protected async Task<List<Dictionary<string, AttributeValue>>> GetItems(string tableName, string indexName, string keyName, string keyValue)
        {
            var queryRequest = new QueryRequest
            {
                TableName = tableName,
                IndexName = indexName,
                KeyConditionExpression = $"{keyName} = :keyValue",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":keyValue", new AttributeValue { S = keyValue } },
                }
            };

            var response = await client.QueryAsync(queryRequest);

            return response.Items;
        }

        /// <summary>
        /// テーブルにアイテムを追加する
        /// </summary>
        /// <param name="tableName">テーブル名</param>
        /// <param name="item">アイテム</param>
        protected async Task Update<T>(string tableName, T item)
        {
            var config = new DynamoDBOperationConfig
            {
                OverrideTableName = tableName
            };

            await context.SaveAsync(item, config);
        }
    }
}
