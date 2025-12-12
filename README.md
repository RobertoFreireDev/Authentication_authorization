# Overview of Authentication and Authorization in HTTP

Authentication -> who are you? identity

Authorization -> what are you allowed to do? permissions

Http is stateless protocol -> each request is independent, no memory of who you are or what you can do.

# Basic

Send {user}:{password} in base64 encoded format in Authorization header.

Base64 is not encryption. base64 is for http header compatibility


Vunerabilites:

- Use https to encrypt the whole request including headers.
- Requests logged with authorization header will expose the credentials.
- Avoid in Production env
- Avoid in public networks.
- Avoid in Front end applications (Web, mobile, etc)
- Can't revoke or expire credentials without changing password.
- No session control, replay attacks possible.
- Credentials sent with every request increases exposure risk.


