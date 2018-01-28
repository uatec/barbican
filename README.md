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

## Running in Docker

Build a configuration file:

```yaml
AzureAd:
  Instance: https://login.microsoftonline.com/
  Domain: [Enter the domain of your tenant, e.g. contoso.onmicrosoft.com]
  TenantId: [Enter the Tenant Id (Obtained from the Azure portal. Select 'Endpoints' from the 'App registrations' blade and use the GUID in any of the URLs), e.g. da41245a5-11b3-996c-00a8-4d99re19f292]
  ClientId: [Enter the Client Id (Application ID obtained from the Azure portal), e.g. ba74781c2-53c2-442a-97c2-3d60re42f403]
  CallbackPath: /signin-oidc
```

Map the config file in to the container to inject the configuration, and run:

```sh
docker run --name barbican -v `pwd`/config.yml:/config.yml uatec/barbican
```