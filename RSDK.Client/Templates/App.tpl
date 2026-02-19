using Revuo.Chat.Abstraction.Client;
using Revuo.Chat.Base;
using Revuo.Chat.Client.Base.Abstractions;
using Revuo.Chat.Base.I18N;
using System.Threading.Tasks;
using Revuo.Chat.Abstraction;
using Revuo.Chat.Abstraction.Base;

namespace {{ProjectName}};

public class {{ProjectName}}App : BaseThinClientApp
{
    public {{ProjectName}}App() : base(new StaticTranslator(I18N.set))
    {
    }

    protected override Task OnInit()
    {
        this.AddAction(Hello);
        
        this.AddControl<Component1>();

        return Task.CompletedTask;
    }

    private async Task<HelloPayload> Hello(IThinClientContext context)
    {
        return new HelloPayload();
    }
}

public class HelloPayload : BasePayload
{
}