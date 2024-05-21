﻿using Microsoft.UI.Xaml;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Notes.AI.Phi
{
    public partial class Phi3
    {
        public IAsyncEnumerable<string> SummarizeTextAsync(string userText, CancellationToken ct = default)
        {
            return InferStreaming("", $"Summarize this text in three to five bullet points:\r {userText}", ct);
        }

        public IAsyncEnumerable<string> FixAndCleanUpTextAsync(string userText, CancellationToken ct = default)
        {
            var systemMessage = "Your job is to fix spelling, and clean up the text from the user. Only respond with the updated text. Do not explain anything.";
            return InferStreaming(systemMessage, userText, ct);
        }

        public IAsyncEnumerable<string> AutocompleteSentenceAsync(string sentence, CancellationToken ct = default)
        {
            var systemMessage = "You are an assistant that helps the user complete sentences. Ignore spelling mistakes and just respond with the words that complete the sentence. Do not repeat the begining of the sentence.";
            return InferStreaming(systemMessage, sentence, ct);
        }

        public Task<List<string>> GetTodoItemsFromText(string text)
        {
            return Task.Run(async () =>
            {

                var system = "you are a helper that reads text and responds in json array format with any todo items in that text. If no todo items, respond with <no-todo>";
                string response = string.Empty;

                await foreach (var partialResult in InferStreaming(system, text))
                {
                    response += partialResult;
                }

                var todos = Regex.Matches(response, @"""([^""]*)""", RegexOptions.IgnoreCase | RegexOptions.Multiline)
                    .Select(m => m.Groups[1].Value)
                    .Where(t => !t.Contains("todo")).ToList();

                return todos;
            });
        }

        public IAsyncEnumerable<string> AskForContentAsync(string content, string question, CancellationToken ct = default)
        {
            var systemMessage = "You are a helpful assistant answering questions about this content";
            return InferStreaming($"{systemMessage}: {content}", question, ct);
        }

    }
}
