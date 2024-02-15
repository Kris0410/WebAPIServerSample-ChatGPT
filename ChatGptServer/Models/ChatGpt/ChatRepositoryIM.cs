using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;

namespace ChatGptServer.Models.ChatGpt
{
    public class ChatRepositoryIM : IChatRepository
    {
        public const string ApiSettingName = @"ApiSettings.json";
        
        private string? ApiKey;

        private string ApiUrl = "https://api.openai.com/v1/chat/completions";

        private Dictionary<int, Chat> ChatList;

        public ChatRepositoryIM()
        {
            ChatList = new Dictionary<int, Chat>();

            this.Initialize();
        }

        private void Initialize()
        {

            if (!File.Exists(ApiSettingName))
            {
                // Log 및 종료
                //MessageBox.Show(Path.GetFullPath(ApiSettingName));
                //MessageBox.Show("ApiKey is null!");
                return;
            }

            using (StreamReader file = File.OpenText(ApiSettingName))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                JObject json = (JObject)JToken.ReadFrom(reader);
                this.ApiKey = json["Secret key"].ToString();
            }

        }

        public Chat this[int id] => this.ChatList.ContainsKey(id) ? this.ChatList[id] : null;

        public IEnumerable<Chat> Chats => this.ChatList.Values;

        public Chat Add(Chat newChat)
        {
            this.ChatList[newChat.Id] = newChat;

            this.CallChatGpt(newChat);

            return newChat;
        }

        private void CallChatGpt(Chat chat)
        {
            // API 호출을 위한 HttpClient 생성
            using (HttpClient httpClient = new HttpClient())
            {
                // API 요청 헤더 설정
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {this.ApiKey}");

                // API 요청 본문 데이터 준비
                StringBuilder sb = new StringBuilder();
                sb.Append("{");
                sb.Append("\"model\": \"gpt-3.5-turbo\",");
                sb.Append("\"messages\": [{\"role\": \"user\", \"content\": \"");
                sb.Append($"{chat.Message}");
                sb.Append("\"}],");
                sb.Append("\"temperature\": 0.7");
                sb.Append("}");

                // API 호출 및 응답 수신
                Task<HttpResponseMessage> task = httpClient.PostAsync(ApiUrl, new StringContent(sb.ToString(), Encoding.UTF8, "application/json"));
                task.Wait();
                HttpResponseMessage response = task.Result;

                // 응답 데이터 읽기
                if (response.IsSuccessStatusCode)
                {
                    Task<string> task2 = response.Content.ReadAsStringAsync();
                    string responseBody = task2.Result;

                    JObject json = JObject.Parse(responseBody);
                    JToken choices = json["choices"];

                    string content = "";
                    foreach (JObject item in choices)
                    {
                        content += item["message"]["content"].ToString();
                    }

                    chat.Reply = content;
                }
                else
                {
                    chat.Reply = response.StatusCode.ToString();
                }
            }
        }
    }
}
