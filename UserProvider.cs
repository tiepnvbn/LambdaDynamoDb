using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LambdaDynamoDb
{
    public interface IUserProvider
    {
        Task<User[]> GetUserAsync();
        Task<bool> AddAttr();
    }
    public class UserProvider : IUserProvider
    {
        private readonly IAmazonDynamoDB dynamoDb;
        public UserProvider(IAmazonDynamoDB dynamoDb)
        {
            this.dynamoDb = dynamoDb;
        }

        public async Task<User[]> GetUserAsync()
        {
            var result = await dynamoDb.ScanAsync(new ScanRequest
            {
                TableName = "User"
            });
            var users = new List<User>();
            if (result != null && result.Items != null)
            {
                foreach (var item in result.Items)
                {
                    item.TryGetValue("Id", out var id);
                    item.TryGetValue("Name", out var name);

                    users.Add(new User()
                    {
                        Id = id?.S,
                        Name = name?.S
                    });
                }
                return users.ToArray();
            }

            return Array.Empty<User>();
        }

        public async Task<bool> AddAttr()
        {

            // Create the request
            var putItemRequest = new PutItemRequest
            {
                TableName = "User",
                Item = new Dictionary<string, AttributeValue>
                {
                    {"Id", new AttributeValue {S = "id5"}},
                    {"Company", new AttributeValue {S = "abc"}},
                    {"Email", new AttributeValue {S = "abc@gmail.com"}},
                    {"ttl_user", new AttributeValue {N = "1646043153"}}
                }
            };


            var result = await dynamoDb.PutItemAsync(putItemRequest);

            return true;

        }
    }
}
