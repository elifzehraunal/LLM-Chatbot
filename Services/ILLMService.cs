using System;
using System.Threading.Tasks;

namespace LLM_Chatbot.Services
{
    public interface ILLMService
    {
        Task<string> GetResponseAsync(string history);
    }
}
