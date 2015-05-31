#r "packages/FSharp.Data/lib/net40/FSharp.Data.dll"

open System
open FSharp.Data
open FSharp.Data.JsonExtensions

let http_request url =
  Http.RequestString(url,
    httpMethod="GET",
    headers = [ "User-Agent", "" ])
  |> JsonValue.Parse

let issue = http_request "https://api.github.com/repos/shortsn/docdog/issues/1"
issue?title.AsString()
  |> printfn "%s"
