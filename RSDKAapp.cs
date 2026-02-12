// Azure.Comet SimpleApp - A Thin Client Application
// Created by Revuo SDK

using Revuo.Chat.Abstraction;
using Revuo.Chat.Abstraction.Client;
using Revuo.Chat.Base;

namespace Azure_Comet;

public class SimpleApp : BaseThinClientApp
{
    public SimpleApp() : base(null)
    {
    }

    protected override Task OnInit()
    {
        this.AddAction<HellowWorldResponse>(HellowWorld);

        return Task.CompletedTask;
    }

    public async Task<HellowWorldResponse> HellowWorld(IThinClientContext context)
    {
        return new HellowWorldResponse
        {
            Message = "Welcome to SimpleApp!"
        };
    }
}
