namespace stickyServer

open System.Text
open Microsoft.IdentityModel.Tokens

type AuthOptions() = 
    static let issuerField = "Server" // издатель токена
    static let audienceField: string = "Client" // потребитель токена
    static let keyField = "mysupersecret_123"   // ключ для шифрации
    static let lifetimeField = 60.0 // время жизни токена - 1 минута
    static member ISSUER with get() = issuerField
    static member AUDIENCE with get() = audienceField
    static member KEY with get() = keyField
    static member LIFETIME with get() = lifetimeField
    static member GetSymmetricSecurityKey() : SymmetricSecurityKey =
        SymmetricSecurityKey(Encoding.ASCII.GetBytes(keyField));
