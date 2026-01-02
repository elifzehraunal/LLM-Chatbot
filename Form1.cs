using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using LLM_Chatbot.Services;

namespace LLM_Chatbot
{
    public partial class Form1 : Form
    {
        private readonly ILLMService _llmService;

        public Form1()
        {
            InitializeComponent();

           
            string apiKey = "Buraya Gemini API'nizi girmeniz gerekmektedir.";
            _llmService = new GeminiService(apiKey);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AppendMessage("Bot", "Merhaba, Ben senin kisisel asistaninim. Bugun senin icin ne yapabilirim.", Color.Blue);
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            string message = txtMessage.Text.Trim();
            if (string.IsNullOrWhiteSpace(message)) return;

            // UI Updates
            AppendMessage("You", message, Color.Black);
            txtMessage.Clear();
            btnSend.Enabled = false;

            try
            {
                // Call LLM
                string response = await _llmService.GetResponseAsync(message);
                AppendMessage("Bot", response, Color.Blue);
            }
            catch (Exception ex)
            {
                AppendMessage("Error", ex.Message, Color.Red);
            }
            finally
            {
                btnSend.Enabled = true;
                txtMessage.Focus();
            }
        }

        private void AppendMessage(string sender, string message, Color color)
        {
            rtbHistory.SelectionStart = rtbHistory.TextLength;
            rtbHistory.SelectionLength = 0;

            rtbHistory.SelectionColor = color;
            rtbHistory.SelectionFont = new Font(rtbHistory.Font, FontStyle.Bold);
            rtbHistory.AppendText($"{sender}: ");

            rtbHistory.SelectionColor = Color.Black;
            rtbHistory.SelectionFont = rtbHistory.Font;
            rtbHistory.AppendText($"{message}{Environment.NewLine}{Environment.NewLine}");

            rtbHistory.ScrollToCaret();
        }
    }
}
