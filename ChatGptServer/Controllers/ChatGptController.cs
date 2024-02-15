using ChatGptServer.Models.ChatGpt;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.ComponentModel;
using System.Xml.Serialization;

namespace ChatGptServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatGptController : ControllerBase
    {
        private IChatRepository ChatRepository;

        public ChatGptController(IChatRepository repo)
        {
            this.ChatRepository = repo;
        }

        [HttpGet("get.{format}"), FormatFilter]
        public IEnumerable<Chat> Get()
        {
            return this.ChatRepository.Chats;
        }

        [HttpGet("{id}/get.{format}"), FormatFilter]
        public Chat Get(int id) => this.ChatRepository[id];

        [HttpPost("post.{format}"), FormatFilter]
        public Chat Post([FromBody] Chat chat)
        {
            this.ChatRepository.Add(chat);

            return chat;
        }
    }
}
