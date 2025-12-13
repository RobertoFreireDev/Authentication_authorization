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