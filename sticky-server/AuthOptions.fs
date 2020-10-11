namespace stickyServer

open System.Text
open Microsoft.IdentityModel.Tokens

type AuthOptions() = 
    static let issuerField: string = "Server" // издатель токена
    static let audienceField: string = "Client" // потребитель токена
    static let keyField: string = "gFZpR_<[DuH(fQjfL<VRBHuT,xa:8w4xVr%bf^ExAh52fC)C"   // ключ для шифрации
    static let lifetimeField: float = 60.0 // время жизни токена
    static member ISSUER with get() = issuerField
    static member AUDIENCE with get() = audienceField
    static member KEY with get() = keyField
    static member LIFETIME with get() = lifetimeField
    static member GetSymmetricSecurityKey() : SymmetricSecurityKey =
        SymmetricSecurityKey(Encoding.ASCII.GetBytes(keyField));
