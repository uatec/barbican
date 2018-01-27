# Project Bastion: Barbican

A secure proxy to allow authorised access to internal services without exposing them directly.


## Routes

```
/proxy/<scheme>/<host_and_port>/<path> ==> <scheme>://<host_and_port>/<path>
```

e.g. 
```
/proxy/https/www.bbc.co.uk/news ==> http://www.bbc.co.uk/news
```

## Authentication

| Key                | Example | Description |
|--------------------|---------|-------------|
| `AzureAd:Instance` | `https://login.microsoftonline.com/` | |
| `AzureAd:Domain`   | `contoso.onmicrosoft.com` | Enter the domain of your tenant |
| `AzureAd:TenantId` | `da41245a5-11b3-996c-00a8-4d99re19f292` | Enter the Tenant Id (Obtained from the Azure portal. Select 'Endpoints' from the 'App registrations' blade and use the GUID in any of the URLs) |
| `AzureAd:ClientId` | `ba74781c2-53c2-442a-97c2-3d60re42f403` | Enter the Client Id (Application ID obtained from the Azure portal) |
| `AzureId:CallbackPath` | `/signin-oidc`| |