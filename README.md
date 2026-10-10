# Member Integration Adapter

An HTTP-triggered Azure Function that validates a member registration and maps it
to the JSON format expected by the legacy membership system.
 
The function currently returns the mapped payload and does not call a live legacy API.

## Prerequisites
- .NET 8 SDK
- Azure Functions Core Tools v4
- Node.js/npm

## Build and Test
 
From the solution folder:
 
```powershell
dotnet build
dotnet test
```

## Run locally

Start Azurite in one terminal:

```powershell
New-Item -ItemType Directory -Force "$env:USERPROFILE\.azurite" | Out-Null
npx --yes azurite --location "$env:USERPROFILE\.azurite"
```

In a second terminal, start the function from MemberIntegration.Function project folder:

```powershell
func start
```
In another PowerShell terminal, send a test registration:

```powershell
$body = @{
    firstName = "Alex"
    lastName = "Nguyen"
    dateOfBirth = "1990-04-12"
    email = "alex.nguyen@example.com"
    membershipType = "Single"
    registeredAt = "2026-06-01T09:00:00Z"
} | ConvertTo-Json

$response = Invoke-RestMethod `
    -Uri "http://localhost:7071/api/RegisterMember" `
    -Method Post `
    -ContentType "application/json" `
    -Body $body

$response | ConvertTo-Json -Depth 10
```

Expected response:

```json
{
  "member": {
    "given_name": "Alex",
    "family_name": "Nguyen",
    "dob": "12/04/1990",
    "contact": {
      "email": "alex.nguyen@example.com"
    },
    "plan_code": "S",
    "source": "MILKYWAY"
  }
}
```

## Legacy API Integration

If this was connected to the real legacy API, I would send the mapped payload using a typed HttpClient registered with IHttpClientFactory. The API endpoint would come from configuration so that it can be changed between environments without hardcoding.

Authentication would depend on what the legacy API supports. Any credentials or secrets would be kept outside the application code, for example in Azure Key Vault.

I would also add a timeout and retry handling for temporary failures such as HTTP 408, 429 and 5xx responses. I wouldn't retry other 4xx responses, since sending the same invalid request again is unlikely to succeed. For a 429 response, I would respect the Retry-After value when the API provides one.

If the legacy API supports idempotency, I would include an idempotency key so that retrying a request doesn't accidentally create the same registration twice.

For logging, I would capture enough information to trace and troubleshoot a request, such as a correlation ID and the result of the legacy API call, without unnecessarily logging the full member payload or sensitive member details.
