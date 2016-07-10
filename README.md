## SessionToken

A light weight session token with integrity assurance, written in C#.

#### Why

This was written with the express purpose of creating session tokens which meet following requirements:

1) The token requires encryption
2) The token requires integrity assurance

SessionToken uses ProtoBuf and [Authenticated Encryption with Associated Data](https://en.wikipedia.org/wiki/Authenticated_encryption) which meets both requirements listed above.

> Authenticated Encryption provides both data confidentiality and data integrity assurances to the information being protected.
>
> The concept of data authentication appeared in the 1970s in the banking industry. Banks did not want to transmit data and allow an attacker to flip a bit undetected.
>
> In this situation, the attacker would not decrypt the message, instead he or she would only flip a bit so that the encrypted message "Post $100" would be changed to "Post $800".
>
> [Crypto++](https://www.cryptopp.com/wiki/Authenticated_Encryption)

#### Usage

````
// AuthenticatedEncryption
IAuthenticatedEncryption authenticatedEncryption = new AuthenticatedEncryption("secreteKey");

// TokenFactory with default expiry of 1 day
ITokenFactory tokenFactory = new TokenFactory(authenticatedEncryption);

// TokenFactory with custom expiry
ITokenFactory tokenFactory = new TokenFactory(authenticatedEncryption, TimeSpan.FromDays(30));

int userId = 123;
string sessionId = "123"

// Create token. Returns encrypted token as base64 string.
string tokenEncrypted = tokenFactory.GenerateToken(userId, sessionId);

// Renew token. Returns boolean on renewal success/failure.
Token token;
bool successRenew = tokenFactory.RenewToken(tokenEncrypted, out token);

// Decrypt token. Returns boolean on decryption success/failure.
bool successDecrypt = tokenFactory.DencryptToken(tokenEncrypted, out token);
````

#### Dependancies

* inferno ([http://securitydriven.net/inferno/](http://securitydriven.net/inferno/))
* protobuf-net ([https://github.com/mgravell/protobuf-net](https://github.com/mgravell/protobuf-net))

#### License

MIT License

Copyright (c) 2016 Daniel Franklin

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.



