namespace stickyServer.AuthorizeAttribute
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Mvc.Filters
open System

[<AttributeUsage(AttributeTargets.Class ||| AttributeTargets.Method)>]
type AuthorizeAttribute() =
    interface IAuthorizationFilter with
        member __.OnAuthorization(context: AuthorizationFilterContext) =
            let user = context.HttpContext.Items.["UserName"]
            match user with
            | null -> context.Result <- JsonResult("Unauthorized",
                                    StatusCode = Nullable<int>(StatusCodes.Status401Unauthorized))
            | _ -> ()
                
