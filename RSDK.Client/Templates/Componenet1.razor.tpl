@namespace {{ProjectName}}

@using Microsoft.AspNetCore.Components.Web

@inherits Revuo.Chat.Client.Base.Abstractions.BasePayloadControlThinClient<HelloPayload, {{ProjectName}}App>

<div class="container mt-4 mb-4">
  <h4>Hello from {{ProjectName}}</h4>
  <div class="mb-2">This template includes a simple control and StaticTranslator-based translations.</div>
</div>
