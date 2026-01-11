# Overview of Authentication and Authorization in HTTP

Authentication -> who are you? identity

Authorization -> what are you allowed to do? permissions

Http is stateless protocol -> each request is independent, no memory of who you are or what you can do.

Base64 is not encryption. base64 is for http header compatibility

# Building blocks

## Random value/string

- random value generated from a secure source

## Symmetric Cryptography (Shared Secret)

- Fast and simple but requires secure key sharing.
- Same secret used to encrypt and decrypt

## Asymmetric Cryptography (Public / Private Key)

- Public key (shared)
- Private key (kept secret)

## Hash Functions

- Deterministic, irreversible function
- Validate if data has changed

# Basic

Send {user}:{password} in base64 encoded format in Authorization header.

Vunerabilites:

- data is in Base64 format which is easily decoded.
- Use https to encrypt the whole request including headers.
- Requests logged with authorization header will expose the credentials.
- Avoid in Production env
- Avoid in public networks.
- Avoid in Front end applications (Web, mobile, etc)
- Can't revoke or expire credentials without changing password.
- No session control.
- Credentials sent with every request.

# Bearer JWT

Send Authorization: Bearer {{token}} in base64 encoded format in Authorization header.

Client gets "401 Unauthorized" if:

- Client creates JWT using wrong secret key
- Client modifies JWT and Server doesn't validate JWT after uses its own secret key on server side
- JWT is expired

HS256 - symmetric key (same key to sign and verify). Fast. 

Example: API sends tokens to client side just to send it in next request to the same API

RS256 - assymetric Key - (private key to sign, public key to verify). Slower, more secure.

Example: Distruibuted systems where multiple APIs need to verify the same token. Only the central auth API has the private key.

Advantages:

- Fast to parse and validate user permissions. 
- No need to look up at some database or shared session storage between multiple api instances.
- Refresh Tokens (database) to revoke access.

Vunerabilites:

- data is in Base64 format which is easily decoded.
- Use https
- Use short expiration time for token.
- Use longer expiration time for refresh token.
- Claims are visible to anyone who has the token. https://www.jwt.io/
- Store tokens securely to avoid XSS and CSRF attacks.

# Token Storage

## Local Storage

- Easy to use
- Javascript access
- Vunerable to XSS attacks
- Scripts can steal tokens

## Cookies

- HttpOnly flag -> not accessible via javascript
- Secure flag
- Vunerable to CSRF attacks (Use same site flag to prevent that)

# OAuth 2.0

OAuth 2.0 is not authentication. It is authorization framework.

OAuth grant access to data that lives somewhere else without sharing password.

## Client credentials

Machine-to-machine. No user involved. Example: Scheduled job to sync data

##  Authorization code + PKCE

- Resource Owner -> User who owns the data
- Client -> Application that wants to access the data
- Authorization server -> Server that issues access tokens to the client after successfully authenticating the resource owner and obtaining authorization.
- Resource server -> Server that hosts the protected data and accepts access tokens to allow access to the data.

Notes: 

- Confidential client is protected/private. Example: server side
- Public client is unprotected/public. Don't keep secrets on client side. Examples: mobile app and SPA application.

### Example: User opens a developer dashboard app that displays GitHub activity

- Resource Owner -> The GitHub user who owns the data and grants permission.
- Client -> The dashboard application requesting access to the user�s GitHub data
- Authorization Server -> GitHub OAuth service that authenticates the user and issues access tokens
- Resource Server -> GitHub API that hosts the data and accepts access tokens to return it

Steps:

1. User opens a developer dashboard app that displays GitHub activity.
2. The app asks the user to sign in with GitHub
3. The user is redirected to GitHub�s OAuth Authorization Server and logs in.
4. GitHub shows a consent screen asking the user to allow the app to some access specific data (examples: repositories, commits, issues).
5. After approval, GitHub�s Authorization Server issues an access token to the dashboard app.
6. The dashboard app uses the access token to call the GitHub API
7. The Resource Server validates the token and returns the user�s GitHub data.
8. The dashboard app displays the GitHub dashboard (repositories, commits, issues) to the user.

### Build OAuth request

- URL : oauth authorize endpoint
- ClientId: your client app id. Value provided by authorization server when you register your app
- RedirectUri: where to send the user after authorization. Must match exactly the registered value. no wildcards.
- ResponseType: Request an authorization code
- Scope: permissions your app is requesting
- State: random string to prevent CSRF attacks

Notes: 

- State parameter should be random and unique for each request. Store it temporarily (in memory or session storage) to validate it when the user is redirected back to your app.
- Without state, an attacker could start the flow in their browser and trick the user into authorizing a request they did not intend, leading to unauthorized actions.
- PKCE (Proof Key for Code Exchange) is extension to OAuth 2.0 to prevent authorization code interception attacks.

### Exchanging the code for tokens. Server to server request

- URL: oauth token endpoint
- GrantType: authorization_code
- Code: the authorization code received from the authorization server
- RedirectUri: callback URL.
- ClientId: your client app id. 
- ClientSecret: your client secret

Note: 

- Never share client secret on client side applications. If it is a client side application (mobile for example), use PKCE instead of client secret.

### Use token to get data from resource server

- Token expires fast. Example: 1 hour. Refresh token can last for days.
- Use refresh token to get new access token without user interaction. Treat like password. Don't use refresh token on client side applications or log token.
- Send bearer token to resource server in Authorization header.

### PKCE. Use everywhere (recommended for client and server apps)

# OpenId 

OpenId is an authentication standard built on top of OAuth 2.0

# Notes:

## localStorage vs sessionStorage

| Feature  | localStorage | sessionStorage |
|--------|-------------|----------------|
| Lifespan | **Persistent**: Data has no expiration time and remains after the browser is closed and reopened. | **Temporary**: Data is available only for the duration of the page session and is deleted when the tab/window is closed. It survives a page refresh but not a new tab. |
| Scope | Shared across all tabs/windows from the same origin (protocol, domain, and port). | Isolated to the specific tab/window that created it. A new tab for the same website has its own separate `sessionStorage`. |

## XSS attack:

Both localStorage and sessionStorage are highly vulnerable to Cross-Site Scripting (XSS) attacks because any JavaScript code running on the page, including malicious scripts, has full access to the data stored within them.

## CSRF (Cross-Site Request Forgery) attack:

CSRF (Cross-Site Request Forgery) attacks exploit how browsers automatically send session cookies with requests to a logged-in site, tricking the user's browser into sending unauthorized, state-changing commands (like transferring money or changing settings) to that site, as if the user initiated them