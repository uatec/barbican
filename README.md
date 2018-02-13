# Project Bastion: Barbican

A secure proxy to allow authorised access to internal services without exposing them directly.


## Routes

### DNS Routing

Specifying the scheme and hostname/port, barbican will simply resolve these itself to relay the service. This will include localhost and private network addresses like the AWS metadata endpoint, so it is important to be aware when deciding where to host barbican.

```
/proxy/<scheme>/<host_and_port>/<path> ==> <scheme>://<host_and_port>/<path>
```

e.g. 
```
/proxy/https/www.bbc.co.uk/news ==> https://www.bbc.co.uk/news
/proxy/http/some_api.internal:8080/ ==> http://some_api.internal:8080/
```

## Authentication

If not specified, the default auth provider is Basic Authentication

### Basic Access Authentication

As the default auth provider, if no configuration is provided Basic Authentication will generate a username and password logging them to stdout.

```yaml
Authentication:
  Provider: Basic

---

Using Basic Auth - Username: "LlSO4iBLPlp0vTZk7yx1QL9830I=" Password: "6gEuyvgzLGum6jVnymPKPY+Y1pY="
```

or

```yaml
Authentication:
  Provider: Basic
  Username: john.smith
  Password: Password123
```

### Azure AD

```yaml
Authentication:
  Provider: AzureAd
  Instance: https://login.microsoftonline.com/
  Domain: contoso.onmicrosoft.com # Enter the domain of your tenant
  TenantId: 'da41245a5-11b3-996c-00a8-4d99re19f292' # Enter the Tenant Id (Obtained from the Azure portal. Select 'Endpoints' from the 'App registrations' blade and use the GUID in any of the URLs)
  ClientId: ba74781c2-53c2-442a-97c2-3d60re42f403 # Enter the Client Id (Application ID obtained from the Azure portal) 
  CallbackPath: /signin-oidc
```

## Running in Docker

Build a configuration file:

```yaml
Authentication:
  Provider: Basic
  Username: john.smith
  Password: Password123
```

Map the config file in to the container to inject the configuration, and run:

```sh
docker run --name barbican -v `pwd`/config.yml:/config.yml uatec/barbican
```
