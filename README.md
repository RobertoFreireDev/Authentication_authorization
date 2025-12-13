# Overview of Authentication and Authorization in HTTP

Authentication -> who are you? identity

Authorization -> what are you allowed to do? permissions

Http is stateless protocol -> each request is independent, no memory of who you are or what you can do.

Base64 is not encryption. base64 is for http header compatibility

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
- Use https to encrypt the whole request including headers.
- Use short expiration time for avoiding token reuse.
- Claims are visible to anyone who has the token. https://www.jwt.io/
- Store tokens securely (HttpOnly cookies, secure storage, etc)
- Expire credentials with a predetermined time

# Token Storage

## Local Storage

- Easy to use
- Javascript access
- Vunerable to XSS attacks
- Scripts can steal tokens

## Cookies

- HttpOnly flag -> not accessible via javascript
- Vunerable to CSRF attacks

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